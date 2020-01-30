using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeLife : MonoBehaviour
{
    public Text mText;

    // Update is called once per frame
    void Update()
    {
        mText.text = SingletonMaster.getInstance().getLife().ToString();
    }
}
