using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.IO;

public class Layout_UserCreate : ScenePrefab {

	[SerializeField] InputField inputField;
    [SerializeField] TMP_InputField text;
	[SerializeField] TextMeshPro text2;
	bool pushFlag = false;

    [SerializeField ] SpriteRenderer backGround;

    [SerializeField] GameObject popupObejct;

    void Start()
    {
        StartCoroutine(SetStart());
    }

    public IEnumerator SetStart()
    {
        AddPopup("Popup_AlphaLoding");
        ResourceLoaderOrigin.GetBackGroundImage(5, (bgobj) =>
        {
            AlphaLoding.Close();
            backGround.sprite = bgobj;
            popupObejct.SetActive(true);
        });
 
        yield return null;
    }



	public void SetName()
	{
		if( pushFlag )
			return;

		if ( text.text.Length<=1 || text.text.Length >= 8) {
			text2.text = "1文字以上7文字以下で入力してください。";
			return;
		} else {
			text2.text = "";
		}

        AddPopup("Popup_AlphaLoding");
		StartCoroutine (RegistUser(text.text)); 
		
	}


	public IEnumerator RegistUser(string userName)
	{
		Debug.Log("新規登録" + userName);

		string m_url = APITest.m_urlBase + "regist_user";

		//api接続
		WWWForm form = new WWWForm();
		//パラメータをセットします
		form.AddField("name", userName);

        Dictionary<string, string> systemInfo = GetDeviceInfo();
        form.AddField("os", targetPlatFormId);
        form.AddField("unique_id", systemInfo["unique_id"]);
        form.AddField("os_info", systemInfo["os_info"]);
        form.AddField("device_name", systemInfo["device_name"]);
        form.AddField("model_name", systemInfo["model_name"]);
        form.AddField("device_type", systemInfo["device_type"]);


        WWW result = new WWW( m_url , form.data );

		yield return result;

		CheackResponse (result, "regist_user");

        Debug.Log( result.text );
		//エラー判定
		if (result.error == null) {

            JSONObject rdbUserGet = new JSONObject(result.text);
            if (!VersionCheack(rdbUserGet.GetField("app_version").str))
                yield break;
            
			JSONObject data = new JSONObject(result.text);
			JSONObject jsonUid = data.GetField("data").GetField("id");
			string uid = jsonUid.str;
                
            UserData.SetUserName ( userName );
			UserData.SetUserID(int.Parse(uid));
            UserData.SetUserUUID ( data.GetField("data").GetField("duid").str );
            UserData.SetFriendID( data.GetField("data").GetField("number").str);

            pushFlag = true;

            Debug.Log(APIdebug("regist_user:"+result.text));
            MasterLoad(() =>
            {
                //AlphaLoding.Close();
                //ChangeLayout("Menu");
                Layout_Story.storyID = 999;
                Layout_Story.backScene = "Menu";
                ChangeLayout("Story");

            });
         
		} else {
			Debug.Log ("失敗しました");

            AlphaLoding.Close();
            APIERROR();
		}
	}

    public void FirstDL(Action action)
    {
        if (Directory.Exists(Application.persistentDataPath + "/assetbundles"))
        {
            action();
            return;
        };

        PopupGeneral.textValue = GameSetting.assetbundleSize + "MBダウンロードいたします。ダウンロード開始いたしますか？";
        PopupGeneral.action = () =>
        {
            action();
        };
        AddPopup("PopupGeneral");

    }


    public void Doui()
    {
        popupObejct.SetActive(false);
    }
}
