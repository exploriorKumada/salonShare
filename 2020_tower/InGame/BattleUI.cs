using UniRx;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using System.Collections.Generic;
using Explorior;
using TW.GameSetting;
using Michsky.UI.ModernUIPack;

public class BattleUI : MonoBehaviour
{
    [SerializeField] GameObject baseObject;
    [SerializeField] Button attackButton;
    [SerializeField] ProgressBar progressBar;

    [SerializeField] GeneralData castleControllerGeneralData;
    [SerializeField] Transform castelRoot;

    [SerializeField] GameObject moveJoyStick;

    List<CharaInfo> charaInfos = new List<CharaInfo>();

    private Dictionary<CharaInfo, GeneralData> statusDic = new Dictionary<CharaInfo, GeneralData>();
    CastleController castleController;

    public void Initialize(List<CharaInfo> _charaInfos, CastleController _castleController,PlayerCharaController playerCharaController)
    {
        charaInfos = _charaInfos;
        castleController = _castleController;

        attackButton.onClick.AddListener(playerCharaController.AttackMotion);

        baseObject.ParentInitialize();
        foreach(var Value in charaInfos)
        {
            GeneralData generalData = Instantiate(baseObject, baseObject.transform.parent).GetComponent<GeneralData>();
            statusDic.Add(Value, generalData);
            generalData.gameObject.SetActive(true);
            SetHp(Value, statusDic[Value]);
            SetModel(Value, generalData.transform,CharaAttribute.Player);
        }

        statusDic.Add(castleController.charaInfo, castleControllerGeneralData);
        CastelSetHp(castleController.charaInfo, statusDic[castleController.charaInfo]);
        SetModel(castleController.charaInfo, castelRoot,CharaAttribute.Castel);

        //moveJoyStick.SetActive(false);

        Observe();
    }

    public void OnPointDown()
    {

    }

    public void OnPointUp()
    {

    }

    public void Observe()
    {
        foreach(var Value in charaInfos)
        {
            this.ObserveEveryValueChanged(x => Value.hp)
           .Where(x => true)
           .Subscribe(_ =>
           {
               SetHp(Value, statusDic[Value]);
           });
        }

        this.ObserveEveryValueChanged(x => castleController.charaInfo.hp)
           .Where(x => true)
           .Subscribe(_ =>
           {
               CastelSetHp(castleController.charaInfo, statusDic[castleController.charaInfo]);
           });

    }

    public void SetHp(CharaInfo charaInfo, GeneralData generalData)
    {
        generalData.textMeshProUGUIs[0].text = charaInfo.hp.ToString();
        generalData.textMeshProUGUIs[1].text = charaInfo.charaMaster.charaName;
        generalData.images[0].fillAmount = (float)((float)charaInfo.hp / (float)charaInfo.maxHP);
    }

    //キャラ設置
    private GameObject SetModel(CharaInfo charaInfo, Transform parent, CharaAttribute charaAttribute)
    {
        int id = charaAttribute == CharaAttribute.Player ? charaInfo.id : -1;
        GameObject model;
        if (charaAttribute == CharaAttribute.Player)
        {
            model = Instantiate(ResourceManager.Instance.GetCharaModel(id), parent);
            model.transform.localScale = Vector3.one * 150f;      
            model.transform.localPosition = new Vector3(130, -80, 0);
        }
        else
        {
            model = Instantiate(ResourceManager.Instance.GetInGameModel("SelfCastel"), parent);
            model.transform.localScale = Vector3.one * 100f;
            model.transform.localPosition = new Vector3(-715, 180, 0);
        }

        model.transform.localRotation = new Quaternion(0, 180, 0, 0);
        model.SetLayer(5);
        model.SetActive(true);

        var basic = model.GetComponent<CharacterObjectBasic>();

        if(basic!=null)
        {
            if (basic.foundFunction != null) basic.foundFunction.enabled = false;
            if (basic.attackActionFunctions != null) basic.attackActionFunctions.enabled = false;
            basic.weaponFunctions.ForEach(x => x.enabled = false);
        }        

        return model;
    }

    public void CastelSetHp(CharaInfo charaInfo, GeneralData generalData)
    {
        float per = (float)(((float)((float)charaInfo.hp / (float)charaInfo.maxHP)) * 100);
        progressBar.currentPercent = per;
        progressBar.UpdateUI();
    }
}
;