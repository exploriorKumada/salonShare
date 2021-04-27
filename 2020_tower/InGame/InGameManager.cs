using System;
using System.Collections.Generic;
using System.Linq;
using TW.GameSetting;
using UniRx;
using UnityEngine;

public class InGameManager : CharaControllBaseManager
{
    List<CharaInfo> teamChara = new List<CharaInfo>();

    [SerializeField] Transform enemyCharaParent;
    [SerializeField] BattleUI battleUI;
    [SerializeField] public StageController stageController;
    [SerializeField] public CastleController castleController;
    [SerializeField] public Transform grobalObjectRoot;
    [SerializeField] CameraFilterPack_AAA_Blood_Hit bloodHot;
    [SerializeField] NPCSystem nPCSystem;

    int wave = 1;
    int selectParty = 1;
    [NonSerialized]public QuestDetailInfo questDetailInfo;

    private void Start()
    {

        Loding(() =>
        {       
            if (DataManager.Instance.currentChangeSceneInfo != null )
            {
                Debug.Log("実データモード");
                questDetailInfo = (QuestDetailInfo)DataManager.Instance.currentChangeSceneInfo.data_hash["questInfo"];
                ResouceSetUp();
            }
            else
            {
                Debug.Log("開発データモード");
                //開発モード
                questDetailInfo = DataManager.Instance.QuestMasters.First().questDetailInfos.First();

                Dictionary<string, object> questStartData = new Dictionary<string, object>();
                questStartData.Add("team_id", 1);
                questStartData.Add("quest_id", questDetailInfo.questId);
                questStartData.Add("quest_detail_id", questDetailInfo.questDetailId);

                DataManager.Instance.QuestStart(questStartData, () =>
                {
                    ResouceSetUp();
                });
            }   
        });
   
    }

    private void ResouceSetUp()
    {
        teamChara = DataManager.Instance.userTeamCharaInfos(selectParty);
        ResourceManager.Instance.globalObjectRoot = grobalObjectRoot;

        var loadCharaData = new List<CharaInfo>();
        questDetailInfo.questWaveInfos.ForEach(x=> loadCharaData.Add(x.charaInfo));
        loadCharaData.AddRange(teamChara);

        ResourceManager.Instance.LoadCharaModels(loadCharaData, () =>
        {
            ResourceManager.Instance.LoadInGameModel(() =>
            {
                Initialized();
            });
        });
    }

    public void Initialized()
    {
        moveButtonUIBasePosi = moveButtonUI.localPosition;
        castleController.Initialized(ResultChack);

        nPCSystem.Initialized(this);
        PlayerCharaInitialize();

        battleUI.Initialize(teamChara, castleController, playerCharaController);
        CharaControllBaseManagerInit();
    }

    public void PlayerCharaInitialize()
    {
        foreach (Transform Value in playerCharaParent)
            Destroy(Value.gameObject);

        CharaInfo charaInfo = teamChara.First();
        GameObject playerModel = ResourceManager.Instance.GetCharaModel(charaInfo.charaMaster.id);
        GameObject player = Instantiate(playerModel, playerCharaParent);

        playerCharaController = new PlayerCharaController();
        playerCharaController = player.AddComponent<PlayerCharaController>();
        playerCharaController.charaInfo = charaInfo;
        playerCharaController.camera = camera;
        playerCharaController.Initialize();
        playerCharaController.charaInfo.hp = playerCharaController.charaInfo.hp * 10;

        cameraTaget.transform.parent = playerCharaController.transform;
        cameraTaget.transform.localPosition = new Vector3(0, 1, 0);
        playerCharaController.transform.localPosition = stageController.SetPosition(UnityEngine.Random.Range(1, 3));

    }

    public void MobileSet(Vector2 vector2)
    {
        if(playerCharaController !=null)
        {
            playerCharaController.MobileSet(vector2);
        }
    }


    public void ResultChack()
    {       
        //城負けチェック
        if(castleController.charaInfo.hp <= 0 )
        {
            Lose();
            return;
        }

        //敵全滅チェック
        if (nPCSystem.npcControllers.Where(n => n.charaInfo.hp <= 0).Count() == nPCSystem.npcControllers.Count())
        {
            Debug.Log("敵全滅");
            GoToRersult();

        }
    }


    public void Lose()
    {
        Debug.Log("お前の負けぇ");
        bloodHot.enabled = true;
        bloodHot.LightReflect = 0.5f;
        ChangeScene(SceneType.FreeHome);
    }

    public void GoToRersult()
    {
        Dictionary<string, object> questStartData = new Dictionary<string, object>();
        questStartData.Add("team_id", selectParty);
        questStartData.Add("quest_id", questDetailInfo.questId);
        questStartData.Add("quest_detail_id", questDetailInfo.questDetailId);
        DataManager.Instance.QuestFinish(questStartData, (resultInfo) =>
        {
            Dictionary<string, object> resultInfoData = new Dictionary<string, object>();
            resultInfoData.Add("resultInfo", resultInfo);

            ChangeScene(new ChangeSceneInfo()
            {
                sceneType = SceneType.InGameResult,
                data_hash = resultInfoData
            });
        });
    }
}
