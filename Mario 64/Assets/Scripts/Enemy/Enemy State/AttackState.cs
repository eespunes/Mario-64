using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AttackState : MonoBehaviour, IEnemyState
{
    private Enemy mDroneEnemy;
    private float mShootMax = 1.5f;
    private float timer;

    public AttackState(Enemy droneEnemy)
    {
        init(droneEnemy);
    }

    public void init(Enemy droneEnemy)
    {
        this.mDroneEnemy = droneEnemy;
        timer = 2;
        mDroneEnemy.mNavMeshAgent.isStopped = true;
        mDroneEnemy.mNavMeshAgent.destination = mDroneEnemy.transform.position;
    }

    public void UpdateState()
    {
        timer += Time.deltaTime;
        if (timer > 1)
        {
            mDroneEnemy.mGameController.mPlayerController.mEnemy = mDroneEnemy.gameObject;
            mDroneEnemy.mGameController.mPlayerController.AddLife(-1);
            timer = 0;
        }

        if (mDroneEnemy.GetSqrDistanceXZToPosition(mDroneEnemy.mGameController.mPlayerController.transform.position) >
            mShootMax)
        {
            mDroneEnemy.SetState(new ChaseState(mDroneEnemy));
        }
    }
    
}