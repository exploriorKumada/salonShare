using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RealCharaData
{

    public int charaIdNumber;
    public int user_character_id;

    public int lv = 1;

    public int haveFlag;
    public int addGold;

    public string charaName;

    public string rare;
    public int rareId;

    public string charaType;

    /// <summary>
    /// 1:red 2:blue 3:yelow
    /// </summary>
    public int charaTypeId;

    //attack
    public int mastertAttack;
    public int realAttack;
    public float attackOds;

    //guard
    public float masterGuard;
    public float realGuard;
    public float guardOds;

    //hp
    public int masterHP;
    public int realMaxHP;
    public float HPOds;

    public int groupID;

    public float cri;
    public int effectId;

    public int skillMaxNumber;
    public int skilRealNumber;

    public int team1No;
    public int team2No;
    public int team3No;


    //master
    public RealCharaMasterData realCharaMasterData;

    /// <summary>
    /// 1;normal 2:beforeUp 3:afterUp
    /// </summary>
    public int paturn;

    //equipment
    public int item_master_id;
    public int item_attack;
    public float item_cri;
    public float item_guard;
    public float item_repair;

    public bool enemyFlag = false;
    public bool masterFlag = false;
    public bool friendFlag = false;

    /////////////////////////////////////////////
    ///  for battle
    ///////////////////////////////////////////// 

    public int realHP;
    public bool liveFlag = true;
    public string actionType;


    public int GetRealHP(int _lv = 0)
    {
        if (_lv == 0)
            _lv = lv;
        
        int returnValue = CalculationManager.GetStatusValue(realCharaMasterData.hp_max, realCharaMasterData.hp, realCharaMasterData.hp_ods, _lv);

        if (!enemyFlag)
            returnValue += CharaAPISetting.allHP;

        return returnValue;
    }


    public int GetRealHPSimple(int _lv = 0)
    {
        if (_lv == 0)
            _lv = lv;

        int returnValue = CalculationManager.GetStatusValue(realCharaMasterData.hp_max, realCharaMasterData.hp, realCharaMasterData.hp_ods, _lv);

        return returnValue;
    }

    public int GetRealAttack(int _lv = 0, bool itemFlag = true)
    {
        if (_lv == 0)
            _lv = lv;

        int returnValue = CalculationManager.GetStatusValue(realCharaMasterData.attack_max, realCharaMasterData.attack, realCharaMasterData.attack_ods, _lv);

        //for (float i = -1f; i < 1f; i++)
        //{
        //    Debug.Log(i+":" + CalculationManager.GetStatusValue(realCharaMasterData.attack_max, realCharaMasterData.attack, i, _lv));
        //}



        if (item_master_id != 0 && itemFlag)
            returnValue += item_attack;

        if (!enemyFlag)
            returnValue += CharaAPISetting.allAttack;

        return returnValue;
    }


    public int GetReralAttackSimple(int _lv = 0)
    {
        if (_lv == 0)
            _lv = lv;
        
        int returnValue = CalculationManager.GetStatusValue(realCharaMasterData.attack_max, realCharaMasterData.attack, realCharaMasterData.attack_ods, _lv);
    
        return returnValue += item_attack;

    }

    public float GetRealGuard(int _lv = 0, bool itemFlag = true)
    {
        if (_lv == 0)
            _lv = lv;

        //for (int i = 1; i <= 80; i++)
        //{
        //    Debug.Log("guard:" + i + ":" + CalculationManager.GetStatusValue(realCharaMasterData.guard_max, realCharaMasterData.guard, realCharaMasterData.guard_ods, i));
        //}
        if (item_master_id != 0 && itemFlag)
        {
            //Debug.Log("item add:" + item_attack);
            return item_guard + CalculationManager.GetStatusValue(realCharaMasterData.guard_max, realCharaMasterData.guard, realCharaMasterData.guard_ods, _lv);
        }
        else
        {
            float returnValue = CalculationManager.GetStatusValue(realCharaMasterData.guard_max, realCharaMasterData.guard, realCharaMasterData.guard_ods, _lv);
            return returnValue;
        }
    }

    public string GetRealGuardForStatusView(int _lv = 0, bool itemFlag = true)
    {
        return (GetRealGuard(_lv,itemFlag) * 100).ToString();
    }

    public float GetRealCri()
    {
        return ( item_cri + cri );
    }

    public string GetRealCriForStatusView()
    {
       return (GetRealCri() * 100).ToString();
    }

    public string GetRealRepairForStatusView()
    {
        return (item_repair * 100).ToString();
    }
}
