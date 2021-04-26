using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class CharaController : ScenePrefab {

    [SerializeField] GameObject charaBase;
    [SerializeField] Transform charaParentTF;
    [SerializeField] BattleManager battleManager;

    public List<RealCharaData> charaNumberList = new List<RealCharaData>();

	public void SetImageOn( Action action )
	{
        gameObject.SetActive(true);
		StartCoroutine( SetImage(action) );	
	}

    int count = 0;
	public IEnumerator SetImage(Action action)
    {
        count = 0;
		charaBase.SetActive(false);

		//float enemyFirstPosition = charaBase.transform.localPosition.x + ( (charaNumberList.Count-1) * 80f );
		//今あるやつ全部消す
		for( int i=0; i < charaParentTF.childCount; ++i )
		{
			Destroy( charaParentTF.GetChild( i ).gameObject );
		}
		
        float firstPosition =  -((charaNumberList.Count - 1) * 250f);
        //Debug.Log("CutinStart========================-" + battleManager.turn);
		
		foreach( var Value in charaNumberList )
		{
			//このキャラのカットイン表示する
            var newGO = Instantiate( charaBase,charaParentTF,true );
			Transform newGoTF = newGO.transform;
            newGO.name = Value.charaName;
            newGO.SetActive(true);
            ResourceLoaderOrigin.GetBattleCharaImage(Value.charaIdNumber, (Sprite obj) => { newGoTF.Find("mask/charaImage").GetComponent<Image>().sprite = obj; });

            Vector3 endPosition;
            endPosition = new Vector3( firstPosition + ((count) * 500), newGoTF.localPosition.y, -100 + (10 * count));


            //Debug.Log(charaNumberList.Last().charaName+ "Value " + Value.charaName);
            StartCoroutine ( CutInAnimation(action,newGoTF, endPosition,Value, (charaNumberList.First() == Value)));

			count++;
		}
        yield return null;
		
	}



    private IEnumerator CutInAnimation(Action action,Transform TF, Vector3 endPosition,RealCharaData realCharaData,bool endFlag = false )
	{       
		float endXposi = TF.localPosition.x;
        Singleton<SoundPlayer>.instance.CharaVoice(realCharaData.charaIdNumber, "skill");
        var sequence = DOTween.Sequence();
        sequence.Append(
            TF.DOLocalMove(
                endPosition,    // 移動終了地点座標
                0.5f                            // アニメーション時間
            )
        );

        sequence.Append(
            TF.DOLocalMove( new Vector3(3000f, TF.localPosition.y, TF.localPosition.z),    // 移動終了地点座標
            0.2f                            // アニメーション時間
            ).OnComplete(() => {
               
                //Debug.Log( TF.gameObject.name+ " endFlag:" + endFlag);
                if (endFlag)           
                {
                    //Debug.Log("cutinend=================");
                    action();
                    gameObject.SetActive(false);
                }
                Destroy(TF.gameObject);
            }).SetDelay(0.5f * (charaNumberList.Count-count ) )
        );

        yield return null;
	}

}
