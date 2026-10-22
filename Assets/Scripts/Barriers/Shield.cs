using System;
using System.Collections;
using UnityEngine;

public class Shield : MonoBehaviour
{
    //-0.45 a -38
    private Material material;

    void Start()
    {
        material = GetComponent<Renderer>().material;
    }

    void OnTriggerEnter(Collider other)
    {


    }
}
