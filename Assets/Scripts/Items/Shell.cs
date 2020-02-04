using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shell : MonoBehaviour
{
    private Rigidbody mRigidbody;

    // Start is called before the first frame update
    void Start()
    {
        mRigidbody = GetComponent<Rigidbody>();
    }

    public void AddForce(Vector3 force)
    {
        mRigidbody.isKinematic = false;
        mRigidbody.AddForce(force);
    }
}