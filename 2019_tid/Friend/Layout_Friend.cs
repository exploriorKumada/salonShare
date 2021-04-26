using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class Layout_Friend : ScenePrefab {

    [SerializeField] GameObject friendList;
    [SerializeField] GameObject friendListByOsusume;

	[SerializeField] GameObject baseObject;
	[SerializeField] Transform parentTF;

    [SerializeField] GameObject baseObjectOsusume;
    [SerializeField] Transform parentTFOsusume;

	[SerializeField] GameObject ContentGO;
    [SerializeField] TextMeshProUGUI friendID;
    [SerializeField] TextMeshProUGUI inputId;

    [SerializeField] GameObject confirmPopup;
    [SerializeField] ConfirmPopup confirmPopupScript;

    [SerializeField] GameObject nothingObject;

    [SerializeField] GameObject centerGO;
    [SerializeField] SpriteRenderer backGround;

    [SerializeField] TextMeshProUGUI frinedLimit;

    // Use this for initialization
    void Start () {
        AddSubLayout("Footer");
        SetFriend(); ;
	}

    public void SetFriend()
    {
        AddPopup("Popup_AlphaLoding");
        UserDataSetting.LoadRealUserData(UserData.GetUserID(), () =>
        {
            FriendAPISetting.GetFriend(() =>
            {
                FriendAPISetting.GetFriendForBattle(() =>
                {
                    ResourceLoaderOrigin.GetBackGroundImage(1, (bgobj) =>
                    {
                        backGround.sprite = bgobj;
                        AlphaLoding.Close();
                        Debug.Log("LoadRealUserDat complite!!  " + UserDataSetting.returnValue.duid);
                   
                        StartCoroutine(SetStart());
                    });

                },true );

            });
        });
    }

    public IEnumerator SetStart()
    {
        iphoneXAjust(centerGO);
        UserData.TutoSet(UserData.TutoType.friend);
        friendID.text = "フレンドID：" + UserData.GetFriendID();
        SetImage(FriendAPISetting.realFriendUserDatas,baseObject,parentTF);
        SetImage(FriendAPISetting.realFriendUserDatasForBattle, baseObjectOsusume, parentTFOsusume);
        frinedLimit.text = FriendAPISetting.realFriendUserDatas.Count + "/" + friendCount;
        Debug.Log("friendCount:" + friendCount);
        friendList.SetActive(true);
        friendListByOsusume.SetActive(false);

        yield return null;
    }

    private void SetImage( List<RealFriendUserData> realUserDatas,GameObject objectBase ,Transform parentPort)
	{

    
        //今あるやつ全部消す
        for (int i = 0; i < parentPort.childCount; ++i)
        {
            if (parentPort.GetChild(i).gameObject == objectBase)
                continue;
                
            Destroy(parentPort.GetChild(i).gameObject);
        }

       
        foreach( var Value in  realUserDatas)
        {
            var newGO = Instantiate(objectBase,parentPort,true);
            FriendController friendController = newGO.GetComponent<FriendController>();
            friendController.Init(Value);
            newGO.SetActive(true);
        }

        nothingObject.SetActive(FriendAPISetting.realFriendUserDatas.Count == 0);

        baseObject.SetActive(false);
        baseObjectOsusume.SetActive(false);
	} 


    public void PushEvent()
    {

        inputId.text = inputId.text.TrimStart();
        inputId.text = inputId.text.TrimEnd();
        inputId.text = inputId.text.Substring(0, inputId.text.Length - 1);
        Debug.Log("koitudeksannsaku:" + inputId.text + ":" + UserData.GetFriendID() );
        Debug.Log("koitudeksannsaku:" + inputId.text.Length + ":" + UserData.GetFriendID().Length);

       
        foreach(char c in inputId.text)
        {
            Debug.Log(":" +c + ":");
        }


        string textValue = inputId.text;

        if(inputId.text == UserData.GetFriendID() )
        {
            Debug.Log("お小言ポップアップ");
            AddPopup("ErrorConfirm");
            Popup_ErrorConfirm.Init("IDが自分のものです");
            return;
        }


        if (inputId.text == string.Empty|| inputId.text == "0")
        {
            AddPopup("ErrorConfirm");
            Popup_ErrorConfirm.Init("IDが入力されていません");
            return;
        }


        var friends = FriendAPISetting.realFriendUserDatas;
        foreach( var Value in friends)
        {
            if(inputId.text == Value.userData)
            {
                AddPopup("ErrorConfirm");
                Popup_ErrorConfirm.Init("既にフレンドです。");
                return;
            }
        }


        AddPopup("Popup_Loding");
        FriendAPISetting.SearchingFriend(inputId.text,(number) =>
        {
            Loading.Close();
            if( number == 0 )
                AddConfirm(FriendAPISetting.realFriendUserData);
        });
    }


    public void AddConfirm( RealFriendUserData realFriendUserData )
    {
        if (FriendAPISetting.realFriendUserDatas.Count == friendCount)
        {
            Debug.Log("friend over");
            PopupGeneral.textValue = "フレンド最大枠を越えています。";
            AddPopup("PopupGeneral");
        }
        else
        {
            Debug.Log("friend add");
            confirmPopupScript.realFriendUserData = realFriendUserData;
            confirmPopup.gameObject.SetActive(true);
            confirmPopupScript.Init();
        }
    }


    public void GetOsusume()
    {
        friendList.SetActive(false);
        friendListByOsusume.SetActive(true);
    }

    public void GetFriend()
    {
        friendList.SetActive(true);
        friendListByOsusume.SetActive(false);
    }

    public void CopyText()
    {
        UniClipboard.Clipboard.Text = UserData.GetFriendID();;
    }

}
