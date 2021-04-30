using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TW.GameSetting;

public class SkinDataSetting : MonoBehaviour
{
    public static List<SkinData> SkinDatas()
    {
        List<SkinData> returnValue = new List<SkinData>();

        for(int i = 1;i<=65;i++)
        {
            returnValue.Add(new SkinData() { id = i, skinType = SkinType.Human });
        }

        return returnValue;

    }

    public static GameObject GetSkinObj(int id, bool isGet)
    {
        GameObject returnValue = null;
        SkinData rewardData = SkinDatas().FirstOrDefault(x => x.id == id);

        if(rewardData.skinType == SkinType.Human)
        {
            returnValue = Resources.Load<GameObject>("Skin/Object/Stickman_heads_sphere");
        }

        returnValue.GetComponent<StickManManager>().skinnedMeshRenderer.material = GetMaterialPath(id, isGet);

        return returnValue;
    }

    public static Material GetMaterialPath(int id,bool isGet)
    {
        string materialPath = "Skin/Material/";

        if (isGet)
        {
            materialPath += id.ToString();
        }
        else
        {
            materialPath += "-1";
        }

        return Resources.Load<Material>(materialPath); ;
    }

    public static bool IsAllGet => SkinDatas().Count == ES3.Load("gettingIds", new List<int>() { 1 }).Count();

}

public class SkinData
{
    public int id;
    public SkinType skinType;
}