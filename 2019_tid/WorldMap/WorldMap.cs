using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;
using DG.Tweening;

public class WorldMap : ScenePrefab {

    [SerializeField] GameObject baseObject;
    [SerializeField] Transform baseTF;
    [SerializeField] TextMeshProUGUI worldName;

    [SerializeField] GameObject right;
    [SerializeField] GameObject left;
    [SerializeField] SpriteRenderer backGround;

    [SerializeField] GameObject questTitle;

    [SerializeField] GameObject centerGo;

	int userLevel;

    public Dictionary<int, Dictionary<int, RealCharaData>> partyCharaDataList = new Dictionary<int, Dictionary<int, RealCharaData>>();
    public List<RealCharaData> charaDataList = new List<RealCharaData>();

    [System.NonSerialized]public int mapID = 0;

    void Start()
    {
        StartCoroutine(SetStart());
        UIdirectionAnimation(right.transform, 1);
        UIdirectionAnimation(left.transform, 0);
        iphoneXAjust(centerGo);
    }


    public IEnumerator SetStart () {

        //load
        AddPopup("Popup_AlphaLoding");
        CharaAPISetting.LoadRealCharaData(UserData.GetUserID(), () =>
        {
            QuestAPISetting.LoadQuestList(QuestAPISetting.QuestType.story,() =>
            {
                FriendAPISetting.GetFriendForBattle(() =>
                {
                    mapID = QuestAPISetting.mapInitNumber; Debug.Log("mapInitNumber:" + QuestAPISetting.mapInitNumber);

                    ResourceLoaderOrigin.GetBackGroundImage(QuestAPISetting.realQuestSettings[mapID].worldImageId, (bgobj) =>
                    {
                        backGround.sprite = bgobj;

                        AlphaLoding.Close();

                        //mapID初期値を設定

                        partyCharaDataList = CharaAPISetting.partyCharaDataList;
                        charaDataList = CharaAPISetting.charaDataList;
                        //AddSubLayout("Footer");
                        AddSubLayout("Header");

                        userLevel = UserData.GetUserRank();
                        StartCoroutine(SetImage());
                        UIcutin(questTitle.transform, 0, 200);

                    });

                });
              
            });
        });

        yield return null;
	}


    public IEnumerator SetImage()
	{
        BgmManager.Instance.Play(ResourceLoaderOrigin.GetBGMnameById(7));

        RealQuestSetting realQuestSetting = QuestAPISetting.realQuestSettings[mapID];

        right.SetActive(realQuestSetting.clear_flag == 1 && realQuestSetting != QuestAPISetting.realQuestSettings.Last());

        left.SetActive(mapID != 0);

   
        worldName.text = realQuestSetting.quest_name;
     

        for (int i = 0; i < baseTF.childCount; ++i)
            Destroy(baseTF.GetChild(i).gameObject);
        
        baseObject.SetActive(false);

        yield return new WaitForSeconds(0.3f);

        foreach( var Value in realQuestSetting.realQuestDetails )
        {
            var newGO = GameObject.Instantiate(baseObject,baseTF);
            var newGOTF = newGO.transform;

            newGOTF.localPosition = new Vector3( Value.display_position_x, Value.display_position_y, newGOTF.localPosition.z);
            newGO.SetActive(true);
            newGOTF.GetComponent<QuestController>().Init( Value );
            yield return new WaitForSeconds(0.1f);
        }

        Debug.Log(　realQuestSetting.quest_name+　":章クリアフラグ:" + realQuestSetting.clear_flag);

       

        yield return null;
	}


    public void PushEvent( int number )
    {
        mapID += number;

        if( mapID < 0 )
        {
            mapID = QuestAPISetting.realQuestSettings.Count-1;
        }

        if( mapID > QuestAPISetting.realQuestSettings.Count-1 )
        {
            mapID = 0;  
        }

        StopAllCoroutines();
        StartCoroutine(SetImage());
    }




}
