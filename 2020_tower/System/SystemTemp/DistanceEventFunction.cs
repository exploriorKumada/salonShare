using System;
using System.Collections;
using System.Collections.Generic;
using TW.GameSetting;
using UnityEngine;
using UniRx;

public class DistanceEventFunction : MonoBehaviour
{
    [SerializeField] public ColEventType eventType;
    [SerializeField] public GameObject tagetObject;
    [SerializeField] public float limitDistance;

    [NonSerialized] public Action enterAction;
    [NonSerialized] public Action exitAction;
    [NonSerialized] public Action<ColEventCase> colEvenEnterAction;
    [NonSerialized] public Action<ColEventCase> colEvenExitAction;

    [NonSerialized] public Action<ColEventFunction> colEvenFuncEnterAction;

    private void Start()
    {
        Observe();
    }

    public void Observe()
    {
        float distance = (transform.localPosition - tagetObject.transform.localPosition).magnitude;

        this.ObserveEveryValueChanged(x => distance)
       .Where(x => true)
       .Subscribe(_ =>
       {
           if(distance >= limitDistance)
           {
               //範囲に入った
               enterAction();
           }
           else
           {
               //範囲から出てる
               exitAction();
           }
       });
    }
}
