using System.Collections;
using System.Collections.Generic;
using TMPro;
using TW.GameSetting;
using UnityEngine;

public class TitleManager : SystemBaseManager
{
    [SerializeField] TextInput textInput;

    TextMeshProUGUI userName;

    public void Tap()
    {
        if (string.IsNullOrEmpty(ES3.Load<string>(SaveType.user_id.ToString(), defaultValue: null)))
        {
            userName = textInput.Initilize(_title:"Name", _discription:"",_pushAction: UserCreate);
            textInput.DialogSystemInitialize();
        }
        else
        {
            ChangeScene(SceneType.FreeHome);
        }
    }

    public void UserCreate()
    {
        Dictionary<string, object> m_data_hash = new Dictionary<string, object>();
        m_data_hash.Add("name", userName.text);

        APIManager.Instance.StartInfoAPI(APIType.user_info, APIDetail.create, m_data_hash, (result) =>
        {
            Debug.Log(result.text);
            JSONObject resultJson = new JSONObject(result.text).GetField("data").GetField("user_info");

            string userid = "" + resultJson.GetField("user_id").i;
            Debug.Log("userid:" + userid);

            ES3.Save<string>(SaveType.user_id.ToString(), userid);
            ES3.Save<string>(SaveType.user_name.ToString(), resultJson.GetField("name").str);
            ES3.Save<string>(SaveType.user_duid.ToString(), resultJson.GetField("duid").str);
            ES3.Save<string>(SaveType.last_login_time.ToString(), resultJson.GetField("last_login_time").str);

            ChangeScene(SceneType.FreeHome);
        });
    }

}
