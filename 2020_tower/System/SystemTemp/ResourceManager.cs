using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PW;
using System;
using TW.GameSetting;
using System.Linq;

public class ResourceManager : NLSingletonDontDestroyObject<ResourceManager>
{
    [SerializeField] private static bool isLocal = true;

    //キャラ３Dモデル
    private Dictionary<int, GameObject> characterObjectsData = new Dictionary<int, GameObject>();

    //アイテム3Dモデル
    private Dictionary<int, GameObject> itemObjectsData = new Dictionary<int, GameObject>();

    //ウェポン3Dモデル
    private Dictionary<int, GameObject> weaponObjectsData = new Dictionary<int, GameObject>();

    //バトル用3Dモデル
    private Dictionary<string, GameObject> inGameObjectsData = new Dictionary<string, GameObject>();
    private List<string> loadIngameModel = new List<string>() { "SelfCastel", "summon" };

    public Transform globalObjectRoot;

    public void LoadCharaModel(int id,Action callBack = null)
    {
        GameObject go = null;

        if (characterObjectsData == null) characterObjectsData = new Dictionary<int, GameObject>();

        //含まれてれば
        if (characterObjectsData.ContainsKey(id))
        {
            callBack?.Invoke();
            return;
        }

        //ローカルから取得
        if (isLocal) go = Resources.Load<GameObject>("CharaModel/" + id);

        //nullじゃなければセット
        if (go==null)
            Debug.LogError("キャラモデルデータを取得できません。" + id);
        else
            characterObjectsData.Add(id, go);

        callBack?.Invoke();
    }

    public void LoadCharaModels(List<CharaInfo> charaInfos, Action callBack)
    {
        List<int> ids = new List<int>();
        charaInfos.ForEach( info => ids.Add(info.charaMaster.id));

        LoadCharaModels(ids, callBack);
    }

    public void LoadCharaModels(List<int> ids,Action callBack)
    {
        int count = 0;
        foreach (int id in ids)
        {
            count++;

            Action lastCallBack = (count == ids.Count) ? callBack : null;

            LoadCharaModel(id, lastCallBack);
        }
    }

    public GameObject GetCharaModel(int id)
    {
        if(characterObjectsData.ContainsKey(id))
        {
            return characterObjectsData[id];
        }else
        {
            string ids = string.Empty;            
            foreach (var KV in characterObjectsData) ids+= KV.Key + ",";

            Debug.LogError("GetCharaModel is not Contained: Id is " + id + ": ids are "+ids);
            return null;
        }
    }

    public void LoadItemModel(ItemInfo itemInfo, Action callBack = null)
    {
        GameObject itemModel = null;

        var master = itemInfo.itemMaster;

        //含まれてればしない
        if (itemObjectsData.ContainsKey(master.id))
        {
            callBack?.Invoke();
            return;
        }

        //ローカルから取得
        if (isLocal) itemModel = Resources.Load<GameObject>("Item/" + master.id);

        //nullじゃなければセット
        if (itemModel == null)
            Debug.LogError("アイテムモデルデータを取得できません。" + master.id);
        else
            itemObjectsData.Add(master.id, itemModel);

        callBack?.Invoke();
    }

    public void LoadWeaponModel(WeaponInfo weaponInfo, Action callBack = null)
    {
        GameObject weaponModel = null;

        var master = weaponInfo.weaponMaster;

        //含まれてればしない
        if (weaponObjectsData.ContainsKey(master.id))
        {
            callBack?.Invoke();
            return;
        }

        //ローカルから取得
        if (isLocal) weaponModel = Resources.Load<GameObject>("Weapon/"+ master.type.ToString()+ master.id);

        //nullじゃなければセット
        if (weaponModel == null)
            Debug.LogError("武器モデルデータを取得できません。" + master.id);
        else
            weaponObjectsData.Add(master.id, weaponModel);

        callBack?.Invoke();
    }


    public void LoadItemModels(List<ItemInfo> itemInfos, Action callBack)
    {
        if (itemInfos.Count == 0) callBack();

        foreach (var itemInfo in itemInfos)
        {
            Action lastCallBack = (itemInfo == itemInfos.Last()) ? callBack : null;

            LoadItemModel(itemInfo, lastCallBack);
        }
    }


    public void LoadWeaponModels(List<WeaponInfo> weaponInfos, Action callBack)
    {
        if (weaponInfos.Count == 0) callBack();

        foreach (var weaponInfo in weaponInfos)
        {
            Action lastCallBack = (weaponInfo == weaponInfos.Last()) ? callBack : null;

            LoadWeaponModel(weaponInfo, lastCallBack);
        }
    }

    public GameObject GetItemModel(int id)
    {
        if (itemObjectsData.ContainsKey(id))
        {
            return itemObjectsData[id];
        }
        else
        {
            string ids = string.Empty;
            foreach (var KV in itemObjectsData) ids += KV.Key + ",";

            Debug.LogError("GetWeaponModel is not Contained: Id is " + id + ": ids are " + ids);
            return null;
        }
    }

    public GameObject GetWeaponModel(int id)
    {
        if(weaponObjectsData.ContainsKey(id))
        {
            return weaponObjectsData[id];
        }else
        {
            string ids = string.Empty;
            foreach (var KV in weaponObjectsData) ids += KV.Key + ",";

            Debug.LogError("GetWeaponModel is not Contained: Id is " + id + ": ids are " + ids);
            return null;
        }
    }


    public GameObject GetInGameModel(string id)
    {
        if (inGameObjectsData.ContainsKey(id))
        {
            return inGameObjectsData[id];
        }
        else
        {
            Debug.LogError("GetCharaModel is not Contained: Id is " + id);
            return null;
        }
    }

    public void LoadInGameModel(Action callBack)
    {
        GameObject go = null;

        if (inGameObjectsData == null) inGameObjectsData = new Dictionary<string, GameObject>();

        foreach (var Value in loadIngameModel)
        {
            //ローカルから取得
            if (isLocal) go = Resources.Load<GameObject>("Effect/" + Value);

            //nullじゃなければセット
            if (go == null)
                Debug.LogError("InGameモデルデータを取得できません。" + Value);
            else
                inGameObjectsData.Add(Value, go);
        }

        callBack?.Invoke();
    }



    public GameObject SetEffect(string effectName,GameObject owner, Transform parentTF,float scale = 1f)
    {
        GameObject ef = null;
        var effectMaster = DataManager.Instance.GetEffectInfo(effectName);

        switch(effectMaster.effrctRangeType)
        {
            case 1:

                break;

            case 2:
                ef = Instantiate(Resources.Load<GameObject>("Effect/" + effectMaster.name), parentTF);
                ef.transform.position = new Vector3(owner.transform.position.x, ef.transform.position.y, owner.transform.position.z);
                ef.transform.localScale = ef.transform.localScale * scale;
                //Debug.Log(owner.name + ":" + owner.transform.localPosition);
                break;
               

        }

        StopEffect(ef.GetComponent<ParticleSystem>(), effectMaster.destroySecond);

        return ef;

    }

    //public void SetEffect(int effectID, Transform parentTF)
    //{
    //    SetEffect((EffectType)Enum.ToObject(typeof(EffectType), effectID), parentTF);
    //}


    public Sprite GetCardImage(int id)
    {
        return Resources.Load<Sprite>("Image/Card/" + id);
    }

}
