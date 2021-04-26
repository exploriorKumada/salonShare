using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
using DG.Tweening;

public class Layout_Ranking : ScenePrefab {

	[SerializeField] GameObject baseObject;
	[SerializeField] Transform parentTF;
	[SerializeField] GameObject ContentGO;

    [SerializeField] GameObject pastBaseObject;
    [SerializeField] Transform pastParentTF;
    [SerializeField] GameObject pastContentGO;

    [SerializeField] Transform animatorTF;



	int rankingCount = 30;
	// Use this for initialization
	void Start () {
      
        StartCoroutine(SetStart());
	}

    public IEnumerator SetStart()
    {
        AddSubLayout("Footer");
        AddSubLayout("Header");
        baseObject.SetActive (false);
        AddPopup("Popup_AlphaLoding");       
        RankingAPI.GetRankingOn(rankingCount, () =>
        {
            AlphaLoding.Close();
            UserData.TutoSet(UserData.TutoType.ranking);
            SetImage ();
            PastSetImage();
        });

        yield break;
    }


	private void SetImage()
	{
		int count = 1;
        baseObject.SetActive(false);
        foreach ( var KV in RankingAPI.userData )
		{
            if (KV.Value == 0)
            {
                Debug.Log(KV.Value + " is 0");
                continue;
            }
            var newGO = GameObject.Instantiate(baseObject);
			newGO.transform.parent = parentTF; 
			var newTF = newGO.transform;
			newTF.Find("name").GetComponent<TextMeshProUGUI> ().text = KV.Key;
			newTF.Find("score").GetComponent<TextMeshProUGUI> ().text = ""+KV.Value;
			newTF.Find ("rank").GetComponent<TextMeshProUGUI> ().text = count + ". ";
            if( count == 1 )
            {
                newTF.Find("goldMark").gameObject.SetActive(true);
            }else if (count == 2)
            {
                newTF.Find("shilverMark").gameObject.SetActive(true); 
            }

			newGO.SetActive(true);

			count++;
		}  
	

		//ContentGO.GetComponent<VerticalLayoutGroup> ().spacing = 0;
	}

    private void PastSetImage()
    {
        int count = 1;
        pastBaseObject.SetActive(false);
        foreach (var KV in RankingAPI.pastUserData)
        {
            if(KV.Value == 0)
            {
                Debug.Log(KV.Value+" is 0");
                continue;
            }

            var newGO = GameObject.Instantiate(pastBaseObject);
            newGO.transform.parent = pastParentTF;
            var newTF = newGO.transform;
            newTF.Find("name").GetComponent<TextMeshProUGUI>().text = KV.Key;
            newTF.Find("score").GetComponent<TextMeshProUGUI>().text = "" + KV.Value;
            newTF.Find("rank").GetComponent<TextMeshProUGUI>().text = count + ". ";

            if (count == 1)
            {
                newTF.Find("goldMark").gameObject.SetActive(true);
            }
            else if (count == 2)
            {
                newTF.Find("shilverMark").gameObject.SetActive(true);
            }
            newGO.SetActive(true);

            count++;
        }
  
    }


    bool setFlag = false;
    public void Move()
    {
        if(!setFlag)
        {
            setFlag = true;

            animatorTF.DOLocalMove(
                    new Vector3(2800f, 0, 0),
                    0.5f//時間
            );
        }else
        {
            setFlag = false;

            animatorTF.DOLocalMove(
                    new Vector3(0, 0, 0),
                    0.5f//時間
            ); 
        }
        
    }
}
