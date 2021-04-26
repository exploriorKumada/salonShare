using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class DeleteConfirmPopup : ScenePrefab {

    public RealFriendUserData realFriendUserData;
    public Layout_Friend layout_Friend;

    public void Delete()
    {
        gameObject.SetActive(false);
        AddPopup("Popup_AlphaLoding");
        FriendAPISetting.DeleteFriend(realFriendUserData.userData, () =>
        {
            AlphaLoding.Close();
            layout_Friend.SetFriend();
        });
        
    }


    public void Close()
    {
        gameObject.SetActive(false);
    }

}
