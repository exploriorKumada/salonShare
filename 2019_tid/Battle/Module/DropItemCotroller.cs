using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class DropItemCotroller : MonoBehaviour {

	[SerializeField] GameObject objectBase;
	[SerializeField] Transform parentTF;
	[SerializeField] Transform aTreasure;
	[SerializeField] Transform sTreasure;
	[SerializeField] BattleLayoutManager battleLayoutManager;
	[SerializeField] Transform itemBase;
    [SerializeField] BattleManager battleManager;


	public void SetImage( DropItemData dropItemData )
	{

		var newGO = GameObject.Instantiate( objectBase,parentTF );
		Transform newGoTF = newGO.transform;

		Transform targetTF;
		if (dropItemData.rare == "A") {
			targetTF = aTreasure;
			battleLayoutManager.aTresureNumber++;
		} else {
			targetTF = sTreasure;
			battleLayoutManager.sTresureNumber++;
		}


		newGoTF.DOLocalJump(
			new Vector3(newGoTF.localScale.x + Random.Range(-30,30 ), newGoTF.localPosition.y, newGoTF.localPosition.z-300   ),    // 移動終了地点
			300,                        // ジャンプする力
			1,                        // 移動終了までにジャンプする回数
			0.3f                        // アニメーション時間
		).SetDelay(0.3f).OnComplete(() => {
			// アニメーションが終了時によばれる
			newGoTF.DOMove(
				targetTF.localPosition,    // 移動終了地点座標
				0.3f                            // アニメーション時間
			).OnComplete(() => {
				// アニメーションが終了時によばれる
				battleLayoutManager.TreasureNumberSet(dropItemData.rare);
				Destroy(newGO);
			}).OnStart(() => {
				// アニメーション開始時によばれる
				newGoTF.parent = itemBase;
			});

		});

		newGO.SetActive(true);

	}




}
