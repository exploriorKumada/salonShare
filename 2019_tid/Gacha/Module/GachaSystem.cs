using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GachaSystem : ScenePrefab {

    public bool pushFlag = false;

    public void GachaAction(int count)
    {
        pushFlag = true;
        AddPopup("Popup_AlphaLoding");
        StartCoroutine(GachaActionON(count, () =>
        {
            Debug.Log(UserData.GetUserID() + "   " + count);
            AlphaLoding.Close();
            pushFlag = false;

        }));
    }

    private IEnumerator GachaActionON(int count, Action action)
    {
        string m_url = APITest.m_urlBase + "put_character";
        //api接続
        WWWForm form = new WWWForm();
        //パラメータをセットします
        form.AddField("user_id", UserData.GetUserID());
        form.AddField("character_id", 2);
        WWW result = new WWW(m_url, form.data);
        yield return result;

        //エラー判定
        if (result.error == null)
        {

            JSONObject rdbUserGet = new JSONObject(result.text);
            if (!VersionCheack(rdbUserGet.GetField("app_version").str))
                yield break;
            Debug.Log( "korezo--- "+ rdbUserGet.GetField("data"));
            action();
        }
        else
        {
            Debug.Log("取得に失敗しました");
            AddPopup("NotConnectBack");
        }
    }

	public static string GachaResult()
	{

		//まずクラスの抽選
		string rankResult = RankResult();
		string charaID;

		CharaStatus charaStatus = Resources.Load ("Data/CharaStatus") as CharaStatus; //=> Resourcesからデータファイルの読み込み

		//全部から思いっきりひく
		do
		{
			charaID = CharaSetting.ForGacha();
		}while( charaStatus.param[CharaSetting.CharaInfoIndex(charaID)].rare != rankResult );

		return charaID;
	}


	private static string RankResult()
	{


		int number = UnityEngine.Random.Range(1,101);

//		Debug.Log( number + "これはどうなの");	
		
		if( 1<=number && number <= 5 )
		{
			return "UR";
		}
		else if( 6<=number && number <= 15 )
		{
			return "SR";
		}
		else if( 16<=number && number <= 41 )
		{
			return "R";
		}
		else
		{
			return "N";
		}

	}



}
