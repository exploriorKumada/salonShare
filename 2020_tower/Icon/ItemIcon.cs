using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TW.GameSetting;
using UnityEngine.SceneManagement;
using MText;

public class ItemIcon : SystemBaseManager
{
    [SerializeField] Transform weaponRoot;
    [SerializeField] public Modular3DText modular3DText;
    [SerializeField] MeshRenderer meshRenderer;

    Action pushAction;
    GameObject model;

    public void Initialize(ItemCommonInfo itemCommonInfo, Action action = null)
    {
        pushAction = action;

        if (model != null)
            Destroy(model.gameObject);

        //配下に武器モデルをおく
        switch(itemCommonInfo.itemType)
        {
            case ItemType.WeaponItem:
                model = Instantiate(ResourceManager.Instance.GetWeaponModel(itemCommonInfo.weaponInfo.weaponMaster.id), weaponRoot);
                // Tiling
                meshRenderer.material.SetTextureScale("_MainTex", new Vector2(1f, 1f));

                model.transform.localScale = Vector3.one * 3;

                break;
            case ItemType.ConsumeItem:
                model = Instantiate(ResourceManager.Instance.GetItemModel(itemCommonInfo.itemInfo.itemMaster.id), weaponRoot);
                modular3DText.gameObject.SetActive(true);
                modular3DText.UpdateText("×" + itemCommonInfo.itemInfo.number);
                // Tiling
                meshRenderer.material.SetTextureScale("_MainTex", new Vector2(-1f, 1f));

                //model.transform.localScale = Vector3.one * 2;

                break;
        }

        model.transform.localPosition = new Vector3(0, 1, 0);


    }

    public void ButtonAction()
    {
        pushAction?.Invoke();

        //var data = new Dictionary<string, object>();

        //Action fadingAction = () => pushAction();
        //data.Add("fading", fadingAction);

        //ChangeScene(new ChangeSceneInfo()
        //{
        //    sceneType = SceneType.WeaponSetting,
        //    fadeType = FadeType.Wind,
        //    fadingAction = () => pushAction(),
        //    loadSceneMode = LoadSceneMode.Additive,
        //    data_hash = data
        //});
    }
}
