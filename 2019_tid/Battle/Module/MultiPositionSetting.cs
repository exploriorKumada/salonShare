using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiPositionSetting : MonoBehaviour {
    
    [SerializeField] RectTransform underUIrect;
    [SerializeField] Transform upTF;
    [SerializeField] Transform underTF;

    private float defaultUnderPosition = 540.2f;//デフォルト解像度の場合の位置

    private void Awake()
    {
        StartCoroutine( SetUnderUIposition() );
    }

    public IEnumerator SetUnderUIposition()
    {
        
        while(true)
        {
            //差分を求める
            Vector2 testPosi = GetLocalPosition(underTF.position);
            float ajustNumber = underTF.localPosition.y + (defaultUnderPosition + testPosi.y);
            underTF.localPosition = new Vector2(underTF.localPosition.x, ajustNumber);

            //処理完了
            if ( (int)(defaultUnderPosition + testPosi.y) == 0)
            {
                //同じ分だけUpUIも広げる
                SetUpUIpositio( ajustNumber);
                yield break;
            }
            yield return null;
        }

    }

    private void  SetUpUIpositio( float yPosition )
    {
        upTF.localPosition = new Vector2( upTF.localPosition.x, -(yPosition) );
    }

    private Vector2 GetLocalPosition(Vector2 screenPosition)
    {
        Vector2 result = Vector2.zero;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(underUIrect, screenPosition, Camera.main, out result);

        return result;
    }

}
