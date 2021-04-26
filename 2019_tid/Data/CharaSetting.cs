
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharaSetting : MonoBehaviour
{


    private static Entity_CharaStatus charaStatus;


	//public static RealCharaData GetRealCharaData( string charaID )
	//{

	//	RealCharaData returnValue = new RealCharaData();
 //       charaStatus = Resources.Load ("Data/CharaStatus") as Entity_CharaStatus; //=> Resourcesからデータファイルの読み込み

 //       returnValue.charaIdNumber =  (int)charaStatus.param[ CharaInfoIndex(charaID) ].No;
	//	//returnValue.charaID = charaStatus.param[ CharaInfoIndex(charaID) ].charaID;
	//	returnValue.charaName = charaStatus.param[ CharaInfoIndex(charaID) ].charaName;
	//	returnValue.rare = charaStatus.param[ CharaInfoIndex(charaID) ].rare;
 //       returnValue.rareId = RareConvert(returnValue.rare);
	//	returnValue.charaType = charaStatus.param[ CharaInfoIndex(charaID) ].charaType;
 //       returnValue.charaTypeId = TyoeConvert( returnValue.charaType );
 //       returnValue.mastertAttack = (int)charaStatus.param[ CharaInfoIndex(charaID) ].attack;
	//	returnValue.attackOds = (float)charaStatus.param[ CharaInfoIndex(charaID) ].attackOds;
 //       returnValue.masterGuard = (float)charaStatus.param[ CharaInfoIndex(charaID) ].guard;
	//	returnValue.guardOds = (float)charaStatus.param[ CharaInfoIndex(charaID) ].guardOds;
 //       returnValue.realHP = (int)charaStatus.param[CharaInfoIndex(charaID)].HP;
 //       returnValue.masterHP = (int)charaStatus.param[ CharaInfoIndex(charaID) ].HP;
	//	returnValue.HPOds = (float)charaStatus.param[ CharaInfoIndex(charaID) ].HPOds;
	//	returnValue.skillMaxNumber = (int)charaStatus.param[ CharaInfoIndex(charaID) ].skillMaxNumber;
	//	returnValue.cri = (float)charaStatus.param[ CharaInfoIndex(charaID) ].crtical;
 //       //returnValue.skillValue = charaStatus.param[CharaInfoIndex(charaID)].skill;
 //       //returnValue.effectId = charaStatus.param[CharaInfoIndex(charaID)].skillEffectName;


	//	return returnValue;

	//}

    //public static RealCharaData GetRealCharaDataByCharaIdNumber(int charaID)
    //{

    //    RealCharaData returnValue = new RealCharaData();
    //    charaStatus = Resources.Load("Data/CharaStatus") as Entity_CharaStatus; //=> Resourcesからデータファイルの読み込み

    //    returnValue.charaIdNumber = (int)charaStatus.param[charaID].No;
    //    //returnValue.charaID = charaStatus.param[charaID].charaID;
    //    returnValue.charaName = charaStatus.param[charaID].charaName;
    //    returnValue.rare = charaStatus.param[charaID].rare;
    //    returnValue.rareId = RareConvert(returnValue.rare);
    //    returnValue.charaType = charaStatus.param[charaID].charaType;
    //    returnValue.charaTypeId = TyoeConvert(returnValue.charaType);
    //    returnValue.mastertAttack = (int)charaStatus.param[charaID].attack;
    //    returnValue.attackOds = (float)charaStatus.param[charaID].attackOds;
    //    returnValue.masterGuard = (int)charaStatus.param[charaID].guard;
    //    returnValue.guardOds = (float)charaStatus.param[charaID].guardOds;
    //    returnValue.realHP = (int)charaStatus.param[charaID].HP;
    //    returnValue.masterHP = (int)charaStatus.param[charaID].HP;
    //    returnValue.HPOds = (float)charaStatus.param[charaID].HPOds;
    //    returnValue.skillMaxNumber = (int)charaStatus.param[charaID].skillMaxNumber;
    //    returnValue.cri = (float)charaStatus.param[charaID].crtical;
    //    //returnValue.skillValue = charaStatus.param[charaID].skill;
    //    //returnValue.effectId = charaStatus.param[charaID].skillEffectName;



    //    return returnValue;

    //}


    public static RealActionData ConvertRealActionData
    (
        int action,
        float amount,
        int range,
        int target,
        int turn,
        int type
    )
    {
        RealActionData realActionData = new RealActionData();
        realActionData.action = action;
        realActionData.amount = amount;
        realActionData.range = range;
        realActionData.target = target;
        realActionData.turn = turn;
        realActionData.type = type;
                    
        return realActionData;
    }



	public static int CharaInfoIndex( string id )
	{
		SetCharaList();
		return charaSetting.IndexOf(id);
	}

	private static void SetCharaList()
	{
        charaStatus = Resources.Load ("Data/CharaStatus") as Entity_CharaStatus; //=> Resourcesからデータファイルの読み込み
		

		for( int i = 0; i <= 1000;i++)
		{
			if( charaStatus.param[i].charaID == "" )
			{
				break;
			}
			charaSetting.Add( charaStatus.param[i].charaID );

		}
	}


//	public static List<RealCharaData> AllCharaList()
//	{
//		List<RealCharaData> returnValue = new List<RealCharaData>();
//        charaStatus = Resources.Load ("Data/CharaStatus") as Entity_CharaStatus; //=> Resourcesからデータファイルの読み込み
//        Entity_CoroseumSetting test = Resources.Load("Data/CoroseumSetting") as Entity_CoroseumSetting; //=> Resourcesからデータファイルの読み込み

////        Debug.Log( test + "   korenaino*******************  " + charaStatus);
	//	for( int i = 0; i <= 1000;i++)
	//	{
	//		if( charaStatus.param[i].charaID == "" )
	//		{
	//			break;
	//		}
	//		returnValue.Add( GetRealCharaData( charaStatus.param[i].charaID ));
	//	}

	//	return returnValue;

	//}



	public static List<string> charaSetting = new List<string>{};

	private string acitonValueText;
	private string acitonTargetText;
	private string acitonNumberText;


    public static string SetText( RealActionData realActionData )
	{	
        
        if( realActionData.type == 1 )
		{
            return AttackAction( realActionData );
		}
        else if( realActionData.type == 2)
		{
            return RepaireAction( realActionData );
		}
        else if( realActionData.type == 3 ) 
		{
            return BuffAction ( realActionData );
		}
        else if( realActionData.type == 4 ) 
		{   
            return BuffAction ( realActionData );
		}
        else if( realActionData.type == 6 ) 
		{
			return "ミス";
        }else if( realActionData.type == 5 )
        {
            return "次のダイス" + realActionData.amount + "\n確定";
        }else if (realActionData.type == 7)
        {
            return "ランダムな相手に" + realActionData.amount + "\n倍のダメージ";
        }

        Debug.LogError(realActionData.type);

        return "エラー SetText " + realActionData.type;


	}

    private static string BuffAction( RealActionData realActionData )
	{
        string text = "";
        if( realActionData.type == 3 )
        {
            text = "上昇";
        }else
        {
            text = "下降";
        }

        return ConvertTarget( realActionData.range ) + "の\n" + ConvertBuffDebuffAction(realActionData.target) + "を" + realActionData.turn +"ターン" + ( 100 * ( realActionData.amount ) ) + "%" + text ;
	}

    private static string AttackAction( RealActionData realActionData )
	{
        return ConvertTarget( realActionData.range ) + "に攻撃力の\n" + realActionData.amount+ "倍のダメージ" ;
	}


    private static string RepaireAction( RealActionData realActionData )
	{
        return ConvertTarget( realActionData.range ) + "に\n" + ( 100 * ( realActionData.amount ) ) + "%の回復" ;
	}

	private static string ConvertValue( string valueText )
	{
		if( valueText == "damage")
		{
			return "攻撃";
		}else if( valueText == "repaire")
		{
			return "回復";
		}else if( valueText == "miss")
		{
			return "ミス";
		}

        Debug.LogError("エラー ConvertValue " + valueText);

		return "エラー ConvertValue " + valueText;

	}


	private static string ConvertTarget( int targetId )
	{
        if( targetId == 1)
		{
			return "青";
        }else if( targetId == 2)
		{
			return "赤";
        }else if( targetId == 3)
		{
			return "黄";
        }else if( targetId == 4 )
		{
			return "全員";
        }else if (targetId == 5)
        {
            return "ランダム";
        }

        Debug.LogError("エラー ConvertValue " + targetId);

        return "エラー ConvertTarget " + targetId ;

	}


	public static string ConvertBuffDebuffAction( int buffDebuffType )
	{
        if( buffDebuffType == 1 )
		{
			return "攻撃力";
        }else if( buffDebuffType  == 2 )
		{
			return "防御力";
        }else if (buffDebuffType == 3)
        {
            return "ダメージ回復";
        }
        Debug.LogError("エラー buffDebuffText " + buffDebuffType);

        return "エラー" + buffDebuffType;
	}

	public static string ForGacha()
	{

		SetCharaList();
		int resultNumber = Random.Range(0,charaSetting.Count);
		return charaSetting[resultNumber];
	}



    //指定したレア度のcharaIdのキャラクターの配列を返す
 //   public static List<RealCharaData> RareUnitCharaId(string rare, int groupID = 0 )
	//{
	//	List<RealCharaData> returnValue = new List<RealCharaData>();
	//	List<RealCharaData> all = AllCharaList();

	//	foreach( var Value in all )
	//	{
	//		if( Value.rare == rare )
	//		{
 //               if( groupID == 0 )
 //               {
 //                   returnValue.Add(Value); 
 //               }else
 //               {
 //                   if( Value.groupID == groupID)
 //                   {
 //                       returnValue.Add(Value); 
 //                   }
                    
 //               }

				
	//		}
	//	}
	//	return returnValue;
	//}

    //public static List<RealCharaData> GroupIdUnitCharaId( int groupID )
    //{
    //    List<RealCharaData> returnValue = new List<RealCharaData>();
    //    List<RealCharaData> all = AllCharaList();

    //    foreach (var Value in all)
    //    {
    //        if (Value.groupID == groupID)
    //        {
    //            returnValue.Add(Value);
    //        }
    //    }
    //    return returnValue;
    //}

    private static int RareConvert( string rare)
    {
        if( rare == "N")
        {
            return 1;
        }else if( rare == "R")
        {
            return 2;
        }else if ( rare == "SR")
        {
            return 3;
        }else
        {
            return 4;
        }
    }


    private static int TyoeConvert( string type )
    {
        if( type == "red")
        {
            return 1;
        }else if( type == "blue" )
        {
            return 2;
        }else
        {
            return 3;
        }
    }



    public static int AjustSpinePositionByChara(int charaId)
    {

        Dictionary<int, int> ajustValue = new Dictionary<int, int>()
        {
            { 1,480},//mikito
            { 2,680},//izanami
            { 3,830},//forin
            { 4,430},//vanoba

        };

        return ajustValue[charaId];
        
    }


    public static Dictionary<int, float> charaSpineAjust = new Dictionary<int, float>()
    {
        {1,-115},
        {2,0},
        {3,153},
        {4,-256},
        {5,-115},
    };

    public static Dictionary<int, float> charaSpineAjustX = new Dictionary<int, float>()
    {
        {1,-20},
        {2,-20},
        {3,-20},
        {4,-20},
        {5,-206},
    };

}
