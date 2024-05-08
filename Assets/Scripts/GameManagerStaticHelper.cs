using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameManagerStaticHelper
{
    static public GameManager MyGameManager;
    static public Camera MyMainCamera;

    static public GameManager GetGameManager()
    {
        if (MyGameManager == null)
        {
            MyGameManager = GameObject.FindGameObjectWithTag("GameManager")?.GetComponent<GameManager>();
        }


        return MyGameManager;
    }

    static public Camera GetMainCamera()
    {
        GetGameManager();
        MyMainCamera = MyGameManager.GetMainCamera();
        return MyMainCamera;
    }
}
