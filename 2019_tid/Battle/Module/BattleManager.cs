using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using System.Linq;
using System;

public class BattleManager : ScenePrefab
{

    [SerializeField] public TeamCharacterController teamCharacterController;
    [SerializeField] public EnemyController enemyController;
    //[SerializeField] CharaController charaController;
    [SerializeField] LeaderCharaController leaderCharaController;
    [SerializeField] BuffDebuffController buffDebuffController;
    [SerializeField] BattleLayoutManager battleLayoutManager;
    [SerializeField] DiceSelectManager diceSelectManager;
    [SerializeField] DiceManager diceManager;
    [SerializeField] Transform effectTextControllerTF;
    [SerializeField] CharaController charaController;

    [SerializeField] GameObject autoStatusText;
    [SerializeField] TextMeshProUGUI stageNumberText;

    public RealQuestDetail realQuestDetail;
    public QuestStartInfo questStartInfo;

    public bool debugmode = false;

    public int stage = 1;
    public int step = 1;
    public Transform thisTF;
    public BattleManager battleManager;
    private int waveMacCount;
    public int turn = 1;
    public bool autoFlag = false;
    public new Dictionary<int, Sprite> charaSpriteDic = new Dictionary<int, Sprite>();

    [HideInInspector]
    public bool autoStartEnable = true;

    

    //must init
    public static int questId;
    public static int teamNo = 0;
    public static RealCharaData selectFriendRealCharaData;

    public bool actionFlag = false;

    LeaderCharaSettingBase leaderCharaSettingBase;
    public bool leaderSkillSetFlag = false;

    //ミッション用
    public int deadCount = 0;

    public List<RealCharaData> playerParty;

    public bool enemyReady;


    public enum BattleStatus : int
    {
        PlayerActionBattleStatus = 0,
        PlayerAttackBattleStatus = 1,
        EnemyActionBattleStatus = 2
    }
    public BattleStatus battleStatus = BattleStatus.PlayerActionBattleStatus;

    public void InitStart()
    {
        Debug.Log("questDetailID:" + realQuestDetail.quest_detail_id);
        Debug.Log("questType:" + realQuestDetail.type);
        Debug.Log("GameSettingAPI.enemyAttackAjust:" + GameSettingAPI.enemyAttackAjust);
        Debug.Log("GameSettingAPI.enemyHpAjust:" + GameSettingAPI.enemyHpAjust);

   
        step = 1;
        battleManager = this;
        thisTF = transform;
        teamCharacterController.Init();


        if (UserData.GetUserName() != "")
        {
            questStartInfo = QuestStartAPISetting.questStartInfo;
            leaderCharaSettingBase = CharaAPISetting.leaderCharaDataList[UserData.GetLeaderChara()];
            leaderCharaSettingBase.realCharaData = BattleCharaDataSetting.CreateData(CharaAPISetting.GetCharaMasterDataByLeaderId(leaderCharaSettingBase.No).id, leaderCharaSettingBase.attack, leaderCharaSettingBase.guard, leaderCharaSettingBase.cri);
            leaderCharaController.Init(leaderCharaSettingBase);

        }
        else
        {
            questStartInfo = TutorialData.questStartInfo;
            //leaderCharaSettingBase = TutorialData.leaderCharaDataList[1];

        }

  
        autoStartEnable = true;
        waveMacCount =questStartInfo.battleWaveInfos.Count;
        //story corosseum emergency
        enemyController.questId = questId;            
        enemyController.InitStart();
        battleLayoutManager.Init();
        diceManager.Init();

        battleLayoutManager.BattleStartEffect(()=>
        {
            MyCharaStatusInit(); 
            UserData.TutoSet(UserData.TutoType.battle);
            Debug.Log("UserData.GetBaisokuFlag():" + UserData.GetBaisokuFlag());
             if(UserData.GetBaisokuFlag())
            {
                battleLayoutManager.SetBaisoku();
            }

            Debug.Log("UserData.GetAUTOFlag():" + UserData.GetAUTOFlag());
            if (UserData.GetAUTOFlag())
            {
                AutoOn();
            }
        });

    }

    public void BattleAssetInstall( System.Action action)
    {
        List<int> realCharaDataIDs = new List<int>();

        string installchaaID = "";
        foreach (var Value in playerParty)
            if(Value!=null)
                realCharaDataIDs.AddToNotDuplicate(Value.charaIdNumber);
            
        foreach (var KV in QuestStartAPISetting.questStartInfo.battleWaveInfos)
        {
            foreach (var Value2 in KV.Value.battleWaveEnemyInfos)
            {
                realCharaDataIDs.AddToNotDuplicate(Value2.enemy_character_id);
            }
        }

       realCharaDataIDs.AddToNotDuplicate(selectFriendRealCharaData.charaIdNumber);
       realCharaDataIDs.RemoveDuplicate();

        foreach (var Value in realCharaDataIDs)
            installchaaID += Value+",";

        Debug.Log("installchaaID:"+ installchaaID);
       ResourceLoaderOrigin.InstallBattleCharaImage(realCharaDataIDs,(spriteDic)=>            
        {
           
            charaSpriteDic = spriteDic;
            ResourceLoaderOrigin.InstallVoice(realCharaDataIDs, () =>
            {
                Debug.Log("BattleAssetInstall完了");
                action();
            });
        });

    }



    /// <summary>
    /// 味方攻撃開始
    /// </summary>
    /// <param name="diceNumber">Dice number.</param>
    public void MyAttackStart(int diceNumber){StartCoroutine(MyAttackStartOn(diceNumber));}
    private IEnumerator MyAttackStartOn(int diceNumber)
    {
        //Debug.Log("MyAttackStartOn========================-:" + turn);
        battleStatus = BattleStatus.PlayerAttackBattleStatus;
        actionFlag = true;
        battleLayoutManager.EnableActionButtons(false);
        //buffDebuffController.CheackBuffDebuff();
        battleLayoutManager.MoveRightUI(true,()=>{});            

    

        yield return new WaitForSeconds(0.2f);

        charaController.charaNumberList.Clear();
        diceManager.enableAction = true;
        //スキル状態
        foreach (var Value in teamCharacterController.LiveList())
        {
            //もしオートで溜まってたらskillにする
            if (autoFlag && Value.SkillMaxGuage())
                Value.realCharaData.actionType = "skill";

            if (Value.realCharaData.actionType == "skill")
                charaController.charaNumberList.Add(Value.realCharaData);
        }

        List<RealActionData> thisGetAttackInfo = new List<RealActionData>();
        thisGetAttackInfo = teamCharacterController.GetAttackInfo(diceNumber);
        //スキルアニメーション完了まで待たせる
        if ( charaController.charaNumberList.Count > 0 )
        {
            charaController.SetImageOn(() =>
            {
                PlayerAction(diceNumber);
            });

        }else
        {
            PlayerAction(diceNumber);
        }
    }

    public void PlayerAction( int diceNumber)
    {
        //Debug.Log("MyAttackActionStart========================-");
        teamCharacterController.ActionStart(diceNumber, LeaderAction);

    }

    public void LeaderAction()
    {
        if (teamCharacterController.LiveList().Count == 0)
            return;

        if(UserData.GetUserName()=="")
        {
            if (!enemyController.allDeadFlag)
            {
                StartCoroutine(EnemyAttackStart(UnityEngine.Random.Range(1, 7), UIInitCheack));
            }
            return;
        }

        //全員完了した際の処理
        leaderCharaController.ActionMove();
        RealActionData realActionData = CharaSetting.ConvertRealActionData(1, 1, 1, 0, 0, 7);
        realActionData.lastAttackFlag = true;
        TeamCharacterUnit teamCharacterUnit = new TeamCharacterUnit();
        teamCharacterUnit.realCharaData = leaderCharaSettingBase.realCharaData;
        teamCharacterController.AttackPaturn(realActionData, teamCharacterUnit, () =>
        {
            if (!enemyController.allDeadFlag)
            {
                StartCoroutine(EnemyAttackStart(UnityEngine.Random.Range(1, 7), UIInitCheack ));
            }
        });
    }


    public void MyAttackEnd()
    {
        step++;

        Debug.Log("(battleManager.step:" + battleManager.step  + " waveMacCount:" + waveMacCount);
        if (battleManager.step <= waveMacCount )
        {
            SeTextEffectOn("YOU", "WIN");               
            enemyController.InitStart();
            StartCoroutine(TurnInit());                   
        }
        else
        {
            Debug.Log("クエストクリア");
            StartCoroutine(BatttleComplete());
        }
    }


    /// <summary>
    /// バトル完了処理
    /// </summary>
    /// <returns>The complete.</returns>
    private IEnumerator BatttleComplete()
    {
        yield return new WaitForSeconds(1f);
        BgmManager.Instance.Play("11.Win");
       
        battleLayoutManager.ClearEffect(() =>
        {
            BattleResult();
        });
        Singleton<SoundPlayer>.instance.playSE(GameSetting.SEType.Win);
    }



    /// <summary>
    /// 敵攻撃開始
    /// </summary>
    /// <returns>The attack start.</returns>
    /// <param name="diceNumber">Dice number.</param>
    private IEnumerator EnemyAttackStart(int diceNumber, Action action)
    {
        if(battleStatus == BattleStatus.EnemyActionBattleStatus)
        {
            yield break;
        }

        //Debug.Log(action +"EnemyAttackStart========================-:" + turn);
        battleStatus = BattleStatus.EnemyActionBattleStatus;
        //buffDebuffController.CheackBuffDebuff();
        //number のアタック内容をList取得する
        yield return new WaitForSeconds(0.5f);
        enemyController.ActionStart(diceNumber, () =>
        {
            action();
        });
        //Debug.Log("enemy attack end");
    }


    public IEnumerator TurnInit()
    {
        //Debug.Log("TurnInit"+ turn);
        CameraSetting.CameraClear();
        buffDebuffController.CheackBuffDebuff();
        MyCharaStatusInit();
        leaderCharaController.TurnInit();
        teamCharacterController.TurnInit();
        turn++;
        yield break;
    }

    public void UiInit()
    {
        actionFlag = false;
        autoStartEnable = true;
        if (!autoFlag)
        {
            battleLayoutManager.EnableActionButtons(true);
        }
        else
        {
            //Debug.Log("UiInit=>DiceStartOn");
            battleLayoutManager.EnableActionButtons(false);
            diceManager.DiceStartOn();
        }

    }


    public void MyCharaStatusInit()
    {
        foreach (var Value in teamCharacterController.LiveList())
        {
            Value.SetButtonAction(3);
        } 
    }


	private void Gaurd(string type, int charaCount)
	{
        
	}

    /// ダイス確定
    /// </summary>
    /// <param name="setNumber">Set number.</param>
    public void DiceNumberEffet( int setNumber )
    {
        diceManager.SetEffect(14);
        diceManager.DiceEffected(setNumber);        
    }

    void Update()
    {
        if (step > waveMacCount)
            step = waveMacCount;
        
        stageNumberText.text = "WAVE " + step + " / " + waveMacCount;
        SetBuddDebuffImage();
    }

    /// <summary>
    /// バフデバフ表示
    /// </summary>
    public void SetBuddDebuffImage()
    {
        teamCharacterController.BuffDebuffSetimage();
        enemyController.BuffDebuffSetimage();
        
    }
    /// <summary>
    /// カットインテキスト
    /// </summary>
    /// <param name="value1">Value1.</param>
    /// <param name="value2">Value2.</param>
	public void SeTextEffectOn( string value1, string value2 ){	battleManager.StartCoroutine( SeTextEffect( value1, value2 ) );	} 
	public  IEnumerator SeTextEffect( string value1, string value2 )
	{
        effectTextControllerTF.Find("Animator").gameObject.SetActive(true);
        effectTextControllerTF.Find("Animator/textValue/text1").GetComponent<TextMeshPro>().text = value1;
        effectTextControllerTF.Find("Animator/textValue/text2").GetComponent<TextMeshPro>().text = value2;
		yield return new WaitForSeconds(2f);

        effectTextControllerTF.Find("Animator").gameObject.SetActive(false);

	}

    /// <summary>
    /// キャラ情報表示
    /// </summary>
    public void InfoClick()
    {
        if (actionFlag)
            return;
        
        List<RealCharaData> charaIdList = new List<RealCharaData>();
        
        foreach( var Value in teamCharacterController.teamCharacterUnitList )
        {
            charaIdList.Add(Value.realCharaData);
        }

        AllCharaInfo.charaList = charaIdList;
        AllCharaInfo.realQuestDetail = realQuestDetail;
        AddPopup("Popup_AllCharaInfo");
    }


    /// <summary>
    /// ゲームオーバ処理
    /// </summary>
	public void LoseAciton()
	{
        Debug.Log("LoseAciton");
        Singleton<SoundPlayer>.instance.CharaVoice(CharaAPISetting.GetCharaMasterDataByLeaderId(leaderCharaSettingBase.No).id, "lose", true);
        BgmManager.Instance.Stop();
        Singleton<SoundPlayer>.instance.playSE(GameSetting.SEType.Lose);
        battleLayoutManager.BattleEndEffect(() =>
        {
            if( UserData.GetCrystalNumber() >= 40 )
            {
                battleLayoutManager.popup_BattleMenu.SetImage(2);
                battleLayoutManager.popup_BattleMenu.gameObject.SetActive(true);
                battleLayoutManager.EnableActionButtons(true);
            }
            else
            {
                ChangeLayout("SelectQuest");
            }          
        });
		
	}

    /// <summary>
    /// コンテニュー
    /// </summary>
    public void Continue()
    {
        Debug.Log("Continue");
        BgmManager.Instance.Play("05.Battle");
        //仲間全員を生きれらせて、スキルゲージマックスにする
        teamCharacterController.Continue();
        battleLayoutManager.EnableActionButtons(true);

        autoFlag = true;
        AutoOn();
        autoStartEnable = true;

        if (UserData.GetBaisokuFlag())
            battleLayoutManager.SetBaisoku();
    }

    /// <summary>
    /// オート
    /// </summary>
    public void AutoOn()
    {
        Debug.Log( "enemyTurn:"+ autoStartEnable + " autoFlag:" + autoFlag);
        if( autoFlag )
        {
            //auto off
            autoFlag = false;
            autoStatusText.SetActive(false);
            if( !autoStartEnable )
            {
                battleLayoutManager.EnableActionButtons(false); 
            }else
            {
                battleLayoutManager.EnableActionButtons(true); 
            }
            UserData.SetAUTOFlag(false);

        }else
        {
            //auto on 
            battleLayoutManager.EnableActionButtons(false);
            autoFlag = true;
            autoStatusText.SetActive(true);
            Debug.Log(autoStartEnable);
            UserData.SetAUTOFlag(true);
            if( autoStartEnable )
            {
                diceManager.DiceStartOn();
            }   
            else
            {
                Debug.Log(" you can not start auto ");
            }
        }
       
    }


    /// <summary>
    /// ダイスボーナスによる回復
    /// </summary>
    /// <param name="amountByDice">Amount by dice.</param>
    public void DiceEffectAction( RealActionData realActionData, Action action)
    {
        if(realActionData.type == 2)
        {
            teamCharacterController.RepairAction(realActionData, action);
        }else if(realActionData.type == 3)
        {
            teamCharacterController.BuffAction(realActionData, action);
        }
       
    }



    /// <summary>
    /// バトル結果処理
    /// </summary>
    public void BattleResult()
    {

        if(UserData.GetUserName() == "")
        {

            FirstDL(() =>
            {
                Debug.Log("FirstDL開始");
                AlphaLoding.timeOutTime = 60;
                AddPopup("Popup_AlphaLoding");
                ResourceLoaderOrigin.LoadAssetList(() =>
                {
                    Debug.Log("LoadAssetList完了");
                    ResourceLoaderOrigin.GetEfectAll(() => { Debug.Log("ResourceLoaderOrigin.GetEfectAll完了"); });
                    AlphaLoding.Close();
                    ChangeLayout("UserCreate");
                });
            });
       
            return;
        }

        AddPopup("Popup_Loding");

        int misstion_clear_status = 2;
        if (deadCount == 0)
            misstion_clear_status = 1;
        QuestStartAPISetting.QuestFinish(realQuestDetail.quest_detail_id, questStartInfo.quest_history_id,misstion_clear_status, () =>
        {
            Loading.Close();
            battleLayoutManager.popup_CompleteBattle.questStartInfo = questStartInfo;
            battleLayoutManager.popup_CompleteBattle.realQuestDetail = realQuestDetail;
            battleLayoutManager.popup_CompleteBattle.misstionClearFlag = (deadCount ==0);
            battleLayoutManager.popup_CompleteBattle.gameObject.SetActive(true);
        });
    }

    private void UIInitCheack()
    {
        DOVirtual.DelayedCall(1f, () =>
        {           
            Debug.Log("next go:" + turn);

            if (turn % 5 == 10)
            {
                GC.Collect();
                Resources.UnloadUnusedAssets();
            }
               

            battleStatus = BattleStatus.PlayerActionBattleStatus;
            UiInit();
            StartCoroutine(TurnInit());
        });
       
    }

    public void AllDeath()
    {
        enemyController.AllDeadAction();
    }


}
