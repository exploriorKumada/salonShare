using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CameraSetting : MonoBehaviour
{

    public static Camera camera;
    private static Transform cameraTF;
    private static Transform cameraBaseTF;
    private static Vector3 cameraBasePosi;
    public static float cameraSize;

    public void Initialize()
    {
        camera = GetComponent<Camera>();
        cameraTF = transform;
        cameraBasePosi = transform.localPosition;
        //cameraSize = camera.orthographicSize;
    }

    public static void Zoom()
    {
        // 数値の変更
        DOTween.To(
            () => camera.orthographicSize,          // 何を対象にするのか
            num => camera.orthographicSize = num,   // 値の更新
            3,                  // 最終的な値
            0.1f                  // アニメーション時間
        );
    }

    public static void Move(Transform TF, float ajustX = 0, float ajustY = 0, float ajustTime = 0.1f)
    {
        //1秒で座標（1,1,1）に移動
        cameraTF.DOLocalMove(
            new Vector3(TF.localPosition.x + ajustX, TF.localPosition.y + ajustY, cameraTF.localPosition.z),
            ajustTime//時間
        );
    }


    public static void CutMove(Transform TF, float ajustX = 0, float ajustY = 0)
    {
        cameraTF.localPosition = new Vector3(TF.localPosition.x + ajustX, TF.localPosition.y + ajustY, cameraTF.localPosition.z);
    }


    public static void PickUp(Transform TF, float ajustX = 0, float ajustY = 0)
    {
        Zoom();
        Move(TF, ajustX, ajustY);
    }

    public static void CameraClear()
    {

        //1秒で座標（1,1,1）に移動
        cameraTF.DOLocalMove(
            cameraBasePosi,
            0.1f//時間
        );

        // 数値の変更
        DOTween.To(
            () => camera.orthographicSize,          // 何を対象にするのか
            num => camera.orthographicSize = num,   // 値の更新
            cameraSize,                  // 最終的な値
            0.1f                  // アニメーション時間
        );
    }


    //がががってする
    public static void Shake(int count = 5)
    {
        var sequence = DOTween.Sequence();
        int amount = 15;

        for (int i = 0; i < count; i++)
        {
            float xPosition = 0;
            float unitDuration = 0.05f;
            //円の方程式から、ガクガク感を表現 ( 正しか出ないから、ランダムで負にする)
            float x = Random.Range(-1 * amount, amount);
            float y = Mathf.Sqrt((amount * amount) - (x * x));

            if (Random.Range(0, 2) == 0)
            {
                y = y * (-1);
            }

            sequence.Append(
                cameraTF.DOLocalMove(
                    new Vector3(cameraTF.localPosition.x + x, cameraTF.localPosition.y + y, cameraTF.localPosition.z),
                    0.02f//時間
                ).SetEase(Ease.OutCubic)
            ).OnComplete(() =>
            {
                cameraTF.DOLocalMove(
                    cameraBasePosi,
                    0.02f//時間
                ).SetEase(Ease.InCubic);
            });
        }
    }
}
