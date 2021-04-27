using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Explorior;
using TMPro;
using TW.GameSetting;
using UnityEngine.SceneManagement;
using System.Linq;
using Sirenix.OdinInspector;
using DG.Tweening;

public class WeaponSetting : SystemBaseManager
{
    [SerializeField] Transform weaponTF;
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] TextMeshProUGUI explanationText;
    [SerializeField] TextMeshProUGUI lvText;

    [SerializeField] UCharts.RadarChart radarChart;

    [DictionaryDrawerSettings(KeyLabel = "PowerUpMenuType", ValueLabel = "オブジェクト")] public Dictionary<PowerUpMenuType, GameObject> menuTypeObject;

    private const float PAbasis = 10f;
    private const float PDbasis = 10f;
    private const float MAbasis = 10f;
    private const float MDbasis = 10f;
    private const float HPbasis = 60f;
    private const float MPbasis = 60f;
    private const float MVbasis = 10f;
    private const float CRbasis = 10f;
    private int weaponId = 8;

    PowerUpMenuType powerUpMenuType = PowerUpMenuType.Status;

    

    [System.NonSerialized] public WeaponInfo weaponInfo;

    void Start()
    {
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(SceneType.WeaponSetting.ToString()));

        Loding(() =>
        {
            weaponInfo = (WeaponInfo)DataManager.Instance.currentChangeSceneInfo.data_hash["weaponinfo"];

            if (weaponInfo == null)
            {
                weaponInfo = new WeaponInfo().GetDebug(8);
            }
            else
            {
                weaponInfo = DataManager.Instance.userWeaponInfos.FirstOrDefault();        
            }

            ResourceManager.Instance.LoadWeaponModel(weaponInfo, () =>
            {
                SetUp(weaponInfo);
            });

        });
    }


    public void SetUp(WeaponInfo weaponInfo)
    {
        weaponTF.ParentTransInitialize();
        var weaponObj = Instantiate(ResourceManager.Instance.GetWeaponModel(weaponInfo.weaponMaster.id), weaponTF);

        weaponObj.GetComponent<Renderer>().material.shader = Shader.Find("UnityChan/Eye");

        nameText.text = weaponInfo.weaponMaster.weaponName;
        lvText.text = "Lv." + weaponInfo.lv + "/" + 3;
        explanationText.text = weaponInfo.weaponMaster.description;

        SeStatus(weaponInfo);

        weaponTF.UIRoateRoop(5f);

    }



    public void SeStatus(WeaponInfo weaponInfo)
    {
        var master = weaponInfo.weaponMaster;

        nameText.text = master.weaponName;
        explanationText.text = master.description;
        lvText.text = "Lv." + weaponInfo.lv + "/" + GameSettingData.WeaponMaxLv;
        radarChart.m_Data = new List<float>();
        radarChart.statusValues = new List<string>();
        radarChart.m_Data.Add(master.physical_attack / PAbasis);
        radarChart.statusValues.Add(master.physical_attack.ToString());
        radarChart.m_Data.Add(master.physical_defence / PDbasis);
        radarChart.statusValues.Add(master.physical_defence.ToString());
        radarChart.m_Data.Add(master.magic_attack / MAbasis);
        radarChart.statusValues.Add(master.magic_attack.ToString());
        radarChart.m_Data.Add(master.magic_defence / MDbasis);
        radarChart.statusValues.Add(master.magic_defence.ToString());
        radarChart.m_Data.Add(master.hp / HPbasis);
        radarChart.statusValues.Add(master.hp.ToString());
        radarChart.m_Data.Add(master.mp / MPbasis);
        radarChart.statusValues.Add(master.mp.ToString());
        radarChart.m_Data.Add(master.movement / MVbasis);
        radarChart.statusValues.Add(master.movement.ToString());
        radarChart.m_Data.Add(master.cri / CRbasis);
        radarChart.statusValues.Add(master.cri.ToString());

        radarChart.m_Data.Add(0.5f);

        radarChart.Init();
    }

    bool moving;
    public void SetMenu()
    {
        if (moving) return;

        moving = true;

        var nextMenu = powerUpMenuType == PowerUpMenuType.Status ? PowerUpMenuType.PowerUp : PowerUpMenuType.Status;

        UI.UIcutOut(menuTypeObject[powerUpMenuType].transform, ajustX: 1300, ajustY: 0, callBack: null);
        UI.UIcutin(menuTypeObject[nextMenu].transform, ajustX: 1300, ajustY: 0, callBack: () => moving = false);

        powerUpMenuType = nextMenu;
    }


    public void SceneClose()
    {
        WindSceneClose(SceneType.WeaponSetting);
    }
}
