using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentSetting : MonoBehaviour {

	private static EquipmentsSetting equipmentSetting;

//	public static RealEquipmentData GetRealRealEquipmentData( string equipmentId )
//	{
//		var returnValue = new RealEquipmentData();

////		Debug.Log ("lpregamaoi    " + equipmentId);
//		if( GetIdNumber( equipmentId ) == 999)
//		{
//			return null;
//		}

//		if( equipmentSetting == null)
//			equipmentSetting =  Resources.Load ("Data/EquipmentsSetting") as EquipmentsSetting; //=> Resourcesからデータファイルの読み込み

//		returnValue.No = GetIdNumber( equipmentId );
//		returnValue.equipmentID = equipmentSetting.param[ GetIdNumber( equipmentId ) ].equipmentID;
//		returnValue.equipmentName = equipmentSetting.param[ GetIdNumber( equipmentId ) ].equipmentName;
//		//returnValue.charaId = equipmentSetting.param[ GetIdNumber( equipmentId ) ].charaId;
//		returnValue.rank = equipmentSetting.param[ GetIdNumber( equipmentId ) ].rank;
//		returnValue.attack = equipmentSetting.param[ GetIdNumber( equipmentId ) ].attack;
//		returnValue.guard = equipmentSetting.param[ GetIdNumber( equipmentId ) ].guard;
//		returnValue.cri = equipmentSetting.param[ GetIdNumber( equipmentId ) ].cri;
//		returnValue.repair = equipmentSetting.param[ GetIdNumber( equipmentId ) ].repair;
//		returnValue.materialA = equipmentSetting.param[ GetIdNumber( equipmentId ) ].materialA;
//		returnValue.materialB = equipmentSetting.param[ GetIdNumber( equipmentId ) ].materialB;
//		returnValue.materialS = equipmentSetting.param[ GetIdNumber( equipmentId ) ].materialS;


//		returnValue.enableSet = UserData.GetEquipmentOpenFlag (equipmentId);
////		Debug.Log ("returnValue.enableSet  " + returnValue.enableSet + "    returnValue.equipmentName  " + returnValue.equipmentName);


	//	return returnValue;
	//}

	//public static List<RealEquipmentData> RealDataListByCharaId( string charaId )
	//{
	//	List<RealEquipmentData> returnValue = new List<RealEquipmentData> ();

	//	equipmentSetting = Resources.Load ("Data/EquipmentsSetting") as EquipmentsSetting; //=> Resourcesからデータファイルの読み込み

	//	for( int i=0;i<equipmentSetting.param.Count;i++)
	//	{
	//		if( equipmentSetting.param[i].charaId == charaId )
	//		{
	//			returnValue.Add ( GetRealRealEquipmentData( equipmentSetting.param[i].equipmentID ) );
	//		}
	//	}

	//	return returnValue;
	//}


	public static int GetIdNumber( string equipmentId )
	{
		equipmentSetting = Resources.Load ("Data/EquipmentsSetting") as EquipmentsSetting;
		int roopCount =  equipmentSetting.param.Count;

		for( int i = 0; i < roopCount; i++ )
		{
			if( equipmentSetting.param[i].equipmentID == equipmentId )
			{
				return i;
			}
		}
		return 999;
	}


	public static List<string> charaSetting = new List<string>
	{
		"mikito",
		"izanami",
		"forin",
		"vanoba",
	};


}
