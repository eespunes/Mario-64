using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Item : MonoBehaviour
{
    protected GameController mGameController;

    private void Start()
    {
        mGameController = GameObject.Find("GameController").GetComponent<GameController>();
    }

    public abstract void UseItem();
    protected void DestroyItem()
    {
        Destroy(gameObject);
    }
}
