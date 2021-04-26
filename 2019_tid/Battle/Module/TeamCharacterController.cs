using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using UnityEngine.UI;
using System.Linq;
using System;

public class TeamCharacterController : ScenePrefab
{


    [SerializeField] LeaderCharaController leaderroller;
    [SerializeField] BattleManager battleManager;
    [SerializeField] BuffDebuffController buffDebuffController;
    [SerializeField] EnemyController enemyController;
    [SerializeField] Layout_Battle layout_Battle;
    [SerializeField] DiceSelectManager diceSelectManager;

    public List<TeamCharacterUnit> teamCharacterUnitList = new List<TeamCharacterUnit>();

    Vector3 firstPosi;
    Vector3 firstCharaPosi;

    List<float> partyWaku = new List<float>() { };

    private Coroutine cardMoveAction;
    private bool firstSetImage = true;

    public void Init()
    {
        StartCoroutine(SetImage());
    }



    private IEnumerator SetImage()
    {

        teamCharacterUnitList.Clear();

        var charaBase = transform.Find("TeamCharacterBase").gameObject;
        var charaTF = transform.Find("TeamCharacterParent");
        int baseCount = 0;
        charaBase.SetActive(false);
        for (int i = 0; i < charaTF.childCount; ++i)
        {
            Destroy(charaTF.GetChild(i).gameObject);
        }

        int count = 0;
        firstCharaPosi = charaBase.transform.localPosition;
        foreach (var KV in layout_Battle.partyCharaDataList[BattleManager.teamNo])
        {
            var newGO = Instantiate(charaBase, charaTF);
            newGO.SetActive(false);
            Transform newGoTF = newGO.transform;
            newGoTF.localPosition = new Vector3(charaBase.transform.localPosition.x + (count * 470), firstCharaPosi.y, newGoTF.localPosition.z);
            TeamCharacterUnit teamCharacterUnit = newGoTF.GetComponent<TeamCharacterUnit>();
            teamCharacterUnit.damagedBairitu = GameSetting.damagedBairitu[count];
            teamCharacterUnit.Init(KV.Value);
            count++;
            if (KV.Value == null)
                continue;

            newGO.name = KV.Value.charaName;
            teamCharacterUnitList.Add(teamCharacterUnit);
        }

        firstSetImage = false;

        yield return null;
        StartCoroutine(InitAnimation());
    }

    public IEnumerator InitAnimation()
    {
		var actionList = new List<TeamCharacterUnit>(teamCharacterUnitList);
		actionList.Shuffle();

		foreach (var Value in actionList)
        {
            Value.gameObject.SetActive(false);
            Value.gameObject.SetActive(true);
            Value.InitSetAnimation();
            yield return new WaitForSeconds(0.1f);
        }
    }



    public void ActionStart(int diceNumber, Action action)
    {
        StartCoroutine(ActionStartOn(diceNumber,action));
    }
    public IEnumerator ActionStartOn( int diceNumber, Action action)
    {
        foreach( var Value in LiveList())
        {
            if (enemyController.LiveList().Count == 0)
                break;

            Value.ActionMove();

            if(Value.realCharaData.actionType == "skill")
            {
                var actionData = Value.realCharaData.realCharaMasterData.realSkillActionData;
                actionData.lastAttackFlag = (Value == LiveList().Last());

                AttackPaturn(actionData, Value,action);
                Value.SetSkill(0, true);
            }else
            {
                
                int actionNumber = UserData.GetCharacterSkillSetList(Value.realCharaData.charaIdNumber)[diceNumber];
              
                if (!Value.realCharaData.realCharaMasterData.realActionDataDic.ContainsKey(actionNumber))
                {
                    Debug.LogError("action error actionNumber:" + actionNumber);
                    actionNumber = 1;
                }

                RealActionData _actionData =  Value.realCharaData.realCharaMasterData.realActionDataDic[actionNumber];
                RealActionData actionData = _actionData.GetNewRealActionData(_actionData);
                actionData.lastAttackFlag = (Value == LiveList().Last());
                AttackPaturn(actionData, Value,action);
                yield return new WaitForSeconds(0.3f);
            }


            Value.realCharaData.actionType = "action";

        }
    }




    public void RepairAction(RealActionData realActionData, Action action)
    {
        StartCoroutine( RepairActionOn(realActionData, action) );
    }
    public IEnumerator RepairActionOn(RealActionData realActionData, Action action)
	{

        var targetTeamList = new List<TeamCharacterUnit>();
        foreach (var Value in LiveList())
            if ((Value.realCharaData.charaTypeId == realActionData.range || realActionData.range == 4) && Value.realCharaData.liveFlag)
                targetTeamList.Add(Value);

        if (targetTeamList.Count == 0 && realActionData.lastAttackFlag)
        {
            action();
            yield break;
        }

        foreach( var Value in targetTeamList)
        {
            int amountNumber = (int)(Value.realCharaData.realMaxHP * realActionData.amount);
            string resultText = "回復反映情報";
            resultText += "\n 反映前:" + Value.realCharaData.item_repair + " amountNumber:" + amountNumber;
            amountNumber = (int)(amountNumber * (1 + (0.01 * Value.realCharaData.item_repair)));
            resultText += "\n 反映後:" + amountNumber;
            if (UserData.GetLeaderChara() == 1)
                amountNumber = (int)(amountNumber * 1.05f);

            resultText += "\n リーダー特性反映後:" + amountNumber;
            amountNumber = (int)(amountNumber * (1 + (0.01 * Value.realCharaData.item_repair)));          
            amountNumber = (int)(amountNumber * UnityEngine.Random.Range(1f, 1.3f));
            Value.SetHp(amountNumber);
            Value.TextSetOn("<color=#00F9A9>" + amountNumber);

            Action m_compAction = null;
            if (realActionData.lastAttackFlag && Value == targetTeamList.Last())
                m_compAction = action;

            Value.Repair(realActionData, m_compAction);

            if (battleManager.debugmode)
                Debug.Log(resultText);

            yield return new WaitForSeconds(0.1f);

        }
	}

    public bool allDeadFlag = false;
    public void DamegedAction(RealActionData realActionData, RealCharaData realCharaData, Action action)
    {
        StartCoroutine(DamegedActionOn(realActionData,realCharaData, action));
    }
    public IEnumerator DamegedActionOn( RealActionData realActionData, RealCharaData realCharaData, Action action)
	{   	
        leaderroller.SetSkill(10);
        if (realActionData.type == 7)
        {
            var teamCharacterUnit = LiveList().GetAtRandom();
            teamCharacterUnit.Damage(realActionData, realCharaData);

            if (action!=null)
                action();

            if (LiveList().Count == 0)
            {
                allDeadFlag = true;
                battleManager.LoseAciton();
                yield break;
            }
            yield break;
        }

        foreach( var Value in  LiveList() )
		{
            if (Value == LiveList().Last() && action != null)
                action();

            if ( allDeadFlag )
                yield break;

            if (!Value.realCharaData.liveFlag)
                continue;

            if( (Value.realCharaData.charaTypeId == realActionData.range || realActionData.range == 4 ) && Value.realCharaData.liveFlag )
			{
                //Value.realCharaData.realHP = Value.realCharaData.realHP - amount;
				Value.SetSkill (10);
				//ここで被ダメージ表示
                Value.Damage(realActionData,realCharaData);

                yield return new WaitForSeconds(0.1f);
            }

            if (LiveList().Count == 0)
            {
                allDeadFlag = true;
                battleManager.LoseAciton();
                yield break;
            }

           
		}
	}

    public void BuffAction(RealActionData realActionData, Action action)
    {
        StartCoroutine(BuffActionOn(realActionData, action));
    }
    public IEnumerator BuffActionOn(RealActionData realActionData, Action action)
	{
        //Debug.Log("baffDebuff Action");
        string textColorCode = "";
        if (realActionData.type == 3)
        {
            textColorCode = "<color=#ADFF2F>+";
        }
        else
        {
            textColorCode = "<color=#FFA500>-";
        }

        foreach( var Value in  LiveList() )
		{
            if (allDeadFlag)
                yield break;

            if( (Value.realCharaData.charaTypeId == realActionData.range || realActionData.range  == 4) && Value.realCharaData.liveFlag )
			{
                string resultText = "魔法力反映情報";
                float amountNumber = realActionData.amount;

                if(realActionData.type == 3)
                {
                    resultText += "\n 反映前:" + Value.realCharaData.item_repair + " amountNumber:" + amountNumber;
                    amountNumber = amountNumber + Value.realCharaData.item_repair;
                    resultText += "\n 反映後:" + amountNumber;

                    if (UserData.GetLeaderChara() == 5 )
                        amountNumber = (amountNumber + 0.08f);
                }


                Value.TextSetOn(textColorCode+ ( amountNumber* 100) + "%");
                realActionData.amount = amountNumber;

                buffDebuffController.AddBuffDebuff
                                    (Value,
                                     realActionData,
                                     battleManager.turn + realActionData.turn
                                    );

                if(battleManager.debugmode)
                    Debug.Log(resultText);
    
                Value.SetEffect(realActionData);
                yield return new WaitForSeconds(0.1f);
            }

            if (Value == LiveList().Last()&&realActionData.lastAttackFlag)
            {
                action();
            }            
		}

        //buffDebuffController.CheackBuffDebuff();
	}


    public List<RealActionData> GetAttackInfo( int number )
	{
        List<RealActionData> getAttackInfo = new List<RealActionData>();

        leaderroller.SetSkill(5);
        foreach( var Value in teamCharacterUnitList )
		{
			if( !Value.realCharaData.liveFlag )
                continue;

            List<RealActionData> charaDiceTextList = new List<RealActionData>();
			if( Value != null )
			{	
				if (Value.realCharaData.actionType == "gaurd") {
                    Debug.Log (Value.realCharaData.charaName + "はガードしようとしてる");
					Value.SetActionTextImage ("ガード");
                    continue;
				} 
                else 
                {
                    var attackValue = UserData.GetCharacterSkillSetList(Value.realCharaData.charaIdNumber)[number];

                    if(  !Value.realCharaData.realCharaMasterData.realActionDataDic.ContainsKey(attackValue) )
                    {
                        Debug.LogError("action error:" + attackValue);
                        attackValue = 1;
                    }
                    RealActionData realActionData = Value.realCharaData.realCharaMasterData.realActionDataDic[attackValue];
                    getAttackInfo.Add( realActionData );

                    if (realActionData.type == 6) {
                        Value.SetActionTextImage("miss!");					
					} else {
                        Value.SetSkill(5);
					}
				}

			}else
			{				
				//getAttackInfo.Add(null);
			}
		}
			
		return getAttackInfo;
	}



    //public RealActionData GetUnitAction( int number, TeamCharacterUnit teamCharacterUnit )
    //{
    //    string returnValue;
    //    number = number - 1;
    //    if (teamCharacterUnit.realCharaData.actionType == "gaurd")
    //    {
    //        teamCharacterUnit.SetActionTextImage("ガード");
    //        returnValue = "gaurd";

    //    }else if (teamCharacterUnit.realCharaData.actionType == "skill")
    //    {
    //        returnValue = teamCharacterUnit.realCharaData.skillValue;
    //    }else
    //    {
    //        returnValue = CharaSetting.CharaDiceSettingInfo(teamCharacterUnit.realCharaData.charaIdNumber)[number];
    //    }

    //    return returnValue;
    //}


	public int GetAttackNumber( int partyIndex )
	{
		//charaのAttack量を返す
        return teamCharacterUnitList[partyIndex].realCharaData.realAttack ;
	}


 //   private void DeadAction( Transform charaTF, TeamCharacterUnit teamCharacterUnit )
	//{
 //       //charaTF.Find("dead").gameObject.SetActive(true);
 //       //Debug.Log("sinndasyori ");
 //       teamCharacterUnit.Dead();
	//}
		
	private IEnumerator CardMoveAction( GameObject newGO )
	{
		while( true )
		{
			Vector3 screenPos = Input.mousePosition;
			Vector3 worldPos = Camera.main.ScreenToWorldPoint (screenPos);

			newGO.transform.position = new Vector3 ( worldPos.x, worldPos.y, -100 );

			yield return null;
		} 
	}

    public void AllEffect( int effectId )
    {
        foreach (var Value in LiveList())
            Value.SetEffect(effectId);
        
    }

    public List<TeamCharacterUnit> LiveList()
    {
        var returnValue = new List<TeamCharacterUnit>();
        foreach( var Value in teamCharacterUnitList )
        {
            if( Value.realCharaData.liveFlag )
            {
                returnValue.Add( Value );
            }
        }
        return returnValue;
    }


    public void BuffDebuffSetimage()
    {
        foreach (var Value in LiveList())
        {
            Value.SetBuffDebuffImage();
        }

    }

    public void TurnInit()
    {
        foreach (var Value in teamCharacterUnitList)
            Value.TurnInit();
    }

    public void Continue()
    {
        foreach (var Value in teamCharacterUnitList)
            Value.Live();

        allDeadFlag = false;
    }



   


    //AttackPaturn
    public void AttackPaturn(RealActionData realActionData, TeamCharacterUnit tcu, Action action)
    {
        //Debug.Log("realActionData:" + realActionData.type);
        if(realActionData.type == 1)
        {
            Attack(realActionData, tcu, action);
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
            enemyController.BuffAction(realActionData, action);
            //Debuff(realActionData, tcu);
        }
        else if (realActionData.type == 7)// random
        {
            Attack(realActionData, tcu, action);
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
            if(realActionData.lastAttackFlag)
                action();
        }
        else
        {
            Debug.LogError("attackPaturnError:" + realActionData.type);
        }

    }


    /// <summary>
    /// アタック処理
    /// </summary>
    /// <param name="type">Type.</param>
    /// <param name="amount">Amount.</param>
    /// <param name="tcu">Tcu.</param>
    private void Attack(RealActionData realActionData, TeamCharacterUnit tcu, Action action)
    {
        //ボイス
        Singleton<SoundPlayer>.instance.CharaVoiceByProbabilityAtBattle(tcu.realCharaData.charaIdNumber, "attack");

        string resultText = tcu.realCharaData.charaName +  ":味方の攻撃情報";
        int amountNumber = (int)(tcu.realCharaData.realAttack * realActionData.amount);
        resultText += "\n tcu.realCharaData.realAttack " + tcu.realCharaData.realAttack + ":" + realActionData.amount + "realActionData.amount";

        //リーダーバフ TODO
        if (UserData.GetLeaderChara() == 4)
            amountNumber = (int)(amountNumber * 1.05f);

        //baffDebaff
        float buffDebuffTotalAttack = 0;
        foreach (var Value in tcu.buffDebuffDatas)
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
        //diceBornus
        if (diceSelectManager.effectType == 1)
            amountNumber = (int)(amountNumber * (1 + diceSelectManager.amountByDice));

        resultText += "\n damage0:" + amountNumber;
        amountNumber = (int)(amountNumber + UnityEngine.Random.Range(0.98f, 1.02f));
        realActionData.damageAmount = amountNumber;
        resultText += "\n Damage:" + amountNumber;

        if ( battleManager.debugmode)
            Debug.Log(resultText);

        enemyController.DamegedAction(realActionData, tcu.realCharaData, action);//Debug.Log ("test now");
    }


}
