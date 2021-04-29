using System;
using GameSetting;
using UnityEngine;

public class ColEventFunction : MonoBehaviour
{
    [SerializeField] public ColEventType eventType;
    [SerializeField] public GameObject cheackPoint = null;
    [SerializeField] public GameObject tagetObject;

    [NonSerialized] public Action enterAction;
    [NonSerialized] public Action exitAction;
    [NonSerialized] public Action<ColEventCase> colEvenEnterAction;
    [NonSerialized] public Action<ColEventCase> colEvenExitAction;

    [NonSerialized] public Action<ColEventFunction> colEventFuncEnterAction;
    [NonSerialized] public Action<ColEventFunction> colEventFuncExitAction;


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
            case ColEventCase.AB:
            case ColEventCase.BC:
            case ColEventCase.AC:
                colEvenEnterAction?.Invoke(eventCase);
                colEventFuncEnterAction?.Invoke(over.GetComponent<ColEventFunction>());
                break;

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
            case ColEventCase.AB:
            case ColEventCase.BC:
            case ColEventCase.AC:
                colEvenExitAction?.Invoke(eventCase);       
                break;


        }
    }

}
