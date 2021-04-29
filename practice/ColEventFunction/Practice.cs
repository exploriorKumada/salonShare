using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Practice : MonoBehaviour
{
    [SerializeField] ColEventFunction colA;
    [SerializeField] ColEventFunction colB;
    [SerializeField] ColEventFunction colC;

    void Start()
    {
        colA.colEvenEnterAction = (col) =>
        {
            if (col == GameSetting.ColEventCase.AB)
            {
                Debug.Log("Aからみて AとBがぶつかった時の処理");
            }
            else if (col == GameSetting.ColEventCase.AC)
            {
                Debug.Log("Aからみて　AとCがぶつかった時の処理");
            }
         
        };


        colA.colEventFuncEnterAction = (colFun) =>
        {
            if(colFun.eventType == GameSetting.ColEventType.B)
            {
                Debug.Log(colFun.gameObject.transform.localPosition);

                colFun.tagetObject.SetActive(false);
            }
        };

        colA.colEvenExitAction = (col) =>
        {
            if (col == GameSetting.ColEventCase.AB)
            {
                Debug.Log("Aからみて AとBが離れた時の処理");
            }
            else if (col == GameSetting.ColEventCase.AC)
            {
                Debug.Log("Aからみて　AとCが離れた時の処理");
            }

        };
    }
}
