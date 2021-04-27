using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;
using Explorior;
using TW.GameSetting;

public class CharaSettingManager : SystemBaseManager
{
    [SerializeField] Transform charaObjectParent;
    [SerializeField] UCharts.RadarChart radarChart;
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] TextMeshProUGUI explanationText;
    [SerializeField] TextMeshProUGUI lvText;
    [SerializeField] TeamController teamController;

    [SerializeField] Transform teamMenu;
    [SerializeField] Transform statusMenu;

    private const float PAbasis = 100f;
    private const float PDbasis = 100f;
    private const float MAbasis = 100f;
    private const float MDbasis = 100f;
    private const float HPbasis = 600f;
    private const float MPbasis = 600f;
    private const float MVbasis = 10f;
    private const float CRbasis = 5f;

    List<SetiingCharaController> setiingCharaControllers = new List<SetiingCharaController>();

    void Start()
    {
        Loding(() =>
        {
            Initialized();
        });
    }

    public void Initialized()
    {
        ResourceManager.Instance.LoadCharaModels(DataManager.Instance.userCharaInfos, () =>
        {
            SetCharacter();
            SetTeams();
        });
    }

    public void SetCharacter()
    {
        foreach (Transform Value in charaObjectParent)
            Destroy(Value.gameObject);


        setiingCharaControllers = new List<SetiingCharaController>();

        foreach (var Value in DataManager.Instance.userCharaInfos)
        {
            var chara = Instantiate(ResourceManager.Instance.GetCharaModel(Value.id), charaObjectParent);
            chara.transform.localPosition = Vector3.zero;
            chara.transform.localScale = Vector3.one * 8;
            var settingChara = chara.AddComponent<SetiingCharaController>();
            settingChara.charaInfo = Value;
            settingChara.SetiingCharaInitialized();
            setiingCharaControllers.Add(settingChara);
            chara.gameObject.name = Value.id + "_" + Value.charaMaster.charaName;
        }

        StartCoroutine(SetDeproy());

    }

    IEnumerator SetDeproy()
    {
        charaObjectParent.GetComponent<CircleDeployer>().Deploy();
        yield return null;
        charaObjectParent.GetComponent<CircleDeployer>().Deploy();
        yield return null;
        charaObjectParent.GetComponent<CircleDeployer>().Deploy();

        SetCharaStatus(setiingCharaControllers.FirstOrDefault());
        StartCoroutine(RotationInit(-1));
    }

    public void SetTeams()
    {
        teamController.Initilize(DataManager.Instance.userTeamPreInfos, SetChara);
    }

    public IEnumerator RotationInit(int direction)
    {
        var obj = GetComponent<ObjCtrl>();
        obj.eventTrigger.enabled = false;
        var newAngle = obj.obj.transform.localEulerAngles;
        float starty = newAngle.y;
        var nowSettingCharaInfo = teamController.settingCharaInfo;
        int add = 0;

        while (true)
        {     
            newAngle.y = starty + (add * direction);
            obj.obj.transform.localEulerAngles = newAngle;
            add++;

            //Debug.Log(nowSettingCharaInfo.charaInfo.id +":" + settingCharaInfo.charaInfo.id);

            if (nowSettingCharaInfo != teamController.settingCharaInfo) break;

            yield return null;                 
        }

        obj.eventTrigger.enabled = true;
    }


    public void DirectionMove(int direction)
    {
        StartCoroutine(RotationInit(direction));
    }


    public void SetChara(int id)
    {
        var settingChara = setiingCharaControllers.FirstOrDefault(x => x.charaInfo.id == id);
        SetCharaStatus(settingChara);
        StartCoroutine(RotationInit(-1));
    }

    public void SetCharaStatus(SetiingCharaController setiingCharaController)
    {
        if (setiingCharaController == teamController.settingCharaInfo)
            return;

        teamController.settingCharaInfo = setiingCharaController;

        var charaInfo = setiingCharaController.charaInfo;
        //Debug.Log("charaInfo.lv:" + charaInfo.lv);
        //Debug.Log("charaInfo.physicalAttack:" + charaInfo.physicalAttack);
        //Debug.Log("charaInfo.physicalDefence:" + charaInfo.physicalDefence);
        //Debug.Log("charaInfo.magicAttack:" + charaInfo.magicAttack);
        //Debug.Log("charaInfo.magicDefence:" + charaInfo.magicDefence);
        //Debug.Log("charaInfo.magicDefence:" + charaInfo.base_hp);
        nameText.text = charaInfo.charaMaster.charaName;
        explanationText.text = charaInfo.charaMaster.description;
        lvText.text = "Lv." + charaInfo.lv + "/" + GameSettingData.CharaMaxLv;
        radarChart.m_Data = new List<float>();
        radarChart.statusValues = new List<string>();
        radarChart.m_Data.Add(charaInfo.physicalAttack / PAbasis);
        radarChart.statusValues.Add(charaInfo.physicalAttack.ToString());
        radarChart.m_Data.Add(charaInfo.physicalDefence / PDbasis);
        radarChart.statusValues.Add(charaInfo.physicalDefence.ToString());
        radarChart.m_Data.Add(charaInfo.magicAttack / MAbasis);
        radarChart.statusValues.Add(charaInfo.magicAttack.ToString());
        radarChart.m_Data.Add(charaInfo.magicDefence / MDbasis);
        radarChart.statusValues.Add(charaInfo.magicDefence.ToString());
        radarChart.m_Data.Add(charaInfo.base_hp / HPbasis);
        radarChart.statusValues.Add(charaInfo.base_hp.ToString());
        radarChart.m_Data.Add(charaInfo.base_mp / MPbasis);
        radarChart.statusValues.Add(charaInfo.base_mp.ToString());
        radarChart.m_Data.Add(0.5f);

        radarChart.Init();
    }


    public void AutoSelect(int charaid)
    {

    }

    public void SceneClose()
    {
        WindSceneClose(SceneType.CharaSetting);
    }


    //1:status 2:team
    public void MenuSet(int id)
    {
        if (id == 2)
        {
            UI.UIcutin(teamMenu, -1000f);
            UI.UIcutOut(statusMenu, -1000f);
        }else
        {
            UI.UIcutin(statusMenu, -1000f);
            UI.UIcutOut(teamMenu, -1000f);
        }
    }

}
