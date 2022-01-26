using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed = 5;

    [SerializeField]
    private float jumpForce = 5;

    [SerializeField]
    private float jumpTime = 0.25f;
    private float jumpTimeCounter = 0.25f;

    [SerializeField]
    private bool isJumping = false;

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

    private Collider2D col;
    private Rigidbody2D rb;
    private Animator animator;

    private int lastMove = 1;
    private int yPos = 0;

    private bool buttonJump = false;

    public bool buttomPress = false;

    public bool jumpingPrevious = false;

    public double[] sensors;


    void Start()
    {
        col = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        jumpTimeCounter = jumpTime;
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

        if (!isJumping) {
            //Procesar inputs

            

            double[] inputs;
            /*
            if (inputs[0] >= inputs[1])
            {
                lastMove = 0;
                buttonJump = true;
            }
            
            else
            {
                lastMove = 1;
                buttonJump = false;
            }
            */
            if (buttonJump && !isDoubleJumping)
            {
                isDoubleJumping = isJumping ? true : false;
                animator.SetBool("IsDoubleJumping", isDoubleJumping);
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                buttonJump = false;
                buttomPress = true;

            }
           
            
        }
        if (isJumping && !buttonJump)
        {
            buttomPress = false;
        }
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.transform.tag == "KillZone")
        {
            float deathX = transform.position.x;
            double input=0;

            //Calcular la distancia entre x2 y la deathX comprendido entre -1 y 1 
            //siendo el valor maximo sensors.maxdistance y el minimo sensors.min distance

            input /= 10;

            ChangeWeights(input);

            Gestor.singleton.RestartGame();
        }
    }

    void SetIniWeights()
    {
        double[] weights = new double[NN.Layers[0].Weights.Length];

        weights[0] = 1;
        weights[1] = -1;
        weights[2] = Random.Range(-1,1);
        weights[3] = Random.Range(-1, 1);
        weights[4] = Random.Range(0, 1);
        weights[5] = Random.Range(-1, 0);

        NN.Layers[0].SetWeights(weights);

        weights = new double[NN.Layers[1].Weights.Length];

        weights[0] = Random.Range(0, 1);
        weights[1] = 0;
        weights[2] = 0;
        weights[3] = Random.Range(0, 1);

        NN.Layers[1].SetWeights(weights);
    }

    void ChangeWeights(double change)
    {
        double[] weights = new double[NN.Layers[0].Weights.Length];

        weights[0] = 1;
        weights[1] = -1;
        weights[2] = NN.Layers[0].Weights[1,0] + change;
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

        weights = new double[NN.Layers[1].Weights.Length];

        float random = Random.Range(0, 0.2f);
        if (lastMove == 1)
        {
            random = -random;
        }

        weights[0] = NN.Layers[1].Weights[0, 0] - random;
        weights[1] = 0;
        weights[2] = 0;
        weights[3] = NN.Layers[1].Weights[0, 1] + random;

        weights[0] = weights[0] > 1 ? 1 : weights[0];
        weights[0] = weights[0] < 0 ? 0 : weights[0];
        weights[3] = weights[3] > 1 ? 1 : weights[3];
        weights[3] = weights[3] < 0 ? 0 : weights[3];

        NN.Layers[1].SetWeights(weights);

        lastMove = 1;
    }

    double[] ProcessInputs(double[] inputs)
    {
        //Procesar distancias aqui en referencia a los sensors.maxdistance/mindistance y guardar la x2

        double[] inputsReturned = inputs;

        double dx1 = inputs[0];
        double dx2 = inputs[1];
        double dy2 = inputs[2];

        if (dx1 == 0)
        {
            dx1 = 1;
        }
        else
        {
            dx1 = 0;
        }

        if (dy2>=0)
        {
            dx2 = 1 - dx2;
            yPos = 0;
        }
        else
        {
            dx1 = 0;
            yPos = 1;
        }

        inputsReturned[0] = dx1;
        inputsReturned[1] = dx2;
        inputsReturned[2] = dy2;

        return inputsReturned;
    }
}