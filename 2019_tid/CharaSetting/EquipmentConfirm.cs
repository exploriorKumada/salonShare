using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class EquipmentConfirm : ScenePrefab {
    
    public RealCharaData realCharaData;
    public RealItemData beforeRealItemData; 
    public RealItemData afterRealItemData;

    [SerializeField] TextMeshProUGUI aATK;
    [SerializeField] TextMeshProUGUI aDEF;
    [SerializeField] TextMeshProUGUI aCRI;
    [SerializeField] TextMeshProUGUI aMGC;
    [SerializeField] Image aImage;
    [SerializeField] TextMeshProUGUI aName;

    [SerializeField] TextMeshProUGUI bATK;
    [SerializeField] TextMeshProUGUI bDEF;
    [SerializeField] TextMeshProUGUI bCRI;
    [SerializeField] TextMeshProUGUI bMGC;
    [SerializeField] Image bImage;
    [SerializeField] TextMeshProUGUI bName;

    [SerializeField] TextMeshProUGUI cATK;
    [SerializeField] TextMeshProUGUI cDEF;
    [SerializeField] TextMeshProUGUI cCRI;
    [SerializeField] TextMeshProUGUI cMGC;

    [SerializeField] TextMeshProUGUI dATK;
    [SerializeField] TextMeshProUGUI dDEF;
    [SerializeField] TextMeshProUGUI dCRI;
    [SerializeField] TextMeshProUGUI dMGC;

    [SerializeField] Layout_CharaSetting layout_CharaSetting;

    [SerializeField] GameObject beforeObject;
    [SerializeField] GameObject noEquipment;

    [SerializeField] Transform baseTF;

	// Use this for initialization
    public void Init () {
        
        bATK.text = "" + realCharaData.GetRealAttack(realCharaData.lv,true);          
        bDEF.text = "" + realCharaData.GetRealGuardForStatusView(realCharaData.lv, true);
        bCRI.text = "" + realCharaData.GetRealCriForStatusView();
        bMGC.text = "" + realCharaData.GetRealRepairForStatusView();
        noEquipment.SetActive(false);
        cATK.text = GetFugou(0);
        cDEF.text = GetFugou(0);
        cCRI.text = GetFugou(0);
        cMGC.text = GetFugou(0);
        if ( realCharaData.item_master_id != 0 )
        {
            //beforeObject.SetActive(true);
            
            bImage.gameObject.SetActive(true);
            beforeRealItemData = ItemAPISetting.realItemMasterDatas[realCharaData.item_master_id];
            Debug.Log("beforeRealItemData.item_master_id:" + realCharaData.item_master_id);
            ResourceLoaderOrigin.GetItemImage(realCharaData.item_master_id, (Sprite obj) => { bImage.sprite = obj; });
            bName.text = beforeRealItemData.name;
           
        }else
        {
            bImage.gameObject.SetActive(false);
            //noEquipment.SetActive(true);
            //beforeObject.SetActive(false);
            bName.text = "装備なし";
        }

        aATK.text = "" + (realCharaData.GetRealAttack(realCharaData.lv,false) + afterRealItemData.attack);
        aDEF.text = "" + (realCharaData.GetRealGuard(realCharaData.lv, false) + afterRealItemData.guard)*100;
        aCRI.text = "" + (realCharaData.cri + afterRealItemData.cri) * 100;
        aMGC.text = "" + ( afterRealItemData.repair) * 100;
        aName.text = afterRealItemData.name;
        Debug.Log("afterRealItemData.item_master_id:" + afterRealItemData.item_master_id);
        aImage.color = new Color(255f, 255, 255, 255);
        if (afterRealItemData.item_master_id != 0 && afterRealItemData.item_master_id != -1)
        {
            ResourceLoaderOrigin.GetItemImage(afterRealItemData.item_master_id, (Sprite obj) => { aImage.sprite = obj; });
        }else
        {
            aImage.color = new Color(255f, 255, 255, 0);
        }
           
           
        dATK.text = GetFugou( afterRealItemData.attack - realCharaData.item_attack );
        dDEF.text = GetFugou( (afterRealItemData.guard - realCharaData.item_guard)*100);
        dCRI.text = GetFugou( (afterRealItemData.cri - realCharaData.item_cri)*100);
        dMGC.text = GetFugou( (afterRealItemData.repair - realCharaData.item_repair)*100);

        UIsclaleUp(baseTF,new Vector3(1,1,1), 0.1f);
    }

    string GetFugou( float number )
    {
        if (number == 0)
            return "";
        
        if (number < 0)
            return "<color=#DD0032FF>("+number.ToString() + ")";
        else
            return "<color=#00B41FFF>(+" + number.ToString() + ")";
    }


    public void PushEvent()
    {
        AddPopup("Popup_Loding");
        ItemAPISetting.SetEquipment( Layout_CharaSetting.realCharaData.charaIdNumber,afterRealItemData.item_master_id,  () =>
        {
            CharaAPISetting.LoadRealCharaData(UserData.GetUserID(), () =>
            {
                ItemAPISetting.LoadItemList(() =>
                {
                    realCharaData = CharaAPISetting.GetRealcharaDataByCharaId(realCharaData.charaIdNumber);
                    Layout_CharaSetting.realCharaData = realCharaData;
                    Loading.Close();
                    Close();
                    //ChangeLayout("CharaSetting");
                    Layout_CharaSetting.realCharaData = CharaAPISetting.GetRealcharaDataByCharaId(realCharaData.charaIdNumber);
                    layout_CharaSetting.SetBase();
                });
            } );         
        });
    }

    public void Close()
    {
        UIscaleDown(baseTF, 0.1f, () =>
        {
            gameObject.SetActive(false);
        });
    }
}
