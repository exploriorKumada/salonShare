using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RealCharaMasterData {

    public int id;
    public string name;
    public int rare;
    public int type;
    public int chart_battle_status_type;
    public int attack;
    public int attack_max;
    public int attack_ods;
    public int critical;
    public int skill_max_number;
    public float guard;
    public float guard_max;
    public float guard_ods;
    public int hp;
    public int hp_max;
    public int hp_ods;
    public int skill_effect_id;
    public int leader_master_id;
    public string cv_name;
    public Dictionary<int, RealActionData> realActionDataDic;
    public RealActionData realSkillActionData;


    public int GetRealHP(int _lv)
    {
        return CalculationManager.GetStatusValue(hp_max, hp, hp_ods, _lv); ;
    }

    public int GetRealHPSimple(int _lv)
    {
        return CalculationManager.GetStatusValue(hp_max, hp, hp_ods, _lv);
    }

    public int GetRealAttack(int _lv)
    {
        return CalculationManager.GetStatusValue(attack_max, attack, attack_ods, _lv); ;
    }

    public float GetRealGuard(int _lv)
    {
        return CalculationManager.GetStatusValue(guard_max, guard, guard_ods, _lv);
    }

    public float GetRealCri()
    {
        return critical;
    }

}

