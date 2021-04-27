using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
using TW.GameSetting;
using System.Linq;
using Random = UnityEngine.Random;
using Timers;

namespace Explorior
{
    public static class ExtentionUtility
    {
        public static void ParentInitialize(this GameObject baseObject)
        {
            baseObject.SetActive(false);
            Transform parentTF = baseObject.transform.parent;

            foreach (Transform tf in parentTF)
                if (tf.gameObject != baseObject)
                    MonoBehaviour.Destroy(tf.gameObject);
        }

        public static void ParentTransInitialize(this Transform parent)
        {
            foreach(Transform tf in parent)
                MonoBehaviour.Destroy(tf.gameObject);
        }

        public static List<GameObject> GetAll(this GameObject obj)
        {
            List<GameObject> allChildren = new List<GameObject>();
            GetChildren(obj, ref allChildren);
            return allChildren;
        }

        public static void CharacterControllerInit(this CharacterController characterController)
        {
            characterController.slopeLimit = 0f;
            characterController.stepOffset = 0f;
            characterController.skinWidth = 0.0001f;
            characterController.minMoveDistance = 0;
            characterController.center = new Vector3(0, 0.5f, 0);
            characterController.radius = 0.5f;
            characterController.height = 0f;
        }

        /// <summary>
        /// 取得する
        /// </summary>
        public static AnimatorStateEvent Get(this Animator animator, int layer)
        {
            return animator.GetBehaviours<AnimatorStateEvent>().First(x => x.Layer == layer);
        }

        public static void WeponTagetInit(this GameObject obj, bool active)
        {
            var list = GetAll(obj);
            foreach (var Value in list)
            {
                var col = Value.GetComponent<Collider>();
                var col2d = Value.GetComponent<Collider2D>();

                if (col != null)
                    col.enabled = active;

                if (col2d != null)
                    col2d.enabled = active;
            }
        }

        public static void GetChildren(GameObject obj, ref List<GameObject> allChildren)
        {
            Transform children = obj.GetComponentInChildren<Transform>();
            //子要素がいなければ終了
            if (children.childCount == 0)
            {
                return;
            }
            foreach (Transform ob in children)
            {
                allChildren.Add(ob.gameObject);
                GetChildren(ob.gameObject, ref allChildren);
            }
        }

        // ディープコピーの複製を作る拡張メソッド
        public static T DeepClone<T>(this T src)
        {
            using (var memoryStream = new System.IO.MemoryStream())
            {
                var binaryFormatter
                  = new System.Runtime.Serialization
                        .Formatters.Binary.BinaryFormatter();
                binaryFormatter.Serialize(memoryStream, src); // シリアライズ
                memoryStream.Seek(0, System.IO.SeekOrigin.Begin);
                return (T)binaryFormatter.Deserialize(memoryStream); // デシリアライズ
            }
        }
    }

    public static class UI
    {
        public static Tweener FadeAction(bool isIn, CanvasGroup canvasGroup, float duration, Action callBack = null)
        {
            float from = isIn ? 0f : 1f;
            float to = isIn ? 1f : 0f;

            canvasGroup.alpha = from;
            return canvasGroup.DOFade(to, duration).OnComplete(() => { callBack?.Invoke(); });
        }

        public static void UIsclaleAction(bool isUp, Transform TF, float duration, Action callBack = null)
        {
            float from = isUp ? 0f : 1f;
            float to = isUp ? 1f : 0f;

            TF.localScale = Vector3.one * from;
            TF.DOScale(Vector3.one * to, duration).OnComplete(() => { callBack?.Invoke(); });

        }

        public static void UIsclaleUpAction(float upAmount, Transform TF, float duration, Action callBack = null)
        {
            float from = 1f;
            float to = upAmount;
            TF.localScale = Vector3.one * from;
            TF.DOScale(Vector3.one * to, duration).OnComplete(() => { callBack?.Invoke(); });

        }

        public static void UIcutin(Transform TF, float ajustX = 0, float ajustY = 0, float second = 0.5f, Action callBack = null)
        {
            Vector3 initPosition = TF.localPosition;

            TF.localPosition = new Vector3(TF.localPosition.x + ajustX, TF.localPosition.y + ajustY, TF.localPosition.z);
            TF.gameObject.SetActive(true);

            TF.DOLocalMove(
                    initPosition,
                    second
            ).OnComplete(() => { callBack?.Invoke(); });
        }

        public static void UIcutOut(Transform TF, float ajustX = 0, float ajustY = 0, float second = 0.5f, Action callBack = null)
        {
            Vector3 initPosition = TF.localPosition;

            TF.DOLocalMove(
                    new Vector3(TF.localPosition.x + ajustX, TF.localPosition.y + ajustY, TF.localPosition.z),
                    second//時間  
            ).OnComplete(() =>
            {
                TF.gameObject.SetActive(false);
                TF.localPosition = initPosition;
                callBack?.Invoke();
            });
        }


        public static void UIjump(this Transform TF)
        {
            Vector3 vector3 = TF.localPosition;

            TF.DOLocalJump(
                vector3,
                20,                        // ジャンプする力
                3,                        // 移動終了までにジャンプする回数
                2f                        // アニメーション時間
            ).SetDelay(1).OnComplete(() =>
            {
                UIjump(TF);
            });
        }

        public static void UIjumpRoop(this Transform TF, float roopTime = 1f)
        {
            UIjump(TF);
            DOVirtual.DelayedCall(roopTime, () =>
            {
                UIjumpRoop(TF, roopTime);
            });
        }

        public static void UIRoateRoop(this Transform TF, float roopTime = 0.5f)
        {
            // 1秒かけて90度まで回転
            TF.DORotate(
                new Vector3(TF.localEulerAngles.x, TF.localEulerAngles.y + 90, TF.localEulerAngles.z),   // 終了時点のRotation
               roopTime                   // アニメーション時間
            ).SetEase(Ease.Linear).OnComplete(() =>
            {
                UIRoateRoop(TF, roopTime);
            }); ;
        }

        public static void UIdirectionAnimation(Transform TF, int type, float amount = 5)
        {
            float side = 0;
            float length = 0;
            switch (type)
            {
                case 0://migi
                    side = -1 * amount;
                    break;
                case 1://hidari
                    side = amount;
                    break;
                case 2://ue
                    length = -1 * amount;
                    break;
                case 3://shita
                    length = amount;
                    break;
            }

            Vector3 vector3 = TF.localPosition;
            Vector3 targetVector3 = new Vector3(vector3.x + side, vector3.y + length, vector3.z);

            TF.DOLocalMove(
                    targetVector3,
                    0.3f//時間            
            ).SetDelay(0).OnComplete(() =>
            {
                TF.DOLocalMove(
                    vector3,
                  0.3f//時間            
                ).OnComplete(() =>
                {
                    UIdirectionAnimation(TF, type, amount);
                });
            });
        }


        public static void Shake(this Transform TF, int interval = 50)
        {
            var sequence = DOTween.Sequence();
            int amount = interval;

            Vector3 baseVevtor3 = TF.localPosition;
            for (int i = 0; i < 5; i++)
            {
                //円の方程式から、ガクガク感を表現 ( 正しか出ないから、ランダムで負にする)
                float x = UnityEngine.Random.Range(-1 * amount, amount);
                float y = Mathf.Sqrt((amount * amount) - (x * x));

                if (UnityEngine.Random.Range(0, 2) == 0)
                {
                    y = y * (-1);
                }

                sequence.Append(
                    TF.DOLocalMove(
                        new Vector3(TF.localPosition.x + x, TF.localPosition.y + y, TF.localPosition.z),
                        0.02f//時間
                    ).SetEase(Ease.OutCubic)
                );
                int icopy = i;
                sequence.Append(
                    TF.DOLocalMove(
                        baseVevtor3,
                        0.02f//時間
                    ).SetEase(Ease.InCubic).OnComplete(() =>
                    {
                    })
                );
            }
        }
    }
}

