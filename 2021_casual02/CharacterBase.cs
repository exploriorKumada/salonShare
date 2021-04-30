using System;
using System.Collections;
using System.Collections.Generic;
using StandardAssets.Characters.Physics;
using UnityEngine;
using TW.GameSetting;
using UniRx;

public class CharacterBase : MonoBehaviour
{
    [NonSerialized] public float walkSpeed = 2f;
    [NonSerialized] public float baseWalkSpeed;
    [NonSerialized] public float jumpPower = 10f;
    [NonSerialized] public float baseJumpPower;

    [NonSerialized] public OpenCharacterController openCharacterController;
    [NonSerialized] public Animator animator;
    [SerializeField] public CapsuleCollider casCol;
    [SerializeField] public Rigidbody rigidbody;
    //[NonSerialized] public Vector3 velocity;
    [NonSerialized] public Vector3 initPostion;
    [NonSerialized] public Vector2 mobileVector2;
    [NonSerialized] public float baseColSize;
    /// <summary> キャラ動中フラグ /// </summary>
    [NonSerialized] public bool movingFlg = false;

    [SerializeField] public ColEventFunction colEventFunction;
    [SerializeField] public SkinnedMeshRenderer skinnedMeshRenderer;

    [NonSerialized] public bool isTouch;
    [NonSerialized] public GameManager gameManager;
    [NonSerialized] public Vector3 initScale;
    [NonSerialized] public Dictionary<KeyKind, bool> kindDic = new Dictionary<KeyKind, bool>();

    [NonSerialized] public CharaAttribute charaAttribute;
  
    public bool isStart
    {
        get
        {
            if (gameManager == null) return false;

            return gameManager.isStart;
        }
    }

    public void BaseInit()
    {

        foreach (var Value in Enum.GetValues(typeof(KeyKind)))
            kindDic[(KeyKind)Value] = false;

        initScale = transform.localScale;
        baseColSize = casCol.radius;
        baseWalkSpeed = walkSpeed;
        baseJumpPower = jumpPower;
    }

    public void Reset()
    {
        SetTrigger("idle");
        isTouch = false;  
    }

    public void SetTrigger(string animaName)
    {
        AnimationInit();
        animator.SetBool(animaName, true);
    }

    public void AnimationInit()
    {

        foreach (var Value in animator.parameters)
            if (Value.type == AnimatorControllerParameterType.Bool)
                animator.SetBool(Value.name, false);

    }


    /// <summary>
    /// 地面に接地しているかどうかを調べる
    /// </summary>
    public bool CheckGrounded()
    {
        //CharacterControlle.IsGroundedがtrueならRaycastを使わずに判定終了
        if (openCharacterController.isGrounded) { return true; }
        //放つ光線の初期位置と姿勢
        //若干身体にめり込ませた位置から発射しないと正しく判定できない時がある
        var ray = new Ray(this.transform.position + Vector3.up * 0.1f, Vector3.down);
        //探索距離
        var tolerance = 0.3f;
        //Raycastがhitするかどうかで判定
        //地面にのみ衝突するようにレイヤを指定する
        return Physics.Raycast(ray, tolerance);
    }

}
