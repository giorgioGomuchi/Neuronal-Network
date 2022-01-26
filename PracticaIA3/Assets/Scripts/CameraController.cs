using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private Transform characterTr;

    private Transform tr;
    private Vector3 lastPosition;
    private float distanceToMove = 0;

    void Start()
    {
        tr = transform;
        lastPosition = characterTr.position;
    }

    // Update is called once per frame
    void Update()
    {
        distanceToMove = characterTr.position.x - lastPosition.x;

        tr.position = new Vector3(tr.position.x+distanceToMove, tr.position.y, tr.position.z);

        lastPosition = characterTr.position;
    }
}
