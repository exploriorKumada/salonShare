using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemController : ScenePrefab {

    public RealItemData realItemData;
    [SerializeField] Image itemImage;
    [SerializeField] Layout_Item layout_Item;
    [SerializeField] TextMeshProUGUI amountText;
    [SerializeField] GameObject starObject;
    [SerializeField] Transform starObjectParent;

    public void Initialize()
    {
        gameObject.SetActive(false);
        ResourceLoaderOrigin.GetItemImage(realItemData.item_master_id, (Sprite obj) => { 

            itemImage.sprite = obj;
      
        });
        gameObject.SetActive(true);
        if (realItemData.amount == 0)
            amountText.gameObject.SetActive(false);
       
        amountText.text = "×" + realItemData.amount;

        Instantiate(starObject, starObjectParent).GetComponent<RareStartController>().Init(realItemData.rank);

    }


    public void PushEvent()
    {
        Debug.Log("realItemData.typeId:" + realItemData.name);
        layout_Item.SetInfo(realItemData);
    }

    public void LongPush()
    {
        Debug.Log("realItemData.typeId:" + realItemData.typeId);
        Popup_EquipmentInfo.realItemData = realItemData;
        AddPopup("EquipmentInfo");
    }

    public void Sell()
    {

    }




}
