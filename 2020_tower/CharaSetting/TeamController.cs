using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Explorior;
using TW.GameSetting;
using UnityEngine.UI;
using System.Linq;
using SRF.UI;
using System;

public class TeamController : SystemBaseManager
{
    [SerializeField] GameObject baseObject;

    List<Transform> teamTfs = new List<Transform>();
    Dictionary<int, Dictionary<int, GameObject>> charaObjDic = new Dictionary<int, Dictionary<int, GameObject>>();

    List<TeamPreData> teamPostDatas = new List<TeamPreData>();

    public SetiingCharaController settingCharaInfo;

    Action<int> setRotaion;

    public void Initilize(List<TeamPreData> _teamPostDatas, Action<int> _setRotaion)
    {
        setRotaion = _setRotaion;
        teamPostDatas = _teamPostDatas;
        baseObject.ParentInitialize();

        for (int i = 0; i < 3; i++)
        {
            var team = Instantiate(baseObject, baseObject.transform.parent);
            team.SetActive(true);

            teamTfs.Add(team.transform);
        }
        TeamSet(_teamPostDatas);
    }

    public void TeamSet(List<TeamPreData> _teamPostDatas)
    {
        int count = 0;
        foreach (var Value in teamTfs)
        {
            var teamInfo = _teamPostDatas[count];
            var charaImageBase = Value.Find("charas/image");
            var charaImageBaseParent = Value.Find("charas");
            foreach (Transform tf in charaImageBaseParent)
            {
                if (tf != charaImageBase)
                    Destroy(tf.gameObject);
            }

            charaObjDic[count] = new Dictionary<int, GameObject>();
            int num = 0;
            foreach (var charaInfo in teamInfo.charaInfos)
            {
                var charaParent = Instantiate(charaImageBase, charaImageBaseParent);
                charaParent.name = count.ToString();
                charaObjDic[count][num] = CharaunitSet(charaInfo, charaParent.transform);

                int _count = count, _num = num; var _charaInfo = charaInfo;
                charaParent.GetComponent<LongPressButton>().onClick.AddListener(() => { ChangeChara(_count, _num); });
                charaParent.GetComponent<LongPressButton>().onLongPress.AddListener(() => { LongPress(_count, _num); });
                charaParent.gameObject.SetActive(true);
                num++;
            }

            charaImageBase.gameObject.SetActive(false);
            count++;
        }
    }


    //キャラ設置
    private GameObject CharaunitSet(CharaInfo charaInfo, Transform parent)
    {
        GameObject chara = Instantiate(ResourceManager.Instance.GetCharaModel(charaInfo.id), parent);
        chara.SetLayer(5);
        chara.transform.localRotation = new Quaternion(0, 180, 0, 0);
        chara.transform.localScale = Vector3.one * 150f;
        chara.transform.localPosition = new Vector3(0, -50, 0);
        chara.SetActive(true);

        return chara;
    }

    public void LongPress(int teamNum, int num)
    {
        Debug.Log("LongPress:" + teamNum + ":" +num);


        setRotaion(teamPostDatas[teamNum].charaInfos[num].id);
    }


    /// <summary>
    /// キャラ差し替え
    /// </summary>
    public void ChangeChara(int teamNum, int num)
    {
        CharaInfo selectCharaInfo = settingCharaInfo.charaInfo;

        TeamPreData charas = teamPostDatas[teamNum];        
        GameObject exitChara = charaObjDic[teamNum][num];

        CharaInfo exisCharaInfo = charas.charaInfos[num];
        Transform setTrans = exitChara.transform.parent;

        int key = charas.charaInfos.IndexOf(selectCharaInfo);
        if (key == -1)
        {
            Debug.Log("含まれてないので、そのまま入れる");
            SetChara(teamNum, num, setTrans, selectCharaInfo);
        }
        else
        {
            //入れようとしたキャラのオブジェクトサーチ 入れる前にどこにいたの？
            Transform serchSelf = charaObjDic[teamNum][key].transform.parent;

            Debug.Log("含まれているので、入れ替える");
            //とりあえず入れたいやつを入れたいとこに入れる
            SetChara(teamNum, num, setTrans, selectCharaInfo);
            //元々いた人はどいてもらう
            SetChara(teamNum, key, serchSelf, exisCharaInfo);
        }

        //サーバー保存
        StartCoroutine(TeamIn(teamNum));
    }

    public void SetChara(int teamNum, int num,Transform parentTf,CharaInfo charaInfo)
    {
        parentTf.ParentTransInitialize();//掃除
        teamPostDatas[teamNum].charaInfos[num] = charaInfo;//データ保存
        charaObjDic[teamNum][num] = CharaunitSet(charaInfo, parentTf);//オブジェクト設置
    }


    public IEnumerator TeamIn(int teamNo)
    {
        Debug.Log("teamNo:" + teamNo);
        TeamPreData charas = teamPostDatas[teamNo];
        TeamPostData teamPostData = new TeamPostData();
        teamPostData.team_id = (teamNo+1).ToString();
        teamPostData.user_id = ES3.Load<string>(SaveType.user_id.ToString());
        teamPostData.chara_id_list = new List<int>();
        charas.charaInfos.ForEach(x => teamPostData.chara_id_list.Add(x.id));

        return APIManager.Instance.StartInfoAPIWithWebRequest(APIType.user_chara, APIDetail.team_regist, JsonUtility.ToJson(teamPostData), () =>
        {
            Debug.Log("TeamIn complete!!");
        });

    }
}


public class TeamPreData
{
    public int teamId;
    public List<CharaInfo> charaInfos;
}


public class TeamPostData
{
    public string user_id;
    public string team_id;
    public List<int> chara_id_list;
}
