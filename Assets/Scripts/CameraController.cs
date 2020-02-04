using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    public Transform mLookAt;
    private bool mAngleLocked;
    public float mYawRotationalSpeed;
    public float mPitchRotationalSpeed;
    public float mMinPitch;
    public float mMaxPitch;
    public float mMaxDistanceToLookAt;
    public float mMinDistanceToLookAt;
    public LayerMask mRaycastLayerMask;
    public float mOffsetOnCollision;
    private float mTimer;
    [HideInInspector] public Vector2 mMovement;

    public Transform mBetterCamera;
    private bool mDoBetterCamera;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void LateUpdate()
    {
#if UNITY_EDITOR
        if (Input.GetKey(KeyCode.I))
            mAngleLocked = !mAngleLocked;
        if (Input.GetKey(KeyCode.O))
        {
            if (Cursor.lockState == CursorLockMode.Locked)
            {
                Cursor.lockState = CursorLockMode.None;
                mAngleLocked = !mAngleLocked;
            }
        }

#endif


        Vector3 lDirection = mLookAt.position - transform.position;
        float lDistance = lDirection.magnitude;

        Vector3 lDesiredPosition = transform.position;

        if (!mAngleLocked && (mMovement.x > 0.01f || mMovement.x < -0.01f || mMovement.y > 0.01f ||
                              mMovement.y < -0.01f))
        {
            Vector3 lEulerAngles = transform.eulerAngles;
            float lYaw = (lEulerAngles.y + 180.0f);
            float lPitch = lEulerAngles.x;
            lYaw += mYawRotationalSpeed * mMovement.x * Time.deltaTime;
            lYaw *= Mathf.Deg2Rad;
            if (lPitch > 180.0f)
                lPitch -= 360.0f;
            lPitch += mPitchRotationalSpeed * (-mMovement.y) * Time.deltaTime;
            lPitch = Mathf.Clamp(lPitch, mMinPitch, mMaxPitch);
            lPitch *= Mathf.Deg2Rad;
            lDesiredPosition = mLookAt.position + new Vector3(Mathf.Sin(lYaw) * Mathf.Cos(lPitch) * lDistance,
                                   Mathf.Sin(lPitch) * lDistance,
                                   Mathf.Cos(lYaw) * Mathf.Cos(lPitch) * lDistance);
            lDirection = mLookAt.position - lDesiredPosition;
        }

        lDirection /= lDistance;
        if (lDistance > mMaxDistanceToLookAt || lDistance < mMinDistanceToLookAt)
        {
            lDistance = Mathf.Clamp(lDistance, mMinDistanceToLookAt, mMaxDistanceToLookAt);
            lDesiredPosition = mLookAt.position - lDirection * lDistance;
        }

        RaycastHit lRaycastHit;
        Ray lRay = new Ray(mLookAt.position, -lDirection);
        if (Physics.Raycast(lRay, out lRaycastHit, lDistance, mRaycastLayerMask.value))
            lDesiredPosition = lRaycastHit.point + lDirection * mOffsetOnCollision;
        transform.forward = lDirection;
        transform.position = lDesiredPosition;


        if (mDoBetterCamera)
        {
            if (transform.position == mBetterCamera.position)
                mDoBetterCamera = false;
            transform.position = Vector3.MoveTowards(transform.position, mBetterCamera.position, 5f);
        }
    }

    public void DoBetterCamera()
    {
        mDoBetterCamera = true;
    }
}