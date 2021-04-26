using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InfoController : ScenePrefab {

    [SerializeField] TextMeshProUGUI titleText;
    public RealInfomationData realInfomationData;

    [SerializeField] GameObject newText;


    public void Init()
    {
        titleText.text = realInfomationData.title;

        Debug.Log("realInfomationData.readFlag  " + realInfomationData.readFlag);
        //２が既読
        if( realInfomationData.publishData != null )
        {
            newText.SetActive(true);
            newText.GetComponent<TextMeshProUGUI>().text = realInfomationData.publishData.ToString();
        }else
        {
            newText.SetActive(false);
        }


    }

    public void PushEvent()
    {

        InformationSetting.SetInfomationData(realInfomationData, () =>
        {
            Popup_InfoValue.title = realInfomationData.title;
            Popup_InfoValue.infoValue = realInfomationData.body;
            AddPopup("InfoValue");

        });


    }
}
