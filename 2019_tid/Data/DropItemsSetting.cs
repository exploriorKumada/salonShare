using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DropItemsSetting : MonoBehaviour {

	private static DropItemSetting dropItemSetting;


	public static RealDropItemData GetRealDropItemData( int dropId )
	{
		dropItemSetting = Resources.Load ("Data/DropItemSetting") as DropItemSetting; //=> Resourcesからデータファイルの読み込み

//		Debug.Log (dropItemSetting + "  masakakoremo??");
		RealDropItemData returnValue = new RealDropItemData();

		int count = GetDropNumber( dropId );

		returnValue.No = dropItemSetting.param[count].No ;
		returnValue.chara = dropItemSetting.param[count].chara ;
		returnValue.charaN = dropItemSetting.param[count].charaN ;
		returnValue.charaR = dropItemSetting.param[count].charaR ;
		returnValue.charaSR = dropItemSetting.param[count].charaSR ;
		returnValue.charaUR = dropItemSetting.param[count].charaUR ;
		returnValue.ranking = dropItemSetting.param[count].ranking ;
		returnValue.rankMin = dropItemSetting.param[count].rankMin ;
		returnValue.rankMax = dropItemSetting.param[count].rankMax ;
		returnValue.gold = dropItemSetting.param[count].gold ;
		returnValue.goldMin = dropItemSetting.param[count].goldMin ;
		returnValue.goldMax = dropItemSetting.param[count].goldMax ;
		returnValue.materilal = dropItemSetting.param[count].materilal ;
		returnValue.materialB = dropItemSetting.param[count].materialB ;
		returnValue.materialA = dropItemSetting.param[count].materialA ;
		returnValue.materialS = dropItemSetting.param[count].materialS ;

		return returnValue;
	}

//	public static DropItemData GetRealItem( int dropId )
//	{
//		RealDropItemData realDropItemData = GetRealDropItemData (dropId);

//		var weightTable = new int[]{
//			realDropItemData.chara,
//			realDropItemData.ranking,
//			realDropItemData.gold,
//			realDropItemData.materilal
//		};

//		int index = GetRandomIndex(weightTable);

//		DropItemData returnValue = new DropItemData ();
//		if (index == 0) {
//			returnValue.type = "chara";

//			string[] charaRare = { "N","R", "SR","UR" };
//			var weightTable2 = new int[]{
//				realDropItemData.charaN,
//				realDropItemData.charaR,
//				realDropItemData.charaSR,
//				realDropItemData.charaUR
//			};

//			string rare = charaRare [GetRandomIndex (weightTable2)];
//			List<RealCharaData> list = CharaSetting.RareUnitCharaId ( rare );
//			RealCharaData realCharaData = list.GetAtRandom ();

//            returnValue.amount = realCharaData.charaIdNumber;
//			if (rare == "N" || rare == "R") {
//				returnValue.rare = "A";
//			} else {
//				returnValue.rare = "S";
//			}

////			Debug.Log (realCharaData.charaID);
	//		//returnValue.imageName = realCharaData.charaID;

	//	} else if (index == 1) {
	//		returnValue.type = "ranking";
	//		returnValue.imageName = "ranking";
	//		returnValue.amount = Random.Range( realDropItemData.rankMin, realDropItemData.rankMax);
	//		returnValue.rare ="A";

	//	} else if (index == 2) {
	//		returnValue.type = "gold";
	//		returnValue.imageName = "gold";
	//		returnValue.amount = Random.Range( realDropItemData.goldMin, realDropItemData.goldMax);
	//		returnValue.rare ="A";

	//	} else {
	//		returnValue.type = "material";

	//		int[] mateRare = { 0,1,2 };
	//		var weightTable2 = new int[]{
	//			realDropItemData.materialB,
	//			realDropItemData.materialA,
	//			realDropItemData.materialS,
	//		};

	//		returnValue.amount = GetRandomIndex (mateRare);

	//		if (returnValue.amount == 0 ) {
	//			returnValue.rare = "A";
	//			returnValue.imageName = "materialB";

	//		} else if(returnValue.amount == 1)
	//		{
	//			returnValue.rare = "A";
	//			returnValue.imageName = "materialA";
	//		}
	//		else {
	//			returnValue.rare = "S";
	//			returnValue.imageName = "materialS";
	//		}			
	//	}

	//	return returnValue;
	//}

	public static int GetDropNumber( int dropId )
	{
		dropItemSetting = Resources.Load ("Data/DropItemSetting") as DropItemSetting; //=> Resourcesからデータファイルの読み込み
		int roopCount =  dropItemSetting.param.Count;

		for( int i = 0; i < roopCount; i++ )
		{
			if( dropItemSetting.param[i].No == dropId )
			{
				return i;
			}
		}
		return 0;
	}


	//渡された重み付け配列からIndexを得る
	public static int GetRandomIndex(params int[] weightTable)
	{
		var totalWeight = weightTable.Sum();
		var value = Random.Range(1, totalWeight + 1);
		var retIndex = -1;
		for (var i = 0; i < weightTable.Length; ++i)
		{
			if (weightTable[i] >= value)
			{
				retIndex = i;
				break;
			}
			value -= weightTable[i];
		}
		return retIndex;
	}
}
