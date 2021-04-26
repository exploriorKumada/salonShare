using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Layout_Battle : ScenePrefab {

	private bool autoFlag = false;

    [SerializeField] AssetBundler assetBundler;
    [SerializeField] BattleManager battleManager;
    [SerializeField] Transform backButton;
    [SerializeField] SpriteRenderer backGround;
    [SerializeField] BattleLayoutManager battleLayoutManager;
    [SerializeField] GameObject centerGO;
    [SerializeField] GameObject centerpopGO;

    public Dictionary<int, Dictionary<int, RealCharaData>> partyCharaDataList = new Dictionary<int, Dictionary<int, RealCharaData>>();

    public static RealQuestDetail realQuestDetail;
    public static int teamNo;

    [SerializeField] List<GameObject> debugObject = new List<GameObject>();

    [SerializeField] List<GameObject> tutoNoObject = new List<GameObject>();

    void Start () {

        foreach(var Value in debugObject)
            Value.SetActive(Debug.isDebugBuild);

        foreach (var Value in tutoNoObject)
            Value.SetActive(!(UserData.GetUserName() == ""));

        StartCoroutine(SetStart());
	}


    public IEnumerator SetStart()
    {

        iphoneXAjust(centerGO,0.8f);
        iphoneXAjust(centerpopGO, 0.8f);

        if (UserData.GetUserName() == "")
        {
            teamNo = 1;
            Debug.Log("チュートリアル");
            TutorialData.SetTutorialData();
            realQuestDetail = TutorialData.realQuestDetail;
            battleManager.realQuestDetail = TutorialData.realQuestDetail;
            partyCharaDataList = TutorialData.partyCharaDataList;
            battleManager.playerParty = new List<RealCharaData>(partyCharaDataList[teamNo].Values);
            backGround.sprite = Resources.Load<Sprite>("Tuto/back_8");
            GameStart();
            yield break;
        }



        if (realQuestDetail != null)
        {
            battleManager.realQuestDetail = realQuestDetail;
        }else
        {
            Debug.LogError("realQuestDetail is null");
        }
           
        AddPopup("Popup_AlphaLoding");
        QuestStartAPISetting.LoadQuestStart(realQuestDetail.quest_detail_id, () =>
        {
            CharaAPISetting.LoadRealCharaData(UserData.GetUserID(), () =>
            {
                CharaAPISetting.GetLeaderAllLeaderCharacter(() =>                       
                {
                    partyCharaDataList = CharaAPISetting.partyCharaDataList;
                    battleManager.playerParty = new List<RealCharaData>(partyCharaDataList[teamNo].Values);
                    ResourceLoaderOrigin.GetEfectAll(() =>
                    {
                        ResourceLoaderOrigin.GetBackGroundImage(realQuestDetail.worldImageId, (bgobj) =>
                         {
                             backGround.sprite = bgobj;
                             battleManager.BattleAssetInstall(() =>
                             {
                                 AlphaLoding.Close();
                                 Debug.Log("Game Start!!!!!!!!!!");
                                 GameStart();
                             });
                         });
    
                    });
                                           
                });                
            });
         });
        yield return null;
    }

    public void GameStart()
    {
        BgmManager.Instance.Play("05.Battle");
        autoFlag = false;
        Debug.Log("realQuestDetail.worldImageId:" + realQuestDetail.worldImageId);

        if(realQuestDetail.worldImageId==0)
        {
            Debug.LogError("realQuestDetail.worldImageId is 0");
            realQuestDetail.worldImageId = 1;
        }

        battleManager.InitStart();
    }


    public void TransScene()
    {
        ChangeLayout("Menu"); 
    }



    public void OpenBattleMenu()
    {

        if (battleManager.actionFlag)
            return;
        
        battleLayoutManager.popup_BattleMenu.SetImage(1);
    }


}
