using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class CoinItem : Item
{
    public override void UseItem()
     {
         mGameController.AddCoin();
         DestroyItem();
     }
 }