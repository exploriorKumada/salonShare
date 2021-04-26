using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;
using DG.Tweening;
using System;

public class EnemyController : ScenePrefab {

    [SerializeField] BattleManager battleManager;
    [SerializeField] BuffDebuffController buffDebuffController;
    [SerializeField] BattleDataManager battleDataManager;
    [SerializeField] BattleLayoutManager battleLayoutManager;
    [SerializeField] TeamCharacterController teamCharacterController;
    [SerializeField] DiceSelectManager diceSelectManager;

    private CharaStatus charaStatus;
	public List<EnemyUnit> enemyUnitList = new List<EnemyUnit> ();
    public Entity_CoroseumSetting coroseumSetting;
    public List<string> effectNameList = new List<string>();

    [HideInInspector] public int questId;


	public void InitStart()
	{

		//BuffDebuffController.enemyDataList.Clear ();

		charaStatus = Resources.Load ("Data/CharaStatus") as CharaStatus; //=> Resourcesからデータファイルの読み込み

		StartCoroutine( SetImage() );
	}

	
	private IEnumerator SetImage()
	{

		enemyUnitList.Clear();
		allDeadFlag = false;

		var charaBase = transform.Find("EnemyBase").gameObject;
		var charaTF = transform.Find("EnemyParent");


		//今あるやつ全部消す
		for( int i=0; i < charaTF.childCount; ++i )
            Destroy(charaTF.GetChild(i).gameObject);

        Debug.Log(battleManager.step + "wave目の敵");
        BattleWaveInfo battleWaveInfo =battleManager.questStartInfo.battleWaveInfos[battleManager.step];
        int enemyCount = battleWaveInfo.battleWaveEnemyInfos.Count;
		float enemyFirstPosition = charaBase.transform.localPosition.x - ( (enemyCount-1) * 240f );

        int count = 0;
        List<BattleWaveEnemyInfo> battleWaveEnemyInfos = battleWaveInfo.battleWaveEnemyInfos;
        foreach( var Value in battleWaveEnemyInfos)
        {
            bool bossFlag = false;
            RealCharaData realCharaData = new RealCharaData();
            realCharaData.enemyFlag = true;
            realCharaData.charaIdNumber = Value.enemy_character_id;
            realCharaData.lv = Value.enemy_lv;

            if (UserData.GetUserName() == "")
            {
                realCharaData.realCharaMasterData = Value.realCharaMasterData;
            }else
            {
                if (CharaAPISetting.realCharaMasterDatas.ContainsKey(realCharaData.charaIdNumber))
                {
                    realCharaData.realCharaMasterData = CharaAPISetting.realCharaMasterDatas[realCharaData.charaIdNumber];
                }
                else
                {
                    Debug.LogError("realCharaData.charaIdNumber is nothing:" + realCharaData.charaIdNumber);
                }
            }
     
            realCharaData.realAttack = (int)(realCharaData.GetRealAttack() * GameSettingAPI.enemyAttackAjust);
            realCharaData.realHP = (int)(realCharaData.GetRealHP() * GameSettingAPI.enemyHpAjust);
            realCharaData.charaTypeId = realCharaData.realCharaMasterData.type;
            realCharaData.realGuard = realCharaData.GetRealGuard();
            realCharaData.realMaxHP = realCharaData.realHP;
            if( battleManager.step  == battleManager.questStartInfo.waveMaxCount)
                bossFlag = true;
 
            var newGO = GameObject.Instantiate(charaBase, charaTF);
            Transform newGoTF = newGO.transform;

            newGoTF.localPosition = new Vector3(enemyFirstPosition + (count * 480), newGoTF.localPosition.y, newGoTF.localPosition.z);
            newGO.name = ""+realCharaData.charaIdNumber;
            newGoTF.Find("Animator/type").gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>("Scenes/Image/UI/" + realCharaData.realCharaMasterData.type);
            if (battleManager.charaSpriteDic.ContainsKey(realCharaData.charaIdNumber))
            { 
                  newGoTF.Find("Animator/charaImage").gameObject.GetComponent<Image>().sprite = battleManager.charaSpriteDic[realCharaData.realCharaMasterData.id];
           
            }else
            {
                Debug.LogError("arimasen:" + realCharaData.charaIdNumber);

				if (UserData.GetUserName() == "")
				{
					newGoTF.Find("Animator/charaImage").gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>("Tuto/tuto_" + realCharaData.charaIdNumber);
				}
				else
				{
					ResourceLoaderOrigin.GetBattleCharaImage(realCharaData.charaIdNumber, (obj) =>
					{
						newGoTF.Find("Animator/charaImage").gameObject.GetComponent<Image>().sprite = obj;

					});
				}
				
            }

              

            newGoTF.Find("Animator").GetComponent<Animator>().Play("enemyAappear", 0, 0);

            EnemyUnit enemyunit = newGoTF.GetComponent<EnemyUnit>();
            enemyunit.realCharaData = realCharaData;

            enemyUnitList.Add(enemyunit);
            newGO.SetActive(true);			
            newGoTF.Find("Animator").gameObject.SetActive(false);

            bool flag = false;
            if (count == enemyCount-1){ flag = true;}
            //Debug.Log(count + " wwwww " + enemyCount);
            enemyUnitList[count].bossFlag = bossFlag;
            enemyUnitList[count].Init(1f + (count * 0.2f),flag);
            count++;

		}
			
		charaBase.SetActive(false);

		yield return null;

	}

    public void ActionStart(int diceNumber, Action action)
    {
        StartCoroutine(ActionStartOn(diceNumber, action));
    }
    public IEnumerator ActionStartOn(int diceNumber,Action action )
    {
        //Debug.Log("EnemyAttackStart========================-:" + battleManager.turn);

        if (battleManager.battleStatus != BattleManager.BattleStatus.EnemyActionBattleStatus)
            yield break;


        foreach ( var Value in LiveList())
        {
            Value.ActionMove();
            RealActionData _realActionData = Value.realCharaData.realCharaMasterData.realActionDataDic[diceNumber];
            var realActionData = _realActionData.GetNewRealActionData(_realActionData);
            realActionData.lastAttackFlag = (Value == LiveList().Last());

            Action comp = null;

            if (!realActionData.lastAttackFlag)
            {
                comp = null;
            }else
            {
                comp = action;
                //Debug.Log( realActionData.type + "こやつが最後：" + Value.name);
            }
               
            AttackPaturn(realActionData, Value, comp);
            yield return new WaitForSeconds(0.3f);
        }
    }


    public void RepairAction(RealActionData realActionData,Action action)
    {
        StartCoroutine(RepairActionOn(realActionData, action));
    }
    public IEnumerator RepairActionOn( RealActionData realActionData, Action action)
    {
        if (realActionData.type == 7)
        {
            EnemyUnit enemyUnitAtRandom = LiveList().GetAtRandom();
            RepairUnitAction(enemyUnitAtRandom, realActionData.amount);

            if(realActionData.lastAttackFlag)
            {
                action();
            }

            yield break;
        }

        foreach (var Value in LiveList())
        {
            if ((Value.realCharaData.charaTypeId == realActionData.range || realActionData.range == 4) && Value.realCharaData.liveFlag)
            {
                //回復するキャラのトータルHPを取得して、そのamaount%回復させる
                int repairNumber = (int)(Value.realCharaData.realMaxHP * realActionData.amount);
                repairNumber = (int)(repairNumber * UnityEngine.Random.Range(1f, 1.3f));
                Value.SetHp(repairNumber);
                Value.TextSetOn("<color=#00F9A9>" + repairNumber);
                Value.SetEffect(realActionData);
                
                yield return new WaitForSeconds(0.1f);
            }

            if (Value == LiveList().Last() && action!=null)
                action();
        }
    }

    private void RepairUnitAction( EnemyUnit enemyUnit, float amount )
    {
        amount = (amount * UnityEngine.Random.Range(1f, 1.3f));

        //回復するキャラのトータルHPを取得して、そのamaount%回復させる
        int repairNumber = (int)(enemyUnit.realCharaData.realMaxHP * amount);
        enemyUnit.realCharaData.realHP = enemyUnit.realCharaData.realHP + repairNumber;

        StartCoroutine(DelayMethod(1.0f, () =>
        {
            enemyUnit.transform.Find("repairNumberText").gameObject.SetActive(false);
        }));

        enemyUnit.TextSetOn("<color=#00F9A9>+" + repairNumber);

        if (enemyUnit.realCharaData.realHP >= enemyUnit.realCharaData.realMaxHP)
            enemyUnit.realCharaData.realHP = enemyUnit.realCharaData.realMaxHP;
    }


	public bool allDeadFlag = false;
    public void DamegedAction( RealActionData realActionData, RealCharaData realCharaData, Action action)
    {
        StartCoroutine( DamegedActionOn(realActionData,realCharaData, action));
    }
    public IEnumerator DamegedActionOn( RealActionData realActionData , RealCharaData realCharaData, Action action)
	{
        if (LiveList().Count == 0)
            yield break;


        Action m_compAction = null;
        effectNameList.Clear();
        if( realActionData.type == 7 )
        {
            var enemyUnit = LiveList().GetAtRandom();
            if (realActionData.lastAttackFlag)
                m_compAction = action;

            enemyUnit.Damage(realActionData, realCharaData, m_compAction);

            if (LiveList().Count == 0)
            {
                AllDeadAction();
                yield break;
            }

            yield break;
        }



        var damagedEnemyList = new List<EnemyUnit>();

        foreach (var Value in LiveList())
            if ((Value.realCharaData.charaTypeId == realActionData.range || realActionData.range == 4) && Value.realCharaData.liveFlag)
                damagedEnemyList.Add(Value);

        if(damagedEnemyList.Count == 0 && realActionData.lastAttackFlag )
        {
            action();
            yield break;
        }

        foreach(var Value in damagedEnemyList)
        {
            if (LiveList().Count == 0)
            {
                yield break;
            }
            if (realActionData.lastAttackFlag && Value == damagedEnemyList.Last())
                m_compAction = action;

            Value.Damage(realActionData, realCharaData, m_compAction);
            yield return new WaitForSeconds(0.1f);

            if (LiveList().Count == 0)
            {
                AllDeadAction();
                yield break;
            }
        }


	}


    public void AllDeadAction()
    {
        if ( battleManager.enemyReady)
            return;

        battleManager.enemyReady = true;
        Debug.Log("敵が全滅してる");
        allDeadFlag = true;
        enemyUnitList = new List<EnemyUnit>();

        DOVirtual.DelayedCall(1.5f, () =>
        {
            battleManager.enemyReady = false;
            battleManager.MyAttackEnd();
        });
    }

    public List<EnemyUnit> LiveList()
    {
        var returnValue = new List<EnemyUnit>();
        foreach (var Value in enemyUnitList)
        {
            if (Value.realCharaData.liveFlag)
            {
                returnValue.Add(Value);
            }
        }
        return returnValue;
    }

    public void  BuffAction( RealActionData realActionData,Action action)
	{
        string textColorCode = "";
        if (realActionData.type == 3)
        {
            textColorCode = "<color=#ADFF2F>+";
        }
        else
        {
            textColorCode = "<color=#FFA500>-";
        }

        //Debug.Log( realActionData.userEnemyUnit.realCharaData.charaName+ ":BuffAction start======================"+realActionData.teamCharaActionFlag);
        foreach (var Value in LiveList())
        {

            if (allDeadFlag) { return; }

            if ((Value.realCharaData.charaTypeId == realActionData.range || realActionData.range == 4) && Value.realCharaData.liveFlag)
            {


                float amountNumber = realActionData.amount;
                if (UserData.GetLeaderChara() == 3 && realActionData.type == 4)
                    amountNumber = (amountNumber + 0.08f);

                Value.TextSetOn(textColorCode + (amountNumber * 100) + "%");
                realActionData.amount = amountNumber;
                buffDebuffController.AddEnemyBuffDebuff
                                    (Value,
                                     realActionData,
                                     battleManager.turn + realActionData.turn
                                    );

                Value.SetEffect(realActionData);
            }


            if (Value == LiveList().Last() && realActionData.lastAttackFlag )
            {
                action();
            }
        }

        //buffDebuffController.CheackBuffDebuff();
    }


	private IEnumerator DeadWait(){ 
        yield return new WaitForSeconds(1.5f);
        Debug.Log("kokodeWaveClear");
        InitStart(); 
    }

    public void BuffDebuffSetimage()
    {
        foreach( var Value in LiveList() )
        {
            Value.SetBuffDebuffImage();
        }

    }


	public int GetAttackNumber( int partyIndex )
	{
        return enemyUnitList[partyIndex].realCharaData.realAttack;
	}






    //AttackPaturn
    public void AttackPaturn(RealActionData realActionData, EnemyUnit ecu, Action action)
    {
        if (realActionData.type == 1)
        {
            EnemyAttack(realActionData, ecu, action);
        }
        else if (realActionData.type == 2)
        {
            RepairAction(realActionData, action);
        }
        else if (realActionData.type == 3)
        {
            BuffAction(realActionData, action);
        }
        else if (realActionData.type == 4)
        {
            teamCharacterController.BuffAction(realActionData, action);
            //Debuff(realActionData, tcu);
        }
        else if (realActionData.type == 7)// random
        {
            EnemyAttack(realActionData, ecu, action);
        }
        else if (realActionData.type == 8)//guard
        {
            //Gaurd(splited[1], charaCount);
        }
        else if (realActionData.type == 5)
        {
            battleManager.DiceNumberEffet((int)realActionData.amount);
            action();
        }
        else if (realActionData.type == 6)
        {
            if (action!=null)
                action();
        }
        else
        {
            Debug.LogError("attackPaturnError:" + realActionData.type);
        }
    }

    /// <summary>
    /// 敵攻撃処理
    /// </summary>
    /// <param name="type">Type.</param>
    /// <param name="amount">Amount.</param>
    /// <param name="ecu">Ecu.</param>
    public void EnemyAttack(RealActionData realActionData, EnemyUnit ecu,Action action)
    {
        //Debug.Log("ecu.realCharaData.charaIdNumber:"+ecu.realCharaData.charaIdNumber);
        //ボイス
        Singleton<SoundPlayer>.instance.CharaVoiceByProbabilityAtBattle(ecu.realCharaData.charaIdNumber, "attack");

        string resultText = "敵の攻撃情報";
        int amountNumber = (int)(ecu.realCharaData.realAttack * realActionData.amount);
        float buffDebuffTotalAttack = 0;
        foreach (var Value in ecu.buffDebuffDatas)
        {
            if (Value.buffDebuffType == 1)
            {
                if (Value.buffDebuffID == 3)
                    buffDebuffTotalAttack = buffDebuffTotalAttack + Value.amount;
                else
                    buffDebuffTotalAttack = buffDebuffTotalAttack - Value.amount;

                resultText += "\n attack buff:" + Value.buffDebuffID + " : " + Value.amount;
            }
        }
        resultText += "\n damageInit:" + amountNumber;
        amountNumber = (int)(amountNumber * (1 + buffDebuffTotalAttack));
        resultText += "\n buffDebuffTotalAttack:" + buffDebuffTotalAttack + " damage effect:" + amountNumber;

        resultText += "\n damage1:" + amountNumber;
        amountNumber = (int)(amountNumber + UnityEngine.Random.Range(0.98f, 1.02f));
        realActionData.damageAmount = amountNumber;
        resultText += "\n Damage:" + amountNumber;

        if ( battleManager.debugmode)
            Debug.Log(resultText);

        teamCharacterController.DamegedAction(realActionData, ecu.realCharaData,action);

    }
}
