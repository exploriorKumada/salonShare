using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffDebuffController : MonoBehaviour {

    [SerializeField] TeamCharacterController teamCharacterController;
    [SerializeField] EnemyController enemyController;
    [SerializeField] BattleManager battleManager;

	public void Init()
	{
	}


    /// <summary>
    /// buffDebuffType
    /// </summary>
    /// <param name="teamCharacterUnit">Team character unit.</param>
    /// <param name="turn">Turn.</param>
    /// <param name="type">Type.</param>
    /// <param name="amount">Amount.</param>
    /// <param name="buffDebuffType">Buff debuff type.</param>
    /// <param name="buffDebuffID">Buff debuff identifier.</param>
    public void AddBuffDebuff( TeamCharacterUnit teamCharacterUnit,RealActionData realActionData, int turn)
	{
		BuffDebuffData data = new BuffDebuffData ();

		//data.charaId = charaId;
        data.turn = turn;
        data.amount = realActionData.amount;
        data.buffDebuffType = realActionData.target;
        data.buffDebuffID = realActionData.type;
        data.ｄescription = CreateDescription(data);

        string effectText;
        if (data.buffDebuffID == 3)
            effectText = "UP";
        else
            effectText = "DOWN";
       
        data.text = CharaSetting.ConvertBuffDebuffAction(data.buffDebuffType) + "を" + turn +"ターン" + ( 100 * realActionData.amount ) + "%" +effectText ;
        teamCharacterUnit.SetBuffDebuff(data);
	}


    public void AddEnemyBuffDebuff( EnemyUnit enemyUnit, RealActionData realActionData,int turn)
	{
		BuffDebuffData data = new BuffDebuffData ();

		//data.charaId = charaId;
        data.turn = turn;
        data.amount = realActionData.amount;
        data.buffDebuffType = realActionData.target;
        data.buffDebuffID = realActionData.type;
        data.ｄescription = CreateDescription(data);

        string effectText;
        if (data.buffDebuffID == 3)
            effectText = "UP";
        else
            effectText = "DOWN";
        data.text = CharaSetting.ConvertBuffDebuffAction(data.buffDebuffType) + "を" + turn + "ターン" + (100 * realActionData.amount) + "%" + effectText;
        enemyUnit.SetBuffDebuff(data);

	}

	public void CheackBuffDebuff()
	{
        foreach( var Value in teamCharacterController.LiveList() )
        {
            for (int i = Value.buffDebuffDatas.Count - 1; i >= 0; i--)
            {
                if (Value.buffDebuffDatas[i].turn <= battleManager.turn)
                {
                    Value.buffDebuffDatas.Remove(Value.buffDebuffDatas[i]); // 要素の削除
                }else
                {
                    //Debug.Log( Value.name + " ni " + Value.buffDebuffDatas[i].buffDebuffType + " no " + Value.buffDebuffDatas[i].turn );
                }
            }
        }

        foreach (var Value in enemyController.LiveList())
        {
            for (int i = Value.buffDebuffDatas.Count - 1; i >= 0; i--)
            {
                if (Value.buffDebuffDatas[i].turn <= battleManager.turn)
                {
                    Value.buffDebuffDatas.Remove(Value.buffDebuffDatas[i]); // 要素の削除
                }
            }
        }
	}

    public static string CreateDescription(BuffDebuffData buffDebuffData)
    {
        string returnValue = "";

        if (buffDebuffData.buffDebuffType == 1)
        {
            returnValue = "攻撃力";
        }
        else
        {
            returnValue = "防御力";
        }

        returnValue += ( buffDebuffData.amount) *100 + "%";

        if (buffDebuffData.buffDebuffID == 3)
        {
            //buff
            returnValue += "UP";
        } else
        {
            returnValue += "DOWN";
        }

        return returnValue;
    }
}
