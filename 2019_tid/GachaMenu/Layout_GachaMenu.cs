using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;

public class Layout_GachaMenu : ScenePrefab {


    [SerializeField] SpriteRenderer backGround;
    [SerializeField] Image type;

    [SerializeField] GameObject menuBase;
    [SerializeField] Transform menuTF;

    [SerializeField] Button ten;
    [SerializeField] Button one;

    [SerializeField] GameObject husokuPopup;

    [SerializeField] Transform gacharaMenuTF;
    [SerializeField] Transform buttonMenuTF;

    [SerializeField] GameObject centerGO;

    [SerializeField] TextMeshProUGUI gachaFinishDate;

    [SerializeField] GameObject teikyopopup;


    [SerializeField] GameObject teikyoUnit;

    public int gachaId = 1; 
	// Use this for initialization
	void Start () {

		AddSubLayout("Footer");
		AddSubLayout("Header");
        BgmManager.Instance.Play(ResourceLoaderOrigin.GetBGMnameById(7));
        AddPopup("Popup_AlphaLoding");
        UserDataSetting.LoadRealUserData(UserData.GetUserID(), () =>
        {
            GacghaAPISetting.LoadGachaSeries(() =>
            {
                ResourceLoaderOrigin.GetBackGroundImage(3, (bgobj) =>
                {
                    AlphaLoding.Close();
                    StartCoroutine(SetStart());
                    backGround.sprite = bgobj;
                });

            });
        });
    

	}

    public IEnumerator SetStart()
    {
     
        iphoneXAjust(centerGO);
        UserData.TutoSet(UserData.TutoType.gacha);
        SetImage();

        yield break;
    }



    public void PushEvent( int count )
    {
        Debug.Log("ositeru");

        string description = "";

        if (UserData.GetCrystalNumber() < 150 && count == 1)
        {
            Debug.Log("daiyahusoku 150");
            husokuPopup.gameObject.SetActive(true);
            return;
        }

        if (UserData.GetCrystalNumber() < 1500 && count >= 10)
        {
            Debug.Log("daiyahusoku 1500");
            husokuPopup.gameObject.SetActive(true);
            return;
        }

        if(count==1)
        {
            description = "クリスタルを１５０個消費して１回、引きます";
        }
        else
        {
            description = "クリスタルを１５００個消費して１１回、引きます";
        }


        Layout_Gacha.gachaSeries = gachaId;
        Layout_Gacha.gachaCount = count;


        PopupGeneral.textValue = description;
        PopupGeneral.action = () =>
        {
            ChangeLayout("Gacha");
        };
        AddPopup("PopupGeneral");


    }




    public void SetImage()
    {
        UIcutin(gacharaMenuTF, -100f, 0);
        UIcutin(buttonMenuTF, 0, -100f);

        Footer.action = () =>
        {
            UIcutOut(gacharaMenuTF,-100f, 0);
            UIcutOut(buttonMenuTF, 0, -100f);
        };

        
        foreach( var Value in GacghaAPISetting.gachaGroupRealDatas )
        {
            var newGO = GameObject.Instantiate(menuBase,menuTF);
            newGO.GetComponent<GachaController>().Init(Value);
        }

        GachaGroupRealData gachaGroupRealData = GacghaAPISetting.gachaGroupRealDatas.FirstOrDefault();
        gachaId = (int)gachaGroupRealData.gahaID;
        gachaFinishDate.text = "" + gachaGroupRealData.finish_date + "まで " +  gachaGroupRealData.limit_time;
        menuBase.SetActive(false);
    }


    public void CloseHusoku()
    {
        husokuPopup.gameObject.SetActive(false);
    }


    bool setFlag;
    public void OpenTeikyo()
    {
        teikyopopup.SetActive(true);
        teikyoUnit.SetActive(false);
        UIsclalUp(teikyopopup.transform, 0.1f);

        if (setFlag)
            return;

        GachaGroupRealData gachaGroupRealData = GacghaAPISetting.gachaGroupRealDatas.FirstOrDefault();

        var dic = new Dictionary<RealCharaMasterData, int>();
        foreach (var KV in CharaAPISetting.realCharaMasterDatas)
            dic[KV.Value] = KV.Value.rare;

        var sorted = dic.OrderByDescending((x) => x.Value);  //降順

        foreach (var KV in sorted)
        {
            GameObject newGo = Instantiate(teikyoUnit, teikyoUnit.transform.parent);
            newGo.SetActive(true);
            string tx = $"{gachaGroupRealData.rare_per[KV.Key.rare]:F2}";
            tx = "★" + KV.Key.rare + " " + KV.Key.name + "  " + tx + "%";

            if (KV.Key.id == gachaGroupRealData.pickup1_character_id)
            {
                string txPick = $"{gachaGroupRealData.rare_per[0]:F2}";
                tx += " + " + txPick + "%";
            }
               

            newGo.GetComponent<GeneralData>().textMeshProUGUI.text = tx;
        }

        setFlag = true;

    }


    public void CloseYeikyo()
    {
        teikyopopup.SetActive(false);
        //UIscaleDown(teikyopopup.transform, 0.1f, () =>
        //{
    
        //});
    }
}
