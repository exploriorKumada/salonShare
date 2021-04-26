using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharaEquipmentController : ScenePrefab{

    public RealItemData realItemData;
    public RealCharaData realCharaData;

    [SerializeField] Image infoItemImage;
    [SerializeField] GameObject equipmentPopup;
    [SerializeField] TextMeshProUGUI amountText;

    public void Initialized( RealItemData realItemData , RealCharaData realCharaData)
    {
        this.realItemData = realItemData;
        this.realCharaData = realCharaData;



        if (realItemData.amount == 0)
            amountText.gameObject.SetActive(false);

        if (realItemData.item_master_id == -1)
        {
            infoItemImage.gameObject.SetActive(false);
            amountText.gameObject.SetActive(true);
            amountText.text = realItemData.name;
        }
        else
        {
            ResourceLoaderOrigin.GetItemImage(realItemData.item_master_id, (Sprite obj) => { infoItemImage.sprite = obj; });
            amountText.text = "x" + realItemData.amount;
        }




    }

    public void PushEvent()
    {
        Debug.Log( realItemData.name + ":" + realCharaData.charaName );
        equipmentPopup.SetActive(true);
        equipmentPopup.GetComponent<EquipmentConfirm>().realCharaData = realCharaData;
        equipmentPopup.GetComponent<EquipmentConfirm>().afterRealItemData = realItemData;
        equipmentPopup.GetComponent<EquipmentConfirm>().Init();
    }


    public void LongPush()
    {
        Debug.Log("nagaoshi:" + realItemData.name);
        Popup_EquipmentInfo.realItemData = realItemData;
        AddPopup("EquipmentInfo");
    }


}
