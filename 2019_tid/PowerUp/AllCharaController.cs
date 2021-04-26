using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class AllCharaController : CharaInfoSystem
{

    [SerializeField] Layout_PowerUp layout_PowerUp;
    [SerializeField] Transform parentTF;
    [SerializeField] TeamCharaController teamCharaController;
    [SerializeField] Image baseImage;


    [SerializeField] GameObject go;
    [SerializeField] Transform parent;
    [SerializeField] Image charaImage;
    [SerializeField] Image typeImage;

    [SerializeField] GameObject nothingObject;

    [SerializeField] Image frameImage;

    [SerializeField] GameObject weaponObject;
    [SerializeField] Image weaponImage;

    Vector3 baseVector;
    public int countCopy;
    public bool partIn = false;

   
    public RealCharaData realCharaData;
    public bool move = false;

    public void Start()
    {
        baseVector = transform.localScale;
    }
    public void Init()
    {
        gameObject.SetActive(false);
        baseVector = transform.localScale;
        baseImage.color = Layout_PowerUp.colorCordList[realCharaData.charaTypeId].ToColor();
        ResourceLoaderOrigin.GetBattleCharaImage(realCharaData.charaIdNumber, (Sprite obj) => { 
            charaImage.sprite = obj;
            gameObject.SetActive(true);
            weaponObject.SetActive(false);
            if (realCharaData.item_master_id != 0)
            {
                ResourceLoaderOrigin.GetItemImage(realCharaData.item_master_id, (Sprite obj2) => {
                    weaponObject.SetActive(true);
                    weaponImage.sprite = obj2;

                });
            }
        });



        SetRareImage();

    }

    public void NotingAction()
    {
        charaImage.gameObject.SetActive(false);
        go.gameObject.SetActive(false);
        typeImage.gameObject.SetActive(false);
        nothingObject.gameObject.SetActive(true);
    }

    public void SetRareImage()
    {
        for (int i = 0; i < realCharaData.rareId; i++)
        {
            var newGO = GameObject.Instantiate(go, parent);
            Transform newGoTF = newGO.transform;
            newGoTF.localPosition = new Vector3(newGoTF.localPosition.x - (30 * i), newGoTF.localPosition.y, newGoTF.localPosition.z);
            newGO.SetActive(true);
        }

        //Debug.Log("キャラ:" + realCharaData.charaName + " レア度:" +realCharaData.rareId );

        frameImage.sprite = Resources.Load<Sprite>("Scenes/Image/UI/charaFrame" + GetFrameNo( realCharaData.rareId)) as Sprite;
        go.SetActive(false);

    }


    public void ClickEvent()
    {
        //Debug.Log("oshiteru   " + realCharaData.charaID);  
        layout_PowerUp.AllScaleBack();
        ChangeScale(true);
        layout_PowerUp.SetCharaInformation(realCharaData);


    }

    public void LongClickEvent()
    {
        move = false;
        StartCoroutine(MoveStart(gameObject));
    }

    GameObject newGOcopy;
    private IEnumerator MoveStart(GameObject newGO)
    {
        newGOcopy = Instantiate(newGO, parentTF);
        while (true)
        {
            //Debug.Log("idousaseru");
            Vector3 screenPos = Input.mousePosition;
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(screenPos);
            newGOcopy.transform.position = new Vector3(worldPos.x, worldPos.y, -100);
            yield return null;
        }
    }

    public void Hanashita()
    {
//        Debug.Log(teamCharaController.move + "   korekroe");

        if (newGOcopy == null)
            return;

        move = false;
        for (int i = 1; i < 6; i++)
        {
            if (newGOcopy.transform.localPosition.y >= 174 && GameSetting.partyWaku[i-1] <= newGOcopy.transform.localPosition.x && newGOcopy.transform.localPosition.x <= GameSetting.partyWaku[i])
            {
                Debug.Log( "teamNo"+ layout_PowerUp.teamNo+ " number:"  + i  + " charaId:" + realCharaData.charaName );
                layout_PowerUp.partyCharaDataList[layout_PowerUp.teamNo][i] = realCharaData;
                Singleton<SoundPlayer>.instance.CharaVoice(realCharaData.charaIdNumber, "select");
            }
        }

        StopAllCoroutines();
        Destroy(newGOcopy);

        Layout_PowerUp.infocharaName = realCharaData;
        StartCoroutine(layout_PowerUp.SetImage());
    }

    public void Nagaoshi()
    {
        //Debug.Log("nagaopshi");
        move = true;
        StartCoroutine(MoveStart(gameObject));
    }

    public void ChangeScale( bool flag )
    {
        if( flag )
        {
            //big
            transform.DOScale(
                new Vector3(baseVector.x * 1.1f, baseVector.y * 1.1f, 0),    // 移動終了地点座標
                0.3f                            // アニメーション時間
            );
        }else
        {
            transform.DOScale(
                new Vector3(baseVector.x, baseVector.y, 0),    // 移動終了地点座標
                0.3f                            // アニメーション時間
            );
        }

        //Debug.Log(flag + ": baseVector.x:" + baseVector.x);
     
    }

    public IEnumerator CheackActive()
    {
        while(true)
        {

            yield return new WaitForSeconds(0.1f);
        }
        yield break;
    }



}
