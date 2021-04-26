using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;

public class DiceSelectManager : MonoBehaviour {

    public List<Dictionary<int, GameObject>> diceList = new List<Dictionary<int, GameObject>>();
    [SerializeField] GameObject baseObject;
    [SerializeField] Transform parentTF;
    [SerializeField] List<Sprite> defaultDiceImage = new List<Sprite>();
    [SerializeField] TextMeshProUGUI effectText;

    [SerializeField] GameObject effectObject;
    [SerializeField] BattleManager battleManager;

    List<int> cheackList = new List<int>();
    Dictionary<int, int> cheackCount = new Dictionary<int, int>();
    public float amountByDice;


	public void Init()
    {
        baseObject.SetActive(false);
        parentTF.localPosition = Vector3.zero;
        effectText.gameObject.SetActive(false);
    }


    public void AddImage( int number, Action action )
    {        
        var newGO = GameObject.Instantiate(baseObject,parentTF);
        Transform newGoTF = newGO.transform;
        cheackList.Add(number);

        Dictionary<int, GameObject> addValue = new Dictionary<int, GameObject>();
        addValue[number] = newGO;
        diceList.Add( addValue );

        addValue[number].transform.Find("dice").GetComponent<Image>().sprite = defaultDiceImage[number];
        newGO.SetActive(true);
        SetMove(action);

    }


    public void SetMove(Action action)//180
    {
        if (diceList.Count >= 5)
        {
            diceList.RemoveAt(0);
            GameObject.Destroy(parentTF.GetChild(0).gameObject);

        }

        foreach( var Value in diceList )
        {
            foreach( var KV in Value)
            {
                // 3秒かけてローカル座標（155,20）に移動

                Transform baseTF = KV.Value.transform;
                baseTF.DOLocalMove(
                    new Vector3(baseTF.localPosition.x+180, baseTF.localPosition.y, 0),    // 移動終了地点座標
                    0.5f                            // アニメーション時間
                );
            
            }
        }

        SetCheack(action);
    }


    public void SetCheack(Action action)
    {
        

        if( diceList.Count <3){
            action();
            return;
        }
        CheackStep2(action);
    }

    public void CheackStep2(Action action)
    {
        //Debug.Log("kokomadehakiteru  " + cheackList.Count);
        if (cheackList.Count < 3)
            return;

        if (cheackList.Count > 3)
            cheackList.RemoveAt(0);

        cheackCount.Clear();

        for (int i = 1; i <= 6; i++ )
        {
            int count = 0;
            foreach(var Value in cheackList)
            {
                if(i-1==Value)
                {
                    count++;
                }
            }
            cheackCount[i] = count;            
        }

        CheackStep3(action);
       
        //cheackList.RemoveAt(3);        
    }


    public void CheackStep3(Action action)
    {
        foreach (var KV in cheackCount)
        {
//            Debug.Log(KV.Key + " count is " + KV.Value);

            if( KV.Value == 3 )
            {
                SetEffect(KV.Key,3, action); 
                return;
            }else if( KV.Value == 2)
            {
                SetEffect(KV.Key,2, action);
                return;
            }

        }

        SetEffect(0, 0, action);
                
    }


    /// <summary>
    /// 1:recovery 2:gaurd 3:aattack
    /// </summary>
    [System.NonSerialized]public int effectType;
    public void SetEffect(int diceNumber, int count, Action action )
    {
        effectText.gameObject.SetActive(true);
        effectObject.SetActive(false);

        RealActionData realActionData;
        switch (diceNumber)
        {
            case 1:
                effectType = 1;//"repair";
                amountByDice = 0.05f * (count-1); 
                effectText.text = "回復ボーナス +" + ( 100* amountByDice　) + "%";
                effectObject.SetActive(true);

                realActionData = new RealActionData
                {
                    range = 4,
                    type = 2,
                    amount = amountByDice,
                    lastAttackFlag = true
                };
                battleManager.DiceEffectAction(realActionData, action);
                break;

            case 2:    
                effectType = 2;//"gaurd";
                amountByDice = 0.05f * (count - 1);
                effectText.text = "防御力上昇ボーナス +" + (100 * amountByDice) + "%";
                effectObject.SetActive(true);

                realActionData = new RealActionData
                {
                    range = 4,
                    type = 3,
                    turn = 1,
                    target = 2,
                    amount = amountByDice,
                    lastAttackFlag = true
                };
                battleManager.DiceEffectAction(realActionData, action);
                break;
            
            case 3:
                effectType = 3;//"attack";
                amountByDice = 0.05f * (count - 1);
                effectText.text = "攻撃力上昇ボーナス +" + (100 * amountByDice) + "%";
                effectObject.SetActive(true);

                realActionData = new RealActionData
                {
                    range = 4,
                    type = 3,
                    turn = 1,
                    target = 1,
                    amount = amountByDice,
                    lastAttackFlag = true
                };
                battleManager.DiceEffectAction(realActionData, action);
                break;

            case 4:
                effectType = 1;//"repair";
                amountByDice = 0.1f * (count - 1);
                effectText.text = "回復ボーナス +" + (100 * amountByDice) + "%";
                effectObject.SetActive(true);

                realActionData = new RealActionData
                {
                    range = 4,
                    type = 2,
                    amount = amountByDice,
                    lastAttackFlag = true
                };

                battleManager.DiceEffectAction(realActionData, action);
                break;


            case 5:
                effectType = 2;//"gaurd";
                amountByDice = 0.1f * (count - 1);
                effectText.text = "防御力上昇ボーナス +" + (100 * ((0.1f * count))) + "%";
                effectObject.SetActive(true);
                realActionData = new RealActionData
                {
                    range = 4,
                    type = 3,
                    turn = 1,
                    target = 2,
                    amount = amountByDice,
                    lastAttackFlag = true
                };
                battleManager.DiceEffectAction(realActionData, action);
                break;


            case 6:
                effectType = 3;//"attack";
                amountByDice = 0.1f * (count - 1);
                effectText.text = "攻撃力上昇ボーナス +" + (100 * ((0.1f * count))) + "%";
                effectObject.SetActive(true);

                realActionData = new RealActionData
                {
                    range = 4,
                    type = 3,
                    turn = 1,
                    target = 1,
                    amount = amountByDice,
                    lastAttackFlag = true
                };
                battleManager.DiceEffectAction(realActionData, action);

                break;
            
            default:
                effectType = 0;
                amountByDice = 0;
                effectText.gameObject.SetActive(false);
                effectObject.SetActive(false);
                action();
                break;
        }


        
    }

}
