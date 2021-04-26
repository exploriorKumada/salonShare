using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FriendControllerForPartySelect : ScenePrefab {

    [SerializeField] Image charaImage;
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] TextMeshProUGUI ranktext;
    [SerializeField] GameObject selectObject;

    public RealFriendUserData realFriendUserData;
    Action<FriendControllerForPartySelect> action;

    public void Init( RealFriendUserData realFriendUserData,Action<FriendControllerForPartySelect> action)
    {
        this.realFriendUserData = realFriendUserData;
        this.action = action;
        SetImage();
    }
    public void SetImage()
    {
        nameText.text = realFriendUserData.name;
        ranktext.text = realFriendUserData.rank.ToString();
        gameObject.SetActive(false);
        ResourceLoaderOrigin.GetBattleCharaImage(realFriendUserData.character_id, (Sprite obj) => { 
            charaImage.sprite = obj;
            gameObject.SetActive(true);
        });
    }

    public void PushSelectEvent()
    {
        action(this);
    }

    public void SetInfo()
    {
        Popup_PublicCharaInfo.realCharaData = realFriendUserData.realFriendCharaData;
        AddPopup("PublicCharaInfo");
    }

    public void SetSelect(bool flag)
    {
        selectObject.SetActive(flag);
    }


}
