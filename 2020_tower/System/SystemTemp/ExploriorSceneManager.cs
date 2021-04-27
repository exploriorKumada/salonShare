using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExploriorSceneManager : SystemBaseManager
{
    [SerializeField] GameObject rootObj;

    public void CameraActive(bool flg)
    {
        rootObj.SetActive(flg);
    }

}
