using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;

public class Popup_ItemRecipe : MonoBehaviour {
    
    [SerializeField] GameObject baseObject;
    [SerializeField] Transform parentTF;

    public Dictionary<int, RealItemData> realItemMasterDatas = new Dictionary<int, RealItemData>();
    bool loaded = false;

    public void Initialize(Action action)
    {
        if(loaded)
        {
            action();
            return;
        }

        loaded = true;
        gameObject.SetActive(true);

        Dictionary<RealItemData, int> dic = new Dictionary<RealItemData, int>();
        realItemMasterDatas = ItemAPISetting.realItemMasterDatas;


        foreach (var KV in realItemMasterDatas)
        {
            if (KV.Value.mate1id == 0)
                continue;
            
            dic[KV.Value] = KV.Value.rank;
        }
         
        List<RealItemData> sortedItemRecip = new List<RealItemData>();
        var sorted = dic.OrderBy((x) => x.Value).ToList();  //降順

        foreach (var KV in sorted)
            sortedItemRecip.Add(KV.Key);
         
        SetImage(sortedItemRecip,action);

    }

    public void SetImage(List<RealItemData> sortedItemRecip,Action action)
    {
        StartCoroutine(SetImageOn(sortedItemRecip,action));
    }
    public IEnumerator SetImageOn(List<RealItemData> sortedItemRecip,Action action)
    {
        baseObject.SetActive(false);
        //今あるやつ全部消す
        for (int i = 0; i < parentTF.childCount; ++i)
        {
            if (parentTF.GetChild(i).gameObject == baseObject)
                continue;

            Destroy(parentTF.GetChild(i).gameObject);
        }

        foreach( var Value in sortedItemRecip)
        {
            var newGO = Instantiate(baseObject, parentTF);
            newGO.SetActive(true);
            newGO.GetComponent<ItemRecipeController>().Init(Value);
            yield return null;
        }
        gameObject.SetActive(false);
        action();
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }
}
