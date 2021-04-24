using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Explorior;
using TW.GameSetting;
using System.Linq;
using System;
using UnityEngine.SceneManagement;

public class SelectMenu : ExploriorSceneManager
{
    Dictionary<ColEventType, string> toriaezu = new Dictionary<ColEventType, string>();

    [SerializeField] QuestDialog questDialog;
    [SerializeField] GameObject buttonObject;
    [SerializeField] ItemListDialog itemListDialog;

    Dictionary<ColEventType, GameObject> buttons = new Dictionary<ColEventType, GameObject>();

    Action<bool> sceneActive;
        
    public void Initialized(Action<bool> _sceneActive)
    {
        sceneActive = _sceneActive;
        buttonObject.ParentInitialize();
    }

    public void　SetButton(ColEventType colEventType)
    {
        if(buttons.ContainsKey(colEventType))
        {
            Debug.LogWarning("消せてないのに追加しようとしてる:" + colEventType);
            return;
        }

        var button = Instantiate(buttonObject, buttonObject.transform.parent);
        var general = button.GetComponent<GeneralData>();
        var data = new Dictionary<string, object>();
        switch (colEventType)
        {
            case ColEventType.Quest:
                general.textMeshProUGUIs.First().Value.text = "クエスト";
                general.buttons.First().Value.onClick.AddListener( ()=>
                {
                    questDialog.Initialize();
                });
                break;
            case ColEventType.CharaSetting:
                general.textMeshProUGUIs.First().Value.text = "キャラ設定";
                general.buttons.First().Value.onClick.AddListener(() =>
                {
                    sceneActive(false);
                    ChangeScene(new ChangeSceneInfo()
                    {
                        sceneType = SceneType.CharaSetting,
                        fadeType = FadeType.Wind,
                        fadingAction = () => sceneActive(true),
                        loadSceneMode = LoadSceneMode.Additive,
                        data_hash = data
                    });
                });
                break;
            case ColEventType.ItemBox:

                general.textMeshProUGUIs.First().Value.text = "アイテム";
                general.buttons.First().Value.onClick.AddListener(() =>
                {
                    sceneActive(false);

                    ChangeScene(new ChangeSceneInfo()
                    {
                        sceneType = SceneType.ItemSettng,
                        fadeType = FadeType.Wind,
                        fadingAction = () => sceneActive(true),
                        loadSceneMode = LoadSceneMode.Additive,
                        data_hash = data
                    });
                    //itemListDialog.Initialize( ()=> sceneActive(false));
                });
                break;

        }

        button.SetActive(true);
        buttons.Add(colEventType, button);
    }


    public void OutButton(ColEventType colEventType)
    {
        Destroy(buttons[colEventType].gameObject);
        buttons.Remove(colEventType);
    }
}
