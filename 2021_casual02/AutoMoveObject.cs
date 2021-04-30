using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class AutoMoveObject : MonoBehaviour
{

    public void Init(float moveX = 0, float moveY = 0, float moveZ = 0,float duration = 3,float delaySecond = 0,bool isReverseRoop = false)
	{

        DOTweenAnimation ani = gameObject.GetComponent<DOTweenAnimation>();
        if(ani==null)
        {
            ani = gameObject.AddComponent<DOTweenAnimation>();
        }

        ani.autoPlay = false;
        ani.easeType = Ease.Linear;
        ani.animationType = DOTweenAnimation.AnimationType.Move;
        ani.loopType = LoopType.Yoyo;
        ani.loops = isReverseRoop? 2 : -1;
        ani.duration = duration;
        ani.delay = delaySecond;
        ani.isRelative = true;
        ani.endValueV3 = new Vector3(moveX, moveY, moveZ);
        ani.autoKill = false;
        ani.target = GetComponent<Rigidbody>();
        ani.targetType = DOTweenAnimation.TargetType.Rigidbody;
     
        ani.tween.Rewind();
        ani.tween.Kill();
        ani.CreateTween();
        ani.tween.Play();

        if(isReverseRoop)
        {
            ani.tween.OnComplete(() =>
            {
                Init(-moveX, -moveY, -moveZ, duration, delaySecond, isReverseRoop);
            });


        }
    }
}
