using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using Live2D.Cubism.Core;
using Live2D.Cubism.Framework;
using Live2D.Cubism.Rendering;
using UnityEngine.UI;
using System.Linq;


public class Layout_Gacha : ScenePrefab {


	public static int gachaCount = 10;
    public static int gachaSeries = 1;

    [SerializeField] Transform beforeEffectTF;
    [SerializeField] SpriteRenderer backGround;
    [SerializeField] Animator tobiraAnimator;
    [SerializeField] CubismRenderController cubismRenderController;
    [SerializeField] Image blackAlpha;
    [SerializeField] GameObject tapButtonTextObject;

    [SerializeField] GameObject charaBase;
    [SerializeField] Transform charaTF;

    [SerializeField] GameObject effectBase;
    [SerializeField] Transform effectTF;

    [SerializeField] Transform exNumber;

    [SerializeField] GameObject resultObject;

    [SerializeField] GameObject effectObject;

    [SerializeField] TextMeshProUGUI resultText;
    [SerializeField] TextMeshProUGUI resultCryText;

    [SerializeField] GameObject againMenu;
    [SerializeField] TextMeshProUGUI skipText;

    [SerializeField] Image touchImage;

    [SerializeField] GameObject centerGO;

    [SerializeField] Transform rareKakuteiTextureParent;
    [SerializeField] Texture2D kakuteiTexture;

    private int lineAjustumber;
	private int columnAjustumber;
	private List<string> charaIDlist = new List<string>();

    List<RealCharaData> charaDataList = new List<RealCharaData>();

    [SerializeField] GachaSystem gachaSystem;

	int pushCount = 0;

	// Use this for initialization
	void Start () {

        StartOn();
        //Todo id      
	}

    public void StartOn()
    {
        AddPopup("Popup_AlphaLoding");
        GacghaAPISetting.LoadGachaSet(gachaCount, gachaSeries, () =>
        {
            UserDataSetting.LoadRealUserData(UserData.GetUserID(), () =>
            {
                ResourceLoaderOrigin.GetBackGroundImage(4, (bgobj) =>
                {
                    backGround.sprite = bgobj;
                    AlphaLoding.Close();
                    charaDataList = GacghaAPISetting.charaDataList;
                    StartCoroutine(SetStart());

                });                  
            });
        });  
    }

    public IEnumerator SetStart()
    {

        beforeEffectTF.gameObject.SetActive(true);
        //レア確定演出
        var rares = charaDataList.Where(x => x.rareId == 5);
        Debug.Log("========="+ rares.Count());
        if (rares.Count() > 0 && Random.RandomRange(0,2)==1)
        {
            for (int i = 0; i < rareKakuteiTextureParent.childCount; i++)
            {
                var CR = rareKakuteiTextureParent.GetChild(i).GetComponent<CubismRenderer>();
                if (CR != null)
                {
                    CR.MainTexture = kakuteiTexture;
                }
            }
        }
        iphoneXAjust(centerGO);
        BgmManager.Instance.Play("05.Battle");
       
        ImageEffect.SetEefectRoop(touchImage);
        yield break;
    }


    public void PushEvent()
    {
        tapButtonTextObject.SetActive(false);
        StartCoroutine(SetImage());
    }


	float startPosition;

	private IEnumerator SetImage()
	{
        
        beforeEffectTF.gameObject.SetActive(true);
        Singleton<SoundPlayer>.instance.playSE(GameSetting.SEType.Stone);
        Singleton<SoundPlayer>.instance.playSE(GameSetting.SEType.Tap);
        Singleton<SoundPlayer>.instance.playSE(GameSetting.SEType.DoorOpen,1.3f);
        CameraSetting.Shake(150); 
        yield return new WaitForSeconds(0.5f);
        tobiraAnimator.enabled = true;
        yield return new WaitForSeconds(1f);
        effectObject.SetActive(true);
        Singleton<SoundPlayer>.instance.playSE(GameSetting.SEType.Explosion, 1f);
        beforeEffectTF.DOScale(
            new Vector3(5f, 5f),     // 終了時点のScale
            0.5f                        // アニメーション時間
        );
        DOTween.To(
            () => cubismRenderController.Opacity,                // 何を対象にするのか
            num => cubismRenderController.Opacity = num,    // 値の更新
            0,                    // 最終的な値
            1f                                // アニメーション時間
        );

        DOTween.ToAlpha(
            () => blackAlpha.color,                // 何を対象にするのか               
            color => blackAlpha.color = color,    // 値の更新
             0,                    // 最終的な値
             1f                                // アニメーション時間
         );

		charaBase.SetActive(false);

		yield return new WaitForSeconds(2f);
        beforeEffectTF.gameObject.SetActive(false);
		startPosition = 0;
		startPosition = ( 150 * ( gachaCount - 1 ) );

		if( gachaCount >= 5 )
            startPosition = 900; 


        int totalAddGold = 0;
        foreach( var Value in charaDataList )
		{
            //Debug.Log( "hiketa " + Value  + "  " + Value.addGold);
            totalAddGold += Value.addGold;
		}

        //resultText.text = totalAddGold.ToString();
        resultText.text = UserData.GetExNumber().ToString();
        resultCryText.text = UserData.GetCrystalNumber().ToString();

        int i = -1;
        Debug.Log( charaDataList.Count );
        foreach (var Value in charaDataList)
		{
            i++;
			var newGO = GameObject.Instantiate( charaBase );
			lineAjustumber = ( i / 6 );
			columnAjustumber = ( i % 6 );

			newGO.transform.parent = charaTF;
			Transform newGoTF = newGO.transform;
			Vector3 setPosition = new Vector3( -1*startPosition + ( charaBase.transform.localPosition.x + ( columnAjustumber * 300) ), newGoTF.localPosition.y - ( lineAjustumber * 500 ), newGoTF.localPosition.z -( i*10 ) );
            newGO.GetComponent<GachaCharaController>().Init(Value);
            if (iphoneXFlag() || notAppleFlag())
            {
                Debug.Log("解像度によるスケール調整");
                newGoTF.localScale = new Vector3(newGoTF.localScale.x * 0.7f, newGoTF.localScale.y * 0.7f, 1);
            }
               
            if ( Value.rareId == 5 )
			{
                CameraSetting.Shake(50);
                Singleton<SoundPlayer>.instance.playSE(GameSetting.SEType.Stone);
				yield return new WaitForSeconds(3f);	
			}
			else
			{
				yield return new WaitForSeconds(1f);		
			}
			
			SetEffect();
			StartCoroutine ( SetMoveAction( setPosition, newGoTF ) ); 
			newGO.name = i + "_shadow";
			newGO.SetActive(true);
			yield return new WaitForSeconds(0.2f);

            if( i == charaDataList.Count-1)
			{
                ResultSet();
				pushCount++;
			}
		}

		yield return null;
	}


	private float xPosi; 
	private IEnumerator SetMoveAction( Vector3 setPosition, Transform newGoTF )
	{
        Singleton<SoundPlayer>.instance.playSE(GameSetting.SEType.Tap2);
		newGoTF.localPosition = new Vector3( 0, -300, setPosition.z );

		//0から指定した位置まで飛ばす
		AddAnimationTask(
			tag : "xPosi" + newGoTF +( setPosition.x * setPosition.y ) ,
			duration : 0.5f,
			startValue : 0,
			endValue : setPosition.x,
			animationFunc : L3_Easing.QuinticOut,
			readyAction : ()=>{},
			firstAction : ()=>{},
			runningAction : (r)=>{
				xPosi = r;
			},
			finishAction : ()=>{}
		);



		AddAnimationTask(
			tag : "yPosi" + newGoTF +( setPosition.x * setPosition.y ) ,
			duration : 0.5f,
			startValue : -300,
			endValue : setPosition.y,
			animationFunc : L3_Easing.QuinticOut,
			readyAction : ()=>{},
			firstAction : ()=>{},
			runningAction : (r)=>{
				newGoTF.localPosition = new Vector3( xPosi, r, setPosition.z );
			},
			finishAction : ()=>{}
		);
		yield return null;
	}


	private void SetEffect()
	{
		var newGO = GameObject.Instantiate( effectBase );
		newGO.transform.parent = effectTF;
		Transform newGoTF = newGO.transform;
		newGO.SetActive(true);
	}


	private void SkipMove()
	{

		StopAllCoroutines();
		//今あるやつを全部消して、一瞬で全部だす
		for( int i = 0 ; i<charaTF.childCount; i++ )
		{
			GameObject.Destroy( charaTF.GetChild( i ).gameObject );
		}
        int count = -1;
        foreach (var Value in charaDataList)
        {
            count++;
            var newGO = GameObject.Instantiate(charaBase);

            lineAjustumber = (count / 6);
            columnAjustumber = (count % 6);

            newGO.transform.parent = charaTF;
            Transform newGoTF = newGO.transform;
            Vector3 setPosition = new Vector3(-1 * startPosition + (charaBase.transform.localPosition.x + (columnAjustumber * 300)), newGoTF.localPosition.y - (lineAjustumber * 500), newGoTF.localPosition.z - (count * 10));
            if (iphoneXFlag() || notAppleFlag())
            {
                Debug.Log("解像度によるスケール調整");
                newGoTF.localScale = new Vector3(newGoTF.localScale.x * 0.7f, newGoTF.localScale.y * 0.7f, 1);
            }
            newGoTF.localPosition = setPosition;
            newGO.GetComponent<GachaCharaController>().Init(Value);
            newGO.name = count + "_shadow";
			newGO.SetActive(true);
           
		}
        ResultSet();
	}
	

    public void SkipAction()
    {
        if( pushCount == 0 )
        {
            pushCount++;
            SkipMove();
        }else
        {
            UserDataSetting.LoadRealUserData(UserData.GetUserID(), () =>
            {
                ChangeLayout("GachaMenu");
            });
           
        }   
    }

    public void ResultSet()
    {
        resultObject.SetActive(true);
        skipText.text = "戻る";
        againMenu.SetActive(false);
        if (UserData.GetCrystalNumber() >= 150 && gachaCount == 1)
            againMenu.SetActive(true);

        if (UserData.GetCrystalNumber() >= 1500 && gachaCount >= 10)
            againMenu.SetActive(true);

    }


    public void PushEventAgain()
    {
        ChangeLayout("Gacha");
    }

}
　