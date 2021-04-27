using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;
using Prime31.TransitionKit;
using System.Linq;
using Explorior;
using UniRx;
using System.Threading.Tasks;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using TW.GameSetting;
using Timers;

public class SystemBaseManager : SerializedMonoBehaviour
{
    private bool systemInit;

    public virtual void Init()
    {
        Debug.Log("SystemBaseManager Init");
        systemInit = true;
    }

    public void ChangeScene(int _sceneType)
    {
        ChangeScene((SceneType)_sceneType);
    }
    public void ChangeScene(SceneType _sceneType)
    {
        ChangeScene(new ChangeSceneInfo()
        {
            sceneType = _sceneType
        }); ;
    }

    //https://baba-s.hatenablog.com/entry/2019/04/12/000000
    public void ChangeScene(ChangeSceneInfo changeSceneInfo) 
    {
        Debug.Log("sceneType:" + changeSceneInfo.sceneType);
        var fader = GetTransitionKitDelegate(changeSceneInfo);

        DataManager.Instance.currentChangeSceneInfo = changeSceneInfo;
        TransitionKit.instance.transitionWithDelegate(fader);
    }

    public TransitionKitDelegate GetTransitionKitDelegate(ChangeSceneInfo _changeSceneInfo)
    {
        switch(_changeSceneInfo.fadeType)
        {
            case FadeType.Fade:
                return new FadeTransition()
                {
                    changeSceneInfo = _changeSceneInfo
                };
            case FadeType.Wind:
                return new WindTransition()
                {
                    changeSceneInfo = _changeSceneInfo
                };

            default:
                return null;
        }
    }

    public void WindSceneClose( SceneType sceneType )
    {
        ChangeScene(new ChangeSceneInfo()
        {
            sceneType = sceneType,
            fadeType = FadeType.Wind,
            isUnload = true,
            loadSceneMode = LoadSceneMode.Additive,
            fadingAction = DataManager.Instance.currentChangeSceneInfo.fadingAction
        });
    }

    public void WindSceneClose()
    {
        ChangeScene(new ChangeSceneInfo()
        {
            sceneType = DataManager.Instance.currentChangeSceneInfo.sceneType,
            fadeType = FadeType.Wind,
            isUnload = true,
            loadSceneMode = LoadSceneMode.Additive,
            fadingAction = DataManager.Instance.currentChangeSceneInfo.fadingAction
        });
    }

    public GameObject SetDialog(int colEventType)
    {
        return SetDialog((ColEventType)colEventType);
    }

    public GameObject SetDialog(ColEventType _colEventType, bool isSlacUp = true)
    {
        TWManger.Instance.gameObject.transform.localRotation = new Quaternion(0, 0, 0, 0);

        var dialogDatas = TWManger.Instance.dialogDatas;

        bool exist = dialogDatas.Any(n => n.colEventType == _colEventType);

        if (exist)
        {
            Debug.LogWarning("The Dialog exist :" + _colEventType);
            return null;
        }


        Debug.Log("Dialog設置:" + _colEventType);
        var _dialogObject = Instantiate(Resources.Load<GameObject>("Dialog/" + _colEventType.ToString()), TWManger.Instance._dialogRoot);
        DialogData data = new DialogData()
        {
            id = dialogDatas.Count,
            colEventType = _colEventType,
            dialogObject = _dialogObject
        };

        //if (isSlacUp)
        //    UI.UIsclaleUp(_dialogObject.transform);

        dialogDatas.Add(data);

        return _dialogObject;
    }

    public void CloseDialog(Action comp = null)
    {
        var dialogDatas = TWManger.Instance.dialogDatas;

        if (dialogDatas.Count == 0)
            return;

        var result = dialogDatas.Last();

        UI.UIsclaleUpAction(2f, result.dialogObject.transform, duration: GameSettingData.DIALOG_EFFECT_TIME,() =>
        {
            Destroy(result.dialogObject);
            dialogDatas.Remove(result);
            comp?.Invoke();
        });
    }

    public void StopEffect( ParticleSystem ps, float second)
    {
        TimersManager.Instance.SetTimer(ps, second, () => {
            ps.Stop(true, ParticleSystemStopBehavior.StopEmitting);

            TimersManager.Instance.SetTimer(ps, 10f, () => Destroy(ps.gameObject));
        });
    }


    public async void Loding(Action comp, bool addAction = false)
    {
        //ローディング画面設置
        SetDialog(ColEventType.Loading);
       
        //マスター取得処理
        if(!DataManager.Instance.isMasterLoad)
          await DataManager.Instance.MasterLoad();

        TimersManager.Instance.Initialized();

        if (!addAction)
        {
            //ローディング画面撤去
            CloseDialog(comp);
        }
        else
        {
            comp();
        }

    }

    public Dictionary<string, JSONObject> GetDictionaryUsingJson(JSONObject jSONObject)
    {
        Dictionary<string, JSONObject> returnValue = new Dictionary<string, JSONObject>();

        List<string> keys = new List<string>();
        List<JSONObject> values = new List<JSONObject>();

        if (jSONObject.keys == null)
            return returnValue;

        foreach (var Value in jSONObject.keys)
            keys.Add(Value);

        foreach (var Value in jSONObject.list)
            values.Add(Value);

        for (int i = 0; i < keys.Count; i++)
            returnValue[keys[i]] = values[i];

        return returnValue;

    }

    /// <summary>
    /// キャンバスに対してのタップ位置を取得する
    /// </summary>
    /// <param name="canvas"></param>
    /// <returns></returns>
    public Vector2 GetMousePosition(Canvas canvas)
    {
        Vector2 localpos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.GetComponent<RectTransform>(), Input.mousePosition, canvas.worldCamera, out localpos);
        return localpos;
    }

    public void DebugLog(bool isActive = true, string value = "")
    {
        if (isActive)
            Debug.Log(value);
    }

}
