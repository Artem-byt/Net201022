using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PLayerGroundChecker : MonoBehaviour
{
    private bool _isGround = false;

    public bool isGround => _isGround;


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name.Contains("Floor"))
        {
            _isGround = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.name.Contains("Floor"))
        {
            _isGround = false;
        }
    }
}
