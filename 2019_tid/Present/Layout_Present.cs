using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Layout_Present : ScenePrefab {

	[SerializeField] GameObject baseObject;
	[SerializeField] Transform parentTF;
    [SerializeField] GameObject Scroll1Object;
    [SerializeField] GameObject Scroll2Object;
    [SerializeField] GameObject allGetButton;

    [SerializeField] GameObject baseRirekiObject;
    [SerializeField] Transform parenRirekitTF;
    [SerializeField] SpriteRenderer backGround;

	[SerializeField] GameObject ContentGO;

    [SerializeField] GameObject nothingObject;
    [SerializeField] GameObject nothingRirekiObject;

    [SerializeField] TextMeshProUGUI buttonText;

    [SerializeField] GameObject centerGO;

    int presentCOunt = 0;

	// Use this for initialization
	void Start () {

		AddSubLayout("Footer");
		AddSubLayout("Header");
        StartCoroutine(SetStart());

	}

    public IEnumerator SetStart()
    {
        iphoneXAjust(centerGO);
        baseObject.SetActive(false);
        AddPopup("Popup_Loding");
        SetImageON();
        yield break;
    }

    public void SetImageON()
    {
       
        PresentSetting.LoadRealPresentData(UserData.GetUserID(), () =>
        {
            ResourceLoaderOrigin.GetBackGroundImage(1, (bgobj) =>
            {
                backGround.sprite = bgobj;
                Loading.Close();
                SetImage(PresentSetting.returnValueList, PresentSetting.returnRirekiValueList);
            });
               
        });
    }

    private void SetImage( List<RealPresentData> returnValueList, List<RealPresentData> returnRirekiValueList )
	{
    
        //今あるやつ全部消す
        for (int i = 0; i < parentTF.childCount; ++i)
        {
            GameObject.Destroy(parentTF.GetChild(i).gameObject);
        }

        presentCOunt = returnValueList.Count;
;        if (presentCOunt == 0 )
        {
            Debug.Log("プレゼントはないです");
            nothingObject.SetActive(true);
        }
        else
        {
            nothingObject.SetActive(false);

        }

        allGetButton.SetActive(presentCOunt > 0);
        foreach ( var Value in returnValueList )
		{
			var newGO = Instantiate(baseObject,parentTF);
			var newTF = newGO.transform;
            newGO.SetActive(true);
            newTF.GetComponent<PresentController>().realPresentData = Value;
            newTF.GetComponent<PresentController>().Init();
            newGO.name = "" + Value.title;		
		}  



        //今あるやつ全部消す
        for (int i = 0; i < parenRirekitTF.childCount; ++i)
        {
            Destroy(parenRirekitTF.GetChild(i).gameObject);
        }

        nothingRirekiObject.SetActive((returnRirekiValueList.Count == 0));

        foreach ( var Value in returnRirekiValueList )
        {
            var newGO = Instantiate(baseRirekiObject,parenRirekitTF);
            var newTF = newGO.transform;
            newGO.SetActive(true);

            newTF.GetComponent<PresentController>().realPresentData = Value;
            newTF.GetComponent<PresentController>().Init();
            newGO.name = "" + Value.title;


        }  
		baseObject.SetActive(false);
        baseRirekiObject.SetActive(false);
		//ContentGO.GetComponent<VerticalLayoutGroup> ().spacing = 0;

	} 


    public void PushImageChange()
    {
        if( Scroll1Object.activeSelf == true )
        {
            Scroll1Object.SetActive(false);
            Scroll2Object.SetActive(true);
            allGetButton.SetActive(false);
            buttonText.text = "プレゼントBOX";
            Debug.Log("履歴画面");
        }else
        {
            Scroll1Object.SetActive(true);
            Scroll2Object.SetActive(false);
            allGetButton.SetActive(presentCOunt > 0);
            buttonText.text = "プレゼント履歴";
            Debug.Log("受け取り画面");
        }
        
    }


    public void AllGet()
    {
        AddPopup("Popup_Loding");
        PresentSetting.GetPresentAllOn(() =>
        {
            SetImageON();
            PopupGeneral.textValue = "全てのレゼントを受け取りました。";
            AddPopup("PopupGeneral");
        });
    }

}
