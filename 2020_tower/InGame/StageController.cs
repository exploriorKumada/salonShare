using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using System.Linq;
using UnityEngine.AI;

public class StageController : SystemBaseManager
{
    //[DictionaryDrawerSettings(KeyLabel = "AnimationType", ValueLabel = "名前")] public Dictionary<int, Vector3> positionSetting;
    [DictionaryDrawerSettings(KeyLabel = "エリアステージ", ValueLabel = "名前")] public Dictionary<int, List<GameObject>> areaSetting;

    public Vector3 SetPosition(int id)
    {

        if(!areaSetting.ContainsKey(id))
        {
            Debug.LogError("Stage　未登録:" + id);
            id = 1;
            return Vector3.zero;
        }

        //Debug.Log("Stage登録:" + id);

        List<GameObject> stageAreas = areaSetting[id];
        List<float> xPosi = new List<float>(), zPosi = new List<float>();
        foreach (var Value in stageAreas)
        {
            xPosi.Add(Value.transform.position.x);
            zPosi.Add(Value.transform.position.z);
        }

        return new Vector3(
            Random.Range(xPosi.Min(), xPosi.Max()),
            0,
            Random.Range(zPosi.Min(), zPosi.Max()));
    }
}
