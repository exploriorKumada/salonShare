using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using DG.Tweening;
using UnityEngine.UI;

public class BattleLayoutManager : ScenePrefab {


	[SerializeField] Transform aTreasure;
	[SerializeField] Transform sTreasure;
    [SerializeField] public List<Button> buttons = new List<Button>();
    [SerializeField] public Popup_BattleMenu popup_BattleMenu;
    [SerializeField] public Popup_CompleteBattle popup_CompleteBattle;
    [SerializeField] DiceManager diceManager;
    [SerializeField] GameObject rightUI;
 
    [SerializeField] GameObject battleStartDialog;
    [SerializeField] GameObject gameOverDialog;
    [SerializeField] GameObject clearDialog;
    [SerializeField] GameObject rankUpDialog;
    [SerializeField] BattleManager battleManager;


    [SerializeField] TextMeshProUGUI baisokuText;

	public int aTresureNumber = 0;
	public int sTresureNumber = 0;

    private static bool pushFlah = false;

    Vector3 rightBasePosi;

	public void Init()
	{
        rightBasePosi = rightUI.transform.localPosition;
	
		TreasureNumberSet ("A",false);
		TreasureNumberSet ("S",false);             
	}

    public void DiceAction()
    {

        if( !pushFlah )
        {
            diceManager.DiceStartOn();
            pushFlah = true;
        }
    }
        


	public void TreasureNumberSet( string rareType, bool moveFlag = true )
	{
		Transform baseTF;
		int baseNumber;
		if (rareType == "A") {
			baseTF = aTreasure;
			baseNumber = aTresureNumber;
		} else {
			baseTF = sTreasure;
			baseNumber = sTresureNumber;
		}
			
		baseTF.Find ("number").GetComponent<TextMeshPro>().text = "×"+baseNumber;


		if( moveFlag )
		{			
			baseTF.Find("object").DOPunchScale(
				new Vector3(1.8f, 1.8f),    // scale1.5倍指定
				0.2f                            // アニメーション時間
			);
		}

	}


    public void EnableActionButtons(bool flag )
    {
        //Debug.Log("ボタン制御処理:" + flag);
        foreach( var Value in buttons )
        {
            if (Value == null) { continue; }
            Value.interactable = flag;
        }
        
    }
    public void MenuOpen()
    {
        if (battleManager.actionFlag)
            return;
        
        popup_BattleMenu.gameObject.SetActive(true);
    }

    public void MoveRightUI( bool flg , Action action)
    {
        action();
        return;
        Vector3 targetVector3;
        targetVector3 = rightBasePosi;


        rightUI.transform.DOLocalMove(
            targetVector3,
                0.3f//時間  
        ).OnComplete(() => {
            // アニメーションが終了時によばれる
            action();
        });
    }

    public void SetBaisoku()
    {
        if( Time.timeScale == DEFAULTSPEED )
        {
            Time.timeScale = BAISOKUSPEED;
            baisokuText.text = "×2";
            UserData.SetBaisokuFlag(true);
        }else if (Time.timeScale == BAISOKUSPEED)
        {
            baisokuText.text = "×1";
            Time.timeScale = DEFAULTSPEED;
            UserData.SetBaisokuFlag(false);
        }
    }

    public void SetBaisokuForce( bool flg )
    {
        if(flg)
            Time.timeScale = BAISOKUSPEED;
        else
            Time.timeScale = DEFAULTSPEED; 
    }

    public void BattleStartEffect( Action action ){StartCoroutine( BattleStartEffectOn(action) );}
    private IEnumerator BattleStartEffectOn(Action action)
    {
        battleStartDialog.SetActive(true);
        yield return new WaitForSeconds(1f);
        battleStartDialog.SetActive(false);
        action();
    }

    public void BattleEndEffect(Action action) { StartCoroutine(BattleEndEffectOn(action)); }
    private IEnumerator BattleEndEffectOn(Action action)
    {
        SetTimeScaler(DEFAULTSPEED);
        gameOverDialog.SetActive(true);
        yield return new WaitForSeconds(2f);
        gameOverDialog.SetActive(false);
        action();
    }


    public void ClearEffect(Action action) { StartCoroutine(ClearEffectOn(action)); }
    private IEnumerator ClearEffectOn(Action action)
    {
        clearDialog.SetActive(true);
        yield return new WaitForSeconds(3f);
        action();
    }


    public void RankUpEffect(Action action) { StartCoroutine(RankUpEffectOn(action)); }
    private IEnumerator RankUpEffectOn(Action action)
    {
        rankUpDialog.SetActive(true);
        yield return new WaitForSeconds(2f);
        rankUpDialog.SetActive(false);
        action();
    }




    public void SetTimeScaler(float setTimeScale)
    {
        Time.timeScale = setTimeScale;
    }

}
