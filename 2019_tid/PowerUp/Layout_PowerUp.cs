using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;

public class Layout_PowerUp : ScenePrefab {

    [SerializeField] GameObject teamBaseObject;
    [SerializeField] Transform teamParentTF;

    [SerializeField] GameObject allBaseObject;
    [SerializeField] Transform allParentTF;
    [SerializeField] Transform parentTF;
    [SerializeField] TextMeshProUGUI buttonText;
    [SerializeField] SpriteRenderer backGround;

    [SerializeField] TextMeshPro teamNoPageText;
    [SerializeField] GameObject lvupButtton;

    [SerializeField] TextMeshPro totalAttackText;
    [SerializeField] TextMeshPro totalHpText;

    [SerializeField] TextMeshPro totalAttackBournusValue;
    [SerializeField] TextMeshPro totalHpBournusValue;

    [SerializeField] GameObject leaderSelect_popup;

    [SerializeField] Image weaponImage;

    [SerializeField] Transform rightDire;
    [SerializeField] Transform leftDire;

    [SerializeField] Transform lvupParentTF;


    [SerializeField] TextMeshPro name;
    [SerializeField] TextMeshPro lv;
    [SerializeField] TextMeshPro attack;
    [SerializeField] TextMeshPro hp;
    [SerializeField] TextMeshPro nextEX;
    [SerializeField] TextMeshPro cri;
    [SerializeField] TextMeshPro mgc;
    [SerializeField] TextMeshPro def;

    [SerializeField] GameObject centerGO;

    [Header("CharaInformation")]
    [SerializeField] GameObject infoBase;
    [SerializeField] Transform infoParent;


    Coroutine moveCoroutine;
    public static RealCharaData infocharaName;
    [System.NonSerialized]public int teamNo = 0;

    List<AllCharaController> allCharaControllers = new List<AllCharaController>();
    List<List<TeamCharaController>> teamCharaControllers = new List<List<TeamCharaController>>();

    public Dictionary<int, Dictionary<int, RealCharaData>> partyCharaDataList = new Dictionary<int, Dictionary<int, RealCharaData>>();
    public List<RealCharaData> charaDataList = new List<RealCharaData>();

    int allAttackBorunusPoint;
    int allHPBorunusPoint;

    [SerializeField] Transform sideUI;

    void Start()
    {
   
        UIdirectionAnimation(rightDire, 1);
        UIdirectionAnimation(leftDire, 0);
        StartCoroutine(SetStart());
        AddSubLayout("Footer");
        AddSubLayout("Header");
       
    }
	// Use this for initialization
    public IEnumerator SetStart () {	
    
        AddPopup("Popup_AlphaLoding");
        iphoneXAjust(centerGO);

        BgmManager.Instance.Play(ResourceLoaderOrigin.GetBGMnameById(7));


        CharaAPISetting.LoadRealCharaData(UserData.GetUserID(), () =>
        {
            ItemAPISetting.LoadItemList(() =>
            {                          
                UserData.TutoSet(UserData.TutoType.powerup);
               
                partyCharaDataList = CharaAPISetting.partyCharaDataList;
                charaDataList = CharaAPISetting.charaDataList;

                ResourceLoaderOrigin.InstallBattleCharaImage(charaDataList, (spriteobj) =>
                 {
                     //voice install    
                     ResourceLoaderOrigin.InstallVoice(charaDataList, () =>
                     {
                         ResourceLoaderOrigin.GetBackGroundImage(14, (bgobj) =>
                         {
                             backGround.sprite = bgobj;
                             SetSort();
                             Init();
                             SetTotalInfo();
                             AlphaLoding.Close();
                         });
                     });
                 });
 

            });
        });
        yield return null;
	}

    public IEnumerator SetUpdate()
    {
        //AddSubLayout("Header");
        AddPopup("Popup_Loding");
        CharaAPISetting.LoadRealCharaData(UserData.GetUserID(), () =>
        {
            ItemAPISetting.LoadItemList(() =>
            {
                Loading.Close();

                AddPopup("LvUp", 1.5f);

                partyCharaDataList = CharaAPISetting.partyCharaDataList;
                charaDataList = CharaAPISetting.charaDataList;
                SetSort();
                Init();
                SetTotalInfo();
            });
        });
        yield return null;
    }

	private void Init()
	{
        teamCharaControllers.Clear();
        Footer.action = () =>
        {
            CharaAPISetting.SaveRealCharaData(partyCharaDataList, () =>
            {
               
            });
        };
        StartCoroutine(SetImage());
    }



	Vector3 firstPosi;
	public static int pasingNumber = 0;
    public IEnumerator SetImage( bool scaleFlaag = false )
	{
       

        for (int i = 0; i < parentTF.childCount; ++i)
            Destroy(parentTF.GetChild(i).gameObject);

        for (int i = 0; i < teamParentTF.childCount; ++i)
            Destroy(teamParentTF.GetChild(i).gameObject);

        teamNoPageText.text = "PARTY " + (teamNo+1);
        teamCharaControllers.Clear();

        for (int i = 0; i < 3; i++)
            teamCharaControllers.Add(new List<TeamCharaController>());                 
            
        teamBaseObject.SetActive(false);
        allBaseObject.SetActive(false);
        pasingNumber++;

        for (int i = 1; i < 6; i++)  
		{
            var newGO = GameObject.Instantiate( teamBaseObject,teamParentTF );;
			Transform newGoTF = newGO.transform;
            newGO.name = ""+i;
			newGO.SetActive(true);

            if( partyCharaDataList[teamNo][i] != null )
			{
                var Value = partyCharaDataList[teamNo][i];
                newGoTF.Find("type").GetComponent<Image>().sprite  = Resources.Load<Sprite>("Scenes/Image/UI/" + Value.charaTypeId);
                newGO.GetComponent<TeamCharaController>().realCharaData = Value;
                newGO.GetComponent<TeamCharaController>().countCopy = i;
                newGO.GetComponent<TeamCharaController>().Init();
                teamCharaControllers[teamNo].Add(newGO.GetComponent<TeamCharaController>());

			}else
			{
                newGO.GetComponent<TeamCharaController>().NotingAction();
            }
            if (scaleFlaag)
            {
                UIsclalUp(newGoTF);
                yield return null;
            }
          
        }
      
		//yield return null;
        teamBaseObject.SetActive(false);


		SetTotalInfo();
		StartCoroutine ( SetAllImage() );

		yield return null;
	}

	private bool enableClickFlag = false;
	private Coroutine setCoroutine;
	private IEnumerator SetAllImage()
	{

        for( int i=0; i < allParentTF.childCount; ++i ){
            GameObject.Destroy( allParentTF.GetChild( i ).gameObject );
		}


       
        allCharaControllers = new List<AllCharaController>();
		int allCount = 0;

        foreach( var Value in charaDataList )
		{
            var newGO = GameObject.Instantiate( allBaseObject,allParentTF );
			Transform newGoTF = newGO.transform;
            newGO.name = Value.charaName;

            int rare = Value.rareId;
            AllCharaController allCharaController = newGO.GetComponent<AllCharaController>();
            allCharaController.realCharaData = Value;
            allCharaController.countCopy = allCount;
            newGoTF.Find("type").GetComponent<Image>().sprite = Resources.Load<Sprite>("Scenes/Image/UI/" + Value.charaTypeId);
			
            foreach( var KV in partyCharaDataList[teamNo] )
			{
                if( Value == KV.Value )
				{
                    newGoTF.Find("Nothing").gameObject.SetActive(true);
                    newGoTF.Find("trriger").gameObject.SetActive(false);
                    allCharaController.partIn = true;
				}
			}
            newGO.SetActive(true);
            allCharaController.Init();

            allCharaControllers.Add(allCharaController);
			allCount++;

		}


        allAttackBorunusPoint = CharaAPISetting.allAttack;
        allHPBorunusPoint = CharaAPISetting.allHP;
        //Debug.Log( CharaAPISetting.allAttack +" allAttackBorunusPoint:" + allAttackBorunusPoint);
        //Debug.Log( CharaAPISetting.allHP +"allHPBorunusPoint:" + allHPBorunusPoint);
        totalAttackBournusValue.text = "" + allAttackBorunusPoint.ToString();
        totalHpBournusValue.text = "" + allHPBorunusPoint.ToString();

        allBaseObject.SetActive(false);
        selectRealCharaData = allCharaControllers[0].realCharaData;


        GetController(infocharaName);

		yield return null;
	}


    private void GetController( RealCharaData realCharaData )
    {
        if(realCharaData == null)
        {
            foreach( var Value in teamCharaControllers[0] )
            {
                if(Value != null)
                {
                    infocharaName = Value.realCharaData;
                    Value.ClickEvent();
                    break;
                }
            }
        }else
        {
            //allchara search
            foreach( var Value in allCharaControllers )
            {
                if(!Value.partIn && Value.realCharaData.charaIdNumber == realCharaData.charaIdNumber)
                {
                    infocharaName = Value.realCharaData;
                    Value.ClickEvent();
                    break;
                }                    
            }

            foreach (var Value in teamCharaControllers[teamNo])
            {
                if (Value.realCharaData.charaIdNumber == realCharaData.charaIdNumber)
                {
                    infocharaName = Value.realCharaData;
                    Value.ClickEvent();
                    break;
                }
            }
        }
    }


	private IEnumerator SetParty( GameObject newGO )
	{
		while( true )
		{
			Vector3 screenPos = Input.mousePosition;
			Vector3 worldPos = Camera.main.ScreenToWorldPoint (screenPos);
			newGO.transform.position = new Vector3 ( worldPos.x, worldPos.y, -100 );

			yield return null;
		}
	}



    public void SetCharaInformation(RealCharaData realCharaData)
    {
        //初回起動は所持キャラクターの最初のキャラの情報を表示
        if( realCharaData == null)
        {
            realCharaData = charaDataList[0];
            Debug.Log("初回：" + realCharaData.charaName);
        }
  
        infocharaName = realCharaData;
        name.text = ""+realCharaData.charaName;
        attack.text = "" +realCharaData.GetRealAttack();
        hp.text = "" +realCharaData.GetRealHP();
        mgc.text = "" + realCharaData.GetRealRepairForStatusView();
        cri.text = "" + ( realCharaData.GetRealCriForStatusView());
        def.text = "" + realCharaData.GetRealGuardForStatusView();

        weaponImage.color = new Color(255f, 255, 255, 1);
        if( realCharaData.item_master_id != 0)
            ResourceLoaderOrigin.GetItemImage(realCharaData.item_master_id, (Sprite obj) => { weaponImage.sprite = obj; });
        else
            weaponImage.color = new Color(255f, 255, 255, 0);

        PowerUpTask( realCharaData );

		infoBase.SetActive(true);

		//今あるやつ全部消す
		for( int i=0; i < infoParent.childCount; ++i )
            Destroy(infoParent.GetChild(i).gameObject);

        int count = 1;
        float ajustY = 105f;
        Vector3 lastPosi = new Vector3();
        foreach (var KV in UserData.GetCharacterSkillSetList(realCharaData.charaIdNumber))
        {
            var newGO = GameObject.Instantiate(infoBase, infoParent);
            Transform newGoThisTF = newGO.transform;
            newGoThisTF.localPosition = new Vector3(infoBase.transform.localPosition.x, infoBase.transform.localPosition.y - (( count- 1) * ajustY), newGoThisTF.localPosition.z - 30);
            lastPosi = newGoThisTF.localPosition;
            newGoThisTF.Find("Dice").GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Scenes/Image/UI/Dice/" + count);          
            newGoThisTF.Find("effectText").GetComponent<TextMeshPro>().text = CharaAPISetting.GetExplane(realCharaData.realCharaMasterData.realActionDataDic[KV.Value], false);          
            count++;
        }

        var newGOskill = GameObject.Instantiate(infoBase, infoParent);           
        Transform newGoThisTFskill = newGOskill.transform;
        newGoThisTFskill.localPosition = new Vector3(infoBase.transform.localPosition.x, lastPosi.y - ajustY , newGoThisTFskill.localPosition.z - 30);
        newGoThisTFskill.Find("Dice").GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Image/UI/Skill");
        newGoThisTFskill.Find("effectText").GetComponent<TextMeshPro>().text = CharaAPISetting.GetExplane(realCharaData.realCharaMasterData.realSkillActionData, false);   

		infoBase.SetActive(false);
	}

    int needEx;
    private void PowerUpTask( RealCharaData realCharaData )
	{
        int nowCharaLV = realCharaData.lv;
        lv.text = "Lv. " + nowCharaLV;
        needEx =  CharaAPISetting.expByLevel[nowCharaLV].next_experience;//exTable.param[nowCharaLV - 1].AllEx /*- UserData.GetCharacterExNumber( charaID )*/;

		if (realCharaData.lv >= GameSetting.maxCharaLv)
			nextEX.text = string.Empty;
		else
			nextEX.text = "" + needEx;

        selectRealCharaData = realCharaData;

	}

    public void PowerUpPushEvent()
    {
        CharaAPISetting.SaveRealCharaData(partyCharaDataList, () =>
        {
       
        });
        Layout_CharaSetting.realCharaData = selectRealCharaData;
        ChangeLayout("CharaSetting");

    }


    RealCharaData selectRealCharaData;
    public void InfoPush()
    {
        if (selectRealCharaData == null)
            return;

        CharaAPISetting.SaveRealCharaData(partyCharaDataList, () =>
        {
      
        });
        LvUpFunction(selectRealCharaData);

    }


    public void SetFriendChara()
    {

        AddPopup("Popup_Loding");
        CharaAPISetting.SetFriendChara(selectRealCharaData.charaIdNumber, () =>
        {
            Loading.Close();
            UserData.SetFriendChara(selectRealCharaData.charaIdNumber);
            Header.SetChara();
        });
    }



    private void LvUpFunction( RealCharaData realCharaData )
	{
        Popup_ConfirmCharaPowerUp.realCharaData = realCharaData;
        Popup_ConfirmCharaPowerUp.needGold = needEx;
        Popup_ConfirmCharaPowerUp.action = (() =>
        {
            LvUp(realCharaData);
        });
        AddPopup("ConfirmCharaPowerUp");
	}

    bool infoReplaceFlag = false;
    RealCharaData infoTargetCharaData;
    public void LvUp( RealCharaData realCharaData)
    {
        //キャラのステータス上がり率の計算
        partyCharaDataList = CharaAPISetting.partyCharaDataList;
        charaDataList = CharaAPISetting.charaDataList;

        Debug.Log(realCharaData.charaName + ";" + realCharaData.charaIdNumber);

        infoReplaceFlag = true;
        infoTargetCharaData = realCharaData;
        Init();
        StopAllCoroutines();
        StartCoroutine(SetUpdate());
    }

	private void SetTotalInfo()
	{

		int totalAttack = 0;
		int totalHP = 0;

        foreach( var KV in partyCharaDataList[teamNo])
		{
            if( KV.Value == null )
            {
                continue;   
            }
            totalAttack += KV.Value.realAttack;
            totalHP += KV.Value.realMaxHP;
		}
        totalAttackText.text = "" + totalAttack;
        totalHpText.text = "" + totalHP;
	}

    public void AllScaleBack()
    {
        foreach( var Value in allCharaControllers )
        {
            Value.ChangeScale(false);
        }

        foreach( var Value in teamCharaControllers[teamNo] )
        {
            Value.ChangeScale(false);
        }
    }


    int caseNumber = 0;
    public void PushEvent()
    {
        UserData.SetSortNumber(UserData.GetSortNumber() + 1);
        if (UserData.GetSortNumber() == 3)
            UserData.SetSortNumber(0);

        SetSort();
        SetTotalInfo();
        Init();

    }



    public void SetSort()
    {
        Dictionary<RealCharaData, int> dic = new Dictionary<RealCharaData, int>();
        if (UserData.GetSortNumber() == 0)
        {
            foreach (var Value in charaDataList)
                dic[Value] = Value.realAttack;
            
            buttonText.text = "攻撃力順";
        }
        else if (UserData.GetSortNumber() == 1)
        {
            foreach (var Value in charaDataList)
                dic[Value] = Value.realMaxHP;

            buttonText.text = "HP順";
        }
        else
        {
            foreach (var Value in charaDataList)
                dic[Value] = Value.rareId;

            buttonText.text = "レア順";
        }

        charaDataList = new List<RealCharaData>();
        var sorted = dic.OrderByDescending((x) => x.Value);  //降順

        foreach (var Value in sorted)
            charaDataList.Add(Value.Key);
    }


    public void BuckAction()
    {
        ChangeLayout("Menu"); 
    }


    public void TeamTransition( int addPage )
    {
        StopAllCoroutines();

        teamNo = teamNo + addPage;

        if( teamNo > 2 )
            teamNo = 0;

        if(teamNo < 0 )
            teamNo = 2;

        StartCoroutine ( SetImage(true) );
        SetCharaInformation(infocharaName);

    }


    //1:火 2:水 3:風 4:雷 9:無属性  
    public static Dictionary<int, string> colorCordList = new Dictionary<int, string>()
    {
        { 1,"#FF0000FF"},//red
        { 2,"#0094FFFF"},//blue
        { 4,"#A9FFB8FF"},//green
        { 3,"#FFE100FF"},//yellow
        { 5,"#FFFFFFFF"},//white

    };

    public void SaveAciton()
    {
        AddPopup("Popup_Loding");
        CharaAPISetting.SaveRealCharaData(partyCharaDataList, () =>
        {
            CharaAPISetting.LoadRealCharaData(UserData.GetUserID(), () =>
            {
                ItemAPISetting.LoadItemList(() =>
                {
                    Loading.Close();
                    partyCharaDataList = CharaAPISetting.partyCharaDataList;
                    charaDataList = CharaAPISetting.charaDataList;
                    SetSort();
                    Init();
                    SetTotalInfo();
                });
            });
        });

    }


    public void OpenLeaderselect()
    {
        AddPopup("Popup_Loding");
        CharaAPISetting.GetLeaderAllLeaderCharacter(() =>
        {
            List<int> ids = new List<int>();
            foreach (var KV in CharaAPISetting.leaderCharaDataList)
                ids.Add(KV.Value.No);


            ResourceLoaderOrigin.InstalCharaSpine(ids, () =>
             {
                 Loading.Close();
                 leaderSelect_popup.gameObject.SetActive(true);
             });

        });

    }


    public void WeponImagePush()
    {
        if (infocharaName.item_master_id == 0)
            return;

        Popup_EquipmentInfo.realItemData = ItemAPISetting.GetRealItemDataById(infocharaName.item_master_id);
        AddPopup("EquipmentInfo");
    }

}
