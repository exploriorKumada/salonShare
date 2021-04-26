using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetSkillController : ScenePrefab {

    [System.NonSerialized]public int number = 0;
    [SerializeField] Layout_CharaSetting layout_CharaSetting;

    //public void ClickEvent()
    //{
    //    SetSelectNow(number);
    //}

    public void ClickEvent()
    {
        if (number == 0)
            return;
        
        layout_CharaSetting.SetSelectNow(number);
    }


	
}
