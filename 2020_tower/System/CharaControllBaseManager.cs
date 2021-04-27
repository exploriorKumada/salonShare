using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class CharaControllBaseManager : SystemBaseManager
{
    [System.NonSerialized] public PlayerCharaController playerCharaController = new PlayerCharaController();

    /// <summary>
    ///　タップコントローラー　初期位置
    /// </summary>
    [System.NonSerialized] public Vector2 moveButtonUIBasePosi;
    [SerializeField] public Transform moveButtonUI;
    [SerializeField] public　Camera camera;
    [SerializeField] public　Transform playerCharaParent;
    [SerializeField] public　Transform cameraTaget;


    public void CharaControllBaseManagerInit()
    {
        //コントローラー差分チェック
        this.ObserveEveryValueChanged(x => moveButtonUI.localPosition)
        .Where(x => true)
        .Subscribe(_ => MoveAmountCheack());
    }

    public void MoveAmountCheack()
    {
        float distance = Vector2.Distance(moveButtonUIBasePosi, moveButtonUI.localPosition);
        playerCharaController.StateCheack(distance);
    }

}
