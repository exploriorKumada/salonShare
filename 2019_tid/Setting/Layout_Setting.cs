using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Layout_Setting : ScenePrefab {

    [SerializeField] TextMeshProUGUI bgmText;
    [SerializeField] GameObject centerGO;
    [SerializeField] Image bg;



    // Use this for initialization
    void Start () {

        AddPopup("Popup_AlphaLoding");
        AddSubLayout("Footer");
        AddSubLayout("Header");

        ResourceLoaderOrigin.GetBackGroundImage(15, (bgobj) =>
        {
            bg.sprite = bgobj;
            BgmManager.Instance.Play(ResourceLoaderOrigin.GetBGMnameById(7));
            iphoneXAjust(centerGO);
            AlphaLoding.Close();
        });


        if(UserData.GetMUTOFlag())
        {
            bgmText.text = "SOUND/ON";
        }else
        {
            bgmText.text = "SOUND/OFF";
        }
         
	}


    public void PushEvent( int caseNumber )
    {
        if( caseNumber == 1 )
        {
            Debug.Log("Twitter");
            Application.OpenURL("https://twitter.com/tid_exp");

        }else if( caseNumber == 2 )
        {
            Debug.Log("ヘルプシーン表示");
        }else if (caseNumber == 3 )
        {
            AddPopup("Assumption");
            Debug.Log("引継ぎ画面表示する");
        }else if ( caseNumber == 4)
        {
            Debug.Log("規約ポップアップ表示");
            Application.OpenURL("https://tidexp.jimdofree.com/%E5%88%A9%E7%94%A8%E8%A6%8F%E7%B4%84/");
            //AddPopup("Kiyaku");
        }else if ( caseNumber == 5 )
        {
            UniClipboard.Clipboard.Text = UserData.GetFriendID(); ;
            Application.OpenURL("https://tidexp.jimdofree.com/%E3%81%8A%E5%95%8F%E5%90%88%E3%81%9B/");

        }else if ( caseNumber == 6 )
        {
            Debug.Log("タイトル画面に飛ぶ");
            ChangeLayout("Title");
        }else if( caseNumber == 7)
        {
            Debug.Log("item");
            ChangeLayout("Item");
        }else if( caseNumber == 8)
        {
            bgmText.text = "SOUND/" +BgmManager.Instance.BGMMuto( !UserData.GetMUTOFlag() );
        }

        
    }


}
