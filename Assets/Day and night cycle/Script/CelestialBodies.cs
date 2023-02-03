using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CelestialBodies : MonoBehaviour
{

    private float _rotateSpeed;
    [SerializeField] float _CycleLength;


    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        _rotateSpeed = (360 / _CycleLength);
        transform.Rotate (Vector3.right * _rotateSpeed * Time.deltaTime);
    }
}