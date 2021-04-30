using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class StageController : MonoBehaviour
{
    public int id;
    [SerializeField] List<Transform> keyPositionList = new List<Transform>();

    public void Initialize(bool ketSet , GameManager gameManager, bool isMove = false)
    {
        if (!ketSet) return;


        float moveDistamce = 0;
        float delay = 0;

        if (isMove)
        {
            moveDistamce = Random.RandomRange(3, 7) * (Random.RandomRange(0, 2) == 1? 1:-1);
            delay = Random.RandomRange(1f, 1.5f);
            DOVirtual.DelayedCall(delay, () =>
            {
          
                gameObject.AddComponent<AutoMoveObject>().Init(moveX: moveDistamce, isReverseRoop: true);
            });
        }

        foreach (var Value in keyPositionList)
        {
            if (Value == null) continue;

            var key = Instantiate(Resources.Load<GameObject>("Key/key"), Value);
            key.transform.localPosition = Vector3.zero;
            key.GetComponent<KeyUnit>().Init(gameManager);

            if (isMove)
            {
                DOVirtual.DelayedCall(delay, () =>
                {
                    key.AddComponent<AutoMoveObject>().Init(moveX: moveDistamce, isReverseRoop: true);
                });
            }                
        }
    }

}
