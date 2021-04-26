using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Layout_SelectColosseum : ScenePrefab {


    [SerializeField] List<Transform> objectBaseTFList = new List<Transform>();
    [SerializeField] SpriteRenderer backGround;
    [SerializeField] List<GameObject> contentsObject;

    [SerializeField] GameObject baseObject;
    [SerializeField] Transform parentTF;

    [SerializeField] GameObject centerGO;

    private void Start()
    {
        StartCoroutine(StartOn());
    }

    public IEnumerator StartOn()
    {
        iphoneXAjust(centerGO);
        AddPopup("Popup_AlphaLoding");
        QuestAPISetting.LoadQuestList(QuestAPISetting.QuestType.arena, () =>
        {
            CharaAPISetting.LoadRealCharaData(UserData.GetUserID(), () =>
            {
                FriendAPISetting.GetFriendForBattle(() =>
                {
                    ResourceLoaderOrigin.GetBackGroundImage(12, (bgobj) =>
                    {
                        backGround.sprite = bgobj;
                        AlphaLoding.Close();
                        BgmManager.Instance.Play(ResourceLoaderOrigin.GetBGMnameById(7));
                        StartCoroutine(SetStart());
                    });

                });

            });
        });
        yield break;
    }


    public void SetInfo()
    {
        iphoneXAjust(centerGO);
        int count = 0;
        foreach( var Value in contentsObject )
        {
            Value.transform.Find("kiroku").GetComponent<TextMeshProUGUI>().text = "最高到達点\nwave " + UserData.GetColosseumNumber(count);

            count++;
        }

    }

    public IEnumerator SetStart()
    {
        AddSubLayout("Footer");
        AddSubLayout("Header");

        SetImage();

        yield return null;
    }

    public void SetImage()
    {
        foreach( var Value in QuestAPISetting.realQuestSettings[0].realQuestDetails)
        {
            var newGO = Instantiate(baseObject, parentTF);
            var newTF = newGO.transform;
            newTF.GetComponent<EmergencyDetailController>().Init(Value);  
            newGO.SetActive(true);  
        }
        baseObject.SetActive(false);
        int myLv = UserData.GetUserRank(); 

    }

    public void Back()
    {
        ChangeLayout("SelectQuest");
    }
}
