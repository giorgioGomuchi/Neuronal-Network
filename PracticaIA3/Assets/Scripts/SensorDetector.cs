using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SensorDetector : MonoBehaviour {


    public float speed;

    public GameObject player;

    public float x1, y1,x2,y2;


    // Start is called before the first frame update
    void Start()
    {
        speed = 11.0f;
    }
    void Update()
    {
        transform.Translate(Vector2.right * Time.deltaTime * speed);

        if (player.GetComponent<ControllerAI>().buttomPress == true)
        {
            player.GetComponent<ControllerAI>().buttomPress = false;
            speed = 11.0f;
        }

        if(player.GetComponent<ControllerAI>().notJump == true && player.GetComponent<ControllerAI>().isJumping == true)
        {
            speed = 17.0f;
            player.GetComponent<ControllerAI>().notJump = false;

        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.gameObject.name == "1")
        {
            GameObject sensorXuno;

            sensorXuno = GameObject.CreatePrimitive(PrimitiveType.Cube);
            sensorXuno.transform.position = new Vector3(collision.transform.position.x + 0.29f, collision.transform.position.y + 0.34f, -4f) ;
            sensorXuno.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);

            x1 = sensorXuno.transform.position.x;
            y1 = sensorXuno.transform.position.y;

            player.GetComponent<ControllerAI>().sensors[0] = x1;
            player.GetComponent<ControllerAI>().sensors[3] = y1;


            Destroy(sensorXuno, 2.5f);



        }

        if (collision.gameObject.name == "2")
        {
            GameObject sensorXuno;

            sensorXuno = GameObject.CreatePrimitive(PrimitiveType.Cube);
            sensorXuno.transform.position = new Vector3(collision.transform.position.x - 0.29f, collision.transform.position.y + 0.34f, -4f);
            sensorXuno.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);


            x2 = sensorXuno.transform.position.x;
            y2 = sensorXuno.transform.position.y;

            player.GetComponent<ControllerAI>().sensors[1] = x2;
            player.GetComponent<ControllerAI>().sensors[2] = y2;

            speed = 0;
            Destroy(sensorXuno, 2.5f);






        }
    }

   


}
