using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Explorior;
using System.Linq;
using TW.GameSetting;
using System;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ItemSeting : SystemBaseManager
{
    //ボタンのベース
    [SerializeField] Transform buttonUnit;
    //ウィンドウのベース
    [SerializeField] Transform itemUnit;

    [SerializeField] Dictionary<ItemType, Sprite> spriteDic = new Dictionary<ItemType, Sprite>();

    [SerializeField] UnitItemSetting unitItemSetting;

    List<ItemWindowInfo> itemWindowInfos = new List<ItemWindowInfo>();

    void Start()
    {
        Loding(() =>
        {
            ResouceLoad();
        });
    }

    public void ResouceLoad()
    {
        ResourceManager.Instance.LoadWeaponModels(DataManager.Instance.userWeaponInfos, () =>
        {
            ResourceManager.Instance.LoadItemModels(DataManager.Instance.userItemInfos, () =>
            {
                Initialize();
            });
        });
    }

    private void Initialize()
    {
        itemUnit.gameObject.ParentInitialize();
        buttonUnit.gameObject.ParentInitialize();


        //アイテム種類ごとにwindow生成
        foreach (ItemType Value in Enum.GetValues(typeof(ItemType)))
        {
            if (Value == ItemType.None) continue;

            var buttonListUnit = Instantiate(buttonUnit, buttonUnit.parent);
            buttonListUnit.name = Value.ToString() + "_button";
            buttonListUnit.gameObject.SetActive(true);
            buttonListUnit.GetComponent<GeneralData>().images.First().Value.sprite = spriteDic[Value];
            buttonListUnit.GetComponent<Button>().onClick.AddListener(()=>ItemButtonPush(Value));

            var itemListUnit = Instantiate(itemUnit, itemUnit.parent);
            itemListUnit.name = Value.ToString() + "_value";
            itemListUnit.gameObject.SetActive(true);

            ItemWindowInfo itemWindowInfo = new ItemWindowInfo()
            {
                itemType = Value,
                buttonListTF = buttonListUnit,
                itemListTF = itemListUnit,
            };


            if(Value == ItemType.ConsumeItem)
                itemWindowInfo.SetItem(GeneralDataClass.ItemCommonInfo(DataManager.Instance.userItemInfos), unitItemSetting);
            else if (Value == ItemType.WeaponItem)
                itemWindowInfo.SetItem(GeneralDataClass.ItemCommonInfo(DataManager.Instance.userWeaponInfos), unitItemSetting);

            itemWindowInfos.Add(itemWindowInfo);

        }

        ItemButtonPush(ItemType.ConsumeItem);

        //var weaponList = DataManager.Instance.userWeaponInfos;

        //foreach(var Value in weaponList)
        //{
        //    var weapon = Instantiate(itemParent, itemParent.transform.parent);
        //    weapon.SetActive(true);
        //    weapon.name = Value.id.ToString();

        //    //配下に武器モデルをおく
        //    weapon.GetComponent<GeneralData>().gameObejcets.First().Value.GetComponent<ItemIcon>().Initialize(null, new ItemCommonInfo(TW.GameSetting.ItemType.WeaponItem,Value));
        //}

    }

    public void ItemButtonPush(ItemType itemType)
    {
        itemWindowInfos.ForEach(x => x.itemListTF.gameObject.SetActive(false));

        var tagetWindow = itemWindowInfos.FirstOrDefault(x => x.itemType == itemType);

        unitItemSetting.SetItemInfo(tagetWindow.selectItem);

        StartCoroutine(WaitAcitve(tagetWindow.itemListTF.gameObject));
    }


    public IEnumerator WaitAcitve(GameObject game)
    {
        yield return new WaitForSeconds(0.0000001f);

        game.SetActive(true);
    }

}



public class ItemWindowInfo : SystemBaseManager
{
    public ItemType itemType;
    public Transform buttonListTF;
    public Transform itemListTF;

    public ItemCommonInfo selectItem;

    //アイテム一覧を一気に作る
    public void SetItem(List<ItemCommonInfo> itemCommonInfos, UnitItemSetting unitItemSetting)
    {
        //アイテムアイコン取得
        //itemListTF.gameObject.ParentInitialize();
        itemListTF.name = itemType.ToString();

        Transform unitBase = itemListTF.GetComponent<GeneralData>().gameObejcets.Values.First().transform;

        foreach (var Value in itemCommonInfos)
        {
            GameObject unit = Instantiate(unitBase, unitBase.parent).gameObject;

            ItemIcon itemIcon = unit.GetComponent<GeneralData>().gameObejcets.First().Value.GetComponent<ItemIcon>();

            itemIcon.Initialize(Value,()=> unitItemSetting.SetItemInfo(Value));

            itemIcon.gameObject.SetActive(true);

            unit.gameObject.SetActive(true);
        }
        unitBase.gameObject.SetActive(false);

        if (selectItem == null)
        {
            selectItem = itemCommonInfos.First();         
        }
    }

}