using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RealActionData {

    public int no;

    /// <summary>
    /// The action.
    /// </summary>
    public int action;

    /// <summary>
    /// 1:攻撃 2:回復 3:バフ 4:デバフ 5:ダイス確定 6:ミス 7:randomAttack
    /// </summary>
    public int type;

    /// <summary>
    /// 1:blue 2:red 3:yellow 4:all 5:self
    /// </summary>
    public int range;

    public int turn;

    /// <summary>
    /// 何のバフ 1:attack 2:guard 3:damageRepair(ジュン)
    /// </summary>
    public int target;

    public float amount;

    public int damageAmount;

    public bool criFlag = false;

    /// <summary>
    /// 0:normal 1:skill
    /// </summary>
    public int actionTypeId = 0;


    /// <summary>
    /// The last attack flag.
    /// </summary>
    public bool lastAttackFlag = false;

    /// <summary>
    /// The team CH ara action flag.
    /// </summary>
    public bool teamCharaActionFlag = true;

    public int effectId = 0;

    public EnemyUnit userEnemyUnit;

    public RealActionData GetNewRealActionData(RealActionData realActionData)
    {
        RealActionData returnValue = new RealActionData();
        returnValue.no = realActionData.no;
        returnValue.action = realActionData.action;
        returnValue.type = realActionData.type;
        returnValue.range = realActionData.range;
        returnValue.turn = realActionData.turn;
        returnValue.target = realActionData.target;
        returnValue.amount = realActionData.amount;
        returnValue.damageAmount = realActionData.damageAmount;
        returnValue.criFlag = realActionData.criFlag;
        returnValue.actionTypeId = realActionData.actionTypeId;
        returnValue.lastAttackFlag = realActionData.lastAttackFlag;
        returnValue.teamCharaActionFlag = realActionData.teamCharaActionFlag;
        returnValue.effectId = realActionData.effectId;
        returnValue.userEnemyUnit = realActionData.userEnemyUnit;

        return returnValue;
    }

}
