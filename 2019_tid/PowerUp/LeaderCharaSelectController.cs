using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LeaderCharaSelectController : ScenePrefab {

    [SerializeField] Popup_Leaderselect popup_Leaderselect;
    [SerializeField] Image image;
    [SerializeField] public GameObject selectObject;
    public LeaderCharaSettingBase leaderCharaSettingBase;

    public Dictionary<int, float> ajsutPosi = new Dictionary<int, float>()
    {
        {1,-869},
        {2,-1103},
        {3,-518},
        {4,-837},
         {5,-869},
    };


    public void Init(LeaderCharaSettingBase leaderCharaSettingBase)
    {
        this.leaderCharaSettingBase = leaderCharaSettingBase;

        gameObject.SetActive(false);
        ResourceLoaderOrigin.GetStandingImage(leaderCharaSettingBase.No, "defa",(Sprite obj) => {
            image.sprite = obj;
            gameObject.SetActive(true);
        });
        image.transform.localPosition = new Vector3( image.transform.localPosition.x,ajsutPosi[leaderCharaSettingBase.No] );

        if(UserData.GetLeaderChara() == leaderCharaSettingBase.No)
        {
            selectObject.SetActive(true);
            PushEvent();
        }

        UIsclalUp(transform);
    }

    public void PushEvent()
    {
        ResourceLoaderOrigin.GetSpine(leaderCharaSettingBase.No, (obj) =>
        {
            selectObject.SetActive(true);
            popup_Leaderselect.SetSpine(this, obj);

        });

    }
}
