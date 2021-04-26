using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using Spine.Unity;
using System.Linq;

public class Layout_Menu : ScenePrefab {

    [SerializeField] GameObject equest;
    [SerializeField] SpriteRenderer backGround;
    [SerializeField] GameObject iconPresentNumberBase;
    [SerializeField] GameObject iconInfoNumberBase;

    [SerializeField] GameObject spineObject;
    [SerializeField] Transform sideUI;
    [SerializeField] Transform mapUI;

    [SerializeField] SkeletonAnimation skeletonAnimation;
    [SerializeField] TextMeshProUGUI leaderCharaName;
    [SerializeField] GameObject fukidashiObject;
    [SerializeField] TextMeshProUGUI fukidashiValue;

    [SerializeField] GameObject centerGO;

    [SerializeField] GameObject presentNumber;


    private bool ebnablePush = true;
    private int leaderCharaId;

    LeaderCharaSettingBase leaderCharaSettingBase;

    // Use this for initialization
    void Start () {

        BgmManager.Instance.Play("03.Orumia(town)");
        sideUI.gameObject.SetActive(false);
        mapUI.gameObject.SetActive(false);
        StartCoroutine(SetStart());
        //UIjumpRoop(presentNumber.transform);

    }

    public IEnumerator SetStart()
    {
        AddPopup("Popup_AlphaLoding");//ローディングポップアップ表示
        AddSubLayout("Footer");
        AddSubLayout("Header");
        int loadCount = 0;
        //ユーザーデータロード
        UserDataSetting.LoadRealUserData(UserData.GetUserID(), () =>
        {
            LoginBonusSetting.LoginCheackOn(() =>
            {
                loadCount++;
            });
        });

        //Debug.Log("ユーザーデータロード完了：　duid：  " + UserDataSetting.returnValue.duid);
        QuestAPISetting.LoadQuestList(QuestAPISetting.QuestType.evemnt, () =>
        {
            //Debug.Log("緊急クエスト有無判定　完了");
            PresentSetting.LoadRealPresentData(UserData.GetUserID(), () =>
            {
                loadCount++;
            });
        });

        //Debug.Log("プレゼント数　完了");
        InformationSetting.LoadReadInfomationData(UserData.GetUserID(), 1, () =>
        {
            CharaAPISetting.GetLeaderAllLeaderCharacter(() =>
            {
                //                                Debug.Log("リーダー情報　完了");
                loadCount++;
               
            });
        });

        while (true)
        {
            if (loadCount >= 3)
                break;

            yield return null;
        }

        leaderCharaId = UserData.GetLeaderChara();
        ResourceLoaderOrigin.GetSpine(leaderCharaId, (obj) =>
         {
             ResourceLoaderOrigin.GetBackGroundImage(1, (bgobj) =>
             {
                 ResourceLoaderOrigin.GetBackGroundImage(11, (bgobjlogin) =>
                 {

                     if (backGround.sprite != null)
                         return;

                     backGround.sprite = bgobj;
                     AlphaLoding.Close();
                     equest.SetActive(QuestAPISetting.realQuestSettings.Count > 0);
                     StartCoroutine(SetLeaderCharaStatusInfo(obj));
                     SetImage();

                
                 });

             });

         });


        yield return null;
    }


    public void SetImage()
    {
        iphoneXAjust(centerGO);
        UserData.TutoSet(UserData.TutoType.mypage);
        UIcutin(sideUI,100f,0);
        UIcutin(mapUI, -100f, 0);

        Footer.action = () =>
        {
            UIcutOut(sideUI, 100f, 0);
            UIcutOut(mapUI, -100f, 0);
        };


        if(  PresentSetting.returnValueList.Count <= 0)
        {
            iconPresentNumberBase.SetActive(false);
        }else
        {
            iconPresentNumberBase.SetActive(true);
            UIjump(iconPresentNumberBase.transform);
            iconPresentNumberBase.transform.Find("number").GetComponent<TextMeshProUGUI>().text = "" + PresentSetting.returnValueList.Count;
        }

        if( InformationSetting.returnValueList.Count <= 0 )
        {
            iconInfoNumberBase.SetActive(false);
        }else
        {
            iconInfoNumberBase.SetActive(true);
            iconInfoNumberBase.transform.Find("number").GetComponent<TextMeshProUGUI>().text = "" + InformationSetting.returnValueList[0].publishData + "更新";
        }

        LoginBournus.data_LoginBonusList = LoginBonusSetting.data_LoginBonusList;

        if(LoginBournus.data_LoginBonusList.Count != 0)
        {
            if (!LoginBournus.data_LoginBonusList.First().setImage)
            {
                AddPopup("Popup_LoginBornus");
            }
        } 
    }

    public void ChangeLayoutOn( string layoutName )
    {
        ChangeLayout(layoutName);
    }




    Transform beforeTF;
    private IEnumerator SetLeaderCharaStatusInfo(SkeletonDataAsset skeletonDataAsset)
    {
        leaderCharaSettingBase = CharaAPISetting.leaderCharaDataList[leaderCharaId];

        leaderCharaName.text = leaderCharaSettingBase.charaName;
        fukidashiValue.text = leaderCharaSettingBase.tapComment;
        skeletonAnimation.skeletonDataAsset = skeletonDataAsset;
        skeletonAnimation.Initialize(true);
        skeletonAnimation.skeletonDataAsset.atlasAssets[0].materials[0].shader = Shader.Find("Spine/Special/SkeletonGhost");
        skeletonAnimation.Initialize(true);
        skeletonAnimation.timeScale = 0.6f;
        skeletonAnimation.transform.localPosition = new Vector3( -20, CharaSetting.charaSpineAjust[leaderCharaId], 0 );

        TalkChara();
        yield return new WaitForSeconds(0.1f);
        ebnablePush = true;
    }

    public void TransScene(string sceneName)
    {
        ChangeLayout(sceneName);
    }      

    float r, g, b;
    public void TalkChara()
    {        
        Singleton<SoundPlayer>.instance.CharaVoice( CharaAPISetting.GetCharaMasterDataByLeaderId(leaderCharaSettingBase.No).id,"select",true);
    }

}
