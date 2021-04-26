using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GachaCharaController : CharaInfoSystem {

    [SerializeField] Image frame;
    [SerializeField] Image charaImage;
    [SerializeField] Image type;
    [SerializeField] Image baseImage;
    [SerializeField] TextMeshProUGUI jemNumber;
    [SerializeField] GameObject jemObject;
    [SerializeField] GameObject leaderObject;

    [SerializeField] GameObject go;
    [SerializeField] Transform parent;

    public RealCharaData m_realCharaData;

    public void Init( RealCharaData realCharaData )
    {
        m_realCharaData = realCharaData;
        frame.sprite = Resources.Load<Sprite>("Scenes/Image/UI/charaFrame" + GetFrameNo(realCharaData.rareId));
        ResourceLoaderOrigin.GetBattleCharaImage(realCharaData.charaIdNumber, (Sprite obj) => { charaImage.sprite = obj; });
        baseImage.color = Layout_PowerUp.colorCordList[realCharaData.charaTypeId].ToColor();
        type.sprite = Resources.Load<Sprite>("Scenes/Image/UI/" + realCharaData.charaTypeId);

        jemObject.SetActive( realCharaData.addGold>0 );
        jemNumber.text = "+"+realCharaData.addGold;

        leaderObject.SetActive(realCharaData.realCharaMasterData.leader_master_id!=0);

        SetRareImage(realCharaData);
    }


    public void SetRareImage(RealCharaData realCharaData)
    {
        for (int i = 0; i < realCharaData.rareId; i++)
        {
            var newGO = GameObject.Instantiate(go, parent);
            Transform newGoTF = newGO.transform;
            newGO.SetActive(true);
        }

        go.SetActive(false);

    }


    public void Info()
    {
        Popup_PublicCharaInfo.realCharaData = m_realCharaData;
        AddPopup("PublicCharaInfo");
        
    }

}
