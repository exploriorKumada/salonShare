using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;
using DG.Tweening;

public class Layout_CharaSetting : ScenePrefab {

    public static RealCharaData realCharaData;   
    [SerializeField] TextMeshPro name;
    [SerializeField] TextMeshPro lv;
    [SerializeField] TextMeshPro attack;
    [SerializeField] TextMeshPro hp;
    [SerializeField] TextMeshPro nextEX;
    [SerializeField] TextMeshPro cri;
    [SerializeField] TextMeshPro mgc;
    [SerializeField] TextMeshPro def;
    [SerializeField] SpriteRenderer backGround;
    [SerializeField] GameObject go;
    [SerializeField] Transform parent;
    [SerializeField] Image type;

    [SerializeField] Image weaponImage;
    [SerializeField] CharaEquipmentController charaEquipmentController;
     
    [SerializeField] GameObject skillObject;
    [SerializeField] Transform skillParentTF;

    [SerializeField] GameObject skillSelectObject;
    [SerializeField] Transform skillSelectParentTF;

    [SerializeField] GameObject skillScrollObject;
    [SerializeField] GameObject equipmentScrollObject;

    [SerializeField] GameObject equipmentGO;
    [SerializeField] Transform equipmentTF;

    [SerializeField] GameObject nothingObject;

    [SerializeField] GameObject nothingText;

    [SerializeField] GameObject centerGO;

    [SerializeField] TextMeshProUGUI cvName;

    [SerializeField] Transform newGoTF;

    [SerializeField] Transform rightUI;
    [SerializeField] Transform leftUI;

    Vector3 skillScrollObjectBasePosi;
    Vector3 equipmentScrollObjectBasePosi;

    public List<float> selectPositionList = new List<float>();
    public List<SetSkillController> setSkillControllerList = new List<SetSkillController>();
    int nowCharaLV;

    Dictionary< int,int > enableSetSkill = new Dictionary<int,int>()
    {
        {7,10}, 
        {8,20}, 
        {9,40}, 
        {10,60} 
    };
    //10,20,40,60
	void Start () {

        ResourceLoaderOrigin.GetBackGroundImage(1, (bgobj) =>
        {
            backGround.sprite = bgobj;
        });
        AddSubLayout("Header");
        UserData.TutoSet(UserData.TutoType.charasetting);
        selectTF.gameObject.SetActive(false);
        setSkillControllerList = new List<SetSkillController>(); 
        skillScrollObjectBasePosi = skillScrollObject.transform.localPosition;
        equipmentScrollObjectBasePosi = equipmentScrollObject.transform.localPosition;
        cvName.text = "CV:" + realCharaData.realCharaMasterData.cv_name;

        UIcutin(rightUI, 100f, 0);
        UIcutin(leftUI, -100f, 0);
        StartCoroutine(SetStart());
	}


    public IEnumerator SetStart()
    {
        iphoneXAjust(centerGO);
        BgmManager.Instance.Play(ResourceLoaderOrigin.GetBGMnameById(7));
        SetBase();

        yield return null;
    }
    public void SetBase()
    {
        SetImage();
        SetEquipmentImage();

    }



	private void SetImage()
	{
        nowCharaLV = realCharaData.lv;
		StartCoroutine( SetSelectSkill() );
		SetAllSkill();
		SetCharaStatus();
		//SetSelectNow(selectNowNumber);
	}

   

    private void SetCharaStatus()
	{

        RealCharaData Value = realCharaData;
        string rare = realCharaData.rare;
        ResourceLoaderOrigin.GetBattleCharaImage(realCharaData.charaIdNumber, (Sprite obj) => { newGoTF.Find("charaImage").gameObject.GetComponent<SpriteRenderer>().sprite = obj; });
        name.text = realCharaData.charaName;
        lv.text = "Lv."+realCharaData.lv;
        attack.text = "" + realCharaData.GetRealAttack();
        hp.text = "" + realCharaData.GetRealHP();
        cri.text = "" + realCharaData.GetRealCriForStatusView();
        def.text = realCharaData.GetRealGuardForStatusView();
        mgc.text = realCharaData.GetRealRepairForStatusView();
        type.sprite = Resources.Load<Sprite>("Scenes/Image/UI/" + Value.charaTypeId);


        if (realCharaData.item_master_id != 0)
        {
            weaponImage.gameObject.SetActive(true);
            ResourceLoaderOrigin.GetItemImage(realCharaData.item_master_id, (Sprite obj) => { weaponImage.sprite = obj; });
        }
        else
        {
            weaponImage.gameObject.SetActive(false);
        }
        
        int needEx = CharaAPISetting.expByLevel[nowCharaLV].next_experience;
        nextEX.text = "" + needEx;
        name.CalculateLayoutInputVertical();

        SetRareImage();

	}

    public void SetRareImage()
    {
        for (int i = 0; i < realCharaData.rareId; i++)
        {
            var newGO = GameObject.Instantiate(go, parent);
            Transform newGoTFRare = newGO.transform;
            newGoTFRare.localPosition = new Vector3(newGoTFRare.localPosition.x + (100 * i), newGoTFRare.localPosition.y, newGoTFRare.localPosition.z);
            newGO.SetActive(true);
        }

        go.SetActive(false);

    }




	private IEnumerator SetSelectSkill()
	{
        for( int i=0; i < skillSelectParentTF.childCount; ++i )
            Destroy(skillSelectParentTF.GetChild(i).gameObject);

        int count = 1;
        foreach(var KV in UserData.GetCharacterSkillSetList(realCharaData.charaIdNumber))
		{
            var newGO = GameObject.Instantiate( skillSelectObject,skillSelectParentTF );
			Transform newGoThisTF = newGO.transform;
            newGoThisTF.localPosition = new Vector3( skillSelectObject.transform.localPosition.x, skillSelectObject.transform.localPosition.y - ( (count-1) * 75), newGoThisTF.localPosition.z - 20 );
            newGoThisTF.Find( "Dice" ).GetComponent<SpriteRenderer>().sprite  = Resources.Load<Sprite>("Scenes/Image/UI/Dice/" + count);    
            newGoThisTF.Find( "effectText" ).GetComponent<TextMeshPro>().text =CharaAPISetting.GetExplane( realCharaData.realCharaMasterData.realActionDataDic[KV.Value], false);
            newGoThisTF.localScale = new Vector3(0.8f,0.8f,1);
			selectPositionList.Add( newGoThisTF.localPosition.y + 11 );
            newGoThisTF.name = ""+KV.Value;
			newGO.SetActive(true);
            SetSkillController setSkillController = newGO.GetComponent<SetSkillController>();
            setSkillController.number = count;
            setSkillControllerList.Add(setSkillController);
            count++;
		}

        var newskillGO = GameObject.Instantiate(skillSelectObject, skillSelectParentTF);
        Transform newGoSkillThisTF = newskillGO.transform;
        newGoSkillThisTF.localPosition = new Vector3(skillSelectObject.transform.localPosition.x, skillSelectObject.transform.localPosition.y - ((count - 1) * 75), newGoSkillThisTF.localPosition.z - 20);
        newGoSkillThisTF.Find("Dice").GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Image/UI/Skill");
        newGoSkillThisTF.Find("effectText").GetComponent<TextMeshPro>().text = CharaAPISetting.GetExplane(realCharaData.realCharaMasterData.realSkillActionData, false);
        newGoSkillThisTF.localScale = new Vector3(0.8f, 0.8f, 1);
        newskillGO.SetActive(true);


		yield return null;
        skillSelectObject.SetActive(false);
	}

	private void SetAllSkill()
	{

        var infoBase = skillObject;
        var infoParent = skillParentTF;
		for( int i=0; i < infoParent.childCount; ++i )
		{
            if (infoParent.GetChild(i).gameObject == infoBase)
                continue;
            
			GameObject.Destroy( infoParent.GetChild( i ).gameObject );
		}
        int count = 1;
        foreach (var KV in realCharaData.realCharaMasterData.realActionDataDic)
		{
            if( KV.Value == null )
                continue;

            var newGO = GameObject.Instantiate( infoBase,infoParent );
			Transform newGoThisTF = newGO.transform;
            newGoThisTF.Find("text").GetComponent<TextMeshProUGUI>().text = CharaAPISetting.GetExplane(KV.Value,true);

            newGO.SetActive(true);

            HavingSkillController havingSkillController = newGO.GetComponent<HavingSkillController>();
            havingSkillController.number = count ;
            havingSkillController.realActionData = KV.Value;
            if (ExistSelectFlag(count, realCharaData.charaIdNumber))
            {
                newGoThisTF.Find("resultBaseShadow").gameObject.SetActive(true);
                havingSkillController.EnableClick(false);
            }
            else
            {
                newGoThisTF.Find("resultBaseShadow").gameObject.SetActive(false);
                havingSkillController.EnableClick(true);
            }


            if (count > 6 && enableSetSkill[count] > nowCharaLV)
            {
                newGoThisTF.Find("resultBaseShadow").gameObject.SetActive(true);
                newGoThisTF.Find("explane").gameObject.SetActive(true);
                newGoThisTF.Find("explane").GetComponent<TextMeshProUGUI>().text = "Lv." + enableSetSkill[count] + "で開放";
                havingSkillController.EnableClick(false);
            }

            if (count > 6 && KV.Value.type == 6)
            {
                newGO.SetActive(false);
            }
            count++;
		}

		infoBase.SetActive(false);

	}


	private bool ExistSelectFlag( int index , int charaId)
	{
        foreach( var KV in UserData.GetCharacterSkillSetList(charaId) )
        {
            if (index == KV.Value)
                return true;
        }
		return false;
	}

    [System.NonSerialized]public int selectNowNumber = -1;
    [SerializeField] Transform selectTF;
	public void SetSelectNow( int selectNumber )
	{
	    //case not set
        if (!selectTF.gameObject.activeSelf)
        {
            selectNowNumber = selectNumber;
            selectTF.localPosition = new Vector3(selectTF.localPosition.x, selectPositionList[selectNumber-1], selectTF.localPosition.z);
            selectTF.gameObject.SetActive(true);
            return;
        }

        if (selectNowNumber == -1)
            return;
        
        if (selectNowNumber == selectNumber)
        {
            //Debug.Log("kaijo");
            selectTF.gameObject.SetActive(false);
            return;
        }

        Debug.Log("交換します" + selectNumber + "番目:マスターNo" + UserData.GetCharacterSkillSet(realCharaData.charaIdNumber, selectNowNumber) + ":"+ selectNowNumber + "目:マスターNo" +UserData.GetCharacterSkillSet(realCharaData.charaIdNumber, selectNumber));

		AddAnimationTask (
			tag : "selectNumber",
			duration : 0.2f,
			startValue : selectTF.localPosition.y,
			endValue : selectPositionList[ selectNumber ],
			animationFunc : L3_Easing.QuinticOut,
			readyAction : () => {
			},
			firstAction : () => 
        {

            //前もって取っておかなければならない
            int before1 = UserData.GetCharacterSkillSet(realCharaData.charaIdNumber, selectNowNumber);
            int before2 = UserData.GetCharacterSkillSet(realCharaData.charaIdNumber, selectNumber);

            UserData.SetCharacterSkillSet(realCharaData.charaIdNumber, selectNumber,before1);
            UserData.SetCharacterSkillSet(realCharaData.charaIdNumber, selectNowNumber, before2);
            selectNowNumber = selectNumber;
            selectTF.gameObject.SetActive(false);
            StartCoroutine(SetSelectSkill());
            SetAllSkill();
        },
            runningAction : (r) => 
        {
				
            selectTF.localPosition = new Vector3( selectTF.localPosition.x, r, selectTF.localPosition.z );		
        },
			finishAction : () => 
        {
           
        }
		);

	}

    public void SetSelect( int selectBeforeNumber, int selectAfterNumber )
	{
        UserData.SetCharacterSkillSet( realCharaData.charaIdNumber,selectBeforeNumber, selectAfterNumber );
        StartCoroutine( SetSelectSkill() );
		SetAllSkill();
        //selectTF.gameObject.SetActive(false);
	}


    public void PowerUpTask()
    {

        Popup_ConfirmCharaPowerUp.realCharaData = realCharaData;
        Popup_ConfirmCharaPowerUp.action = (() =>
        {
            LvUp();
        });
        AddPopup("ConfirmCharaPowerUp");
    }


    public void LvUp()
    {
        //キャラのステータス上がり率の計算
        setSkillControllerList = new List<SetSkillController>();
        realCharaData = CharaAPISetting.GetRealcharaDataByCharaId(realCharaData.charaIdNumber);
        StartCoroutine(SetStart());
        AddPopup("LvUp", 1.5f);
    }


    bool skillSetFlag = true;
    public void ChangeEquipmentSetImage()
    {
        var sequence = DOTween.Sequence();

        //Debug.Log("kaemasu1");

        sequence.Append(
            skillScrollObject.transform.DOLocalMove(
            equipmentScrollObjectBasePosi,
            0.2f//時間  
            ));

        sequence.Append(
              
            equipmentScrollObject.transform.DOLocalMove(           
                skillScrollObjectBasePosi,
                0.2f//時間  
            ));

    }

    public void ChangeSkillSetImage()
    {
        //Debug.Log("kaemasu2");

        var sequence = DOTween.Sequence();
        sequence.Append(
        equipmentScrollObject.transform.DOLocalMove(
            equipmentScrollObjectBasePosi,
            0.2f//時間  
            ));

        sequence.Append(
            skillScrollObject.transform.DOLocalMove(
            skillScrollObjectBasePosi,
           0.2f//時間  
            ));
    }


    public void SetEquipmentImage()
    {
        equipmentGO.SetActive(true);
        //今あるやつ全部消す
        for (int i = 0; i < equipmentTF.childCount; ++i)
        {
            if (equipmentTF.GetChild(i).gameObject == equipmentGO)
                continue;

            Destroy(equipmentTF.GetChild(i).gameObject);
        }

        if (realCharaData.item_master_id != 0)
        {
            RealItemData itemData = new RealItemData()
            {
                item_master_id = -1,
                name = "装備を外す"
            };
            var newGO = GameObject.Instantiate(equipmentGO, equipmentTF);
            Transform newGoThisTF = newGO.transform;
            newGO.GetComponent<CharaEquipmentController>().Initialized(itemData, realCharaData);
        }


        foreach ( var Value in ItemAPISetting.realItemDatas[3])
        {
            var newGO = GameObject.Instantiate(equipmentGO, equipmentTF);
            Transform newGoThisTF = newGO.transform;
            newGO.GetComponent<CharaEquipmentController>().Initialized(Value, realCharaData); 
        }


        equipmentGO.SetActive(false);
        nothingText.SetActive(ItemAPISetting.realItemDatas[3].Count == 0);
           

    }

    public void Back()
    {
        UIcutOut(rightUI, 100f, 0);
        UIcutOut(leftUI, -100f, 0);

    }

    public void PushCharaAction()
    {
        Singleton<SoundPlayer>.instance.CharaVoice(realCharaData.charaIdNumber, "select",true);
    }

    public void PushWeapomAction()
    {
        AddPopupWeaponDetail(realCharaData.item_master_id);
    }
}
