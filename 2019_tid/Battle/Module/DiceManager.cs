using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class DiceManager : MonoBehaviour {

	Coroutine startCoroutine;
	public int diceNumber;

	[SerializeField] List<Sprite> defaultDiceImage = new List<Sprite>();
	[SerializeField] List<Sprite> spinDiceImage = new List<Sprite>();
	[SerializeField] GameObject diceObject;
    [SerializeField] BattleManager battleManager;
    [SerializeField] BattleLayoutManager battleLayoutManager;
    [SerializeField] DiceSelectManager diceSelectManager;
    [SerializeField] TextMeshPro kakuteiText;
    [SerializeField] ParticleManager particleManager;
    [SerializeField] Transform partcleBase;

	public bool enableAction = true;

    int diceEffectedNumber = 0;
    Image diceImage;
    Dictionary<int, Sprite> diceimage = new Dictionary<int, Sprite>();

	// Use this for initialization
	public void Init()
	{
        int count = 0;
        foreach( var Value in defaultDiceImage )
        {
            count++;
            diceimage[count] = Value;           
        }

        diceSelectManager.Init();
        kakuteiText.gameObject.SetActive(false);
        diceImage = diceObject.GetComponent<Image>();
	}


	public void DiceStartOn()
	{
        if (!enableAction)
            return;

        if (battleManager.teamCharacterController.LiveList().Count == 0)
            return;

        battleManager.autoStartEnable = false;
        enableAction = false;
        startCoroutine = StartCoroutine(DiceStart());
	}

	public int GetDiceNumber()
	{
		return diceNumber;	
	}


	private IEnumerator DiceStart()
	{
        //Debug.Log("DiceStart");
        battleManager.actionFlag = true;
        battleLayoutManager.EnableActionButtons(false);
       
        diceNumber = Random.Range(1, 7);


        if (UserData.GetLeaderChara() == 2)
        {           
            if (Random.Range(1, 6)==1)
            {
                Debug.Log("leaderのちから");
                SetEffect(15);
                diceNumber = 3;
            }
        }

        if (diceEffectedNumber != 0)
        {
            Debug.Log("確定処理:" + diceEffectedNumber);
            diceNumber = diceEffectedNumber;
        }

        diceEffectedNumber = 0;

		var wait = new WaitForSeconds (0.05f);
        for (int j = 0; j <= 2;j++)
		{
			for( int i = 0; i< spinDiceImage.Count-1; i++)
			{
                diceImage.sprite  = spinDiceImage[i];
				yield return wait;
			}
		}

        diceImage.sprite = diceimage[diceNumber];
        diceSelectManager.AddImage(diceNumber - 1,()=>
        {
            DOVirtual.DelayedCall(0.5f, () =>
            {
                ImageEffect.SetEffect(() => {
                    battleManager.MyAttackStart(diceNumber);
                }, diceObject.GetComponent<Image>());
                kakuteiText.gameObject.SetActive(false);
            });
  


        });


	}


    /// <summary>
    /// ダイス確定操作
    /// </summary>
    /// <param name="setNumber">Set number.</param>
    public void DiceEffected( int setNumber )
    {
        //particleManager.SetEffect( partcleBase,15 );
        kakuteiText.gameObject.SetActive(true);
        kakuteiText.text = "次ターン " + setNumber+" 確定";
        diceEffectedNumber = setNumber;
    }

    public void SetEffect(int effectId)
    {
        particleManager.SetEffect(partcleBase, effectId,true);
    }

}
