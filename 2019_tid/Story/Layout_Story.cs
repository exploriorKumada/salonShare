using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Layout_Story : ScenePrefab {

	private StorySetting storySetting;



	private int lineCount = 0;

    private double beforeBGid = 0;

    public static int storyID = 1;
    private Entity_QuestSetting questSetting;

    [SerializeField] SpriteRenderer backGround;
    [SerializeField] TextMesh textValue;
    [SerializeField] TextMesh nameValue;
    [SerializeField] GameObject baseObject;
    [SerializeField] Transform parentTF;

    [SerializeField] GameObject centerGO;

    Dictionary<string, Sprite> charaImages = new Dictionary<string, Sprite>();
    Dictionary<int, Sprite> backGroundImages = new Dictionary<int, Sprite>();

    public static string backScene= "";

    void Start () {


        StartCoroutine(StartSet());
	}

    public IEnumerator StartSet()
    {
        iphoneXAjust(centerGO);
        storySetting = Resources.Load("Data/StorySetting") as StorySetting; //=> Resourcesからデータファイルの読み込み

        Debug.Log("storyID:" + storyID);
        //Debug.Log(StorySettingBase.GetStartStoryNumber(storyID) + " この行で始まる");
        //Debug.Log(StorySettingBase.GetEndStoryNumber(storyID) + " この行で終わる");
        AddPopup("Popup_AlphaLoding");
        BackGroundInstall(() =>
        {
            //キャラ画像インストール
            CharImageInstall(() =>
            {
                AlphaLoding.Close();
                lineCount = StorySettingBase.GetStartStoryNumber(storyID);
                SetOneLineImage(lineCount);
            });

          
        });
      
        yield break;

    }

    private void CharImageInstall(Action action)
    {
        if(StorySettingBase.GetImageNameList(storyID).Count==0)
            action();

        foreach (var Value in StorySettingBase.GetImageNameList(storyID))
        {
            ResourceLoaderOrigin.GetStandingImage(Value.charaImageName_data, Value.expression_data, (Sprite obj) => {
                charaImages[Value.charaImageName_data + Value.expression_data] = obj;
                if (Value.endFlag)
                {
                    Debug.Log("全立ち絵画像インスト完了");
                    BgmManager.Instance.Stop();
                    action();

                }
            });
        }

    }

    private void BackGroundInstall(Action action)
    {
        var list = StorySettingBase.GetBackGroundIDList(storyID);
        foreach (var Value in list)
        {
            int valueCopy = Value;
            ResourceLoaderOrigin.GetBackGroundImageForStory(Value, (bgid, bgobj) =>
            {
                Debug.Log("何いいいいいいい:" + bgid + " : " + list.Last());
                backGroundImages[bgid] = bgobj;
                if(bgid == list.Last())
                {
                    Debug.Log("全背景画像インスト完了");
                    action();
                }
            });
        }
    }

    public void PushEvent()
    {
        SetSE(20, true);
        if (!blackAnimaProcces)
            SetOneLineImage(lineCount);
    }

    double backGroundId = 1;
    int bgmId = 1;
    private void SetOneLineImage( int lineCountSet )
	{

        if ( lineCount == StorySettingBase.GetEndStoryNumber( storyID ))
		{
			EndAction();
			return;	
		}

       
        lineCount++;

        for ( int i=0; i < parentTF.childCount; ++i ){
			GameObject.Destroy( parentTF.GetChild( i ).gameObject );
		}



        bgmId = storySetting.param[lineCountSet].bgm;

        if(bgmId==-1)
        {
            BgmManager.Instance.Stop();               
        }else if( bgmId != 0)
        {
            Debug.Log("bgmId:" + bgmId + " in " + lineCountSet);
            BgmManager.Instance.Play(ResourceLoaderOrigin.GetBGMnameById(bgmId));
        }

        backGroundId = storySetting.param[lineCountSet].backGround;
        if (storySetting.param[lineCountSet].backGround != 0)
        {
           
            if (backGroundId != beforeBGid && lineCount != StorySettingBase.GetStartStoryNumber(storyID)+1)
            {
                //背景変わったので、暗転処理
                Debug.Log("背景変わったので、暗転処理 backGroundId:" + backGroundId + " beforeBGid:" + beforeBGid);
                Debug.Log(lineCount + ":"+(StorySettingBase.GetStartStoryNumber(storyID) + 1));
                BlackAnima(()=>
                {
                  
                    SetInfo(lineCountSet);         
                    beforeBGid = backGroundId;               
                });
                return;
            }
            beforeBGid = backGroundId;
        }else
        {
            backGroundId = beforeBGid;
        }


        SetInfo(lineCountSet);
    }

    public void SetInfo(int lineCountSet)
    {
        var newGO = Instantiate(baseObject, parentTF);
        Transform newGoTF = newGO.transform;

        newGoTF.localPosition = new Vector3(newGoTF.localPosition.x, newGoTF.localPosition.y, newGoTF.localPosition.z + 10);

        string charaImageName = storySetting.param[lineCountSet].charaImageName;
        string charaExpression = storySetting.param[lineCountSet].expression;
        //Debug.Log(lineCountSet + ":再生");
        if (string.IsNullOrEmpty(charaImageName))
        {
            newGoTF.Find("animator/charaImage").gameObject.SetActive(false);
        }
        else
        {
            newGoTF.Find("animator/charaImage").GetComponent<Image>().sprite = charaImages[charaImageName+charaExpression];
            newGoTF.Find("animator/charaImage").gameObject.SetActive(true);
        }
        newGO.SetActive(true);
        baseObject.SetActive(false);

        if(backGroundImages.ContainsKey((int)backGroundId))
        {
            backGround.sprite = backGroundImages[(int)backGroundId];
        }
        else
        {
            Debug.LogError("back Groud error :"+ (int)backGroundId);
            backGround.sprite = backGroundImages.First().Value;
        }
     

        textValue.text = storySetting.param[lineCountSet].storyValue;
        nameValue.text = storySetting.param[lineCountSet].charaName;
    }



    public void EndAction()
	{
        if(!string.IsNullOrEmpty(backScene))
        {
            ChangeLayout(backScene);
            backScene = string.Empty;
            return;
        }
		BattleManager.questId = storyID;
		ChangeLayout ("Battle");
	}

}
