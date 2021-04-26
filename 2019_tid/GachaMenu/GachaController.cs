using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GachaController : MonoBehaviour {

    [SerializeField] Layout_GachaMenu layout_GachaMenu;

    public GachaGroupRealData gachaGroupRealData;

    public void Init( GachaGroupRealData gachaGroupRealData )
    {
        this.gachaGroupRealData = gachaGroupRealData;
    }

    public void PushEvent()
    {
        Debug.Log( "gahaID:" +gachaGroupRealData.gahaID);
        layout_GachaMenu.gachaId = (int)gachaGroupRealData.gahaID;
    }

}
