using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Explorior;
using System.Linq;

public class FreeHomeManager : ExploriorSceneManager
{
    private List<int> charaids = new List<int>();

    private readonly int questStuffCharaId = 12;
    private readonly int charaSettingStuffCharaId = 14;
    [SerializeField] Transform questStuffCharaTrans;
    [SerializeField] Transform charaSettingStuffTrans;
    [SerializeField] Transform itemSettingBoxTrans;
    [SerializeField] SelectMenu selectMenu;
    [SerializeField] PlayerCharacterSytem playerCharacterSytem;

    // Start is called before the first frame update
    void Start()
    {
        Loding(() =>
        {
            SetUp();
        });

    }

    // Update is called once per frame
    void SetUp()
    {
        selectMenu.Initialized(SceneActive);
        charaids.Add(questStuffCharaId);
        charaids.Add(charaSettingStuffCharaId);
        charaids.Add(DataManager.Instance.userTeamCharaInfos(0).First().id);
        ResourceManager.Instance.LoadCharaModels(charaids, () =>
        {
            playerCharacterSytem.Initialized();
            StuffInit(questStuffCharaTrans, questStuffCharaId);
            StuffInit(charaSettingStuffTrans, charaSettingStuffCharaId);
            StuffInit(itemSettingBoxTrans);
        });
    }

    public void StuffInit( Transform parentTF, int id = -1 )
    {
        if(id != -1)
        {
            //キャラがいる場合
            var ckack = parentTF.Find("kanban");
            foreach (Transform Value in parentTF)
            {
                if (Value != ckack)
                    Destroy(Value.gameObject);
            }

            GameObject model = Instantiate(ResourceManager.Instance.GetCharaModel(id), parentTF);
            model.transform.localPosition = Vector3.zero;
            var characterController = model.AddComponent<CharacterController>();


            characterController.CharacterControllerInit();
            model.name = id.ToString();
            ckack.parent = model.transform;
        }


        var colEve = parentTF.GetComponent<ColEventFunction>();
        colEve.colEvenFuncEnterAction = (col) =>
        {
            selectMenu.SetButton(col.eventType);
        };

        colEve.colEvenFuncExitAction = (col) =>
        {
            selectMenu.OutButton(col.eventType);
        };

    }

    public void SceneActive(bool isActive)
    {
        CameraActive(isActive);

        if(isActive)
            playerCharacterSytem.playerCharaController.Initialize();
    }

    public void MobileSet(Vector2 vector2)
    {
        if (playerCharacterSytem.playerCharaController != null)
        {
            playerCharacterSytem.playerCharaController.MobileSet(vector2);
        }
    }
}
