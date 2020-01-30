using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour, Platform.IUIActions
{
    public String scene;

    private Platform mPlatform;

    private void Awake()
    {
        if (!SceneManager.GetActiveScene().name.Equals("Level"))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            mPlatform = new Platform();
            mPlatform.UI.SetCallbacks(this);
        }
    }

    private void OnEnable()
    {
        mPlatform.UI.Enable();
    }

    private void OnDisable()
    {
        mPlatform.UI.Disable();
    }

    public void changeScene()
    {
        SceneManager.LoadScene(scene);
        if (SceneManager.GetActiveScene().name.Equals("GameOver"))
            SingletonMaster.getInstance().SubstractLife();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            changeScene();
        }
    }

    public void OnSubmit(InputAction.CallbackContext context)
    {
        if (context.canceled)
            changeScene();
    }
}