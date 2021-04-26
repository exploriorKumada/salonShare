using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaderCharaSetting : MonoBehaviour {


	private static LeaderCharaStatus leaderCharaStatus;


//	public static LeaderCharaSettingBase GetCharaSetting( int charaID )
//	{	

//		var returnValue = new LeaderCharaSettingBase();
////		Debug.Log( "dousekoredeyo     " + realEquipmentData );

 //       returnValue.No = charaID-1;

 //       returnValue.charaID = leaderCharaStatus.param[ returnValue.No ].charaID;
 //       returnValue.charaName = leaderCharaStatus.param[ returnValue.No ].charaName;
 //       returnValue.humanDetail = leaderCharaStatus.param[ returnValue.No ].humanDetail;
 //       returnValue.skillDetail = leaderCharaStatus.param[ returnValue.No ].skillDetail;
 //       returnValue.skillName = leaderCharaStatus.param[ returnValue.No ].skillName;
 //       returnValue.action = leaderCharaStatus.param[returnValue.No].action;
 //       returnValue.typeDetail = leaderCharaStatus.param[ returnValue.No].typeDetail;
 //       returnValue.cri = (float)leaderCharaStatus.param[returnValue.No].cri;
 //       returnValue.tapComment = leaderCharaStatus.param[returnValue.No].tap;

 //       returnValue.attack = GetAttackStatus(  leaderCharaStatus.param[ returnValue.No ].attack, leaderCharaStatus.param[ returnValue.No ].attackOds);


	//	return returnValue;
	//}


	private static int GetAttackStatus( int attack, int attackOds )
	{
        //return CalculationManager.GetStatusValue()
		return attack+( attackOds * UserData.GetUserRank() );
	}

	private static float GetGaurdStatus( float guard, float guardOds )
	{
		return guard-( guardOds * UserData.GetUserRank() );
	}



	public static int CharaInfoIndex( string id )
	{
		return charaSetting.IndexOf(id);
	}

	public static List<string> charaSetting = new List<string>
	{
		"mikito",
		"izanami",
		"forin",
		"vanoba",

	};
}
