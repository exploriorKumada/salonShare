using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System;

public class TeamCharacterUnit : ScenePrefab {

	[SerializeField] BuffDebuffManager buffDebuffManager;
	[SerializeField] Image skillGauge;
	[SerializeField] Image hpGauge;
	[SerializeField] GameObject result;
    [SerializeField] Transform resultPaarent;

	[SerializeField] TextMeshPro hpNumber;
    [SerializeField] GameObject skillButton;
	[SerializeField] GameObject detailButton;
	[SerializeField] GameObject skillSelectingText;

    [SerializeField] ParticleManager particleManager;
    [SerializeField] Transform particleParentTF;
    [SerializeField] Transform statusTF;

    [SerializeField] TextMeshPro statusText;
    [SerializeField] GameObject acitonImage;
    [SerializeField] List<Sprite> actionIconList;
    [SerializeField] Transform typeTF;
    [SerializeField] BattleLayoutManager battleLayoutManager;

    [SerializeField] Image charaImage;
    [SerializeField] BattleManager battleManager;

    [SerializeField] public TextMeshProUGUI textTF;
    [SerializeField] GameObject deadObject;

    [SerializeField] TextMeshProUGUI damagedEx;


    [SerializeField] GameObject exist;
    [SerializeField] GameObject noexist;

    [SerializeField] GameObject friendObject;

    [SerializeField] CharaDetailOfBattle charaDetail;


    public RealCharaData realCharaData;
    Vector3 baseResultPosi;
    Color thisColor;
    Dictionary<string, Sprite> actionIconDic = new Dictionary<string, Sprite>();

    public List<BuffDebuffData> buffDebuffDatas = new List<BuffDebuffData>();

    [NonSerialized]public float damagedBairitu;

    public void Init( RealCharaData replyRealCharaData )
	{
        //gameObject.SetActive(true);
        if ( replyRealCharaData == null )
        {
            exist.SetActive(false);
            noexist.SetActive(true);
            return;
        }else
        {
            exist.SetActive(true);
            noexist.SetActive(false);
            realCharaData = replyRealCharaData;
        }
        SetImageIconDic();
        baseResultPosi = result.transform.localPosition;
        buffDebuffManager.Init();
        deadObject.SetActive(false);
        //result.gameObject.SetActive (false);
        skillSelectingText.SetActive(false);
        realCharaData.actionType = "action";
        typeTF.gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>("Scenes/Image/UI/" + realCharaData.realCharaMasterData.type );

        if (battleManager.charaSpriteDic.ContainsKey(realCharaData.charaIdNumber))
        {
            charaImage.sprite = battleManager.charaSpriteDic[realCharaData.charaIdNumber];
        }
        else 
        {
            Debug.LogError("arimasen:" + realCharaData.charaIdNumber);

            ResourceLoaderOrigin.GetBattleCharaImage(realCharaData.charaIdNumber, (obj) =>
            {
                charaImage.sprite = obj;
            });
        }

        friendObject.SetActive(replyRealCharaData.friendFlag);

        SetSkill(0,true);
        SetHpGauge();
        damagedEx.text = "×" + damagedBairitu;
	}

    public void SetImageIconDic()
    {
        //gameObject.SetActive(true);
        actionIconDic["gaurd"] = actionIconList[0];
        actionIconDic["skill"] = actionIconList[1];
        actionIconDic["attack"] = actionIconList[2];
        
    }


    /// <summary>
    /// 表示開始アニメーション TODO
    /// </summary>
    public void InitSetAnimation()
    {
        gameObject.SetActive(true);
        UIcutin(transform,0,-500f);
    }


    public void SetButtonAction( int type )
    {
        if (battleManager.actionFlag)
            return;

        if( type == 1)
        {
            realCharaData.actionType = "gaurd";
            SetStatusText("ガード");
            //transform.Find("exist/charaImage").GetComponent<_2dxFX_LightningBolt>().enabled = false;

        }else if( type == 2 ){

            if (realCharaData.skilRealNumber != realCharaData.realCharaMasterData.skill_max_number || !realCharaData.liveFlag )
            {
                Debug.Log("tamattenai shinderu");
                return;
            }

            if( !skillSelectingText.activeSelf )
            {
                skillSelectingText.SetActive(true);
                realCharaData.actionType = "skill";  
            }else
            {
                skillSelectingText.SetActive(false);
                realCharaData.actionType = null; 
            }
           
        }else{
        }
        
    }

    public void Damage( RealActionData realActionData, RealCharaData enemyCharaData )
    {

        Singleton<SoundPlayer>.instance.CharaVoiceByProbabilityAtBattle(realCharaData.charaIdNumber, "guard");
        
        int amountNumber = realActionData.damageAmount;
        amountNumber = (int)(amountNumber + UnityEngine.Random.Range(0.98f, 1.02f));
        string resultText = realCharaData.charaName + ":味方の情報";
        float sukumi = CalculationManager.GetBaiType(realCharaData.charaTypeId, enemyCharaData.charaTypeId);
        resultText += "\n 属性すくみ前:" + sukumi + " amountNumber:" + amountNumber;
        amountNumber = (int)(amountNumber * sukumi);
        resultText += "\n 属性すくみ計算後:" + amountNumber;

        resultText += "\n ガード前:" + (realCharaData.GetRealGuard(realCharaData.lv)) + " amountNumber:" + amountNumber;
        amountNumber = (int)(amountNumber * (1-(realCharaData.GetRealGuard(realCharaData.lv))));
        resultText += "\n ガード計算後:" + amountNumber;

        amountNumber = (int)(amountNumber * damagedBairitu);
        resultText += "\n 順番補正計算後:"+damagedBairitu+":" + amountNumber;

        float buffDebuffTotalAttack = 0;
        foreach (var Value in buffDebuffDatas)
        {
            if (Value.buffDebuffType == 2)
            {
                if (Value.buffDebuffID == 3)
                    buffDebuffTotalAttack = buffDebuffTotalAttack + Value.amount;
                else
                    buffDebuffTotalAttack = buffDebuffTotalAttack - Value.amount;

                resultText += "\n guard buff:" + Value.buffDebuffID + " : " + Value.amount;
            }
        }

        resultText += "\n damageInit:" + amountNumber;
        amountNumber = (int)(amountNumber * (1 - buffDebuffTotalAttack));
        resultText += "\n buffDebuffTotalAttack:" + buffDebuffTotalAttack + " damage effect:" + amountNumber;

        if (battleManager.debugmode)
            Debug.Log(resultText);

        realCharaData.realHP = realCharaData.realHP - amountNumber;

        TextSetOn("<color=#FF0000FF>" + amountNumber);
        SetEffect(realActionData);

        if (realCharaData.realHP <= 0 && realCharaData.liveFlag)
        {
            realCharaData.realHP = 0;
            realCharaData.liveFlag = false;
            Dead();
        }
        SetHpGauge();

        CameraSetting.Shake();
    }

    public void Repair( RealActionData realActionData, Action action)
    {
        if (action != null)
        {
            Debug.Log("player Repair Complete");
            action();
        }

        SetEffect(realActionData);
    }

    public void SetEffect( RealActionData realActionData)
    {
        if (realActionData.actionTypeId == 1 && realActionData.effectId != 0)
        {
            particleManager.SetEffect(particleParentTF, realActionData.effectId, true);
        }else
        {
            if(realActionData.type==1 || realActionData.type == 7 )
            {
                if (realActionData.criFlag)
                    particleManager.SetEffect(particleParentTF, 28, true);
                else
                    particleManager.SetEffect(particleParentTF, 32, true);
            }else if( realActionData.type == 2)
            {
                particleManager.SetEffect(particleParentTF, 4, true);
            }
            else if (realActionData.type == 3)
            {
                particleManager.SetEffect(particleParentTF, 20, true);
            }
            else
            {
                particleManager.SetEffect(particleParentTF, 13, true);
            }
        }          
    }

    public void SetEffect( int effectId )
    {
        particleManager.SetEffect(particleParentTF, effectId, true);
    }

	public void SetSkillGauge()
	{
        float endAmount = (float)realCharaData.skilRealNumber / (float)realCharaData.realCharaMasterData.skill_max_number;
        DOTween.To(
            () => skillGauge.fillAmount,                // 何を対象にするのか
            num => skillGauge.fillAmount = num,    // 値の更新
            endAmount,                    // 最終的な値
            0.5f                                // アニメーション時間
        );
	}

	public void SetSkill( int number, bool foceFlag = false )
	{
        if (foceFlag)
        {
            realCharaData.skilRealNumber = number;
        }else
        {
            realCharaData.skilRealNumber = realCharaData.skilRealNumber + number;
        }
	
        if( realCharaData.realCharaMasterData.skill_max_number <= realCharaData.skilRealNumber)
		{
			realCharaData.skilRealNumber = realCharaData.realCharaMasterData.skill_max_number;
            skillButton.GetComponent<Button>().interactable = true;
			skillGauge.GetComponent<_2dxFX_LightningBolt>().enabled = true;

        }else
        {
            skillButton.GetComponent<Button>().interactable = false;
			skillGauge.GetComponent<_2dxFX_LightningBolt>().enabled = false;
        }

		if( realCharaData.skilRealNumber <= 0)
		{
			realCharaData.skilRealNumber = 0;
		}
		SetSkillGauge ();
	}


    public bool SkillMaxGuage()
    {
        return (realCharaData.realCharaMasterData.skill_max_number <= realCharaData.skilRealNumber);
    }

    public void SetSkillBase(int number)
    {
        realCharaData.skilRealNumber = number;

        SetSkillGauge();
    }

    public void SetHpGauge()
    {

        float endAmount = (float)realCharaData.realHP / (float)realCharaData.realMaxHP;

        DOTween.To(
            () => hpGauge.fillAmount,                // 何を対象にするのか
            num => hpGauge.fillAmount = num,    // 値の更新
            endAmount,                    // 最終的な値
            0.5f                                // アニメーション時間
        );
        hpNumber.text = realCharaData.realHP + "/" + realCharaData.realMaxHP;
	}

	public void SetHp( int number )
	{
        realCharaData.realHP = realCharaData.realHP + number;
        if( realCharaData.realMaxHP <= realCharaData.realHP)
		{
            realCharaData.realHP = realCharaData.realMaxHP;
		}

        if( realCharaData.realHP <= 0)
		{
            realCharaData.realHP = 0;
		}
		SetHpGauge ();
	}

    public void Dead()
    {
        SetHp(0);
        //Debug.Log("sinimasita:" + realCharaData.charaName);
        deadObject.SetActive(true);
        buffDebuffDatas.Clear();
        battleManager.deadCount++;
        //フレンドがいれば差し替える処理
        if(BattleManager.selectFriendRealCharaData != null )
        {
            Debug.Log("フレンド差し替え");
            Init(BattleManager.selectFriendRealCharaData);
            BattleManager.selectFriendRealCharaData = null;
            particleManager.SetEffect(particleParentTF,26, true); 
        }else
        {
            realCharaData.liveFlag = false;
            particleManager.SetEffect(particleParentTF,1, true); 
        }
    }

    public void SetStatusText( string type )
    {
        statusText.gameObject.SetActive(true);
        acitonImage.GetComponent<Image>().sprite = actionIconDic[realCharaData.actionType];
        statusText.text = type;  
    }


    List<GameObject> resultList = new List<GameObject>();
	public void SetActionTextImage( string text )
	{
        var newGO = GameObject.Instantiate(result.gameObject,resultPaarent);
        newGO.gameObject.SetActive (true);
        newGO.transform.Find ("effectText").GetComponent<TextMeshProUGUI> ().text = text;

        resultList.Add(newGO);
        int count = 0;
        foreach( var Value in resultList )
        {
            count++;
            Value.transform.DOLocalMove(
                new Vector3(Value.transform.localPosition.x, Value.transform.localPosition.y + 104f, 0),    // 移動終了地点座標
                0.3f                            // アニメーション時間
            );                       
        }

        if( resultList.Count == 3 )
        {
            Destroy(resultList[0]); 
            resultList.RemoveAt(0);
        }
	}

    public void SetBuffDebuff( BuffDebuffData buffDebuffData )
    {
        buffDebuffDatas.Add(buffDebuffData);
        SetBuffDebuffImage();
    }

    public void SetBuffDebuffImage()
    {
        buffDebuffManager.SetImage( buffDebuffDatas );
    }


    public void ActionMove()
    {
        ImageEffect.SetEffect(() => 
        {
           
        },charaImage);
    }


    Coroutine textCoroutine;
    public void TextSetOn(string amountText) { 

        if( textCoroutine != null )
        {
            StopCoroutine(textCoroutine); 
        }

        textCoroutine = StartCoroutine(TextSet(amountText)); 
    }

    public void TurnInit()
    {
        skillSelectingText.SetActive(false);
        realCharaData.actionType = null; 
    }


    private IEnumerator TextSet(string amountText)
    {
        textTF.gameObject.SetActive(false);
        textTF.text = amountText;
        textTF.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        textTF.gameObject.SetActive(false);
    }

    public void Live()
    {
        deadObject.SetActive(false);
        realCharaData.liveFlag = true;
        realCharaData.realHP = realCharaData.realMaxHP;
        realCharaData.skilRealNumber = realCharaData.realCharaMasterData.skill_max_number;
        SetHpGauge();
        SetSkillGauge();
    }

    public void SetDetatilByPush()
    {
		if (battleManager.actionFlag)
			return;

		charaDetail.SetImage(this);
    }

    public void SetDetailByLongPush()
    {
        if (battleManager.actionFlag)
            return;

        Popup_PublicCharaInfo.realCharaData = realCharaData;
        AddPopup("PublicCharaInfo");
    }
			
}
