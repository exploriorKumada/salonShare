using System.Collections;
using System.Collections.Generic;
using TW.GameSetting;
using UnityEngine;
using Timers;
using DG.Tweening;

public class KeyUnit : MonoBehaviour
{
    [SerializeField] public ColEventFunction colEventFunction;

    [SerializeField] MeshRenderer mesh;

    GameManager gameManager;

    KeyKind keyKind = KeyKind.None;

    public void Init(GameManager _gameManager)
    {
        int number = Random.Range(0,100);

        if (number < 10) keyKind = KeyKind.Bigger;
        else if (number < 20) keyKind = KeyKind.Speeder;
        else if (number < 22) keyKind = KeyKind.Adder;
        else if (number < 24) keyKind = KeyKind.RangeSpreeder;
        else if (number < 25) keyKind = KeyKind.BigRangeSpreeder;
        else if (number < 26) keyKind = KeyKind.BigSpeeder;

        transform.localRotation = new Quaternion(Random.RandomRange(0,360), Random.RandomRange(0, 360), Random.RandomRange(0, 360), Random.RandomRange(0, 360));

        if(keyKind==KeyKind.BigRangeSpreeder || keyKind==KeyKind.BigSpeeder)
        {
            transform.localScale = transform.localScale * 3;
        }

        mesh.material = _gameManager.keyMaterials[(int)keyKind];
        gameManager = _gameManager;
        colEventFunction.colEvenFuncEnterAction = (eve) =>
        {
            if (eve.eventType == TW.GameSetting.ColEventType.PlayerGetKey)
            {
                gameManager.GetKey(keyKind);

                MoveOn();
            }

        };
    }

    public void MoveOn()
    {
        GetComponent<CapsuleCollider>().enabled = false;
        transform.Find("value").GetComponent<CapsuleCollider>().enabled = false;

        float sec = 0.2f;
        transform.DOScale(
            Vector3.zero,　　//終了時点のScale
            sec 　　　　　　//時間
        );

        Vector3 posi = gameManager.stickManManager.transform.position;
        transform.DOMove(
            new Vector3(posi.x, posi.y+0.5f, posi.z),　　//移動後の座標
            sec 　　　　　　//時間
        ).OnComplete(()=>
        {
            Destroy(this.gameObject);
        });
    }

    void Update()
    {
        if(transform.position.y < -200)
        {
            Destroy(this.gameObject);
        }
    }

}
