using System.Collections;
using System.Collections.Generic;
using Explorior;
using UnityEngine;
using System;
using TMPro;
using Timers;
using TW.GameSetting;
using UniRx;
using DG.Tweening;
using UnityEngine.SceneManagement;
using System.Linq;
using Febucci.UI;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    [SerializeField] public StickManManager stickManManager;//操作キャラマネージャー
    [SerializeField] GameObject unit;//敵キャラの量産元オブジェクト
    [SerializeField] GameObject keyUnit;//鍵の量産元オブジェクト
    [SerializeField] GameObject readyMenuObject;//操作開始前の表示オブジェクト
    [SerializeField] GameObject justWaitRoot;//敵に捕まった時の表示オブジェクト
    [SerializeField] UIManager uIManager;//操作UIの管理
    [SerializeField] TextAnimatorPlayer textAnimatorPlayer;//テキストアニメーション
    [SerializeField] Menu menu;//2D　表示管理
    [SerializeField] AdmobManager admobManager;//Admob管理

    List<NPCUnit> nPCUnits = new List<NPCUnit>();//敵キャラリスト

    [NonSerialized] public bool isGameOver;//ゲームオーナーか
    [NonSerialized] public bool isJustStop;//捕まったり落ちたりした場合の時間待ち状態か

    [SerializeField] public List<Material> keyMaterials = new List<Material>();//鍵のマテリアル所持

    [NonSerialized] public int keyCount = 0;//今フィールドの落ちてる鍵数
    [NonSerialized] int keyMaxCount = 300;//鍵最低限での最大数　これが満たされてなければこの数になるまで自動で追加される
    [NonSerialized] int keyAddMaxCount = 1500;//鍵の追加アイテムによる最大数　これ以上は追加アイテムを拾っても追加されない
    [NonSerialized] public int keyGetCount = 0;//鍵取得数
    [NonSerialized] float second = 60;//制限時間秒
    [NonSerialized] float addSecond = 30;//広告視聴による制限時間追加秒

    [NonSerialized] private Timer timer;//時間処理管理
    [NonSerialized] public bool isStart;//スタートしているかどうか
    [NonSerialized] public bool isRestart;//リスタートしているかどうか

    [SerializeField] GameObject tutoObject;

    [SerializeField] GameObject bournusOject;


    // Start is called before the first frame update
    void Start()
    {
        tutoObject.SetActive(false);
        bournusOject.SetActive(false);
        if (!ES3.Load<bool>("tuto",false))
        {
            tutoObject.SetActive(true);
            ES3.Save("tuto", true);
        }

        SetTextAnimator("READY?",1,true);
   
        stickManManager.Init(this);
        justWaitRoot.SetActive(false);
        menu.Init(this);
        menu.keyCountText.text = keyGetCount.ToString();

        admobManager.Initialize(this);

        NPCset();

        TimeSet(second);

        SelectingNewSkin();

        this.ObserveEveryValueChanged(x => isStart)
            .Subscribe(_ =>
            {
                if (!isStart) return;

                readyMenuObject.SetActive(false);
                SetTextAnimator("START!",2,false);

                TimersManager.SetTimer(this, second, TimeEnd);
                timer = TimersManager.GetTimerByName(TimeEnd);
                KeyInitSet();
            });
    }


    UnityAction beforeAction;
    /// <summary>
    /// テキストアニメーション設定
    /// </summary>
    public void SetTextAnimator(string value, float second, bool isRoop)
    {
        textAnimatorPlayer.gameObject.SetActive(true);

        UnityAction thisAction = () => SetTextAnimator(value, second, isRoop);

        if (beforeAction != thisAction && beforeAction != null)
        {
            TimersManager.ClearTimer(beforeAction);
        }

        beforeAction = thisAction;
        textAnimatorPlayer.GetComponent<TextMeshProUGUI>().text = value;
        textAnimatorPlayer.DOPlay();

        if (isRoop)
        {
            TimersManager.SetTimer(this, second, thisAction);
        }else
        {
            TimersManager.SetTimer(this, second, ()=>
            {
                Vector3 baseV3 = textAnimatorPlayer.transform.localScale;
                textAnimatorPlayer.transform.DOScale(Vector3.zero, 0.2f).OnComplete(() =>
                {
                    textAnimatorPlayer.gameObject.SetActive(false);
                    textAnimatorPlayer.transform.localScale = baseV3;
                });
            });

        }
    }

    /// <summary>
    /// EnemyNPC配置
    /// </summary>
    void NPCset()
    {
        for (int i = 0; i < 10; i++)
        {
            var npc = Instantiate(unit, unit.transform.parent).GetComponent<NPCUnit>();
            npc.Init(stickManManager,i,this);
            nPCUnits.Add(npc);
        }

        unit.SetActive(false);

    }


    /// <summary>
    /// 鍵を手に入れた時の処理
    /// </summary>
    public void GetKey(KeyKind keyKind)
    {
        if (isGameOver) return;

        keyGetCount++;

        if(isRestart)
        {
            keyGetCount++;
        }

        keyCount--;

        switch (keyKind)
        {
            case KeyKind.Bigger:
                stickManManager.Big();
                break;
            case KeyKind.Speeder:
            case KeyKind.BigSpeeder:
                stickManManager.Speed(keyKind);
                break;
            case KeyKind.Adder:
                KeyUnitSet(400);
                break;
            case KeyKind.RangeSpreeder:
            case KeyKind.BigRangeSpreeder:
                stickManManager.RangeSpreed(keyKind);
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// 鍵の初期化
    /// </summary>
    void KeyInitSet()
    {
        keyUnit.SetActive(false);

        KeyUnitSet(500);

        StartCoroutine(KeyCheack());
    }

    /// <summary>
    /// 1個分の鍵配置
    /// </summary>
    /// <param name="num"></param>
    void KeyUnitSet(int num = 1)
    {
        for(int i = 0; i<num; i++)
        {
            if(keyCount> keyAddMaxCount)
            {
                Debug.LogWarning("鍵数上限値ストップ");
                return;
            }

            int hani = 50;
            var key = Instantiate(keyUnit, keyUnit.transform.parent).GetComponent<KeyUnit>();
            key.gameObject.SetActive(true);
            key.gameObject.name = keyCount.ToString();
            key.transform.localPosition = new Vector3(UnityEngine.Random.RandomRange(-hani, hani), UnityEngine.Random.RandomRange(20, 30), UnityEngine.Random.RandomRange(-hani, hani));
            key.Init(this);
            keyCount++;
        }

    }


    /// <summary>
    /// 鍵の数を常にチェック
    /// </summary>
    /// <returns></returns>
    IEnumerator KeyCheack()
    {
        while(true)
        {
            //鍵がなければ
            if (keyCount < keyMaxCount)
            {
                KeyUnitSet();
            }

            yield return new WaitForSeconds(1f);
        }
    }

    private void Update()
    {
        //残り時間の表示
        if (timer != null)
            TimeSet(timer.RemainingTime());

        //獲得鍵の更新
        menu.keyCountText.text = keyGetCount.ToString();

#if UNITY_EDITOR
        if (Input.GetKey(KeyCode.Space))
        {
            UnityEditor.EditorApplication.isPaused = true;
        }
#endif
    }

    /// <summary>
    /// 残り時間表示
    /// </summary>
    public void TimeSet(float nowsecond)
    {
        menu.timerText.text = (int)(nowsecond / 60)+ ":" + ((int)(nowsecond % 60)).ToString("00");
    }


    /// <summary>
    /// 制限時間終了
    /// </summary>
    public void TimeEnd()
    {
        SetTextAnimator("TIME OUT!", 2, false);
        Debug.Log("時間切れ");
        nPCUnits.ForEach(x => x.gameObject.SetActive(false));
        stickManManager.SetTrigger("end");
        isGameOver = true;
        isJustStop = true;
        timer = null;
        //TimersManager.ClearAllTimer();
        //リスタートしていたら直で終了画面
        if (isRestart)
        {
            isRestart = false;
            TimersManager.SetTimer(this, 3f, End);
        }
        else
        {
            TimersManager.SetTimer(this, 3f, () => menu.AddTimeMenuRootActive(true));
        }  
    }

    /// <summary>
    /// 敵に捕まったり、外に落ちたりした時　ちょっと操作不能時間を付与
    /// </summary>
    public void JustStop()
    {
        if (stickManManager.kindDic[KeyKind.Bigger] || isJustStop) return;

        justWaitRoot.SetActive(true);
        isJustStop = true;

        stickManManager.SetTrigger("dead");

        SetTextAnimator("STOP..", 5, false);

        TimersManager.SetTimer(this, 5f, JustReset);
    }

    /// <summary>
    /// 敵に捕まったりした後にする初期化
    /// </summary>
    public void JustReset()
    {
        stickManManager.Reset();
        EnemyPositionReset();
        justWaitRoot.SetActive(false);
        isJustStop = false;
    }

    /// <summary>
    /// 敵の位置初期化
    /// </summary>
    public void EnemyPositionReset()
    {
        foreach (var Value in nPCUnits)
            Value.Reset();
    }


    /// <summary>
    /// 時間加算してリスタート
    /// </summary>
    public void AddTime()
    {
        Debug.Log("admob リワード動画を流す");

        bournusOject.SetActive(true);
        //以下動画見た後の処理
        isRestart = true;
        isGameOver = false;
        isJustStop = false;
        menu.AddTimeMenuRootActive(false);
        SetTextAnimator("RESTART!", 2, false);
        TimeSet(addSecond);
        TimersManager.SetTimer(this, addSecond, TimeEnd);
        timer = TimersManager.GetTimerByName(TimeEnd);
        JustReset();
    }

    /// <summary>
    /// 終わった時
    /// </summary>
    public void End()
    {
        //Debug.Log("EndEndEndEnd");
        SaveHighScore();
        menu.EndMenu();
    }


    /// <summary>
    /// ハイスコアを保存
    /// </summary>
    public void SaveHighScore()
    {
        List<int> vs = ES3.Load<List<int>>("highScore", defaultValue: new List<int>());
        vs.Add(keyGetCount);
        vs.Sort();
        vs.Reverse();
        IEnumerable<int> result = vs.Distinct();
        ES3.Save("highScore", result.ToList());
    }


    /// <summary>
    /// スキンを新しく変更
    /// </summary>
    public void SelectingNewSkin()
    {
        stickManManager.skinnedMeshRenderer.material = SkinDataSetting.GetMaterialPath(ES3.Load<int>("selectingId",1), true);
    }

}
