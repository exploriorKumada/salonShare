using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Explorior;
using TW.GameSetting;
using System;
using UnityEngine.Events;

public class StageManager : MonoBehaviour
{
    [SerializeField] GameObject initObject;
    [SerializeField] GameObject rootObject;

    int amount = 10;
    int lastPosition;

    private GameManager gameManager;

    public void Initialized(GameManager _gameManager)
    {
        //StageMove();
        stageControllers = new List<StageController>();
        id = 0;
        gameManager = _gameManager;
        for (int i = 0;i<amount;i++)
        {
            SetUnityStage();
        }

    }


    int id = 0;
    List<StageController> stageControllers = new List<StageController>();
    public void SetUnityStage()
    {
        bool isFirst = id == 0;
        string objectName = (isFirst ? "2" : UnityEngine.Random.RandomRange(1, 10).ToString());
        var go = Instantiate(Resources.Load<GameObject>("PuzzleCubeObjects/" + objectName), rootObject.transform);
        //go.transform.Rotate(new Vector3(-90, 0, 0));
        go.transform.localPosition = new Vector3(0,0,lastPosition);
        lastPosition = lastPosition + 5;

        //Debug.Log("lastPosition:"+ lastPosition);

        var sc = go.GetComponent<StageController>();


        sc.Initialize(lastPosition != 5, gameManager, id % UnityEngine.Random.RandomRange(1,4) == 0);
        id++;
        sc.id = id;

        go.name = id + ":" + objectName;
        stageControllers.Add(sc);

        var data = go.AddComponent<GeneralData>();
        var coleve = go.GetComponent<ColEventFunction>();

        int _id = id;

        coleve.colEvenEnterAction = (eve) =>
        {
            if (eve != ColEventCase.BlockReach) return;

            if(!data.flg)
            {
 
                data.flg = true;
          
       
                var _sc = new List<StageController>();
                //自分の前までのオブジェクトを消す
                foreach (var Value in stageControllers)
                {
                    if (Value.id >= _id) continue;

                    _sc.Add(Value);                  
                }


                int beforeSatgeCount = gameManager.stageCount;
                foreach (var Value in _sc)
                {
                    if (stageControllers.Contains(Value))
                    {
                        gameManager.stageCount++;
                        stageControllers.Remove(Value);
                        Destroy(Value.gameObject);
                        SetUnityStage();
                    }
                }

                int skipStage = _sc.Count == 0? 0: gameManager.stageCount - beforeSatgeCount - 1;

                Debug.Log("到達した:" + _id + " skipStage:" + skipStage);

                gameManager.AddBournus(skipStage);
            };
        };
    }

    public void StageInit(GameManager _gameManager)
    {
        lastPosition = 0;
        stageControllers = new List<StageController>();
        rootObject.transform.ParentTransInitialize();
        Initialized(_gameManager);
    }
}
