using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleCharaDataSetting : MonoBehaviour {

    public static RealCharaData CreateData
    (
        int charaIdNumber,
        int attack,
        float gaurd,
        float cri
    )
    {
        RealCharaData realCharaData = new RealCharaData();
        realCharaData.charaIdNumber = charaIdNumber; 
        realCharaData.realAttack = attack;
        realCharaData.realGuard = gaurd;
        realCharaData.cri = cri;
        return realCharaData;
    }


}
