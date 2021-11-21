using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinScript : MonoBehaviour
{

    // Update is called once per frame
    void FixedUpdate()
    {
        gameObject.transform.RotateAround(gameObject.transform.position, Vector3.forward, 1f);
    }
}
