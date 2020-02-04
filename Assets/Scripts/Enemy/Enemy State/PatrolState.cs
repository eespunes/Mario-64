using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Experimental.PlayerLoop;

public class PatrolState : MonoBehaviour, IEnemyState
{
    private Enemy mDroneEnemy;
    private int mCurrentPatrolPositionId;

    public PatrolState(Enemy droneEnemy)
    {
        init(droneEnemy);
    }

    public void init(Enemy droneEnemy)
    {
        mDroneEnemy = droneEnemy;
        mDroneEnemy.mNavMeshAgent.speed = 2;
        
        mCurrentPatrolPositionId = GetClosestPatrolPositionId();
        mDroneEnemy.mNavMeshAgent.isStopped = false;
        mDroneEnemy.mNavMeshAgent.SetDestination(mDroneEnemy.mPatrolPositions[mCurrentPatrolPositionId].position);
    }

    public void UpdateState()
    {
        if (!mDroneEnemy.mNavMeshAgent.hasPath &&
            mDroneEnemy.mNavMeshAgent.pathStatus == NavMeshPathStatus.PathComplete)
            MoveToNextPatrolPosition();
        if (HearsPlayer())
        mDroneEnemy.SetState(new ChaseState(mDroneEnemy));
    }

    bool HearsPlayer()
    {
        return
            mDroneEnemy.GetSqrDistanceXZToPosition(mDroneEnemy.mGameController.mPlayerController.transform.position) <
            (mDroneEnemy.mMinDistanceToAlert * mDroneEnemy.mMinDistanceToAlert);
    }


    void MoveToNextPatrolPosition()
    {
        ++mCurrentPatrolPositionId;
        if (mCurrentPatrolPositionId >= mDroneEnemy.mPatrolPositions.Count)
            mCurrentPatrolPositionId = 0;
        mDroneEnemy.mNavMeshAgent.SetDestination(mDroneEnemy.mPatrolPositions[mCurrentPatrolPositionId].position);
    }

    private int GetClosestPatrolPositionId()
    {
        var lPosition = -1;
        var lCounter = 0;
        var lMinDistance = float.MaxValue;
        foreach (var lActualDistance in mDroneEnemy.mPatrolPositions.Select(position =>
            mDroneEnemy.GetSqrDistanceXZToPosition(position.position)))
        {
            if (lMinDistance > lActualDistance)
            {
                lPosition = lCounter;
                lMinDistance = lActualDistance;
            }

            lCounter++;
        }

        return lPosition;
    }
}