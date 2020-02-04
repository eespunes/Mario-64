using UnityEngine;

public class PunchBehaviour : StateMachineBehaviour
{
    PlayerController mPlayerController;
    public float m_StartPctTime;
    public float m_EndPctTime;

    public enum TPunchType
    {
        LEFT_HAND = 0,
        RIGHT_HAND,
        FOOT
    }

    public TPunchType mPunchType;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        mPlayerController = animator.GetComponent<PlayerController>();
        mPlayerController.NextPunch();
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        bool lEnableHandPunch = stateInfo.normalizedTime > m_StartPctTime && stateInfo.normalizedTime < m_EndPctTime;
        if (mPunchType == TPunchType.LEFT_HAND)
            mPlayerController.EnableLeftHandPunch(lEnableHandPunch);
        else if (mPunchType == TPunchType.LEFT_HAND)
            mPlayerController.EnableRightHandPunch(lEnableHandPunch);
    }
}