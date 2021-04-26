using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CroseumDataSetting : MonoBehaviour {

    private static Entity_CoroseumSetting coroseumSetting;

    public static RealCroseumData GetRealDropItemData(int dropId)
    {
        coroseumSetting = Resources.Load("Data/CoroseumSetting") as Entity_CoroseumSetting; //=> Resourcesからデータファイルの読み込み

        //      Debug.Log (dropItemSetting + "  masakakoremo??");
        RealCroseumData returnValue = new RealCroseumData();

        int count = GetQuestNumber(dropId);

        returnValue.no = coroseumSetting.param[count].No;
        returnValue.stageName = coroseumSetting.param[count].stageName;
        returnValue.stageId = coroseumSetting.param[count].stageId;
        returnValue.questId = coroseumSetting.param[count].questId;
        returnValue.questName = coroseumSetting.param[count].questName;
        returnValue.enemyLv = coroseumSetting.param[count].enemyLv;
        returnValue.enemyRare = coroseumSetting.param[count].enemy1;
        returnValue.dropLv = coroseumSetting.param[count].dropLV1;
        returnValue.bossLv = coroseumSetting.param[count].bossLv;
        returnValue.dropLvBoss = coroseumSetting.param[count].dropLV4;


        return returnValue;
    }


    public static int GetQuestNumber(int corId)
    {
        coroseumSetting = Resources.Load("Data/CoroseumSetting") as Entity_CoroseumSetting; //=> Resourcesからデータファイルの読み込み
        int roopCount = coroseumSetting.param.Count;

        for (int i = 0; i < roopCount; i++)
        {
            if (coroseumSetting.param[i].No == corId)
            {
                return i;
            }
        }
        return 0;
    }

}
