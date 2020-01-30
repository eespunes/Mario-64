using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class PlayerController : MonoBehaviour, Platform.IPlayerActions
{
    float mYaw;
    float mPitch;

    private CharacterController mCharacterController;
    public float mWalkSpeed = 10f;
    public float mRunSpeed = 15f;

    float mVerticalSpeed;
    bool mOnGround;

    public float mJumpSpeed = 10f;

    public Animator mAnimator;
    public GameObject mCameraController;
    public float mBridgeForce;

    private int mJumpCounter;
    private double mJumpTimer;
    private bool mJumping;
    private float mJumpKeyPressed;
    private GameObject mCurrentPlatform;
    private float mLife;
    private float mMaxLife;

    public Image mLifeUI;

    private GameController mGameController;
    private bool mOnce;
    private int mPunchCounter;
    private float mConeAngle = 60;
    public LayerMask mCollisionLayerMask;
    private bool mOnePunch;

    private AudioSource mAudioSource;

    public AudioClip mStep;
    public AudioClip mJump1;
    public AudioClip mJump2;
    public AudioClip mJump3;
    public AudioClip mLongJump;
    public AudioClip mPunchSound1;
    public AudioClip mPunchSound2;
    public AudioClip mPunchSound3;
    public AudioClip mHitSound;
    public AudioClip mDeathSound;
    public AudioClip mCoin;
    public AudioClip mLifeClip;
    public AudioClip mIDLESound;
    private Vector2 mMovement;
    private Platform mPlayerControl;
    private bool mRun;
    private bool mJumpUp, mJumpDown;
    private bool mPunch;
    private bool mOnceJump;
    private float mSpecialIDLETimer;
    private bool mTake;
    public Transform mTakePosition;
    private Vector3 mWallJump;
    private float mBetterCameraTimer;
    public GameObject mEnemy;
    private bool mOnceLongJump;


    void Awake()
    {
        mPlayerControl = new Platform();

        mPlayerControl.Player.SetCallbacks(this);
        mGameController = GameObject.Find("GameController").GetComponent<GameController>();
        mAudioSource = GetComponent<AudioSource>();
        mCharacterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        mJumpCounter = 1;
        mLife = 8f;
        mMaxLife = 8f;
        mPunchCounter = 0;
        mLifeUI.fillAmount = mLife / mMaxLife;
        transform.position = SingletonMaster.getInstance().getPosition();
    }

    private void OnEnable()
    {
        mPlayerControl.Player.Enable();
    }

    private void OnDisable()
    {
        mPlayerControl.Player.Disable();
    }

    void Update()
    {
        Movement();
        mJumpTimer += Time.deltaTime;
        mJumpKeyPressed += Time.deltaTime;
        SpecialIDLE();
        UpdatePlatform();
        BetterCamera();
    }

    private void SpecialIDLE()
    {
        if (mAnimator.GetCurrentAnimatorClipInfo(0)[0].clip.name.Equals("Idle"))
            mSpecialIDLETimer += Time.deltaTime;
        else
            mSpecialIDLETimer = 0;
        if (mSpecialIDLETimer >= 10f)
        {
            mAnimator.SetBool("SpecialIDLE", true);
            mSpecialIDLETimer = 0;
        }
    }

    private void BetterCamera()
    {
        if (mAnimator.GetCurrentAnimatorClipInfo(0)[0].clip.name.Equals("Idle"))
            mBetterCameraTimer += Time.deltaTime;
        else
            mBetterCameraTimer = 0;
        if (mBetterCameraTimer >= 5f)
        {
            mCameraController.GetComponent<CameraController>().DoBetterCamera();
            mBetterCameraTimer = 0;
        }
    }

    private void Movement()
    {
        float lSpeed = mWalkSpeed;

        if (mRun)
            lSpeed = mRunSpeed;

        Vector3 lMovement = Vector3.zero;
        Vector3 lForward = mCameraController.transform.forward;
        Vector3 lRight = mCameraController.transform.right;
        lForward.y = 0.0f;
        lForward.Normalize();
        lRight.y = 0.0f;
        lRight.Normalize();

        if (!mOnce && !mOnceLongJump)
        {
            lMovement = mMovement.y * lForward;
            lMovement += mMovement.x * lRight;
        }
        else if (mOnce)
            lMovement += mWallJump;

        lMovement.Normalize();

        if (mOnceLongJump)
        {
            if (mRun)
                lMovement += transform.forward / lSpeed;
            else
                lMovement += transform.forward;
        }

        bool l_HasMovement = lMovement.sqrMagnitude > 0.01;
        if (l_HasMovement)
            transform.forward = lMovement;

        lMovement *= Time.deltaTime * lSpeed;


        if (mOnGround && mVerticalSpeed == 0f)
            mVerticalSpeed = -mCharacterController.stepOffset / Time.deltaTime;

        mVerticalSpeed += Physics.gravity.y * Time.deltaTime;
        lMovement.y = mVerticalSpeed * Time.deltaTime;

        CollisionFlags l_CollisionFlags = mCharacterController.Move(lMovement);
        if ((l_CollisionFlags & CollisionFlags.Below) != 0 && mVerticalSpeed < 0f)
        {
            if (!mOnGround)
                mJumpTimer = 0;
            mOnceLongJump = false;
            if (mOnce)
            {
                mJumpUp = false;
                mJumpKeyPressed = 0;
            }
            mOnce = false;

            mOnGround = true;
            mVerticalSpeed = 0f;
        }
        else
        {
            mOnGround = false;
        }

        if ((l_CollisionFlags & CollisionFlags.Above) != 0 && mVerticalSpeed > 0f)
        {
            mVerticalSpeed = 0f;
        }

        if (mJumpUp && !mOnceJump)
        {
            mJumpKeyPressed = 0;
            mOnceJump = true;
        }

        if (mOnGround && mJumpDown)
        {
            if (mJumpKeyPressed < .25f)
            {
                if (mJumpTimer <= .15f)
                    mJumpCounter++;
                else
                {
                    mJumpCounter = 1;
                }

                if (mJumpCounter > 3)
                    mJumpCounter = 1;

                mVerticalSpeed = mJumpSpeed * (0.4f * (mJumpCounter + 1));
                mAnimator.SetInteger("NextJump", mJumpCounter);
                mJumpDown = false;
            }
            else
            {
                mJumpCounter = 4;
                mVerticalSpeed = mJumpSpeed * (0.25f * (mJumpCounter + 1));
                mOnceLongJump = true;
                mAnimator.SetInteger("NextJump", mJumpCounter);
            }

            mJumpDown = false;
            mJumpUp = false;
            mOnceJump = false;
        }

        if (mOnGround && mPunch)
        {
            if (mTake)
            {
                mPunchCounter = 0;
                var child = mTakePosition.GetChild(0);
                child.parent = null;
                mTake = false;
                child.GetComponent<Rigidbody>().isKinematic = false;
                child.GetComponent<Shell>().AddForce(transform.forward * 50000 * Time.deltaTime);
            }

            mAnimator.SetBool("OnPunch", true);
            mAnimator.SetInteger("NextPunch", mPunchCounter);
        }


        mAnimator.SetFloat("Speed", l_HasMovement ? (lSpeed == mRunSpeed ? 1.0f : 0.5f) : 0.0f);
        mAnimator.SetBool("OnGround", mOnGround);
    }

    public void NextPunch()
    {
        mPunchCounter++;
        if (mPunchCounter >= 3)
            mPunchCounter = 0;
    }

    private void OnTriggerEnter(Collider other)
    {
        switch (other.tag)
        {
            case "DeadZone":
                Die();
                break;
            case "Platform":
                if (mCurrentPlatform == null)
                    AttachPlatform(other.transform);
                break;
            case "Item":
                other.gameObject.GetComponent<Item>().UseItem();
                if (other.gameObject.name.Contains("Coin"))
                {
                    mAudioSource.clip = mCoin;
                    mAudioSource.Play();
                }
                else if (other.gameObject.name.Contains("Life"))
                {
                    mAudioSource.clip = mLifeClip;
                    mAudioSource.Play();
                }

                break;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Shell"))
            if (mTake)
            {
                other.transform.parent = mTakePosition;
                other.transform.GetComponent<Rigidbody>().isKinematic = true;
                other.transform.localPosition = Vector3.zero;
            }
    }

    public void OnControllerColliderHit(ControllerColliderHit hit)
    {
        switch (hit.collider.tag)
        {
            case "Bridge":
                Rigidbody lBridge = hit.collider.attachedRigidbody;
                lBridge.AddForceAtPosition(-hit.normal * mBridgeForce, hit.point);
                break;
            case "Enemy":
                mEnemy = hit.collider.transform.parent.parent.gameObject;
                if (CanKillWithFeet())
                {
                    hit.collider.transform.parent.parent.GetComponent<Enemy>().Kill();
                    JumpOverEnemy();
                }

                break;
            case "Shell":
                if (hit.collider.transform.parent.GetComponent<Rigidbody>().velocity == Vector3.zero)
                {
                    hit.collider.transform.parent.GetComponent<Shell>()
                        .AddForce(transform.forward * 50000 * Time.deltaTime);
                }
                else
                {
                    AddLife(-1);
                }

                break;
            default:
                if (!mOnGround && mJumpDown && !mOnce && hit.normal.y < 0.1f)
                {
                    mAnimator.SetInteger("NextJump", 5);
                    mOnce = true;
                    mVerticalSpeed = mJumpSpeed;
                    transform.forward = hit.normal;
                    mWallJump = hit.normal;
                    transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
                    mJumpDown = false;
                    mJumpUp = false;
                }

                break;
        }
    }

    void UpdatePlatform()
    {
        if (mCurrentPlatform != null)
        {
            if (mCurrentPlatform.transform.forward.x != 0)
                DetachPlatform();
        }
    }

    private void AttachPlatform(Transform otherTransform)
    {
        transform.parent = otherTransform;
        mCurrentPlatform = otherTransform.gameObject;
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Platform") && mCurrentPlatform != null)
            DetachPlatform();
    }

    private void DetachPlatform()
    {
        transform.parent = null;
        mCurrentPlatform = null;
    }

    public int GetLife()
    {
        return (int) mLife;
    }

    public int GetMaxLife()
    {
        return (int) mMaxLife;
    }

    public void AddLife(int lifePoints)
    {
        if (lifePoints < 0)
            mAnimator.SetBool("Damage", true);
        mLife += lifePoints;
        mLifeUI.fillAmount = mLife / mMaxLife;
        if (mLife <= 0)
            mAnimator.SetBool("Die", true);
        else
        {
            mGameController.EnableUI();
        }
    }


    private bool CanKillWithFeet()
    {
        return mAnimator.GetCurrentAnimatorClipInfo(0)[0].clip.name.Equals("fall");
    }

    public void JumpOverEnemy()
    {
        mVerticalSpeed = mJumpSpeed;
    }

    public void Die()
    {
        SceneManager.LoadScene(SingletonMaster.getInstance().getLife() == 0 ? "GameOverFinal" : "GameOver");
    }

    public void NoDamage()
    {
        mAnimator.SetBool("Damage", false);
    }

    public void ResetJump()
    {
        mAnimator.SetInteger("NextJump", 0);
    }

    public void FinishPunch()
    {
        mOnePunch = false;
        mAnimator.SetBool("OnPunch", false);
    }

    public void EndSpecialIDLE()
    {
        mAnimator.SetBool("SpecialIDLE", false);
    }

    public void EnableLeftHandPunch(bool lEnableHandPunch)
    {
    }

    public void EnableRightHandPunch(bool lEnableHandPunch)
    {
    }

    public void KillEnemy()
    {
        if (mEnemy != null)
            mEnemy.GetComponent<Enemy>().Kill();
    }

    public void Step()
    {
        mAudioSource.clip = mStep;
        mAudioSource.Play();
    }

    public void Jump1()
    {
        mAudioSource.clip = mJump1;
        mAudioSource.Play();
    }

    public void Jump2()
    {
        mAudioSource.clip = mJump2;
        mAudioSource.Play();
    }

    public void Jump3()
    {
        mAudioSource.clip = mJump3;
        mAudioSource.Play();
    }

    public void LongJumpSound()
    {
        mAudioSource.clip = mLongJump;
        mAudioSource.Play();
    }

    public void PunchSound1()
    {
        mAudioSource.clip = mPunchSound1;
        mAudioSource.Play();
    }

    public void PunchSound2()
    {
        mAudioSource.clip = mPunchSound2;
        mAudioSource.Play();
    }

    public void PunchSound3()
    {
        mAudioSource.clip = mPunchSound3;
        mAudioSource.Play();
    }

    public void HitSound()
    {
        mAudioSource.clip = mHitSound;
        mAudioSource.Play();
    }

    public void DeathSound()
    {
        mAudioSource.clip = mDeathSound;
        mAudioSource.Play();
    }

    public void SpecialIDLESound()
    {
        mAudioSource.clip = mIDLESound;
        mAudioSource.Play();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (mLife > 0)
            mMovement = context.ReadValue<Vector2>();
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        mCameraController.GetComponent<CameraController>().mMovement = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (mLife > 0)
            if (context.performed && !mJumpUp)
                mJumpUp = true;
            else if (context.canceled)
                mJumpDown = true;
    }

    public void OnPunch(InputAction.CallbackContext context)
    {
        if (mLife > 0)
            if (context.performed)
                mPunch = true;
            else if (context.canceled)
                mPunch = false;
    }

    public void OnRun(InputAction.CallbackContext context)
    {
        if (mLife > 0)
            if (context.performed)
                mRun = true;
            else if (context.canceled)
                mRun = false;
    }

    public void OnTake(InputAction.CallbackContext context)
    {
        if (context.performed)
            mTake = true;
        else if (context.canceled && mTakePosition.childCount == 0)
            mTake = false;
    }
}