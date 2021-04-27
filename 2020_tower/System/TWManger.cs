using System;
using System.Collections;
using System.Collections.Generic;
using PW;
using TW.GameSetting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TWManger : NLSingletonDontDestroyObject<TWManger>
{
    //private static bool isDebug = false;
    [SerializeField] public Transform _dialogRoot;
    [SerializeField] public Transform characterRoot;
    [SerializeField] Camera _dialogCamera;
    //[SerializeField] CinemachineBrain cinemachineBrain;
    [SerializeField] public Canvas tapCanvas;
    [NonSerialized] public List<DialogData> dialogDatas = new List<DialogData>();
    [NonSerialized] protected static PlayType playType = PlayType.Moving;
    [NonSerialized] public ColEventType onTriCharaType;
    [NonSerialized] public SystemBaseManager systemBaseManager;

    public static PlayType PLAYTYPE
    {
        get { return playType; }
    }

    public void Start()
    {
    }

    public void DilalogInit()
    {
        Transform tf = _dialogRoot.transform;
        for (int i = 0; i < tf.childCount; i++)
            if (tf.GetChild(i).gameObject != _dialogCamera.gameObject)
                Destroy(tf.GetChild(i).gameObject);

        dialogDatas.Clear();

    }

    public void OpenDialog()
    {
        SetDialog(onTriCharaType);
    }

}
