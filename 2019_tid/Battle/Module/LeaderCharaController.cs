using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Spine.Unity;

public class LeaderCharaController : ScenePrefab {

	[SerializeField] Transform leaderChara;
	[SerializeField] Image skillGaurge;
    [SerializeField] Button skillButton;
    [SerializeField] BattleManager battleManager;
    [SerializeField] Transform spineTF;
    [SerializeField] ParticleManager particleManager;
    [SerializeField] Transform prefabBase;
    [SerializeField] _2dxFX_LightningBolt _2DxFX_LightningBolt;
    [SerializeField] SkeletonAnimation skeletonAnimation;
    [SerializeField] BattleLayoutManager battleLayoutManager;

    [SerializeField] GameObject selectGameObject;

	int maxSkillGgauge = 100;
	int realSkillGgauge = 0;

    Dictionary<int,float> charaPosi = new Dictionary<int,float>()
	{
		{1,-293f},
		{2,-452f},
		{3,-76f},
		{4,-326f},
	};

    public void Init(LeaderCharaSettingBase leaderCharaSettingBase)
    {
    
        ResourceLoaderOrigin.GetSpine(leaderCharaSettingBase.No, (obj) =>
        {
            Debug.Log("リーダーマスターID;" + leaderCharaSettingBase.No);
            int charaId = leaderCharaSettingBase.No;
            skeletonAnimation.skeletonDataAsset = obj;
            skeletonAnimation.skeletonDataAsset.atlasAssets[0].materials[0].shader = Shader.Find("Spine/Special/SkeletonGhost");
            skeletonAnimation.Initialize(true);
            skeletonAnimation.timeScale = 0.3f;
            skeletonAnimation.transform.localPosition = new Vector3(skeletonAnimation.transform.localPosition.x, CharaSetting.charaSpineAjust[charaId], 0);

            SetSkill(0, true);

        });
  
	}

    public void SetTCUInfo()
    {
        TeamCharacterUnit teamCharacterUnit = new TeamCharacterUnit();
        //teamCharacterUnit.realCharaData
        
    }

    public void SetSkill( int number , bool forceFlag = false )
	{
        if (forceFlag)
            realSkillGgauge = number;
        else
            realSkillGgauge = realSkillGgauge + number*100;

            
        if( realSkillGgauge >= maxSkillGgauge)
        {
            realSkillGgauge = maxSkillGgauge;

            skillButton.GetComponent<Button>().interactable = true;
            skillGaurge.GetComponent<_2dxFX_LightningBolt>().enabled = true;

            EnableSkill(true);
        }else
        {
            skillButton.GetComponent<Button>().interactable = false;
            skillGaurge.GetComponent<_2dxFX_LightningBolt>().enabled = false;
            EnableSkill(false);
        }
		SetSkillGauge ();
	}


	public void SetSkillGauge()
	{		
		skillGaurge.fillAmount = (float)realSkillGgauge / (float)maxSkillGgauge;
	}


    public void EnableSkill( bool flg )
    {
        skillButton.interactable = flg;
    }


    public void UseSkill()
    {
        if (battleManager.actionFlag)
            return;
        
        if (realSkillGgauge < maxSkillGgauge)
            return;

        if( !selectGameObject.activeSelf )
        {
            battleManager.leaderSkillSetFlag = true;
            selectGameObject.SetActive(true);
        }else
        {
            battleManager.leaderSkillSetFlag = false;
            selectGameObject.SetActive(false);
        }
    }

    public void TurnInit()
    {
        selectGameObject.SetActive(false);
    }

    public void ActionMove()
    {
        Transform charaImageTF = spineTF;
        Vector3 baseV3 = charaImageTF.localScale;
        Vector3 baseV3Position = charaImageTF.localPosition;

        charaImageTF.DOScale(
           baseV3 * 1.1f,     // 終了時点のScale
           0.2f                        // アニメーション時間
        ).OnComplete(() => {
            charaImageTF.DOScale(
               baseV3,     // 終了時点のScale
               0.2f                        // アニメーション時間
            ).OnComplete(() => {
                charaImageTF.localScale = baseV3;
                charaImageTF.localPosition = baseV3Position;
            }); 
        });
    }

}
