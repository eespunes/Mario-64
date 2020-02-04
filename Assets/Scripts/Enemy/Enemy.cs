using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Object = UnityEngine.Object;

public class Enemy : MonoBehaviour
{
    public NavMeshAgent mNavMeshAgent;
    public IEnemyState mEnemyState;
    public List<Transform> mPatrolPositions;
    public float mMinDistanceToAlert = 1f;
    const float mMaxLife = 100.0f;
    float mLife = mMaxLife;

    public GameObject mDrop;
    [HideInInspector] public GameController mGameController;
    public LayerMask mCollisionLayerMask;

    private AudioSource mAudioSource;
    public AudioClip mStepClip;

    private float mTimer;

    void Start()
    {
        mAudioSource = GetComponent<AudioSource>();
        mEnemyState = new PatrolState(this);
        mGameController = GameObject.Find("GameController").GetComponent<GameController>();
    }

    void Update()
    {
        mTimer += Time.deltaTime;
        if (mNavMeshAgent.speed == 2 && mTimer >= 1)
        {
            Step();
            mTimer = 0;
        }
        else if (mTimer >= .5f)
        {
            Step();
            mTimer = 0;
        }

        mEnemyState.UpdateState();
    }

    public void SetState(IEnemyState enemyState)
    {
        mEnemyState = enemyState;
    }

    public float GetSqrDistanceXZToPosition(Vector3 transformPosition)
    {
        return Mathf.Sqrt(Vector3.Distance(transformPosition, transform.position));
    }

    public void Hit(float f)
    {
        mLife -= f;
        if (mLife <= 0)
            SetState(new DieState(this));
    }

    public void Kill()
    {
        SetState(new DieState(this));
    }

    public void Step()
    {
        mAudioSource.clip = mStepClip;
        mAudioSource.Play();
    }

    private void OnCollisionEnter(Collision other)
    {
        if(other.collider.CompareTag("Shell"))
            SetState(new DieState(this));
    }
}