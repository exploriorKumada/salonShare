using System.Collections;
using System.Collections.Generic;
using TW.GameSetting;
using System;
using UnityEngine;
using Explorior;
using System.Linq;

public class ResultManager : SystemBaseManager
{

    BattleResultInfo resultInfo;

    [SerializeField] GameObject rewardBase;

    private void Start()
    {
        Loding(() =>
        {
            if (DataManager.Instance.currentChangeSceneInfo == null)
            {
                DebugResult(ResouceLoad);
            }
            else
            {
                resultInfo = (BattleResultInfo)DataManager.Instance.currentChangeSceneInfo.data_hash["resultInfo"];
                ResouceLoad();
            }
        });    
    }

    public void ResouceLoad()
    {
        ResourceManager.Instance.LoadWeaponModels(resultInfo.weaponInfos, () =>
        {
            ResourceManager.Instance.LoadItemModels(resultInfo.itemInfos, () =>
            {
                Initialize();
            });
        });
    }

    public void Initialize()
    {
        DebugLog(true, "ResultManager Initialize");

        StartCoroutine(RewardSet());
    }

    private IEnumerator RewardSet()
    {
        rewardBase.ParentInitialize();

        foreach(var Value in resultInfo.weaponInfos)
        {
            var weapon = Instantiate(rewardBase, rewardBase.transform.parent);
            weapon.SetActive(true);

            weapon.name = Value.id.ToString();
           
            //配下に武器モデルをおく
            weapon.GetComponent<GeneralData>().gameObejcets.First().Value.GetComponent<ItemIcon>().Initialize(new ItemCommonInfo(ItemType.WeaponItem, Value));

            yield return null;
        }

        foreach(var Value in resultInfo.itemInfos)
        {
            var itemModel = Instantiate(rewardBase, rewardBase.transform.parent);
            itemModel.SetActive(true);

            itemModel.name = Value.id.ToString();

            //配下に武器モデルをおく
            itemModel.GetComponent<GeneralData>().gameObejcets.First().Value.GetComponent<ItemIcon>().Initialize(new ItemCommonInfo(ItemType.ConsumeItem, Value));


            yield return null;
        }
    }


    public void SetHome()
    {
        ChangeScene(SceneType.FreeHome);
    }

    public void SetNeXtQuest()
    {

    }

    public void DebugResult( Action callBack )
    {
        Dictionary<string, object> questStartData = new Dictionary<string, object>();
        questStartData.Add("team_id", 1);
        questStartData.Add("quest_id", 1);
        questStartData.Add("quest_detail_id", 2);

        DataManager.Instance.QuestStart(questStartData, () =>
        {
            Dictionary<string, object> questFinishData = new Dictionary<string, object>();
            questFinishData.Add("team_id", 1);
            questFinishData.Add("quest_id", 1);
            questFinishData.Add("quest_detail_id", 2);
            DataManager.Instance.QuestFinish(questFinishData, (_resultInfo) =>
            {
                resultInfo = _resultInfo;
                callBack.Invoke();
            });
        });
    }

}
