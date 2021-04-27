using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TW.GameSetting;
using Explorior;
using System.Linq;
using HMLabs.JsonConsole;

public class GeneralDataClass : MonoBehaviour
{
    public static List<ItemCommonInfo> ItemCommonInfo(List<ItemInfo> itemInfos)
    {
        List<ItemCommonInfo> returnValue = new List<ItemCommonInfo>();
        itemInfos.ForEach(x => returnValue.Add(new ItemCommonInfo(ItemType.ConsumeItem, x)));
        return returnValue;
    }

    public static List<ItemCommonInfo> ItemCommonInfo(List<WeaponInfo> weaponInfos)
    {
        List<ItemCommonInfo> returnValue = new List<ItemCommonInfo>();
        weaponInfos.ForEach(x => returnValue.Add(new ItemCommonInfo(ItemType.WeaponItem, x)));
        return returnValue;
    }

}


public class QuestMaster
{
    public int questId;
    public string questName;
    public QuestType questType;
    public List<QuestDetailInfo> questDetailInfos = new List<QuestDetailInfo>();
}

public class QuestDetailInfo
{
    public int questDetailId;
    public int questId;
    public string questDetailName;
    public int user_stamina;
    public int experience_point;
    public int gold;
    public int proper_level;
    public List<QuestWaveInfo> questWaveInfos = new List<QuestWaveInfo>();

    public List<QuestWaveInfo> charaInfosBySecond(int waveSecond) => questWaveInfos.Where(x => x.wave_no == waveSecond).ToList();
}

public class QuestWaveInfo
{
    public int id { get; private set; }
    public int quest_detail_id { get; private set; }
    public int wave_no { get; private set; }
    public int enemy_character_id { get; private set; }
    public int enemy_lv { get; private set; }
    public int drop_type_id { get; private set; }
    public int drop_group_id { get; private set; }
    public int area_id { get; private set; }
    public CharaInfo charaInfo { get; private set; }

    public QuestWaveInfo(JSONObject jSONObject)
    {
        id = (int)jSONObject.GetField("id").n;
        quest_detail_id = (int)jSONObject.GetField("quest_detail_id").n;
        wave_no = (int)jSONObject.GetField("wave_no").n;
        enemy_character_id = (int)jSONObject.GetField("enemy_character_id").n;
        enemy_lv = (int)jSONObject.GetField("enemy_lv").n;
        drop_type_id = (int)jSONObject.GetField("drop_type_id").n;
        drop_group_id = (int)jSONObject.GetField("drop_group_id").n;
        area_id = (int)jSONObject.GetField("area_id").n;
        charaInfo = new CharaInfo(DataManager.Instance.GetByCharaId(enemy_character_id), enemy_lv);
    }
}

//jSONObject:{"id":1,"action_type_id":1,"range_size_id":0,"effect_target":0,"amount":10,"second":0}
public class ActionMaster
{
    public int id { get; private set; }
    public ActionType action_type { get; private set; }
    public RangeType range_size_id { get; private set; }
    public StatusEfectType effect_target { get; private set; }
    public float amount { get; private set; }
    public int second { get; private set; }
    public AttackMotionMaster.Param attackMotionInfo { get; private set; }

    public ActionMaster(JSONObject jSONObject)
    {
        id = (int)jSONObject.GetField("id").n;
        action_type = (ActionType)jSONObject.GetField("action_type_id").n;
        range_size_id = (RangeType)jSONObject.GetField("range_size_id").n;
        effect_target = (StatusEfectType)jSONObject.GetField("effect_target").n;
        amount = (float)jSONObject.GetField("amount").f;
        second = (int)jSONObject.GetField("second").n;

        JsonConsole.ExploriorLog("Response", "ActionMaster/id: " + id, jSONObject.ToString(), JsonConsole.ExploriorLogType.ActionMaster);
    }
}


public class StatusEffectInfo
{
    public int id;
    public StatusEfectType statusEfectType;
    public ActionType actionType;
    public int second;
    public float amount;
    public GameObject imageObject;
}



/// <summary>
/// ダイアログ情報
/// </summary>
public class DialogData
{
    public int id;
    public ColEventType colEventType;
    public GameObject dialogObject;
}


public class CharaInfo
{
    public CharaType charaType;
    public CharaMaster charaMaster;
    public int id => charaMaster.id;
    public int lv = 3;
    public int hp;
    public int maxHP;

    public List<SkillMaster> skillMasters　=> charaMaster.skillInfos;

    public CharaInfo(CharaMaster _charaMaster,int _lv)
    {
        charaMaster = _charaMaster;
        lv = _lv;
        maxHP = base_hp;
        hp = base_hp;
    }

    public int physicalAttack => CalculationManager.GetStatusValue(charaMaster.physical_attack_max, charaMaster.physical_attack, charaMaster.physical_attack_rate,lv);
    public int physicalDefence => CalculationManager.GetStatusValue(charaMaster.physical_defence_max, charaMaster.physical_defence, charaMaster.physical_defence_rate, lv);
    public int magicAttack => CalculationManager.GetStatusValue(charaMaster.magic_attack_max, charaMaster.magic_attack, charaMaster.magic_attack_rate, lv);
    public int magicDefence => CalculationManager.GetStatusValue(charaMaster.magic_defence_max, charaMaster.magic_defence, charaMaster.magic_defence_rate, lv);
    public int base_hp => CalculationManager.GetStatusValue(charaMaster.hp_max, charaMaster.hp, charaMaster.hp_rate, lv);
    public int base_mp => CalculationManager.GetStatusValue(charaMaster.mp_max, charaMaster.mp, charaMaster.mp_rate, lv);
    public float movement => CalculationManager.GetStatusValue(charaMaster.movement_max, charaMaster.movement, charaMaster.movement_rate, lv);
}

public class CharaMaster
{
    public CharaType charaType;
    public string charaName;
    public string description;
    public int id;
    public int type;
    public int physical_attack;
    public int physical_attack_max;
    public int physical_attack_rate;
    public int magic_attack;
    public int magic_attack_max;
    public int magic_attack_rate;
    public int physical_defence;
    public int physical_defence_max;
    public int physical_defence_rate;
    public int magic_defence;
    public int magic_defence_max;
    public int magic_defence_rate;
    public int hp;
    public int hp_max;
    public int hp_rate;
    public int mp;
    public int mp_max;
    public int mp_rate;
    public int movement;
    public int movement_max;
    public int movement_rate;
    public int cri;

    public List<SkillMaster> skillInfos;

    public CharaMaster(JSONObject jSONObject = null)
    {
        if (jSONObject == null) return;

        id = (int)jSONObject.GetField("id").n;
        charaName = jSONObject.GetField("name").str;
        description = jSONObject.GetField("description").str;
        type = (int)jSONObject.GetField("type").n;
        physical_attack = (int)jSONObject.GetField("physical_attack").n;
        physical_attack_max = (int)jSONObject.GetField("physical_attack_max").n;
        physical_attack_rate = (int)jSONObject.GetField("physical_attack_rate").n;
        magic_attack = (int)jSONObject.GetField("magic_attack").n;
        magic_attack_max = (int)jSONObject.GetField("magic_attack_max").n;
        magic_attack_rate = (int)jSONObject.GetField("magic_attack_rate").n;
        physical_defence = (int)jSONObject.GetField("physical_defence").n;
        physical_defence_max = (int)jSONObject.GetField("physical_defence_max").n;
        physical_defence_rate = (int)jSONObject.GetField("physical_defence_rate").n;
        magic_defence = (int)jSONObject.GetField("physical_defence").n;
        magic_defence_max = (int)jSONObject.GetField("physical_defence").n;
        magic_defence_rate = (int)jSONObject.GetField("physical_defence").n;
        hp = (int)jSONObject.GetField("hp").n;
        hp_max = (int)jSONObject.GetField("hp_max").n;
        hp_rate = (int)jSONObject.GetField("hp_rate").n;
        mp = (int)jSONObject.GetField("mp").n;
        mp_max = (int)jSONObject.GetField("mp_max").n;
        mp_rate = (int)jSONObject.GetField("mp_rate").n;
        movement = (int)jSONObject.GetField("movement").n;
        movement_max = (int)jSONObject.GetField("movement_max").n;
        movement_rate = (int)jSONObject.GetField("movement_rate").n;
        cri = (int)jSONObject.GetField("cri").n;
        skillInfos = new List<SkillMaster>();
        skillInfos.Add(DataManager.Instance.GetSkillMaster((int)jSONObject.GetField("skill1_master_id").n));
        skillInfos.Add(DataManager.Instance.GetSkillMaster((int)jSONObject.GetField("skill2_master_id").n));
        skillInfos.Add(DataManager.Instance.GetSkillMaster((int)jSONObject.GetField("skill3_master_id").n));

        JsonConsole.ExploriorLog("Response", "CharaMaster/id: " + id + " : " + charaName, jSONObject.ToString(), JsonConsole.ExploriorLogType.CharaMaster);

    }
}

//スキルマスター
public class SkillMaster
{
    public int id { get; private set; }
    public string skillName { get; private set; }
    public ActionMaster actionMaster { get; private set; }
    public AttackMotionMaster.Param attackMotionInfo { get; private set; }

    public SkillMaster(JSONObject jSONObject)
    {
        if (jSONObject == null) return;

        id = (int)jSONObject.GetField("id").n;
        skillName = jSONObject.GetField("name").str;
        actionMaster = DataManager.Instance.GetActionMaster((int)jSONObject.GetField("action_id").n);
        attackMotionInfo = DataManager.Instance.GetAttackMotionInfo((int)jSONObject.GetField("effect_id").n);

        JsonConsole.ExploriorLog("Response", "SkillMaster/id: " + id + " : " + skillName, jSONObject.ToString(), JsonConsole.ExploriorLogType.SKillMaster);
    }
}

public class ItemMaster
{
    public int id { get; private set; }
    public string itemName { get; private set; }
    public int rare { get; private set; }
    public string description { get; private set; }
    public int type { get; private set; }
    public int detail_type { get; private set; }

    public ItemMaster(JSONObject jSONObject)
    {
        if (jSONObject == null) return;      
        id = (int)jSONObject.GetField("id").n;
        itemName = jSONObject.GetField("name").str;
        rare = (int)jSONObject.GetField("rare").n;
        description = jSONObject.GetField("description").str;
        type = (int)jSONObject.GetField("type").n;
        detail_type = (int)jSONObject.GetField("detail_type").n;

        JsonConsole.ExploriorLog("Response", "ItemMaster/id: " + id + " : " + itemName, jSONObject.ToString(), JsonConsole.ExploriorLogType.ItemMaster);
    }
}


public class ItemCommonInfo
{
    public ItemType itemType { get; private set; }
    public ItemInfo itemInfo { get; private set; }
    public WeaponInfo weaponInfo { get; private set; }

    public ItemBase itemBase
    {
        get
        {
            switch(itemType)
            {
                case ItemType.ConsumeItem: return itemInfo;
                case ItemType.WeaponItem: return weaponInfo;
                default: return new ItemBase();
            }
        }
    }

    public ItemCommonInfo(ItemType _itemType, ItemInfo _itemInfo)
    {
        itemType = _itemType;
        itemInfo = _itemInfo;
    }

    public ItemCommonInfo(ItemType _itemType, WeaponInfo _weaponInfo)
    {
        itemType = _itemType;
        weaponInfo = _weaponInfo;
    }
}

public class ItemBase
{
    public int number;
    public int id;
    public string itemName;
    public string itemDiscription;
}


public class ItemInfo : ItemBase
{
    public ItemMaster itemMaster { get; private set; }

    public ItemInfo(JSONObject jSONObject = null)
    {
        if (jSONObject == null) return;

        //アイテムは量でまとめるけど武器はまとめないって認識
        number = (int)jSONObject.GetField("num").n;
        id = (int)jSONObject.GetField("item_id").n;
        itemMaster = DataManager.Instance.GetItemMaster(id);
        itemName = itemMaster.itemName;
        itemDiscription = itemMaster.description;

        //JsonConsole.ExploriorLog("Response", "ItemInfo/id: " + id + " : " + itemMaster.itemName, jSONObject.ToString(), JsonConsole.ExploriorLogType.QuestResult);
    }
}


public class WeaponInfo : ItemBase
{
    public WeaponMaster weaponMaster { get; private set; }
    public int lv { get; private set; }

    public int physical_attack { get; private set; }
    public int magic_attack { get; private set; }
    public int physical_defence { get; private set; }
    public int magic_defence { get; private set; }
    public int hp { get; private set; }
    public int mp { get; private set; }
    public int movement { get; private set; }
    public int cri { get; private set; }

    public WeaponInfo(JSONObject jSONObject = null)
    {
        if (jSONObject == null) return; 

        lv = (int)jSONObject.GetField("lv").n;
        number = (int)jSONObject.GetField("number").n;
        id = (int)jSONObject.GetField("weapon_id").n;
        weaponMaster = DataManager.Instance.GetWeaponMaster(id);
        itemName = weaponMaster.weaponName;
        itemDiscription = weaponMaster.description;
        physical_attack = CalculationManager.GetStatusValue(weaponMaster.physical_attack_max, weaponMaster.physical_attack, weaponMaster.physical_defence_rate,lv);
        magic_attack = CalculationManager.GetStatusValue(weaponMaster.magic_attack_max, weaponMaster.magic_attack, weaponMaster.magic_attack_rate, lv);
        physical_defence = CalculationManager.GetStatusValue(weaponMaster.physical_defence_max, weaponMaster.physical_defence, weaponMaster.physical_defence_rate, lv);
        magic_defence = CalculationManager.GetStatusValue(weaponMaster.magic_defence_max, weaponMaster.magic_defence, weaponMaster.magic_defence_rate, lv);
        hp = CalculationManager.GetStatusValue(weaponMaster.hp_max, weaponMaster.hp, weaponMaster.hp_rate, lv);
        mp = CalculationManager.GetStatusValue(weaponMaster.mp_max, weaponMaster.mp, weaponMaster.mp_rate, lv);
        movement = CalculationManager.GetStatusValue(weaponMaster.movement_max, weaponMaster.movement, weaponMaster.movement_rate, lv);
        cri = CalculationManager.GetStatusValue(weaponMaster.cri_max, weaponMaster.cri, weaponMaster.cri_rate, lv);

        JsonConsole.ExploriorLog("Response", "WeaponInfo/id: " + id + " : " + weaponMaster.weaponName, jSONObject.ToString(), JsonConsole.ExploriorLogType.QuestResult);
    }

    public WeaponInfo GetDebug(int _id = 1)
    {
        return new WeaponInfo(null)
        {
            id = _id,
            number = 1,
            lv = 3,
            weaponMaster = DataManager.Instance.GetWeaponMaster(_id)
        };
    }
}


public class WeaponMaster
{
    public int id { get; private set; }
    public string weaponName { get; private set; }
    public int rare { get; private set; }
    public string description { get; private set; }
    public WeaponType type { get; private set; }

    public int physical_attack { get; private set; }
    public int physical_attack_max { get; private set; }
    public float physical_attack_rate { get; private set; }
    public int magic_attack { get; private set; }
    public int magic_attack_max { get; private set; }
    public float magic_attack_rate { get; private set; }
    public int physical_defence { get; private set; }
    public int physical_defence_max { get; private set; }
    public float physical_defence_rate { get; private set; }
    public int magic_defence { get; private set; }
    public int magic_defence_max { get; private set; }
    public float magic_defence_rate { get; private set; }
    public int hp { get; private set; }
    public int hp_max { get; private set; }
    public float hp_rate { get; private set; }
    public int mp { get; private set; }
    public int mp_max { get; private set; }
    public float mp_rate { get; private set; }
    public int movement { get; private set; }
    public int movement_max { get; private set; }
    public float movement_rate { get; private set; }
    public int cri { get; private set; }
    public int cri_max { get; private set; }
    public float cri_rate { get; private set; }



    public WeaponMaster(JSONObject jSONObject)
    {
        if (jSONObject == null) return;

        id = (int)jSONObject.GetField("id").n;
        weaponName = jSONObject.GetField("name").str;
        rare = (int)jSONObject.GetField("rare").n;
        description = jSONObject.GetField("description").str;
        type = (WeaponType)jSONObject.GetField("type").n;

        physical_attack = (int)jSONObject.GetField("physical_attack").n;
        physical_attack_max = (int)jSONObject.GetField("physical_attack_max").n;
        physical_attack_rate = jSONObject.GetField("physical_attack_rate").n / 10f;

        magic_attack = (int)jSONObject.GetField("magic_attack").n;
        magic_attack_max = (int)jSONObject.GetField("magic_attack_max").n;
        magic_attack_rate = jSONObject.GetField("magic_attack_rate").n/10f;

        physical_defence = (int)jSONObject.GetField("physical_defence").n;
        physical_defence_max = (int)jSONObject.GetField("physical_defence_max").n;
        physical_defence_rate = jSONObject.GetField("physical_defence_rate").n / 10f;

        magic_defence = (int)jSONObject.GetField("magic_defence").n;
        magic_defence_max = (int)jSONObject.GetField("magic_defence_max").n;
        magic_defence_rate = jSONObject.GetField("magic_defence_rate").n / 10f;

        hp = (int)jSONObject.GetField("hp").n;
        hp_max = (int)jSONObject.GetField("hp_max").n;
        hp_rate = jSONObject.GetField("hp_rate").n / 10f;

        mp = (int)jSONObject.GetField("mp").n;
        mp_max = (int)jSONObject.GetField("mp_max").n;
        mp_rate = jSONObject.GetField("mp_rate").n / 10f;

        movement = (int)jSONObject.GetField("movement").n;
        movement_max = (int)jSONObject.GetField("movement_max").n;
        movement_rate = jSONObject.GetField("movement_rate").n/ 10f;

        cri = (int)jSONObject.GetField("cri").n;
        cri_max = (int)jSONObject.GetField("cri_max").n;
        cri_rate = jSONObject.GetField("cri_rate").n /10f;

        JsonConsole.ExploriorLog("Response", "WeaponMaster/id: " + id + " : " + weaponName, jSONObject.ToString(), JsonConsole.ExploriorLogType.WeaponMaster);
    }
}

public class BattleResultInfo
{
    public List<WeaponInfo> weaponInfos { get; private set; }
    public List<ItemInfo> itemInfos { get; private set; }

    public BattleResultInfo
        (
            List<WeaponInfo> _weaponInfos,
            List<ItemInfo> _itemInfos
        )
    {
        weaponInfos = _weaponInfos;
        itemInfos = _itemInfos;
    }

}


public class BattleActionInfo
{
    public SkillMaster skillMaster { get; private set; }
    public CharacterBase characterBase { get; private set; }

    public BattleActionInfo(SkillMaster _skillMaster, CharacterBase _characterBase)
    {
        skillMaster = _skillMaster;
        characterBase = _characterBase;
    }

}

public class TWAnimationData
{
    public string stateName;
    public string stateFullPath;
    public bool endFlag;
    public AnimatorStateInfo stateInfo;
    public CharastateType charastateType
    {
        get
        {
            foreach (var Value in Enum.GetValues(typeof(CharastateType)))
                if (Value.ToString() == stateName)
                    return (CharastateType)Value;

            return CharastateType.none;
        }
    }
}
