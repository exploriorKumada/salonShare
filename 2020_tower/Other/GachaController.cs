using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TW.GameSetting;
using UnityEngine;

public class GachaController : SystemBaseManager
{
    [SerializeField] Transform podRoot;
    void Start()
    {
        Loding(() =>
        {
            GachaStartOn();
        });
     
    }

    public void GachaStartOn()
    {
        Dictionary<string, object> m_data_hash = new Dictionary<string, object>();
        m_data_hash.Add("gacha_id", 1);
        m_data_hash.Add("num", 10);

        APIManager.Instance.StartInfoAPI(APIType.gacha, APIDetail.exec, m_data_hash, (result) =>
        {
            List<CharaInfo> charaInfos = new List<CharaInfo>();

            Debug.Log(result.text);
            JSONObject get_character_Json = new JSONObject(result.text).GetField("data").GetField("get_character");
            foreach(var charasJson in get_character_Json.list)
            {
                CharaInfo charaInfo =
                new CharaInfo(DataManager.Instance.CharaMasters.FirstOrDefault(x => x.id == (int)charasJson.GetField("chara_id").n),1);
                charaInfos.Add(charaInfo);

            }

            ResourceManager.Instance.LoadCharaModels(charaInfos, () =>
            {
                StartCoroutine(Init(charaInfos));
            });
        });
    }


    public IEnumerator Init(List<CharaInfo> charaInfos)
    {
        foreach (var Value in charaInfos)
        {
            GameObject charaGO = Instantiate(ResourceManager.Instance.GetCharaModel(Value.id));
            charaGO.transform.localPosition = Vector3.zero;
        }

        int count = 0;
        foreach(Transform Value in podRoot)
        {
            count++;
            Value.GetComponent<Animator>().enabled = true;
            yield return new WaitForSeconds(0.5f);
            if (count == 10) break;
        }

        yield return new WaitForSeconds(3f);

        ChangeScene(SceneType.FreeHome);
    }


}
