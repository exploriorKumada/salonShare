using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using UnityEngine.UI;

public class QuestController : ScenePrefab {

    public RealQuestDetail realQuestDetail;
    [SerializeField] TextMeshPro questName;
    [SerializeField] Image iconImage;
    [SerializeField] GameObject clearImage;
    [SerializeField] GameObject closeImage;
    [SerializeField] GameObject newImage;
    [SerializeField] Button button;
    [SerializeField] Image missitionImage;
    [SerializeField] GameObject storyObject;

    public void Init( RealQuestDetail realQuestDetail )
    {
        this.realQuestDetail = realQuestDetail;
        iconImage.sprite =  Resources.Load<Sprite>("Scenes/MainScene/Layout/WorldMap/Image/" + realQuestDetail.worldImageId ); 
        Vector3 vector3 = transform.localScale;
        transform.localScale = Vector3.zero;
        transform.DOScale(
            vector3,     // 終了時点のScale
            0.2f                            // アニメーション時間
        );

        questName.text = realQuestDetail.name;
        clearImage.SetActive(realQuestDetail.clear_flag == 1);
        closeImage.SetActive(realQuestDetail.open_Flag == 2);

        if( realQuestDetail.clear_flag == 2 && realQuestDetail.open_Flag == 1)
        {
            newImage.SetActive(true);
            UIjump(newImage.transform);
        }

        if( realQuestDetail.open_Flag == 2 )
        {
            button.interactable = false;
        }

        missitionImage.sprite = ResourceLoaderOrigin.GetMisstionStarImage(realQuestDetail.mission_clear_status);
        storyObject.SetActive(StorySettingBase.IsStoryExist(realQuestDetail.quest_detail_id));
        //Debug.Log( realQuestDetail.name + " open_Flag:"  + realQuestDetail.open_Flag  + " clear_flag:" + realQuestDetail.clear_flag);

    }


    public void PushEvent()
    {
        Popup_PartySelect.realQuestDetail = realQuestDetail;
        BlackAnima(() =>
        {
            AddPopup("PartySelect");        
        });

    
      
    }
}
