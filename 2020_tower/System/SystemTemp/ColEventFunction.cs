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
            //プレイヤー攻撃
            case ColEventCase.PlayerAttack:

                PlayerCharaController playerCharaControllerPlayerAttack = tagetObject.GetComponent<PlayerCharaController>();
 
                if (playerCharaControllerPlayerAttack.charastateType != CharastateType.attack) return;

                Debug.Log("プレイヤー攻撃");

                NPCController npcControllerPlayerAttack = overColEvent.tagetObject.GetComponent<NPCController>();

                //攻撃情報取得
                BattleActionInfo battleActionInfo = new BattleActionInfo
                    (playerCharaControllerPlayerAttack.charaInfo.skillMasters.FirstOrDefault(),
                    playerCharaControllerPlayerAttack
                ) ;

                //プレイヤー攻撃アクション
                playerCharaControllerPlayerAttack.CharaFunction(battleActionInfo);
                ////敵ダメージアクション
                npcControllerPlayerAttack.Damaged(battleActionInfo);

                hitPos = over.ClosestPointOnBounds(this.transform.position);

                ResourceManager.Instance.SetEffect("impact", this.gameObject, ResourceManager.Instance.globalObjectRoot).transform.position = hitPos;

                break;

            case ColEventCase.EnemyAttack:

                NPCController npcControllerEnemyAttack = tagetObject.GetComponent<NPCController>();

                if (npcControllerEnemyAttack.charastateType != CharastateType.attack) return;
              
                var playerCharaControllerEnemyAttack = overColEvent.GetComponent<CharacterBase>();
                if(playerCharaControllerEnemyAttack==null)
                    playerCharaControllerEnemyAttack = overColEvent.tagetObject.GetComponent<CharacterBase>();

                //攻撃情報取得
                BattleActionInfo enemyBattleActionInfo = new BattleActionInfo
                    (npcControllerEnemyAttack.charaInfo.skillMasters.FirstOrDefault(),
                    npcControllerEnemyAttack
                );

                //Debug.Log(npcControllerEnemyAttack.charaInfo.id + "　が攻撃");

                //敵攻撃アクション
                npcControllerEnemyAttack.CharaFunction(enemyBattleActionInfo);
                //プレイヤーダメージアクション
                playerCharaControllerEnemyAttack.Damaged(enemyBattleActionInfo);

                hitPos = over.ClosestPointOnBounds(this.transform.position);

                ResourceManager.Instance.SetEffect("impact", this.gameObject, ResourceManager.Instance.globalObjectRoot,2f).transform.position = hitPos;

                break;

            case ColEventCase.OpenMenu:
                if (overColEvent.cheackPoint != null)
                    overColEvent.cheackPoint.SetActive(true);

                if (eventType == ColEventType.Player)
                {
                    //ダイアログ表示可能状態にする
                    //moveController.canOpemCommand = true;
                    //開くダイアログを設定する
                    TWManger.Instance.onTriCharaType = overColEvent.eventType;
                }

                overColEvent.colEvenFuncEnterAction?.Invoke(over.GetComponent<ColEventFunction>());

                break;

            //プレイヤー発見処理
            case ColEventCase.FoundPlayer:
                //SetDebugLog("Taget見つけたぁ");
                enterAction?.Invoke();
                colEvenFuncEnterAction?.Invoke(over.GetComponent<ColEventFunction>());
                break;
            //敵発見処理
            case ColEventCase.FoundEnemy:
                //Debug.Log("敵を見つけたぞ");
                enterAction?.Invoke();
                break;

            case ColEventCase.EnemyAttackStart:
                //Debug.Log("攻撃範囲に入った");
                enterAction?.Invoke();
                break;

            case ColEventCase.EnableMove:
                if (overColEvent.cheackPoint != null)
                    overColEvent.cheackPoint.SetActive(true);

                if (eventType == ColEventType.Player)
                {
                    //シーン移動状態にする
                    //moveController.canMoveScene = true;
                    //移動するシーンを設定する
                    //PWManger.Instance.onTriCharaType = opponent;
                }

                break;

            case ColEventCase.SettingChara:                
                CharaSettingManager charaSettingManager = overColEvent.tagetObject.GetComponent<CharaSettingManager>();
                charaSettingManager.SetCharaStatus(GetComponent<SetiingCharaController>());
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
            case ColEventCase.PossessionCard:
            case ColEventCase.BattleUseCard:
                colEvenEnterAction?.Invoke(eventCase);
                colEvenFuncEnterAction?.Invoke(over.GetComponent<ColEventFunction>());
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
            case ColEventCase.PlayerAttack:
                break;

            case ColEventCase.EnemyAttack:
                break;

            case ColEventCase.OpenMenu:

                if (overColEvent.cheackPoint != null)
                    overColEvent.cheackPoint.SetActive(false);

                TWManger.Instance.onTriCharaType = ColEventType.None;

                overColEvent.colEvenFuncExitAction?.Invoke(over.GetComponent<ColEventFunction>());

                break;

            case ColEventCase.EnableMove:
                if (overColEvent.cheackPoint != null)
                    overColEvent.cheackPoint.SetActive(false);
                break;

            case ColEventCase.FoundPlayer:
                //SetDebugLog("Taget見失った");
                exitAction?.Invoke();
                colEvenFuncExitAction?.Invoke(overColEvent);
                break;

            case ColEventCase.EnemyAttackStart:
                Debug.Log("攻撃範囲から出た");
                exitAction?.Invoke();
                colEvenFuncExitAction?.Invoke(overColEvent);
                break;

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
            case ColEventCase.PossessionCard:
                colEvenExitAction?.Invoke(eventCase);
       
                break;

            case ColEventCase.BattleUseCard:
                colEvenExitAction?.Invoke(ColEventCase.None);
                break;

        }
    }

}
