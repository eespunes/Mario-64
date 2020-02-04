using System.Collections;
using UnityEngine;

public class DieState : MonoBehaviour, IEnemyState
{
    private Enemy mDroneEnemy;

    public DieState(Enemy droneEnemy)
    {
        init(droneEnemy);
    }

    public void init(Enemy droneEnemy)
    {
        mDroneEnemy = droneEnemy;
        mDroneEnemy.mNavMeshAgent.isStopped = true;
    }

    public void UpdateState()
    {
        GameObject go = Instantiate(mDroneEnemy.mDrop, mDroneEnemy.transform);
        go.transform.parent = null;
        Destroy(mDroneEnemy.gameObject);
    }
}