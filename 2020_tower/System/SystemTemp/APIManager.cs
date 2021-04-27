using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using PW;
using System;
using TW.GameSetting;
using System.Linq;
using HMLabs.JsonConsole;

public class APIManager : NLSingletonDontDestroyObject<APIManager>
{
    public static readonly string testAPIpass = "xxxx";
    public static readonly string prodAPIpass = "xxxxi";

    [SerializeField] bool setDebugText = true;

    public void APIdebug(string debugValue)
    {
        if(setDebugText)
            Debug.Log(" API Response=> <color=blue>" + debugValue + "</color>");
    }


    public void StartInfoAPI(APIType aPIType, APIDetail aPIDetail, Dictionary<string, object> m_data_hash = null , Action<WWW> action = null)
    {
        StartCoroutine(_StartInfoAPI(aPIType, aPIDetail, m_data_hash, action));
    }

    public IEnumerator _StartInfoAPI(APIType aPIType, APIDetail aPIDetail, Dictionary<string, object> m_data_hash = null, Action<WWW> action =null)
    {
        if (m_data_hash == null) m_data_hash = new Dictionary<string, object>();

        string userid = ES3.Load<string>(SaveType.user_id.ToString(), defaultValue:string.Empty);

        //ユーザー以外
        if (aPIType != APIType.user_info && !m_data_hash.ContainsKey("user_id"))
        {
            if (string.IsNullOrEmpty(userid))
            {
                Debug.LogWarning("useridがないので、タイトルに遷移します");
                ChangeScene(SceneType.Title);
                yield break;
            }

            m_data_hash.Add("user_id", userid);
        }

        string url = testAPIpass + "/" + aPIType.ToString() + "/" + aPIDetail.ToString();

        WWWForm form = new WWWForm();

        foreach (var KV in m_data_hash)
        {
            form.AddField(KV.Key, KV.Value.ToString());
        }

        //JsonConsole.Log("Request", aPIType.ToString() + "/" + aPIDetail.ToString(), JsonUtility.ToJson(form.data.ToString()));


        WWW result = new WWW(url, form);

        //StartCoroutine(TestAPI(testurl, form));

        yield return result;

        if (result.error == null)
        {

            //３００００文字以上のログはフリーズするので個別で出してください。
            if (result.text.Count() < 30000)
            {
                JsonConsole.Log("Response", aPIType.ToString() + "/" + aPIDetail.ToString(), result.text);
            }else
            {
                Debug.LogWarning("３００００文字以上のログはフリーズするので個別で出してください。:" + url + ":    " + result.text);
                //string header = 
            }

            action(result);
        }else
        {
            Debug.LogError(  url + " : ¥n" + result.error);
        }
    }



    //https://marsquai.com/28f2a43f-28a8-44d1-9c2b-816da58c1271/bd82e69d-c298-40df-826b-327edf128b30/dfc90725-b59c-47c4-84c3-4ba08194e7fa/
    //https://qiita.com/shun-shun123/items/10c7711b129f8d2b7559
    public IEnumerator StartInfoAPIWithWebRequest(APIType aPIType, APIDetail aPIDetail, string jsonString, Action action)
    {
        string url = testAPIpass + "/" + aPIType.ToString() + "/" + aPIDetail.ToString();

        byte[] byteData = System.Text.Encoding.UTF8.GetBytes(jsonString);
        UnityWebRequest request = new UnityWebRequest(url, "POST");
        request.uploadHandler = new UploadHandlerRaw(byteData);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        Debug.Log("jsonString:" + jsonString);

        yield return request.SendWebRequest();

        if (request.isNetworkError)
        {
            Debug.Log(request.error + ":" + url);
        }
        else if (request.isHttpError)
        {
            Debug.Log(request.error + ":" + url);
        }
        else
        {
            Debug.Log(request.downloadHandler.text);
            action();
        }
    }



    public IEnumerator TestAPI( string testURL, WWWForm form)
    {
        WWW result = new WWW(testURL, form);

        yield return result;
        if (result.error == null)
        {
            APIdebug(testURL + " 送　userid:" + ES3.Load<string>(SaveType.user_id.ToString()) + " : " + result.text);
        }
        else
        {
            Debug.LogError(testURL + " : ¥n" + result.error);
        }
    }

    public IEnumerator TestAPI(string testURL, string testJson)
    {
        byte[] byteData = System.Text.Encoding.UTF8.GetBytes(testJson);
        UnityWebRequest request = new UnityWebRequest(testURL, "POST");
        request.uploadHandler = (UploadHandler)new UploadHandlerRaw(byteData);
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.isNetworkError)
        {
            Debug.Log(request.error);
        }
        else if (request.isHttpError)
        {
            Debug.Log(request.error);
        }
        else
        {
            APIdebug("送　userid:" + ES3.Load<string>(SaveType.user_id.ToString()) + " :" + testURL + " : " + request.downloadHandler.text);
            //Debug.Log(request.downloadHandler.text);
        }
    }

}
