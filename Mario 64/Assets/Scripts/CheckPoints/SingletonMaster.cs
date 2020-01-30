using UnityEngine;

public class SingletonMaster
{
    private Vector3 mPosition;
    private static SingletonMaster _mSingletonMaster;
    private int mLife;
    private int mCoins;

    private SingletonMaster()
    {
        mLife = 7;
        mCoins = 0;
        mPosition = new Vector3(27, 10.25f, 28.5f);
    }

    public static SingletonMaster getInstance()
    {
        if (_mSingletonMaster == null)
            _mSingletonMaster = new SingletonMaster();
        return _mSingletonMaster;
    }

    public Vector3 getPosition()
    {
        return mPosition;
    }

    public void setPosition(Vector3 vector3)
    {
        mPosition = vector3;
    }

    public int getLife()
    {
        return mLife;
    }
    public void SubstractLife()
    {
        mLife--;
    }
    public void AddCoin()
    {
        mCoins++;
    }
    public int GetCoins()
    {
        return mCoins;
    }
}