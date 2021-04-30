using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
using Explorior;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    [SerializeField] GameObject skinUnit;
    [SerializeField] GameObject skinMenuRoot;
    [SerializeField] GameObject addTimeMenuRoot;
    public void AddTimeMenuRootActive(bool flg) => addTimeMenuRoot.SetActive(flg);
    [SerializeField] GameObject endMenuRoot;
    [SerializeField] TextMeshProUGUI ruikeiKeys;
    [SerializeField] TextMeshProUGUI buttonText;
    [SerializeField] TextMeshProUGUI getCountText;
    [SerializeField] GameObject selectingObject;
    [SerializeField] GameObject gachaResultRoot;
    [SerializeField] Transform gachaGetCharaRoot;
    [SerializeField] GameObject skinMenuObjectsRoot;
    [SerializeField] List<GameObject> getButtons;

    [SerializeField] public TextMeshProUGUI keyCountText;
    [SerializeField] public TextMeshProUGUI keyRuikeiCountText;
    [SerializeField] public TextMeshProUGUI stageCountText;
    [SerializeField] public TextMeshProUGUI bournusText;
    [SerializeField] public TextMeshProUGUI timerText;
    [SerializeField] GameObject resultUnit;

    private int gachaNeedKey = 3000;
    private List<int> gettingIds;
    GameManager gameManager;

    private List<GameObject> skinData = new List<GameObject>();

    private void Start()
    {
        skinMenuRoot.SetActive(false);

    }

    /// <summary>
    /// 初期化
    /// </summary>
    /// <param name="_gameManager"></param>
    public void Init(GameManager _gameManager)
    {
        selectingObject.transform.SetParent(transform, false);
        gameManager = _gameManager;
        endMenuRoot.SetActive(false);
        getButtons.ForEach(x => x.SetActive(!SkinDataSetting.IsAllGet));
        gettingIds = ES3.Load<List<int>>("gettingIds", new List<int>() { 1 });

        StartCoroutine(SkinInit());

        buttonText.color = ES3.Load<int>("ruikeiScore", 0) >= gachaNeedKey ? Color.white : Color.red;

    }


    /// <summary>
    /// スキン表示
    /// </summary>
    /// <returns></returns>
    public IEnumerator SkinInit()
    {
        ruikeiKeys.text = ES3.Load<int>("ruikeiScore", 0).ToString();

        var skinList = SkinDataSetting.SkinDatas();

        skinUnit.SetActive(false);
        skinUnit.ParentInitialize();
        skinData = new List<GameObject>();

        foreach (var Value in skinList)
        {
            bool isGet = gettingIds.IndexOf(Value.id) != -1;

            var skinBase = Instantiate(skinUnit, skinUnit.transform.parent);

            var imageBase = skinBase.transform.Find("Image");
            var chara = Instantiate(SkinDataSetting.GetSkinObj(Value.id, isGet), imageBase);

            chara.GetComponent<Rigidbody>().useGravity = false;

            skinBase.transform.Find("hide").gameObject.SetActive(!isGet);
            skinBase.GetComponent<GeneralData>().numbers = new Dictionary<int, int>();
            skinBase.GetComponent<GeneralData>().numbers.Add(0, Value.id);
            skinBase.GetComponent<GeneralData>().flg = isGet;

            imageBase.gameObject.GetComponent<Button>().onClick.AddListener(() =>
            {
                Push(Value.id);
            });

            chara.transform.localScale = Vector3.one * 111;
            chara.transform.localPosition = new Vector3(5, -100, -300);
            chara.transform.localRotation = new Quaternion(0, 180, 0, 0);
            chara.SetLayer(5, true);

            skinBase.SetActive(true);

            skinData.Add(skinBase);

            yield return null;
        }

        Push(ES3.Load<int>("selectingId", 1));
    }


    /// <summary>
    /// ガチャボタン押下
    /// </summary>
    public void GachaOn()
    {
        int having = ES3.Load<int>("ruikeiScore", 0);
        if (having < gachaNeedKey)
        {
            Debug.Log("金なし");
            return;
        }

        ES3.Save<int>("ruikeiScore", having - gachaNeedKey);

        List<int> tagets = new List<int>();

        foreach (var Value in SkinDataSetting.SkinDatas())
            if (gettingIds.IndexOf(Value.id) == -1) tagets.Add(Value.id);

        if (tagets.Count() != 0)
        {
            int get = tagets.GetAtRandom();
            gettingIds.Add(get);
            gettingIds.Sort();
            Debug.Log("get:" + get);
            ES3.Save("gettingIds", gettingIds);
            SetGachaResult(get);
        }

        Init(gameManager);
    }


    GameObject getChara;
    /// <summary>
    /// ガチャ結果
    /// </summary>
    /// <param name="charaId"></param>
    public void SetGachaResult(int charaId)
    {
        gachaResultRoot.SetActive(true);
        skinMenuObjectsRoot.SetActive(false);

        getChara = Instantiate(SkinDataSetting.GetSkinObj(charaId, true), gachaGetCharaRoot);
        getChara.transform.localScale = Vector3.one * 500;
        getChara.transform.localPosition = new Vector3(0, -780, -300);
        getChara.transform.localRotation = new Quaternion(0, 180, 0, 0);
        getChara.SetLayer(5, true);
    }

    /// <summary>
    /// リザルト閉じる
    /// </summary>
    public void ResultClose()
    {
        gachaResultRoot.SetActive(false);
        skinMenuObjectsRoot.SetActive(true);
        Destroy(getChara.gameObject);
    }


    /// <summary>
    ///　終わった時のメニュー画面
    /// </summary>
    public void EndMenu()
    {
        endMenuRoot.SetActive(true);

        List<int> vs = ES3.Load<List<int>>("highScore");

        int beforeRuikei = ES3.Load<int>("ruikeiScore", 0);
        int afterRuikei = ES3.Load<int>("ruikeiScore", 0) + gameManager.keyGetCount;
        Debug.Log(beforeRuikei + ":" + afterRuikei);
        keyRuikeiCountText.text = beforeRuikei.ToString();
        getCountText.text = "+" + gameManager.keyGetCount;

        ES3.Save<int>("ruikeiScore", afterRuikei);
        StartCoroutine(ScoreAnimation(beforeRuikei, afterRuikei, 0.5f));

        resultUnit.SetActive(false);
        for (int i = 0; i < 7; i++)
        {
            var go = Instantiate(resultUnit, resultUnit.transform.parent);
            if (vs.Count() < i + 1)
            {
                go.transform.Find("window/scoreTest").GetComponent<TextMeshProUGUI>().text = i + 1 + ": " + 0;
            }
            else
            {
                go.transform.Find("window/scoreTest").GetComponent<TextMeshProUGUI>().text = i + 1 + ": " + vs[i];
            }
            go.SetActive(true);
        }
    }


    /// <summary>
    /// スコアをドラムアニメーションさせる
    /// </summary>
    public IEnumerator ScoreAnimation(float startScore, float endScore, float duration)
    {
        // 開始時間
        float startTime = Time.time;

        // 終了時間
        float endTime = startTime + duration;

        do
        {
            // 現在の時間の割合
            float timeRate = (Time.time - startTime) / duration;

            // 数値を更新
            float updateValue = (float)((endScore - startScore) * timeRate + startScore);

            // テキストの更新
            keyRuikeiCountText.text = updateValue.ToString("f0"); // （"f0" の "0" は、小数点以下の桁数指定）

            // 1フレーム待つ
            yield return null;

        } while (Time.time < endTime);

        // 最終的な着地のスコア
        keyRuikeiCountText.text = endScore.ToString();
    }


    /// <summary>
    /// キャラ選択
    /// </summary>
    /// <param name="selectId"></param>
    public void Push(int selectId)
    {
        var obj = skinData[selectId - 1];

        if (obj.GetComponent<GeneralData>().flg == false) return;

        Debug.Log("selecId:" + selectId);
        ES3.Save("selectingId", selectId);

        selectingObject.transform.SetParent(obj.transform, false);

        gameManager.SelectingNewSkin();

        selectingObject.SetActive(true);
    }

}
