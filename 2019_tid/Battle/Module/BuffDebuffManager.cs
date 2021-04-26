using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class BuffDebuffManager : MonoBehaviour {

	[SerializeField] GameObject objectBase;
	[SerializeField] Transform parentTF;


	public void Init()
	{
		for( int i=0; i < parentTF.childCount; ++i ){
			GameObject.Destroy( parentTF.GetChild( i ).gameObject );
		}

		objectBase.SetActive (false);
	}

    public void SetImage( List<BuffDebuffData> buffDebuffDatas )
	{


        for (int i = 0; i < parentTF.childCount; ++i)
        {
            GameObject.Destroy(parentTF.GetChild(i).gameObject);
        }
		
		int count = 0;
        foreach( var Value in buffDebuffDatas )
		{
            
			var newGO = GameObject.Instantiate( objectBase, parentTF );
			Transform newGoThisTF = newGO.transform;
			newGoThisTF.localPosition = new Vector3( newGoThisTF.localPosition.x + ( count* 60), newGoThisTF.localPosition.y, newGoThisTF.localPosition.z - 50 );
			newGoThisTF.localScale = new Vector3(1,1,1);

            if( Value.buffDebuffID == 3 )
			{
                
                if( Value.buffDebuffType == 1 )
                {
                    newGoThisTF.Find( "image" ).GetComponent<Image>().sprite  = Resources.Load<Sprite>( "Scenes/Image/Battle/BuffDeBuff/AtkUp" );  
                }else if( Value.buffDebuffType == 2 )
                {
                    newGoThisTF.Find( "image" ).GetComponent<Image>().sprite  = Resources.Load<Sprite>( "Scenes/Image/Battle/BuffDeBuff/DefUp" ); 
                }
            }else
            {
                if (Value.buffDebuffType == 1)
                {
                    newGoThisTF.Find("image").GetComponent<Image>().sprite = Resources.Load<Sprite>("Scenes/Image/Battle/BuffDeBuff/AtkDown");
                }
                else if (Value.buffDebuffType == 2)
                {
                    newGoThisTF.Find("image").GetComponent<Image>().sprite = Resources.Load<Sprite>("Scenes/Image/Battle/BuffDeBuff/DefDown");
                } 
            }

			newGO.SetActive (true);
			count++;
		}

	}

}
