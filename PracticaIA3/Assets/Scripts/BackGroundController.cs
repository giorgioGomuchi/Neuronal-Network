using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackGroundController : MonoBehaviour
{
    [SerializeField]
    private float backgroundSize;
    [SerializeField]
    private float paralaxSpeed;
    [SerializeField]
    private bool scrolling;
    [SerializeField]
    private bool paralax;

    private Transform cameraTransform;
    private Transform tr;
    private Transform[] layers;
    private int leftIndex;
    private int rightIndex;

    private float lastCameraX;
    private float deltaX;

    void Start()
    {
        cameraTransform = Camera.main.transform;
        lastCameraX = cameraTransform.position.x;
        tr = transform;
        layers = new Transform[tr.childCount];
        for(int i = 0; i < tr.childCount; i++)
        {
            layers[i] = tr.GetChild(i);
        }

        leftIndex = 0;
        rightIndex = layers.Length - 1;
    }

    // Update is called once per frame
    void Update()
    {
        if (paralax)
        {
            deltaX = cameraTransform.position.x - lastCameraX;
            tr.position += Vector3.right * (deltaX * paralaxSpeed);
            lastCameraX = cameraTransform.position.x;
        }

        if (scrolling)
        {
            if (cameraTransform.position.x < (layers[leftIndex].transform.position.x))
                ScrollLeft();

            if (cameraTransform.position.x > (layers[rightIndex].transform.position.x))
                ScrollRight();
        }
    }
    private void ScrollLeft()
    {
        layers[rightIndex].position = Vector3.right * (layers[leftIndex].position.x - backgroundSize);
        leftIndex = rightIndex;
        rightIndex--;
        if (rightIndex < 0)
        {
            rightIndex = layers.Length - 1;
        }
    }

    private void ScrollRight()
    {
        layers[leftIndex].position = Vector3.right * (layers[rightIndex].position.x + backgroundSize);
        rightIndex = leftIndex;
        leftIndex++;
        if (leftIndex  == layers.Length)
        {
            leftIndex =  0;
        }
    }
}
