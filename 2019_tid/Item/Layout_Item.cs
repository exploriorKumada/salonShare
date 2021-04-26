using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class Layout_Item : ScenePrefab
{
    [SerializeField] SpriteRenderer backGround;

    [SerializeField] GameObject baseObject;
    [SerializeField] Transform parentTF;

    [SerializeField] GameObject notingObject;
    //info

    [SerializeField] GameObject exist;
    [SerializeField] GameObject noExist;
    [SerializeField] GameObject gousei;
    [SerializeField] Image infoItemImage;
    [SerializeField] Image gouseiItemImage;
    [SerializeField] TextMeshProUGUI name;
    [SerializeField] TextMeshProUGUI description;
    [SerializeField] TextMeshProUGUI nameGousei;
    [SerializeField] TextMeshProUGUI descriptionGousei;
    [SerializeField] TextMeshProUGUI selling;
    [SerializeField] Popup_ItemRecipe popup_ItemRecipe;
    [SerializeField] GameObject useObject;

    [SerializeField] GameObject gouseiBase;
    [SerializeField] Transform gouseiParent;

    [SerializeField] List<GeneralData> nowImageList;

    [SerializeField] GameObject centerGO;
    [SerializeField] RectTransform underLine;
    [SerializeField] RectTransform upLine;

    [SerializeField] TextMeshProUGUI cryText;

    /// <summary>
    /// 4:gousei
    /// </summary>
    [System.NonSerialized]public int tabNumber = 2;

    RealItemData selectedItemData;
    // Use this for initialization
    void Start()
    {
        //ItemAPISetting.SetItem(12,() =>
        //{
        //    ItemAPISetting.LoadItemDetail(1, () =>
        //    {
        //        StartCoroutine(SetStart());
        //    });
        //});

        BgmManager.Instance.Play(ResourceLoaderOrigin.GetBGMnameById(7));
        AddSubLayout("Footer");
        StartCoroutine(SetStart());
      

    }

    private void Update()
    {
        cryText.text = UserData.GetExNumber().ToString();
    }

    public void SetCeackImage()
    {
        foreach (var Value in nowImageList)
            Value.GO.SetActive(false);

        nowImageList.Find(n => n.number == tabNumber).GO.SetActive(true);
    }


    public IEnumerator SetStart()
    {
        if( iphoneXAjust(centerGO) )
        {
            //underLine.offsetMin = new Vector2(-747, -604f);
            //underLine.offsetMax = new Vector2(-751, 604f);

            //upLine.offsetMax = new Vector2(-728f, -220);
            //upLine.offsetMin = new Vector2(-758f, 220f);
     
        }


        AddPopup("Popup_AlphaLoding");
        ItemAPISetting.LoadItemList(() =>
        {
            ResourceLoaderOrigin.GetBackGroundImage(1, (bgobj) =>
            {
                backGround.sprite = bgobj;
                AlphaLoding.Close();
                Debug.Log("LoadItemList COMPLTETE");
                SetImage();
            });


        });
        yield break;
    }

    public void SetImage()
    {
        Debug.Log("tabNUmber:"+tabNumber);

        UserData.TutoSet(UserData.TutoType.item);
        //今あるやつ全部消す
        for (int i = 0; i < parentTF.childCount; ++i)
        {
            if (parentTF.GetChild(i).gameObject == baseObject)
                continue;
                
            GameObject.Destroy(parentTF.GetChild(i).gameObject);
        }

        baseObject.gameObject.SetActive(false);
        foreach( var Value in ItemAPISetting.realItemDatas[tabNumber] )
        {
            var newGO = GameObject.Instantiate(baseObject,parentTF);
            ItemController itemController = newGO.GetComponent<ItemController>();
            itemController.realItemData = Value;
            itemController.Initialize();

            //newGO.SetActive(true);
            //Debug.Log( Value.item_master_id);           
        }

        if(tabNumber == 4)
        {
            notingObject.GetComponent<TextMeshProUGUI>().text = "合成可能なアイテムはありません。";
        }else
        {
            notingObject.GetComponent<TextMeshProUGUI>().text = "何も所持していません。";
        }


        if( ItemAPISetting.realItemDatas[tabNumber].Count != 0 )
        {
            SetInfo(ItemAPISetting.realItemDatas[tabNumber][0]);
            if( tabNumber == 4)
            {
                exist.SetActive(false);
                noExist.SetActive(false);
                gousei.SetActive(true);
                notingObject.SetActive(false); 
            }else
            {
                useObject.SetActive(tabNumber == 2);
                exist.SetActive(true);
                noExist.SetActive(false);
                gousei.SetActive(false);
                notingObject.SetActive(false); 
            }

        }else 
        {
            Debug.Log("no item");
            exist.SetActive(false);
            noExist.SetActive(true);
            gousei.SetActive(false);
            notingObject.SetActive(true);
        }
        SetCeackImage();
        //Debug.Log("tabNumber:" + tabNumber);
       
    }


    [SerializeField] GameObject starObject;
    [SerializeField] Transform starObjectParent;

    GameObject startBaseObejct;
    public void SetInfo( RealItemData realItemData )
    {
        name.text = realItemData.name;
        nameGousei.text = realItemData.name;
        description.text = realItemData.description;
        descriptionGousei.text = realItemData.description;
        selling.text = ""+realItemData.selling;

        ResourceLoaderOrigin.GetItemImage(realItemData.item_master_id, (Sprite obj) => { 
            infoItemImage.sprite = obj; 
        });
        ResourceLoaderOrigin.GetItemImage(realItemData.item_master_id, (Sprite obj) => { 
            gouseiItemImage.sprite = obj; 
            });
        selectedItemData = realItemData;

        if(startBaseObejct!=null)
            Destroy(startBaseObejct);
    
        startBaseObejct = Instantiate(starObject, starObjectParent);
        startBaseObejct.GetComponent<RareStartController>().Init(realItemData.rank);


        if ( tabNumber == 4)
        {
            SetNeedGousetItemImage(realItemData);
        }
    }

    public void SetNeedGousetItemImage( RealItemData realItemData )
    {
        gouseiBase.SetActive(false);

        for (int i = 0; i < gouseiParent.childCount; ++i)
        {
            if (gouseiParent.GetChild(i).gameObject == gouseiBase)
                continue;

            GameObject.Destroy(gouseiParent.GetChild(i).gameObject);
        }

        foreach( var Value in realItemData.GetGouseiItems() )
        {
            var newGO = GameObject.Instantiate(gouseiBase, gouseiParent);
            newGO.SetActive(true);
            GeneralData generalData = newGO.GetComponent<GeneralData>();
            ResourceLoaderOrigin.GetItemImage(Value.itemId, (Sprite obj) => { generalData.image.sprite = obj; });
            generalData.textMeshProUGUI.text = "x" + Value.amount;
        }
    }


    public void ChangeSetItem( int itemType )
    {
        tabNumber = itemType;
        SetImage();
    }


    public void RecipeOpen()
    {
        popup_ItemRecipe.Initialize(() =>
        {
            popup_ItemRecipe.gameObject.SetActive(true);
        });
       
    }

    public void Sell()
    {
        PopupGeneral.textValue = "アイテムを売却します";
        PopupGeneral.action = () =>
        {

            Debug.Log("売ります");
            AddPopup("Popup_Loding");
            ItemAPISetting.SellItem(selectedItemData.item_master_id, () =>
            {
                ItemAPISetting.LoadItemList(() =>
                {
                    UserDataSetting.LoadRealUserData(UserData.GetUserID(), () =>
                    {
                        Loading.Close();
                        AddPopup("ErrorConfirm");
                        Popup_ErrorConfirm.Init("売却が完了しました。");
                        SetImage();

                    });

                });
            });
        };
        AddPopup("PopupGeneral");
    }


    public void Use()
    {
        //selectedItemData

    }

    public void Gousei()
    {
        AddPopup("Popup_Loding");
        ItemAPISetting.Gousei( selectedItemData.item_master_id, () =>
        {
            ItemAPISetting.LoadItemList(() =>
            {
                Loading.Close();
                GeneralEffect.MessageValue = "合成完了";
                AddPopup("GeneralEffect", 1.5f);
                SetImage();
            });
        });
    }

}




