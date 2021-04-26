using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageSetting : MonoBehaviour {


    private static QuestSetting qusetSetting;


	public static int GetStartStageNumber( string questId )
	{
		qusetSetting = Resources.Load ("Data/QuestSetting") as QuestSetting; //=> Resourcesからデータファイルの読み込み

        int roopCount =  qusetSetting.param.Count;

		for( int i = 0; i < roopCount; i++ )
		{
            if( qusetSetting.param[i].questId == questId )
			{
				return i;
			}
		}
		return 0;
	}



	//現在のクエストから次のクエストを取得する
	public static string GetNextQuestId( string questId )
	{
        qusetSetting = Resources.Load ("Data/QuestSetting") as QuestSetting; //=> Resourcesからデータファイルの読み込み
		int nowStageNUmber = GetStartStageNumber(questId);
        return qusetSetting.param[nowStageNUmber+1].questId;
	}


	//dropValue by dropLv
	public static  List<RealDropItemData> GetDropValue( int lv ,int count )
	{
		List<RealDropItemData> returnValue = new List<RealDropItemData> ();

		for( int i = 0; i<count; i++ )
		{
			
		}


		return returnValue;
	}



}
