using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PresentController : ScenePrefab {

	[SerializeField] Image iconImage;
    [SerializeField] Image charaIconImage;
    [SerializeField] Layout_Present layout_Present;
    public RealPresentData realPresentData;

    [SerializeField] GameObject newText;


    [SerializeField] TextMeshProUGUI title;
    [SerializeField] TextMeshProUGUI body;
    [SerializeField] TextMeshProUGUI value;

    [SerializeField] TextMeshProUGUI getDay;
    [SerializeField] TextMeshProUGUI limitDay;

    public void Init()
    {
        string type;
//        Debug.Log( "********   " + realPresentData.type);
        if( realPresentData.type == "2")
        {
            type = "101";
            iconImage.gameObject.SetActive(true);
            charaIconImage.gameObject.SetActive(false);
            ResourceLoaderOrigin.GetItemImage(101, (Sprite obj) => { iconImage.sprite = obj; });

            title.text = realPresentData.title;
            body.text = realPresentData.body;
            value.text = "x" + realPresentData.number;


        }
        else if(realPresentData.type == "1")
        {
            type = "100";
            iconImage.gameObject.SetActive(true);
            charaIconImage.gameObject.SetActive(false);
            ResourceLoaderOrigin.GetItemImage(100, (Sprite obj) => { iconImage.sprite = obj; });

            title.text = realPresentData.title;
            body.text = realPresentData.body;
            value.text = "x" + realPresentData.number;
        }
        else if (realPresentData.type == "3")
        {
            iconImage.gameObject.SetActive(false);
            charaIconImage.gameObject.SetActive(true);
            title.text = realPresentData.title;
            body.text = realPresentData.body;
            value.text = "x" + realPresentData.number;

            ResourceLoaderOrigin.GetBattleCharaImage(realPresentData.character_id, (Sprite obj) => { charaIconImage.sprite = obj; });

        }
        else if( realPresentData.type == "4" )
        {
            iconImage.gameObject.SetActive(true);
            charaIconImage.gameObject.SetActive(false);
            ResourceLoaderOrigin.GetItemImage(realPresentData.item_master_id, (Sprite obj) => { iconImage.sprite = obj; });

            title.text = realPresentData.title;
            body.text = realPresentData.body;
            value.text = "x" + realPresentData.number;

            Debug.Log( ItemAPISetting.realItemMasterDatas[realPresentData.item_master_id].name );
        }

        if(realPresentData.finish_date !=null)
        {
            limitDay.text = realPresentData.finish_date;
        }else if( limitDay!=null)
        {
            limitDay.text = "なし";
        }


    }

    public void PushEvent()
    {
        AddPopup("Popup_Loding");
        PresentSetting.GetPresentOn(UserData.GetUserID(), UserData.GetUserUUID(), realPresentData, () =>
       {
            //Loading.Close();
           layout_Present.SetImageON();
           PopupGeneral.textValue = "プレゼントを受け取りました。";
           AddPopup("PopupGeneral");
        });
    }
}
