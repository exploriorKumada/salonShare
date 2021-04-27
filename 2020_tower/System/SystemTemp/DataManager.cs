using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HMLabs.JsonConsole;
using PW;
using TW.GameSetting;
using UniRx;
using UnityEngine;

public class DataManager : NLSingletonDontDestroyObject<DataManager>
{
    AttackMotionMaster attackMotionMaster;
    EffectMaster effectMaster;

    TextMaster textMaster;

    //[SerializeField] MoveAnimationMaster moveAnimationMaster;


    public List<CharaMaster> CharaMasters { get; private set; }
    public CharaMaster GetCharaMaster(int id) => CharaMasters.FirstOrDefault(x => x.id == id);

    public List<ActionMaster> ActionMasters { get; private set; }
    public ActionMaster GetActionMaster(int id) => ActionMasters.FirstOrDefault(x => x.id == id);

    public List<SkillMaster> SkillMasters { get; private set; }
    public SkillMaster GetSkillMaster(int id) => SkillMasters.FirstOrDefault(x => x.id == id);

    public List<QuestMaster> QuestMasters { get; private set; }

    public List<ItemMaster> ItemMasters { get; private set; }
    public ItemMaster GetItemMaster(int id) => ItemMasters.FirstOrDefault(x => x.id == id);

    public List<WeaponMaster> WeaponMasters { get; private set; }
    public WeaponMaster GetWeaponMaster(int id) => WeaponMasters.FirstOrDefault(x => x.id == id);

    //所持キャラ情報
    public List<CharaInfo> userCharaInfos { get; private set; }
    //チーム情報
    public List<TeamPreData> userTeamPreInfos { get; private set; }
    public List<CharaInfo> userTeamCharaInfos(int num) => userTeamPreInfos[num].charaInfos;//.DeepClone();
    //所持アイテム情報
    public List<ItemInfo> userItemInfos { get; private set; } = new List<ItemInfo>();
    //所持武器情報
    public List<WeaponInfo> userWeaponInfos { get; private set; } = new List<WeaponInfo>();

    public bool isMasterLoad { get; private set; }

    public ChangeSceneInfo currentChangeSceneInfo;

    private void Start()
    {
        attackMotionMaster = Resources.Load<AttackMotionMaster>("Master/AttackMotionMaster");
        effectMaster = Resources.Load<EffectMaster>("Master/EffectMaster");
        textMaster = Resources.Load<TextMaster>("Master/TextMaster");
    }


    public async Task MasterLoad()
    {
        //アイテムマスター
        await StartCoroutine(ItemMasterLoad());

        //キャラマスター
        await StartCoroutine(CharaMasterLoad());

        //クエストマスター
        await StartCoroutine(QuestMasterLoad());

        //プレイヤー情報
        await StartCoroutine(LoadIndex());


        isMasterLoad = true;
    }

    public AttackMotionMaster.Param GetAttackMotionInfo(int id) => attackMotionMaster.sheets.First().list.FirstOrDefault(x => x.id == id);
    public EffectMaster.Param GetEffectInfo(int id) => effectMaster.sheets.First().list.FirstOrDefault(x => x.id == id);
    public EffectMaster.Param GetEffectInfo(string name) => effectMaster.sheets.First().list.FirstOrDefault(x => x.name == name);
    public TextMaster.Param GetText(int id) => textMaster.sheets.First().list.FirstOrDefault(x => x.id == id);

    public IEnumerator LoadIndex()
    {
        return APIManager.Instance._StartInfoAPI(APIType.load, APIDetail.index, new Dictionary<string, object>(), (result) =>
        {
            var dataJson = new JSONObject(result.text).GetField("data");

            GetUserCharaInfo(dataJson);

            GetUserItemInfo(dataJson);
        });

    }


    /// <summary>
    /// キャラマスターロード
    /// </summary>
    /// <returns></returns>
    public IEnumerator CharaMasterLoad()
    {
        return APIManager.Instance._StartInfoAPI(APIType.master, APIDetail.chara_data, new Dictionary<string, object>(), (result) =>
        {
            var dataJson = new JSONObject(result.text).GetField("data");

            JSONObject resultActionJson = dataJson.GetField("actions");
            ActionMasters = new List<ActionMaster>();
            resultActionJson.list.ForEach(x => ActionMasters.Add(new ActionMaster(x)));

            JSONObject resultSkillJson = dataJson.GetField("skills");
            SkillMasters = new List<SkillMaster>();
            resultSkillJson.list.ForEach(x => SkillMasters.Add(new SkillMaster(x)));

            JSONObject resultJson = dataJson.GetField("charas");
            CharaMasters = new List<CharaMaster>();
            resultJson.list.ForEach(x => CharaMasters.Add(new CharaMaster(x)));


        });


    }

    public CharaMaster GetByCharaId(int charaId)
    {
        var charaMaster = CharaMasters.FirstOrDefault(x => x.id == charaId);

        if (charaMaster == null)
        {
            Debug.LogError("マスターないです：" + charaId);
        }

        return charaMaster;
    }

    /// <summary>
    /// クエストロード
    /// </summary>
    public IEnumerator QuestMasterLoad()
    {
        return APIManager.Instance._StartInfoAPI(APIType.master, APIDetail.quest_data, new Dictionary<string, object>(), (result) =>
        {
            JSONObject resultJson = new JSONObject(result.text).GetField("data");
            JSONObject questJson = resultJson.GetField("quest");
            JSONObject querstDetaJson = resultJson.GetField("quest_detail");
            JSONObject querstWaveJson = resultJson.GetField("quest_wave");

            //wave
            //Debug.Log(querstWaveJson);
            List<QuestWaveInfo> questWaveInfos = new List<QuestWaveInfo>();
            foreach (var Value in querstWaveJson.list)
            {
                QuestWaveInfo questDetailInfo = new QuestWaveInfo(Value);
                questWaveInfos.Add(questDetailInfo);
            }

            //detail
            List<QuestDetailInfo> questDetailInfos = new List<QuestDetailInfo>();
            foreach (var Value in querstDetaJson.list)
            {
                QuestDetailInfo questDetailInfo = new QuestDetailInfo()
                {
                    questDetailId = (int)Value.GetField("id").n,
                    questId = (int)Value.GetField("quest_id").n,
                    questDetailName = Value.GetField("name").ToString(),
                    user_stamina = (int)Value.GetField("user_stamina").n,
                    experience_point = (int)Value.GetField("experience_point").n,
                    gold = (int)Value.GetField("gold").n,
                    proper_level = (int)Value.GetField("proper_level").n
                };

                questDetailInfo.questWaveInfos.AddRange(questWaveInfos.Where(x => x.quest_detail_id == questDetailInfo.questDetailId));
                questDetailInfos.Add(questDetailInfo);
            }

            //master
            QuestMasters = new List<QuestMaster>();
            foreach (var Value in questJson.list)
            {
                QuestMaster questMaster = new QuestMaster()
                {
                    questId = (int)Value.GetField("id").n,
                    questName = Value.GetField("name").ToString(),
                    questType = (QuestType)Enum.ToObject(typeof(QuestType), ((int)Value.GetField("type").n))
                };

                questMaster.questDetailInfos.AddRange(questDetailInfos.Where(x => x.questId == questMaster.questId));
                QuestMasters.Add(questMaster);
            }
        });
    }

    /// <summary>
    /// クエスト詳細ロード
    /// </summary>
    public IEnumerator QuestDetailMasterLoad()
    {
        return APIManager.Instance._StartInfoAPI(APIType.master, APIDetail.quest_data, new Dictionary<string, object>(), (result) =>
        {
            JSONObject resultJson = new JSONObject(result.text).GetField("data");

            foreach (var Value in resultJson.list)
            {
                QuestDetailInfo questDetailInfo = new QuestDetailInfo()
                {
                    questDetailId = (int)Value.GetField("id").n,
                    questId = (int)Value.GetField("quest_id").n,
                    questDetailName = Value.GetField("name").ToString(),
                    user_stamina = (int)Value.GetField("user_stamina").n,
                    experience_point = (int)Value.GetField("experience_point").n,
                    gold = (int)Value.GetField("gold").n,
                    proper_level = (int)Value.GetField("proper_level").n
                };
                QuestMasters.FirstOrDefault(item => item.questId == questDetailInfo.questId).questDetailInfos.Add(questDetailInfo);
            }

        });
    }


    public IEnumerator ItemMasterLoad()
    {
        return APIManager.Instance._StartInfoAPI(APIType.master, APIDetail.item_data, new Dictionary<string, object>(), (result) =>
        {
            JSONObject resultJson = new JSONObject(result.text).GetField("data");

            JSONObject itemJson = resultJson.GetField("item");
            JSONObject weaponJson = resultJson.GetField("weapon");

            ItemMasters = new List<ItemMaster>();
            itemJson.list.ForEach(x => ItemMasters.Add(new ItemMaster(x)));

            WeaponMasters = new List<WeaponMaster>();
            weaponJson.list.ForEach(x => WeaponMasters.Add(new WeaponMaster(x)));


        });
    }




    //所持キャラ
    public void GetUserCharaInfo(JSONObject dataJson)
    {
        userCharaInfos = GetCharaInfos(dataJson.GetField("chara_array"));
        JsonConsole.ExploriorLog("Response", "chara_array", dataJson.GetField("chara_array").ToString(), JsonConsole.ExploriorLogType.UserCharaInfo);

        userTeamPreInfos = GetTeamPreDatas(dataJson.GetField("team_array"));
        JsonConsole.ExploriorLog("Response", "team_array", dataJson.GetField("team_array").ToString(), JsonConsole.ExploriorLogType.UserCharaInfo);
    }

    public void GetUserItemInfo(JSONObject dataJson)
    {

        JsonConsole.ExploriorLog("Response", "weapon_list", dataJson.GetField("weapon_list").ToString(), JsonConsole.ExploriorLogType.UserItemInfo);

        userWeaponInfos = GetWeaponInfos(dataJson.GetField("weapon_list"), "GetUserItemInfo");

        JsonConsole.ExploriorLog("Response", "item_list", dataJson.GetField("item_list").ToString(), JsonConsole.ExploriorLogType.UserItemInfo);

        userItemInfos = GetItemInfos(dataJson.GetField("item_list"), "GetUserItemInfo");

    }




    public List<CharaInfo> GetCharaInfos(JSONObject jSONObject)
    {
        List<CharaInfo> charaInfos = new List<CharaInfo>();
        foreach (var Value in jSONObject.list)
        {
            CharaInfo charaInfo = new CharaInfo(
                CharaMasters.FirstOrDefault(x => x.id == (int)Value.GetField("chara_id").n),
                (int)Value.GetField("lv").n);
            charaInfos.Add(charaInfo);
        }
        return charaInfos;
    }

    //チーム設定
    public List<TeamPreData> GetTeamPreDatas(JSONObject jSONObject)
    {
        List<TeamPreData> teamPreDatas = new List<TeamPreData>();

        foreach (var KV in jSONObject.GetDictionaryUsingJson())
        {
            TeamPreData teamPostData = new TeamPreData();
            teamPostData.teamId = Int32.Parse(KV.Key);
            teamPostData.charaInfos = new List<CharaInfo>();

            foreach (var Value in KV.Value.list)
            {
                CharaInfo charaInfo = userCharaInfos.FirstOrDefault(x => x.id == (int)Value.GetField("chara_id").n);
                teamPostData.charaInfos.Add(charaInfo);
            }

            teamPreDatas.Add(teamPostData);

        }

        return teamPreDatas;
    }

    public List<ItemInfo> GetItemInfos(JSONObject jSONObject, string logMemo)
    {
        List<ItemInfo> itemInfos = new List<ItemInfo>();
        foreach (var Value in jSONObject.list)
        {
            var itemInfo = new ItemInfo(Value);
            itemInfos.Add(itemInfo);
            JsonConsole.ExploriorLog("Response", logMemo + " : weapon id:" + itemInfo.id + " number:" + itemInfo.number, Value.ToString(), JsonConsole.ExploriorLogType.UserItemInfo);
        }
        return itemInfos;
    }

    public List<WeaponInfo> GetWeaponInfos(JSONObject jSONObject, string logMemo)
    {
        List<WeaponInfo> weaponInfos = new List<WeaponInfo>();
        foreach (var Value in jSONObject.list)
        {
            var weaponInfo = new WeaponInfo(Value);
            weaponInfos.Add(weaponInfo);
            JsonConsole.ExploriorLog("Response", logMemo + " : weapon id:" + weaponInfo.id + " number:" + weaponInfo.number + " level:" + weaponInfo.lv, Value.ToString(), JsonConsole.ExploriorLogType.UserItemInfo);
        }
        return weaponInfos;
    }


    //クエスト開始
    public void QuestStart(Dictionary<string, object> datas, Action callBack) => StartCoroutine(QuestStartOn(datas, callBack));
    private IEnumerator QuestStartOn(Dictionary<string, object> datas, Action callBack)
    {
        return APIManager.Instance._StartInfoAPI(APIType.quest, APIDetail.start, datas, (result) =>
        {
            JSONObject resultJson = new JSONObject(result.text).GetField("data");
            callBack?.Invoke();
        });
    }


    //クエスト終了
    public void QuestFinish(Dictionary<string, object> datas, Action<BattleResultInfo> callBack) => StartCoroutine(QuestFinishOn(datas, callBack));
    private IEnumerator QuestFinishOn(Dictionary<string, object> datas, Action<BattleResultInfo> callBack)
    {
        return APIManager.Instance._StartInfoAPI(APIType.quest, APIDetail.finish, datas, (result) =>
        {
            JSONObject resultJson = new JSONObject(result.text).GetField("data");
            JSONObject weaponJson = resultJson.GetField("weapon_list");
            JSONObject itemJson = resultJson.GetField("item_list");

            //獲得後の所持武器
            var afterWeapons = GetWeaponInfos(weaponJson, "QuestFinishOn　weapon");
            var resultWeapons = new List<WeaponInfo>();
            foreach (var Value in afterWeapons)
            {
                //元々の所持武器差分だけ追加
                if (userWeaponInfos.FirstOrDefault(x => x.number == Value.number) == null)
                {
                    resultWeapons.Add(Value);
                }
            }

            //獲得後の所持アイテム
            var afterItems = GetItemInfos(itemJson, "QuestFinishOn item");
            var resultItems = new List<ItemInfo>();
            foreach (var Value in afterItems)
            {
                //アイテム差分だけ追加
                var userItemInfo = userItemInfos.FirstOrDefault(x => x.id == Value.id);
                if (userItemInfo == null)
                {
                    resultItems.Add(Value);
                }
                else if(userItemInfo.number != Value.number)
                {
                    //持ってたけど数が増えた場合
                    Value.number = Value.number - userItemInfo.number;
                    resultItems.Add(Value);
                }
            }



            callBack?.Invoke(

                 new BattleResultInfo(resultWeapons, resultItems) //情報挿入

                 );
        });
    }


}
