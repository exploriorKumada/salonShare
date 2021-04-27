using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Explorior;
using System.Linq;
using TW.GameSetting;
using System;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UnitItemSetting : ExploriorSceneManager
{
    [SerializeField] TextMeshProUGUI itemName;
    [SerializeField] TextMeshProUGUI itemDiscrption;
    [SerializeField] GameObject unitBase;
    [SerializeField] ItemIcon itemIcon;
    [SerializeField] GameObject deteilButton;

    ItemCommonInfo itemCommonInfo;

    public void SetItemInfo(ItemCommonInfo _itemCommonInfo)
    {
        itemCommonInfo = _itemCommonInfo;
        unitBase.ParentInitialize();
        itemName.text = itemCommonInfo.itemBase.itemName;
        itemDiscrption.text = itemCommonInfo.itemBase.itemDiscription;

        itemIcon.Initialize(itemCommonInfo);
        itemIcon.gameObject.SetActive(true);
        itemIcon.modular3DText.gameObject.SetActive(false);

        deteilButton.SetActive(itemCommonInfo.itemType == ItemType.WeaponItem);
        if (itemCommonInfo.itemType == ItemType.WeaponItem)
        {
            var weaponInfo = itemCommonInfo.weaponInfo;

            UnitInstantiate("Lv", weaponInfo.lv.ToString());

            if(weaponInfo.physical_attack != 0)
                UnitInstantiate("物理攻撃力", weaponInfo.physical_attack.ToString());

            if (weaponInfo.magic_attack != 0)
                UnitInstantiate("魔法攻撃力", weaponInfo.magic_attack.ToString());

            if (weaponInfo.physical_defence != 0)
                UnitInstantiate("物理防御力", weaponInfo.physical_defence.ToString());

            if (weaponInfo.magic_defence != 0)
                UnitInstantiate("魔法防御力", weaponInfo.magic_defence.ToString());

            if (weaponInfo.hp != 0)
                UnitInstantiate("HP", weaponInfo.hp.ToString());

            if (weaponInfo.mp != 0)
                UnitInstantiate("MP", weaponInfo.mp.ToString());

            if (weaponInfo.movement != 0)
                UnitInstantiate("稼働力", weaponInfo.movement.ToString());

            if (weaponInfo.cri != 0)
                UnitInstantiate("クリティカル力", weaponInfo.cri.ToString());
        }

    }

    private void UnitInstantiate( string statusName, string statusValue )
    {
        var stausData = Instantiate(unitBase, unitBase.transform.parent).GetComponent<GeneralData>();
        stausData.textMeshProUGUIs[0].text = statusName;
        stausData.textMeshProUGUIs[1].text = statusValue;
        stausData.gameObject.SetActive(true);
    }

    public void ChangeWeaponUnitSetting()
    {
        CameraActive(false);
        var data = new Dictionary<string, object>();
        data.Add("weaponinfo", itemCommonInfo.weaponInfo);
        ChangeScene(new ChangeSceneInfo()
        {
            sceneType = SceneType.WeaponSetting,
            fadeType = FadeType.Wind,
            fadingAction = () => CameraActive(true),
            loadSceneMode = LoadSceneMode.Additive,
            data_hash = data
        });
    }

}
