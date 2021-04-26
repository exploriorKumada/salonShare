using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RealItemData
{

    public int user_item_Id;
    public string name;
    public int typeId;
    public string description;
    public int rank;
    public float attack;
    public float guard;
    public float cri;
    public float repair;
    public int mate1id;
    public int mate1amount;
    public int mate2id;
    public int mate2amount;
    public int mate3id;
    public int mate3amount;
    public int selling;
    public int item_master_id;
    public int amount;

    public List<RealGouseiItemData> GetGouseiItems()
    {
        List<RealGouseiItemData> returnValue = new List<RealGouseiItemData>();

        if( mate1id != 0 )
        {
            returnValue.Add(new RealGouseiItemData()
            {
                itemId = mate1id,
                amount = mate1amount
            });
        }

        if (mate2id != 0)
        {
            returnValue.Add(new RealGouseiItemData()
            {
                itemId = mate2id,
                amount = mate2amount
            });
        }

        if (mate3id != 0)
        {
            returnValue.Add(new RealGouseiItemData()
            {
                itemId = mate3id,
                amount = mate3amount
            });
        }

        return returnValue;
    }
}
