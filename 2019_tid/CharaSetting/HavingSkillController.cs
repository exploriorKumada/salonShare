using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HavingSkillController : MonoBehaviour {

    [SerializeField] Layout_CharaSetting layout_CharaSetting;


    public RealActionData realActionData;
    public Button button;
    public int number = 1;

    public void ClickEvent()
    {
        Debug.Log("HavingSkillController??");
        layout_CharaSetting.SetSelect(layout_CharaSetting.selectNowNumber, number);
    }

    public void EnableClick( bool flag )
    {
        button.enabled = flag;
    }

}
