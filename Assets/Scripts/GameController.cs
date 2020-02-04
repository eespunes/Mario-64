using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public PlayerController mPlayerController;

    public Animator mUI;

    private void Start()
    {
        EnableUI();
    }

    public void AddCoin()
    {
        SingletonMaster.getInstance().AddCoin();
    }

    public void EnableUI()
    {
        mUI.SetBool("Enable", true);
        Invoke("DisableUI", 3f);
    }

    public void DisableUI()
    {
        mUI.SetBool("Enable", false);
    }
}