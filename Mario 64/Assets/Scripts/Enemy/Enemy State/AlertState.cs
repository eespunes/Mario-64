using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AlertState : MonoBehaviour,IEnemyState
{
    private float mConeAngle;
    private float mAngle;
    private float mAngleCounter;
    private Enemy mEnemy;

    public AlertState(Enemy enemy)
    {
        init(enemy);
    }
    
    public void init(Enemy enemy)
    {
        mEnemy = enemy;
        mConeAngle = 60.0f;
        mAngle = 1;
        mEnemy.mNavMeshAgent.isStopped = true;
    }

    public void UpdateState()
    {
        if (SeesPlayer())
        {
            mEnemy.SetState(new ChaseState(mEnemy));
        }
        else if (mAngleCounter >= 360)
        {
            mEnemy.SetState(new PatrolState(mEnemy));
        }
        else
        {
            mAngleCounter++;
            mEnemy.transform.Rotate(0, mAngle, 0, Space.Self);
        }
    }

    bool SeesPlayer()
    {
        Vector3
            lDirection = (mEnemy.mGameController.mPlayerController.transform.position + Vector3.up * 0.9f
                         ) - mEnemy.transform.position;
        Ray lRay = new Ray(mEnemy.transform.position, lDirection);
        float lDistance = lDirection.magnitude;
        lDirection /= lDistance;
        bool lCollides = Physics.Raycast(lRay, lDistance, mEnemy.mCollisionLayerMask.value);
        float lDotAngle = Vector3.Dot(lDirection, mEnemy.transform.forward);
        Debug.DrawRay(mEnemy.transform.position, lDirection * lDistance, lCollides ? Color.red : Color.yellow);
        return !lCollides && lDotAngle > Mathf.Cos(mConeAngle * 0.5f * Mathf.Deg2Rad);
    }
}