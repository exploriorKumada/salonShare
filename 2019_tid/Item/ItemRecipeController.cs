using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemRecipeController : MonoBehaviour {

    [SerializeField] GameObject weapon1Object;
    [SerializeField] Image weapon1;
    [SerializeField] CommonWeaponPrefabController common1;
    [SerializeField] TextMeshProUGUI weapon1Number;
    [SerializeField] GameObject weapon2Object;
    [SerializeField] Image weapon2;
    [SerializeField] TextMeshProUGUI weapon2Number;
    [SerializeField] CommonWeaponPrefabController common2;
    [SerializeField] GameObject weapon3Object;
    [SerializeField] Image weapon3;
    [SerializeField] TextMeshProUGUI weapon3Number;
    [SerializeField] CommonWeaponPrefabController common3;
    [SerializeField] GameObject weapon4Object;
    [SerializeField] Image weapon4;
    [SerializeField] TextMeshProUGUI weapon4Number;
    [SerializeField] CommonWeaponPrefabController common4;
    [SerializeField] TextMeshProUGUI ATK;
    [SerializeField] TextMeshProUGUI DEF;
    [SerializeField] TextMeshProUGUI CRI;
    [SerializeField] TextMeshProUGUI MGC;
    [SerializeField] TextMeshProUGUI itemName;

  
    public void Init( RealItemData realItemData )
    {
        if( realItemData.mate1id == 0 )
        {
            Debug.LogError("error itemData");
        }else
        {
            ResourceLoaderOrigin.GetItemImage(realItemData.mate1id, (obj) =>
            {
                weapon1.sprite = obj;
            });
            weapon1Number.text = "×" + realItemData.mate1amount;
            common1.Init(realItemData.mate1id);
        }

        if (realItemData.mate2id == 0)
        {
            weapon2Object.SetActive(false);
        }else
        {
            ResourceLoaderOrigin.GetItemImage(realItemData.mate2id,(obj) =>
            {
                weapon2.sprite = obj;
            });
            weapon2Number.text = "×" + realItemData.mate2amount;
            common2.Init(realItemData.mate2id);
        }

        if (realItemData.mate3id == 0)
        {
            weapon3Object.SetActive(false);
        }
        else
        {
            ResourceLoaderOrigin.GetItemImage(realItemData.mate3id, (obj) =>
            {
                weapon3.sprite = obj;
            });
            weapon3Number.text = "×" + realItemData.mate3amount;
            common3.Init(realItemData.mate3id);

        }

        ResourceLoaderOrigin.GetItemImage(realItemData.item_master_id, (obj) =>
        {
            weapon4.sprite = obj;
        });
        ATK.text = (realItemData.attack).ToString();
        DEF.text = (realItemData.guard*100).ToString();
        MGC.text = (realItemData.repair*100).ToString();
        CRI.text = (realItemData.cri*100).ToString();
        itemName.text = realItemData.name.ToString();
        common4.Init(realItemData.item_master_id);


    }

}
