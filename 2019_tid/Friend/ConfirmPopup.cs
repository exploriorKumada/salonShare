using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class ConfirmPopup : ScenePrefab
{
    public RealFriendUserData realFriendUserData;

    [SerializeField] Image chara;
    [SerializeField] TextMeshProUGUI name;
    [SerializeField] TextMeshProUGUI rank;
    [SerializeField] Layout_Friend layout_Friend;

    public void Init()
    {
        name.text = realFriendUserData.name;
        rank.text = ""+realFriendUserData.rank;

        ResourceLoaderOrigin.GetBattleCharaImage(realFriendUserData.character_id, (Sprite obj) => { chara.sprite = obj; });
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }


    public void FriendRegist()
    {
        AddPopup("Popup_AlphaLoding");
        FriendAPISetting.SetFriend(realFriendUserData.userData, () =>
        {
            AlphaLoding.Close();
            gameObject.SetActive(false);
            layout_Friend.SetFriend();
        });
    }
}
