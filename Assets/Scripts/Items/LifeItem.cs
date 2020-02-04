using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class LifeItem : Item
{
    public int mLifePoints;
    public override void UseItem()
    {
        if (mGameController.mPlayerController.GetLife() <
            mGameController.mPlayerController.GetMaxLife())
        {
            mGameController.mPlayerController.AddLife(mLifePoints);
            DestroyItem();
        }
    }
}