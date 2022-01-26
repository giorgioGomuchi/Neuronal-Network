using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;
using UnityEngine;

public class ControllerAI : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed = 5;

    [SerializeField]
    private float jumpForce = 5;

    [SerializeField]
    private float jumpTime = 0.25f;
    private float jumpTimeCounter = 0.25f;

    public bool isJumping = false;

    [SerializeField]
    private bool isDoubleJumping = false;

    [SerializeField]
    private LayerMask maskGround;

    [SerializeField]
    private Transform groundCheck;

    [SerializeField]
    private float groundCheckRadius;

    [SerializeField]
    private NeuralNetwork NN;

    [SerializeField]
    private uint[] NNTopology;


    private Collider2D col;
    private Rigidbody2D rb;
    private Animator animator;

    private int lastMove = 1;
    private int yPos = 0;

    private bool buttonJump = false;

    public bool buttomPress = false;

    public bool jumpingPrevious = false;

    public double[] sensors;

    private double x2;
    private double posCaida = 0;

    public bool notJump  = false;



    void Start()
    {
        col = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        jumpTimeCounter = jumpTime;
        NN = new NeuralNetwork(NNTopology);        
        SetIniWeights();

        
    }

    void Update()
    {
        //isJumping = !Physics2D.IsTouchingLayers(col, maskGround);

        jumpingPrevious = isJumping;
        isJumping = !Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, maskGround);
        isDoubleJumping = isJumping ? isDoubleJumping : false;
        animator.SetBool("IsJumping", isJumping);
        animator.SetBool("IsDoubleJumping", isDoubleJumping);

        if (!isJumping && jumpingPrevious)
        {
            posCaida = transform.position.x;
        }

        if (isJumping && !jumpingPrevious)
        {
            buttomPress = true;
        }
        else
        {
            buttomPress = false;
        }

        if (isDoubleJumping)
        {
            animator.SetBool("DoubleJumpDid", true);
        }
        if (!isJumping)
        {
            animator.SetBool("DoubleJumpDid", false);
        }

        rb.velocity = new Vector2(moveSpeed, rb.velocity.y);

        if (!isJumping)
        {
            //Procesar inputs
            double[] inputController;



            double[] inputs = (double[]) sensors.Clone();
            inputs = ProcessInputs(inputs);

            inputController = NN.ProcessInputs(inputs);


            
            if (inputController[0] >= inputController[1])
            {
                lastMove = 0;
                buttonJump = true;
            }
            
            else
            {
                lastMove = 1;
                buttonJump = false;
            }
            
            if (buttonJump && !isDoubleJumping)
            {
                isDoubleJumping = isJumping ? true : false;
                animator.SetBool("IsDoubleJumping", isDoubleJumping);
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                buttonJump = false;
                buttomPress = true;
            }


        }
        


    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.transform.tag == "KillZone")
        {
            float deathX = transform.position.x;
            double input = 0;
            float maxDistance = 10;

            //Calcular la distancia entre x2 y la deathX comprendido entre -1 y 1 
            //siendo el valor maximo sensors.maxdistance y el minimo sensors.min distance

            input = deathX - x2;

            if (input < 0)
            {
                maxDistance = -10;
            }

            if(input > x2 + deathX)
            {
                input = x2 + deathX;
            }

            input = Remap(input, 0, maxDistance, -1, 1);

            if (maxDistance == -10)
            {
                input = 1 - input;
            }

            ChangeWeights(input);

            Gestor.singleton.RestartGame();
        }
    }

    void SetIniWeights()
    {
        double[] weights = new double[NN.Layers[0].Weights.Length];

        weights[0] = 1;
        weights[1] = -1;
        weights[2] = Random.Range(0, 1);
        weights[3] = Random.Range(0, 1);
        weights[4] = Random.Range(0, 1);
        weights[5] = Random.Range(-1, 0);

        NN.Layers[0].SetWeights(weights);

        weights = new double[NN.Layers[1].Weights.Length];

        weights[0] = 1;
        weights[1] = 0;
        weights[2] = 0;
        weights[3] = 1;

        NN.Layers[1].SetWeights(weights);
    }

    void ChangeWeights(double change)
    {
        double[] weights = new double[NN.Layers[0].Weights.Length];

        weights[0] = 1;
        weights[1] = -1;
        weights[2] = NN.Layers[0].Weights[1, 0] + change;
        weights[3] = NN.Layers[0].Weights[1, 1] - change;

        if (yPos == 1)
        {
            weights[4] = NN.Layers[0].Weights[2, 0] - Random.Range((float)change, (float)change * 2);
            weights[5] = NN.Layers[0].Weights[2, 1] - Random.Range((float)change, (float)change * 2);
        }
        else
        {
            weights[4] = NN.Layers[0].Weights[2, 0] + Random.Range((float)change / 2, (float)change * 2);
            weights[5] = NN.Layers[0].Weights[2, 1] + Random.Range((float)change / 2, (float)change * 2);
        }

        weights[2] = weights[2] > 1 ? 1 : weights[2];
        weights[2] = weights[2] < -1 ? -1 : weights[2];
        weights[3] = weights[3] > 1 ? 1 : weights[3];
        weights[3] = weights[3] < -1 ? -1 : weights[3];
        weights[4] = weights[4] > 1 ? 1 : weights[4];
        weights[4] = weights[4] < 0 ? 0 : weights[4];
        weights[5] = weights[5] > 0 ? 0 : weights[5];
        weights[5] = weights[5] < -1 ? -1 : weights[5];

        NN.Layers[0].SetWeights(weights);


        Debug.Log("Peso N1 en SI: " + weights[2] + " en NO: " + weights[3]);
        Debug.Log("Peso N2 en SI: " + weights[4] + " en NO: " + weights[5]);


        weights = new double[NN.Layers[1].Weights.Length];

        float random = 0.1f;
        if (lastMove == 1)
        {
            random = -random;
        }

        weights[0] = NN.Layers[1].Weights[0, 0] - random;
        weights[1] = 0;
        weights[2] = 0;
        weights[3] = NN.Layers[1].Weights[1, 1] + random;

        weights[0] = weights[0] > 1 ? 1 : weights[0];
        weights[0] = weights[0] < 0 ? 0 : weights[0];
        weights[3] = weights[3] > 1 ? 1 : weights[3];
        weights[3] = weights[3] < 0 ? 0 : weights[3];


        NN.Layers[1].SetWeights(weights);

        lastMove = 1;

        Debug.Log("Peso Final, en SI: " + weights[0] + " en NO: " + weights[3]);

    }

    double[] ProcessInputs(double[] inputs)
    {
        //Procesar distancias aqui en referencia a los sensors.maxdistance/mindistance y guardar la x2
        x2 = inputs[1];

        float maxDisY = 5.0f;
        
        double[] inputsReturned = new double[3];

        double dx1 = inputs[0];
        double dx2 = inputs[1];
        double dy2 = inputs[2];

        //dx1 = posición comprendida entre el jugador y la posoción del sensor x1 
        dx1 = transform.position.x;

        //En caso de que detecte un sensor que está por detras de ti o un sensor que no interesa
        if(dx1 > inputs[0])
        {
            dx1 = inputs[0];
        }
        if(dx1 < posCaida)
        {
            dx1 = posCaida;
        }

        //Ese resultado tiene que estar comprendido entre 0 y 1
        dx1 = Remap(dx1, posCaida, inputs[0], 0, 1);

        dx2 = transform.position.x;

        if (dx2 > inputs[1])
        {
            dx2 = inputs[1];
        }
        if (dx2 < posCaida)
        {
            dx2 = posCaida;
        }
        //La minima a la que puede estar no es la posicion actual, sino la posicion donde cae
        //La maxima a la que puede estar es x2
        dx2 = Remap(dx2, posCaida, inputs[1], 0, 1);
        dy2 = inputs[2] - inputs[3];

        if (dy2 > maxDisY)
        {
            dy2 = maxDisY;
        }
        
        if (dy2 < - maxDisY)
        {
            dy2 = - maxDisY;
        }

        dy2 = Remap(dy2, - maxDisY, maxDisY, -1, 1);

        if (dx1 < 1)
        {
            dx1 = 0;
        }
        else
        {
            dx1 = 1;
        }
        //cuanto más cerca esté mas fuerza tome el peso de saltar
        if (dy2 >= 0)
        {
            yPos = 0;
        }
        //la plataforma está abajo el hecho de llegar al borde de la plataforma no tenga ningún peso
        //intente saltar lo más al principio de la plataforma 
        else
        {
            double p1 = inputs[1] - inputs[0];
            double p2 = dy2;

            p1 = Remap(p1, 3.6, 6.6, 1,0);
            float x = Mathf.Clamp((float)p1, 0, 1);
            p2 = Remap(p2, -0.28, 0, 1, 0);
            float y = Mathf.Clamp((float)p2, 0, 1);
            Debug.Log("X: " + x + " Y: " + y);
            //si la plataforma no está escesivamente lejos que se deje caer y sino que salte
            if (x+y >= 1.5f) {

                dx1 = 0;
                dx2 -= 0.2f;
                dx2 = dx2 >= 0 ? dx2 : 0;
                notJump = true;

            }
            yPos = 1;
        }

        inputsReturned[0] = dx1;
        inputsReturned[1] = dx2;
        inputsReturned[2] = dy2;

        Debug.Log("dx1: " + dx1 + " dx2: " + dx2 + " dy2: " + dy2);

        return inputsReturned;

    }
    double Remap(double value, double from1, double to1, double from2, double to2)
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }
}