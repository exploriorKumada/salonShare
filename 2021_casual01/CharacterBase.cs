using System;
using System.Collections;
using System.Collections.Generic;
using StandardAssets.Characters.Physics;
using UnityEngine;
using TW.GameSetting;

public class CharacterBase : MonoBehaviour
{
    [NonSerialized] public float walkSpeed = 7f;
    [NonSerialized] public float baseWalkSpeed;
    [NonSerialized] public float jumpPower = 5f;

    [NonSerialized] public OpenCharacterController openCharacterController;
    [NonSerialized] public Animator animator;
    [SerializeField] public CapsuleCollider casCol;
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

}
