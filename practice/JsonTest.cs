using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JsonTest : MonoBehaviour
{
    void Start()
    {        
        //前回含めて　今回表示するデータ宣言
        DateTestClass dateTestClass = new DateTestClass();
        dateTestClass.dateTestClasses = new List<DateTestClassUnit>();

        //前回保存したJsonを取得
        string saveJson = PlayerPrefs.GetString("timeJson", string.Empty);
        if (saveJson != string.Empty)
        {
            dateTestClass = JsonUtility.FromJson<DateTestClass>(saveJson);

            Debug.Log("前のやつ: データ数は" + dateTestClass.dateTestClasses.Count);
            if (dateTestClass.dateTestClasses == null)
            {
                Debug.LogWarning("json エラー");

            }
            else
            {
                foreach (var Value in dateTestClass.dateTestClasses)
                {
                    //ここでInstantiateして生成して再現構築
                    Debug.Log(Value.date + ":" + Value.value);
                }
            }
        }

        //テストデータ入れる
        for (int i = 0; i < 5; i++)
        {
            DateTestClassUnit dateTestClassUnit = new DateTestClassUnit();
            dateTestClassUnit.date = Time.deltaTime.ToString();
            dateTestClassUnit.value = Random.RandomRange(0, 300).ToString();
            dateTestClass.dateTestClasses.Add(dateTestClassUnit);

        }
        Debug.Log("前のやつ加えた今のやつ:　データ数は" + dateTestClass.dateTestClasses.Count);
        string jsonstr = JsonUtility.ToJson(dateTestClass);       
        Debug.Log(jsonstr);
        PlayerPrefs.SetString("timeJson", jsonstr);
    }

}


[System.Serializable]
public class DateTestClass
{
    public List<DateTestClassUnit> dateTestClasses;
}

[System.Serializable]
public class DateTestClassUnit
{
    public string date;
    public string value;
}
