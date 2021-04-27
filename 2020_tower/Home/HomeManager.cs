using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Explorior;
using DG.Tweening;
using System.Linq;
using System;
using Random = UnityEngine.Random;
using TW.GameSetting;

public class HomeManager : SystemBaseManager
{
    [SerializeField] QuestDialog questDialog;
    [SerializeField] Camera mainCamera;

    [SerializeField] Transform mainObj;
    [SerializeField] Transform charaObj;
    [SerializeField] Transform blockObj;

    [SerializeField] Transform mainContents;
    [SerializeField] Transform charaContents;
    [SerializeField] Transform charaSettingContents;

    [SerializeField] Transform charaPowerupContents;
    [SerializeField] Transform charaObjectParent;

    private Transform nowContents;
    [SerializeField] Animator animator;
    Vector3 baseVector3;
    Vector3 blockBaseVector3;

    Tweener sequence;

    float inUI = -500f;
    float outUI = 1000f;
    bool floating = true;

    bool tapBlock = false;

    void Start()
    {
        Loding(() =>
        {
            Initialized();
        });
    }

    public void Initialized()
    {
        UI.UIRoateRoop(mainObj);

        baseVector3 = charaObj.localPosition;
        blockBaseVector3 = blockObj.localPosition;

        charaPowerupContents.gameObject.SetActive(false);

        UI.UIcutin(charaObj, ajustY: 1000f, second: 3f, callBack: () => { FloatObject(charaObj, baseVector3, 5f, 5f); });
        FloatObject(blockObj, blockBaseVector3, 2000f, 8f);

        foreach (Transform Value in mainContents.parent.transform)
            (Value as Transform).gameObject.SetActive(false);

        StartCoroutine(EffectIn(mainContents));
    }

    public IEnumerator EffectIn(Transform parentTF, float x = 0, float y = 0, Action callBack = null)
    {
        tapBlock = true;

        nowContents = parentTF;

        parentTF.gameObject.SetActive(true);

        List<Transform> buttons = new List<Transform>();

        foreach (var Value in parentTF) buttons.Add(Value as Transform);

        buttons.ForEach(button => button.gameObject.SetActive(false));

        foreach(var Value in buttons)
        {
            Action callBackOn = null;

            if (Value == buttons.LastOrDefault())
            {
                callBackOn = () =>
                {
                    tapBlock = false;
                    callBack?.Invoke();
                };
            }

            UI.UIcutin(Value,ajustX:0,ajustY: y,callBack: callBackOn);
            yield return new WaitForSeconds(0.2f);               
        }

        yield break;
    }


    public IEnumerator EffectOut(Transform parentTF, float x = 0, float y = 0, Action callBack = null)
    {
        List<Transform> buttons = new List<Transform>();

        foreach (var Value in parentTF)
            buttons.Add(Value as Transform);


        foreach (var Value in buttons)
        {
            Action callBackOn = null;

            if (Value == buttons.LastOrDefault())
            {
                callBackOn = () =>
                {
                    parentTF.gameObject.SetActive(false);
                    callBack?.Invoke();
                };
            }

            UI.UIcutOut(Value, ajustX: x, ajustY: y, callBack: callBackOn);
            yield return new WaitForSeconds(0.1f);

        }

        yield break;
    }

    //指定の範囲でランダムに移動させる
    public void FloatObject(Transform _transform,Vector3 vector3, float range,float second)
    {
        if (floating == false) return; 

        Vector3 tagetPositon =
            new Vector3(
                Random.Range(vector3.x - range, vector3.x + range),
                Random.Range(vector3.y - range, vector3.x + range),
                baseVector3.z);

        sequence = _transform.DOLocalMove(
                tagetPositon,
                second
                ).OnComplete(() => { FloatObject(_transform, vector3, range, second); });
    }

    /// <summary>
    /// キャラ設定画面に遷移
    /// </summary>
    public void CharaInitSetON(List<CharaInfo> charaInfos)
    {
        sequence.Pause();
        sequence.Kill();
        floating = false;

        foreach (Transform Value in charaObjectParent)
            Destroy(Value.gameObject);

        foreach (var Value in charaInfos)
        {
            var chara = Instantiate(ResourceManager.Instance.GetCharaModel(Value.id),charaObjectParent);
            chara.transform.localPosition = Vector3.zero;
            chara.transform.localScale = Vector3.one * 800;
        }

        charaObjectParent.GetComponent<CircleDeployer>().Deploy();

        mainCamera.transform.DORotate(
         new Vector3(0,-40,0),
         0.5f
        );

        charaObj.transform.DOLocalMove(
            new Vector3(baseVector3.x, baseVector3.y-1500f, baseVector3.z),
            0.4f
            ).OnComplete(()=>
            {
                charaObj.gameObject.SetActive(false);
                charaPowerupContents.gameObject.SetActive(true);

                UI.UIcutin(charaPowerupContents, ajustX: 0, ajustY: -10000f);


            });
    }




    public void QuestOpen()
    {
        questDialog.Initialize();
    }

    public void CharaMenuOpen()
    {
        if (tapBlock) return; tapBlock = true;

        StartCoroutine(EffectOut( parentTF:mainContents,x:0,y: outUI,()=> { StartCoroutine(EffectIn( charaContents,y:inUI)); }));
    }

    public void BackHome(bool CameraInit)
    {
        if (tapBlock) return; tapBlock = true;

        if (CameraInit)
        {

            UI.UIcutOut(charaPowerupContents, ajustX: 0, ajustY: -10000f, callBack:()=>
            {
                mainCamera.transform.DORotate(Vector3.zero, 0.5f);
                charaObj.gameObject.SetActive(true);
                CharaStateTypeSetting.AnimationInit(animator);
                animator.SetBool("float", true);
                StartCoroutine(EffectOut(parentTF: nowContents, x: 0, y: -outUI, () => { StartCoroutine(EffectIn(mainContents, y: inUI)); }));

                charaObj.DOLocalMove(
                baseVector3,
                5f
                ).OnComplete(() =>
                {
                    floating = true;
                    FloatObject(charaObj, baseVector3, 5f, 5f);
                });


            });


        }
        else
        {
            StartCoroutine(EffectOut(parentTF: nowContents, x: 0, y: outUI, () => { StartCoroutine(EffectIn(mainContents, y: inUI)); }));
        }

    }



}
