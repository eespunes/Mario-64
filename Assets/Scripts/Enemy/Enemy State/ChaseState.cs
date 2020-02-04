using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ChaseState : MonoBehaviour, IEnemyState
{
    private float mMinDistanceToAttack = 1.25f;
    private float mMaxDistanceToAttack = 4.0f;
    private Enemy mDroneEnemy;

    public ChaseState(Enemy droneEnemy)
    {
        init(droneEnemy);
    }

    public void init(Enemy droneEnemy)
    {
        mDroneEnemy = droneEnemy;
        mDroneEnemy.mNavMeshAgent.speed = 4;
        mDroneEnemy.mNavMeshAgent.isStopped = false;
    }

    public void UpdateState()
    {
        mDroneEnemy.mNavMeshAgent.SetDestination(mDroneEnemy.mGameController.mPlayerController.transform.position);
        float lDistanceToPlayer =
            mDroneEnemy.GetSqrDistanceXZToPosition(mDroneEnemy.mGameController.mPlayerController.transform.position);
        if (lDistanceToPlayer <= mMinDistanceToAttack)
            mDroneEnemy.SetState(new AttackState(mDroneEnemy));
        else if (lDistanceToPlayer > mMaxDistanceToAttack)
            mDroneEnemy.SetState(new PatrolState(mDroneEnemy));
    }
}