using System.Collections;
using System.Collections.Generic;
using TW.GameSetting;
using UnityEngine;

public class CalculationManager
{
    /// <summary>
    /// 上り率から
    /// </summary>
    public static int GetStatusValue
    (
        int statusMax,
        int statusMin,
        float rate,
        int Lv
    )
    {
        int returnValue = 0;
        int charaMaxLv = GameSettingData.CharaMaxLv;
        int charaMinLv = GameSettingData.CharaMinLv;

        int LvMaxHikuMin = charaMaxLv - charaMinLv;
        int StatusMaxHikuMin = statusMax - statusMin;
        float LvAjust = ((charaMaxLv * charaMaxLv) - (charaMinLv * charaMinLv)) * rate;
        float b = (StatusMaxHikuMin - LvAjust) / LvMaxHikuMin;
        float c = statusMin - rate - b;

        returnValue = (int)(rate * (Lv * Lv) + b * Lv + c);

        return returnValue;
    }

    public static float GetStatusValue
    (
        float statusMax2,
        float statusMin2,
        float rate,
        int Lv
    )
    {
        float returnValue = 0;
        int statusMax = (int)(statusMax2 * 100) + 1;
        int statusMin = (int)(statusMin2 * 100) + 1;
        int charaMaxLv = GameSettingData.CharaMaxLv;
        int charaMinLv = GameSettingData.CharaMinLv;

        int LvMaxHikuMin = charaMaxLv - charaMinLv;
        float StatusMaxHikuMin = statusMax - statusMin;
        float LvAjust = ((charaMaxLv * charaMaxLv) - (charaMinLv * charaMinLv)) * rate;
        float b = (StatusMaxHikuMin - LvAjust) / LvMaxHikuMin;
        float c = statusMin - rate - b;

        returnValue = (int)(rate * (Lv * Lv) + b * Lv + c);

        return returnValue / 100;
    }



    public static float GetBaiType(int a, int b)
    {
        if (a == 1)
        {
            if (a == 1)
                return 1f;
            if (b == 2)
                return 0.8f;
            if (b == 3)
                return 1.2f;
        }
        else if (a == 2)
        {
            if (a == 1)
                return 1.2f;
            if (b == 2)
                return 1f;
            if (b == 3)
                return 0.8f;
        }
        else if (a == 3)
        {
            if (a == 1)
                return 0.8f;
            if (b == 2)
                return 1.2f;
            if (b == 3)
                return 1f;
        }
        return 1f;
    }
}
