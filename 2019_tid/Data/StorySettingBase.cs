using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StorySettingBase : ScenePrefab {


	private static StorySetting storySetting;

	public static int GetStorySettingEndLine()
	{
		storySetting = Resources.Load ("Data/StorySetting") as StorySetting; //=> Resourcesからデータファイルの読み込み
		//まずセリフがない、つまりセルの最終行取得
		int count = 0;
		while( true || count == 100000)
		{
			if( storySetting.param[count].storyValue == "" )
			{
				break;
			}
			count++;
		}

		return count-1;

	}



	public static int GetStartStoryNumber( int storyID )
	{

		storySetting = Resources.Load ("Data/StorySetting") as StorySetting; //=> Resourcesからデータファイルの読み込み
		int roopCount = GetStorySettingEndLine();
        //Debug.Log("roopCount" + roopCount);
		for( int i = 0; i < roopCount; i++ )
		{
			if( storySetting.param[i].storyID == storyID )
			{              
				return i;
			}
		}

		return 0;
	}

    public static bool IsStoryExist( int quest_detail_id)
    {
        return ( GetStartStoryNumber(quest_detail_id) != GetEndStoryNumber(quest_detail_id) );
    }

    public static List<int> GetLengthStoryIDs()
    {
        var list = new List<int>();
        storySetting = Resources.Load("Data/StorySetting") as StorySetting;

        foreach( var Value in storySetting.param)
        {
            if (Value.storyID != 0)
                list.Add(Value.storyID);
        }
        return list;
    }



    public static List<Data_CharaImage> GetImageNameList( int storyID )
    {
        List<Data_CharaImage> returnValue = new List<Data_CharaImage>();

        for (int i = GetStartStoryNumber(storyID); i < GetEndStoryNumber(storyID); i++)
        {
            string charaImageName = storySetting.param[i].charaImageName;
            string charaExpression = storySetting.param[i].expression;
            if (!string.IsNullOrEmpty(charaImageName) )
            {
                returnValue.Add(new Data_CharaImage()
                {
                    charaImageName_data = charaImageName,
                    expression_data = charaExpression,
                    id = charaImageName + charaExpression

                });
            }

        }
        if(returnValue.Count==0)
            return new List<Data_CharaImage>();

        List<Data_CharaImage> newLists = returnValue.GroupBy(u => u.id).Distinct().Select(u => u.FirstOrDefault()).ToList();

//        Debug.Log("newLists.Count:" + newLists.Count);
        newLists.Last().endFlag = true;
        return newLists; 
    }

    public static List<int> GetBackGroundIDList(int storyID)
    {
        List<int> returnValue = new List<int>();

        for (int i = GetStartStoryNumber(storyID); i < GetEndStoryNumber(storyID); i++)
        {
            int backGroundID = (int)storySetting.param[i].backGround;
            if (backGroundID != 0)
            {
                returnValue.Add(backGroundID);
            }

        }

        returnValue = JuuhukuSakujo(returnValue);

        Debug.Log("い赤加賀名も花芽なものか：" + returnValue.Count);
           

        return returnValue;
    }




    public static int GetEndStoryNumber( int storyID )
	{

		storySetting = Resources.Load ("Data/StorySetting") as StorySetting; //=> Resourcesからデータファイルの読み込み


		int startCount = GetStartStoryNumber( storyID );
		int endCount = GetStorySettingEndLine();

		int storyEnd = startCount;

		for( int i = startCount; i < endCount; i++ )
		{

            if( storySetting.param[i].storyID != 0 && storySetting.param[i].storyID != storyID )
			{
				break;                                                                                                                                                                                                                                                                                                                                                                                                                                                                         
			}
			storyEnd++;
		}

		return storyEnd;
	}
}

public class Data_CharaImage: MonoBehaviour{
    public string charaImageName_data;
    public string expression_data;
    public string id;
    public bool endFlag = false;

}