using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FriendController : ScenePrefab
{
    RealFriendUserData thisRealFriendUserData;
    [SerializeField] Image chara;
    [SerializeField] TextMeshProUGUI name;
    [SerializeField] TextMeshProUGUI rank;
    [SerializeField] DeleteConfirmPopup deleteConfirmPopup;

    [SerializeField] Layout_Friend layout_Friend;

    public void Init( RealFriendUserData realFriendUserData )
    {
        thisRealFriendUserData = realFriendUserData;
        name.text = realFriendUserData.name;
        rank.text = "" + realFriendUserData.rank;
        gameObject.name = gameObject.GetInstanceID().ToString();
        ResourceLoaderOrigin.GetBattleCharaImage(thisRealFriendUserData.character_id, (Sprite obj) => { chara.sprite = obj; });
    }


    public void Remove()
    {
        deleteConfirmPopup.realFriendUserData = thisRealFriendUserData;
        deleteConfirmPopup.gameObject.SetActive(true);
    }

    public void Detatil()
    {
        Debug.Log( "GetInstanceID:"+ gameObject.GetInstanceID());
        Debug.Log( "realFriendUserData:" + thisRealFriendUserData.name);
        Debug.Log("charaDetail:" + thisRealFriendUserData.realFriendCharaData);
        Popup_PublicCharaInfo.realCharaData = thisRealFriendUserData.realFriendCharaData;
        AddPopup("PublicCharaInfo");
    }

    public void Add()
    {
        layout_Friend.AddConfirm(thisRealFriendUserData);
    }

}
