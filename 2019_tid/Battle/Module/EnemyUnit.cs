using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System;

public class EnemyUnit : ScenePrefab
{

    [SerializeField] BuffDebuffManager buffDebuffManager;
    [SerializeField] Transform itemBaseTF;
    [SerializeField] DropItemCotroller dropItemCotroller;
    [SerializeField] ParticleManager particleManager;
    [SerializeField] Transform particleParentTF;
    [SerializeField] Image hpGauge;
    [SerializeField] BattleLayoutManager battleLayoutManager;
    [SerializeField] TextMeshProUGUI textTF;

    //effect
    [SerializeField] _2dxFX_DestroyedFX _2DxFX_DestroyedFX;
    [SerializeField] _2dxFX_PixelDie _2DxFX_PixelDie;

    [SerializeField] Image charaImage;

    [SerializeField] BattleManager battleManager;

    public RealCharaData realCharaData;
    public List<BuffDebuffData> buffDebuffDatas = new List<BuffDebuffData>();

    public bool bossFlag = false;

    public void Init( float delaySecond, bool flag = false )
	{
        buffDebuffManager.Init();
        //Debug.Log(realCharaData.realCharaMasterData.name + ":" + realCharaData.lv);
        StartCoroutine( SetAnimation( delaySecond, flag));

	}

    public IEnumerator SetAnimation( float delaySecond, bool flag )
    {
       
        yield return new WaitForSeconds(delaySecond);
        _2DxFX_PixelDie._Value1 = 1;
        if( bossFlag )
        {
            particleManager.SetEffect(particleParentTF,2,true);
            CameraSetting.Shake(200);
            yield return new WaitForSeconds(4f);
            transform.Find("Animator").gameObject.SetActive(true);
            transform.Find("Animator").localScale = transform.Find("Animator").localScale * 1f;

        }else
        {
            particleManager.SetEffect(particleParentTF, 31, true);
            yield return new WaitForSeconds(0.1f);
            transform.Find("Animator").gameObject.SetActive(true);
        }

        _2DxFX_DestroyedFX.enabled = false;
        _2DxFX_PixelDie.enabled = true;

     
        DOTween.To(
            () => _2DxFX_PixelDie._Value1,          // 何を対象にするのか
            num => _2DxFX_PixelDie._Value1 = num,   // 値の更新
            0,                  // 最終的な値
            0.5f                  // アニメーション時間
        ).OnComplete(() => {
            if (flag)
            {
                Debug.Log("敵の出現完了");
                battleManager.UiInit();
            }
            Destroy(_2DxFX_PixelDie);
        }); 

    }




    public void SetBuffDebuff(BuffDebuffData buffDebuffData)
    {
        buffDebuffDatas.Add(buffDebuffData);
        SetBuffDebuffImage();
    }

    public void SetBuffDebuffImage()
    {
        buffDebuffManager.SetImage(buffDebuffDatas);
    }


    public void Damage( RealActionData realActionData ,RealCharaData teamRealCharaData, Action action)
    {

        Singleton<SoundPlayer>.instance.CharaVoiceByProbabilityAtBattle(realCharaData.charaIdNumber, "guard");

        int amountNumber = realActionData.damageAmount;
        amountNumber=(int)(amountNumber + UnityEngine.Random.Range(0.98f, 1.02f));
        string resultText = realCharaData.charaName + ":敵の情報";
        float sukumi = CalculationManager.GetBaiType(realCharaData.charaTypeId, teamRealCharaData.charaTypeId);
        resultText += "\n 敵属性情報:" + realCharaData.charaTypeId + " 味方属性情報:" + teamRealCharaData.charaTypeId;
        resultText += "\n 属性すくみ前:" + sukumi + " amountNumber:" + amountNumber;
        amountNumber = (int)( amountNumber * sukumi);
        resultText += "\n 属性すくみ計算後:" + amountNumber;

        amountNumber = (int)(amountNumber * (1 - (realCharaData.GetRealGuard())));
        resultText += "\n 防御計算後:" + amountNumber + " realCharaData.GetRealGuard():" + realCharaData.GetRealGuard();

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
        amountNumber = (int)(amountNumber * (1  - buffDebuffTotalAttack));
        resultText += "\n buffDebuffTotalAttack:" + buffDebuffTotalAttack + " damage effect:" + amountNumber;

        resultText += "\n teamRealCharaData.GetRealCri():" + teamRealCharaData.GetRealCri();
        //クリティカル率
        int criValue = (int)( (0.1f + teamRealCharaData.GetRealCri())*100 );
        int criValue2 = UnityEngine.Random.Range(0, 100);

        Debug.Log(teamRealCharaData.charaName + " chara cri:" + criValue + "  cri ad random:"+ criValue2);
        if (criValue > criValue2)
        {
            Debug.Log("critical!!!!");
            amountNumber = (int)(amountNumber * 1.7f);
            realActionData.criFlag = true;
            resultText += "\n cri:" + amountNumber;
        }

        if ( battleManager.debugmode)
            Debug.Log(resultText);

        realCharaData.realHP = realCharaData.realHP - amountNumber;

        string damageText = "<color=#FF0000FF>" + amountNumber;
        if (realActionData.criFlag)
        {
            damageText = "<color=#FF0000FF>Critical\n" + damageText;
        }

        TextSetOn(damageText);
        SetEffect(realActionData);

        if ( realCharaData.realHP <= 0 && realCharaData.liveFlag  )               
        {
            realCharaData.realHP = 0;
            realCharaData.liveFlag = false;
            DeadAction();
        }
        SetHpGauge();
        Shake(transform);

        if (action != null)
        {
            //Debug.Log("player Acctack Complete:" + realActionData.type + ":" + realCharaData);
            action();
        }
        
    }


    private void DeadAction()
    {
        //Debug.Log("宝箱ドロップ処理");
        Dead(1);
    }

    public void Dead( int dropLv){ StartCoroutine(( DeadOn(dropLv)));}
    public IEnumerator DeadOn( int dropLv)
	{
        SetHpGauge();
        particleManager.SetEffect(particleParentTF,1, true);
        yield return new WaitForSeconds(0.1f);

        _2DxFX_DestroyedFX.enabled = false;
        _2DxFX_DestroyedFX.enabled = true;

        DOTween.To(
            () => _2DxFX_DestroyedFX.Destroyed,          // 何を対象にするのか
            num => _2DxFX_DestroyedFX.Destroyed = num,   // 値の更新
            1,                  // 最終的な値
            1.5f                  // アニメーション時間
        ).OnComplete(() => {
                transform.Find("Animator").gameObject.SetActive(false);
                //var dropItemData = DropItemsSetting.GetRealItem(dropLv);
                //dropItemCotroller.SetImage(dropItemData);
        });
	}


    public void SetActionTextImage(string text)
    {
        //result.gameObject.SetActive(true);
        //result.transform.Find("effectText").GetComponent<TextMeshPro>().text = text;
        //result.transform.localPosition = baseResultPosi;

        //result.transform.DOLocalMove(
        //    new Vector3(result.transform.localPosition.x, result.transform.localPosition.y + 10, 0),    // 移動終了地点座標
        //    3f                            // アニメーション時間
        //).OnStart(() => {
        //    result.transform.localPosition = baseResultPosi;
        //}).OnComplete(() => {
        //    result.gameObject.SetActive(false);
        //});

    }

    public void ActionMove()
    {
        ImageEffect.SetEffect(() =>
        {         

        }, charaImage);
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
		//hpNumber.text = realBattleCharaData.realhp + "/" + realBattleCharaData.maxHp;
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

    public void SetEffect(RealActionData realActionData)
    {
        if (realActionData.actionTypeId == 1 && realActionData.effectId != 0)
        {
            particleManager.SetEffect(particleParentTF, realActionData.effectId, true);
        }
        else
        {
            
            if (realActionData.type == 1 || realActionData.type == 7 )
            {
                if (realActionData.criFlag)
                    particleManager.SetEffect(particleParentTF, 28, true);
                else
                    particleManager.SetEffect(particleParentTF, 32, true);
            }
            else if (realActionData.type == 2)
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

    Coroutine textCoroutine;
    public void TextSetOn(string amountText)
    {
        StartCoroutine(TextSet(amountText));
    }
    private IEnumerator TextSet(string amountText)
    {
        GameObject newGo = Instantiate(textTF.gameObject, textTF.transform.parent);
        newGo.transform.localPosition = new Vector2( UnityEngine.Random.Range(-1f,1f) * newGo.transform.localPosition.x* UnityEngine.Random.Range(0.8f,1.3f), UnityEngine.Random.Range(-1f, 1f)*newGo.transform.localPosition.y * UnityEngine.Random.Range(0.8f, 1.3f));
        TextMeshProUGUI newText = newGo.GetComponent<TextMeshProUGUI>();
        newText.text = amountText;
        newGo.SetActive(true);
        yield return new WaitForSeconds(2f);
        Destroy(newGo);

    }
}
