using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Lauout_Info : ScenePrefab {
    
	[SerializeField] GameObject baseObject;
	[SerializeField] Transform parentTF;
	[SerializeField] GameObject ContentGO;
    [SerializeField] SpriteRenderer backGround;

    [SerializeField] GameObject nothingObject;
    [SerializeField] GameObject centerGO;


    public List<InfoController> infoControllers = new List<InfoController>();

	GameObject selectGo;

	// Use this for initialization
	void Start () {
     
        StartCoroutine(SetStart()); 
	}


    public IEnumerator SetStart()
    {      
        AddPopup("Popup_AlphaLoding");
        iphoneXAjust(centerGO);
        InformationSetting.LoadReadInfomationData(UserData.GetUserID(), 1, () =>
        {
            ResourceLoaderOrigin.GetBackGroundImage(1, (bgobj) =>
            {
                backGround.sprite = bgobj;
                AlphaLoding.Close();
                AddSubLayout("Footer");
                AddSubLayout("Header");
                SetImage(InformationSetting.returnValueList);
            });
              
        });

        yield break;
    }


    private void SetImage(List<RealInfomationData> returnValueList )
	{
       
		int count = 0;
        Debug.Log( "RealInfomationDataCount:" +returnValueList.Count );


        if( returnValueList.Count == 0 )
        {
            nothingObject.SetActive(true);
        }
        foreach( var Value in returnValueList )
        {
            var newGO = GameObject.Instantiate(baseObject);
            newGO.transform.parent = parentTF;
            var newTF = newGO.transform;
            newGO.SetActive(true);
            iphoneXAjust(newGO);

            infoControllers.Add(newGO.GetComponent<InfoController>() );
            infoControllers[count].realInfomationData = Value;
			infoControllers[count].Init();

			count++;
        }
		baseObject.SetActive(false);

		//ContentGO.GetComponent<VerticalLayoutGroup> ().spacing = 0;

	} 
		
}
