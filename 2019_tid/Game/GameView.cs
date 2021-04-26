using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;
using UnityEngine.UI;

public class GameView : ScenePrefab {

    [SerializeField] GameObject voiceCheackWindow;
    [SerializeField] GameObject voiceGo;

    [SerializeField] List<GameObject> screens;

    [SerializeField] ParticleManager particleManager;
    [SerializeField] Transform particleTF;

    [SerializeField] int playerCharaID;
    [SerializeField] int enemyID;
    [SerializeField] int playerCharaLV;
    [SerializeField] int enemyLV;
    RealCharaMasterData playerChara;
    RealCharaMasterData enemyChara;

    [SerializeField] GameObject centerGO;


    List<string> voiceKind = new List<string>()
    {
        "attack","guard","select","skill"
    };



    private void Start()
    {
        iphoneXAjust(centerGO);
        AddPopup("Popup_AlphaLoding");
        ResourceLoaderOrigin.GetEfectAll(() =>
        {
            AlphaLoding.Close();
        });
    }


    public void TestCheack( int caseNumber )
    {
        Debug.Log("osetennno???????/");

        //if (CharaAPISetting.realCharaMasterDatas.Any())
        //return;

     

        voiceCheackWindow.SetActive(true);

        for (int i = 0; i < voiceGo.transform.parent.childCount; ++i)
        {
            if(voiceGo.transform.parent.GetChild(i).gameObject!=voiceGo)
                Destroy(voiceGo.transform.parent.GetChild(i).gameObject);
        }

        switch(caseNumber)
        {
            case 1:
                VoiceSystem();
                break;
            case 2:
                EffectSystem();
                break;
            case 3:
                ActionSystem();
                break;
            case 4:
                BattleSimulationSystem();
                break;
            case 5:
                DebugStoryScene();
                break;

        }


        voiceGo.SetActive(false);
    }

    public void VoiceSystem()
    {
        foreach (var KV in CharaAPISetting.realCharaMasterDatas)
        {
            foreach (var Value in voiceKind)
            {

                var newGO = Instantiate(voiceGo, voiceGo.transform.parent);
                newGO.transform.Find("text").GetComponent<TextMeshProUGUI>().enabled = true;
                newGO.transform.Find("text").GetComponent<TextMeshProUGUI>().text = KV.Value.name + "/n" + Value;
                newGO.GetComponent<Button>().onClick.AddListener(() =>
                {
                    Singleton<SoundPlayer>.instance.CharaVoice(KV.Value.id, Value);
                });
                newGO.SetActive(true);
            }
        }
    }

    public void EffectSystem()
    {
        for(int i = 0;i<=32;i++)
        {
            var newGO = Instantiate(voiceGo, voiceGo.transform.parent);
            newGO.transform.Find("text").GetComponent<TextMeshProUGUI>().enabled = true;
            newGO.transform.Find("text").GetComponent<TextMeshProUGUI>().text = i.ToString();

            int icopy = i;
            newGO.GetComponent<Button>().onClick.AddListener(() =>
            {
                Debug.Log("particelId:"+icopy);
                for (int j = 0; j < particleTF.childCount; ++j)
                {
                  Destroy(particleTF.GetChild(j).gameObject);
                }
                particleManager.SetEffect(particleTF, icopy, true);
            });
            newGO.SetActive(true);
        }
    }

    public void ActionSystem()
    {
        string testText = "";
        foreach (var KV in CharaAPISetting.realCharaMasterDatas)
        {
            foreach(var KV2 in KV.Value.realActionDataDic)
            {
                var newGO = Instantiate(voiceGo, voiceGo.transform.parent);
                newGO.transform.Find("text").GetComponent<TextMeshProUGUI>().enabled = true;
                newGO.transform.Find("text").GetComponent<TextMeshProUGUI>().text = KV.Value.name + ":" + CharaAPISetting.GetExplane(KV2.Value,false);
                testText += KV.Value.name + ":" + CharaAPISetting.GetExplane(KV2.Value,false);
            }

            //testText += "/n";
        }
        Debug.Log(testText);
    }


    public void BattleSimulationSystem()
    {
        StartCoroutine(BattleSimulationSystemOn());
    }
    public IEnumerator BattleSimulationSystemOn()
    {
        playerChara = CharaAPISetting.GetRealcharaMasterDataByCharaId(playerCharaID);
        enemyChara = CharaAPISetting.GetRealcharaMasterDataByCharaId(enemyID);
        int palyerAttack = playerChara.GetRealAttack(playerCharaLV);
        int enemyAttack = enemyChara.GetRealAttack(enemyLV);
        int playerHP = playerChara.GetRealHP(playerCharaLV);
        int enemyHP = enemyChara.GetRealHP(enemyLV);

        Debug.Log("palyerAttack:" + palyerAttack + " playerHP:" + playerHP);
        Debug.Log("enemyAttack:" + enemyAttack + " enemyHP:" + enemyHP);
        int turn = 0;
        while (playerHP > 0 && enemyHP > 0)
        {
            turn++;
            enemyHP -= palyerAttack;
            playerHP -= enemyAttack;
            Debug.Log("enemyHP:" + enemyHP + " playerHP:" + playerHP);
            yield return null;
        }

        Debug.Log("プレイヤーの勝ちか＝＞" + (playerHP > 0) + "  :" + turn + "ターン");
        yield return null;
    }



    public void DebugStoryScene()
    {
        foreach( var Value in StorySettingBase.GetLengthStoryIDs())
        {
            if (Value >= 100)
                continue;

            //Debug.Log("Storyid:" + Value);
            var newGO = Instantiate(voiceGo, voiceGo.transform.parent);
            newGO.transform.Find("text").GetComponent<TextMeshProUGUI>().enabled = true;
            newGO.transform.Find("text").GetComponent<TextMeshProUGUI>().text = "storyid:" + Value.ToString();

            int icopy = Value;
            newGO.GetComponent<Button>().onClick.AddListener(() =>
            {
                Layout_Story.storyID = icopy;
                Layout_Story.backScene = "Game";
                ChangeLayout("Story");
            });
            newGO.SetActive(true);
        }
    }

}














