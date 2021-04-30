using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Explorior;
using System;
using UnityEngine.Events;
using TW.GameSetting;
using System.Linq;

public class ColEventFunction : SystemBaseManager
{
    [SerializeField] public ColEventType eventType;
    [SerializeField] public GameObject cheackPoint = null;
    [SerializeField] public GameObject tagetObject;

    [NonSerialized] public Action enterAction;
    [NonSerialized] public Action exitAction;
    [NonSerialized] public Action<ColEventCase> colEvenEnterAction;
    [NonSerialized] public Action<ColEventCase> colEvenExitAction;

    [NonSerialized] public Action<ColEventFunction> colEvenFuncEnterAction;
    [NonSerialized] public Action<ColEventFunction> colEvenFuncExitAction;

    private void Start()
    {
        if (cheackPoint != null)
        {
            UI.UIdirectionAnimation(cheackPoint.transform, 3, 0.2f);
            UI.UIRoateRoop(cheackPoint.transform);

            cheackPoint.SetActive(false);
        }

    }

    void OnTriggerEnter(Collider over)
    {
        //当たり判定クラス取得
        var overColEvent = over.GetComponent<ColEventFunction>();

        //相手に当たり判定クラスがなければガード
        if (overColEvent == null) return;

        if (overColEvent.enabled == false || this.enabled == false) return;

        //自分と相手の当たり判定タイプを照らし合わせてイベントタイプを取得
        var eventCase = EventTypeSetting.GetColEventCase(eventType, overColEvent.eventType);

        //イベントが設定されていなければガード
        if (eventCase == ColEventCase.None) return;

        Vector3 hitPos;

        switch (eventCase)
        {
            case ColEventCase.Touch:
                enterAction?.Invoke();
                break;
            case ColEventCase.KeyTouch:
                colEvenFuncEnterAction?.Invoke(overColEvent);
                break;
   
         
            default:

                break;
        }
    }


    public void OnTriggerEnter2D(Collider2D over)
    {
        var overColEvent = over.GetComponent<ColEventFunction>();

        if (overColEvent == null) return;

        var eventCase = EventTypeSetting.GetColEventCase(eventType, overColEvent.eventType);

        if (eventCase == ColEventCase.None) return;

        //Debug.Log("eventCase:" + eventCase);
        switch (eventCase)
        {
  

        }
    }


    private void OnTriggerExit(Collider over)
    {
        var overColEvent = over.GetComponent<ColEventFunction>();

        if (overColEvent == null) return;

        var eventCase = EventTypeSetting.GetColEventCase(eventType, overColEvent.eventType);

        if (eventCase == ColEventCase.None) return;

        switch (eventCase)
        {
          
            default:

                break;
        }
    }


    private void OnTriggerExit2D(Collider2D over)
    {
        if (over.GetComponent<ColEventFunction>() == null)
            return;

        var overColEvent = over.GetComponent<ColEventFunction>();

        if (overColEvent == null)
            return;

        var opponent = overColEvent.eventType;

        var eventCase = EventTypeSetting.GetColEventCase(eventType, opponent);

        if (eventCase == ColEventCase.None)
            return;

        switch (eventCase)
        {

        }
    }

}
