using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spin : MonoBehaviour
{
    public Transform target;
    public float speed;
    Vector3 offset;
    void Start()
    {
        offset = transform.position - target.position;
    }

    void Update()
    {
        transform.position = target.position + offset;
        transform.RotateAround(target.position, Vector3.up, speed * Time.deltaTime);
        offset = transform.position - target.position;
    }
}
