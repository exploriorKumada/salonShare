using System.Collections;
using System.Collections.Generic;
using StandardAssets.Characters.Physics;
using UnityEngine;
using TW.GameSetting;
using Timers;
using UnityEngine.Events;
using UniRx;

public class StickManManager : CharacterBase
{
    [SerializeField]
    Transform pivotAjust;

    [SerializeField]
    Camera camera;

    [SerializeField] public CapsuleCollider getCol;

    [SerializeField] GameObject spleedObj;

    // Start is called before the first frame update
    public void Init(GameManager _gameManager)
    {
        gameManager = _gameManager;
        openCharacterController = GetComponent<OpenCharacterController>();
        animator = GetComponent<Animator>();
        charaAttribute = CharaAttribute.Player;
        BaseInit();
        spleedObj.SetActive(false);
        Observe();
    }

    public void BaseInit()
    {
        initPostion = transform.localPosition;
        base.BaseInit();
    }

    public void Big()
    {
        if (!kindDic[KeyKind.Bigger])
        {
            kindDic[KeyKind.Bigger] = true;
            transform.localScale = new Vector3(transform.localScale.x + 0.5f, transform.localScale.y + 0.5f, transform.localScale.z + 0.5f);
            jumpPower = 15f;
        }

        TimersManager.SetTimer(this, 10f, () =>
        {
            kindDic[KeyKind.Bigger] = false;
            transform.localScale = initScale;
            jumpPower = baseJumpPower;
        });
    }


    Coroutine beforeSpeedCol;
    public void Speed(KeyKind keyKind)
    {
        if (keyKind == KeyKind.Speeder && kindDic[KeyKind.BigSpeeder]) return;

        float speed = keyKind == KeyKind.BigSpeeder ? baseWalkSpeed *2.5f: baseWalkSpeed*2;

        if (!kindDic[keyKind])
        {
            kindDic[keyKind] = true;
            walkSpeed = speed;
        }

        if (beforeSpeedCol != null)
            StopCoroutine(beforeSpeedCol);

        beforeSpeedCol = StartCoroutine(ResetSpeed(keyKind));
    }

    IEnumerator ResetSpeed(KeyKind keyKind)
    {
        yield return new WaitForSeconds(5f);

        Debug.Log("スピードもとに戻します！！");
        kindDic[keyKind] = false;
        walkSpeed = baseWalkSpeed;
    }

    Coroutine beforeRangeCol;
    public void RangeSpreed(KeyKind keyKind)
    {
        if (keyKind == KeyKind.RangeSpreeder && kindDic[KeyKind.BigRangeSpreeder]) return;

        int rad = keyKind == KeyKind.BigRangeSpreeder ? 5 : 3;
        spleedObj.SetActive(true);
        spleedObj.transform.Find("range").localScale = Vector3.one * (5*rad);

        if (!kindDic[keyKind])
        {
            kindDic[keyKind] = true;
            getCol.radius = rad;
        }

        if (beforeRangeCol != null)
            StopCoroutine(beforeRangeCol);

        beforeRangeCol = StartCoroutine(ResetRange(keyKind));
    }

    IEnumerator ResetRange(KeyKind keyKind)
    {
        yield return new WaitForSeconds(10f);
        kindDic[keyKind] = false;
        getCol.radius = baseColSize;
        spleedObj.SetActive(false);
    }

    void Observe()
    {

    }


    // Update is called once per frame
    void Update()
    {

        if (openCharacterController == null || gameManager.isJustStop) return;

        if (Input.GetKeyDown(KeyCode.A))//  もし、スペースキーがおされたら、
        {
            Jump();
        }

        //動作フラグ
        movingFlg = mobileVector2 != Vector2.zero;

        Moving(new Vector3(mobileVector2.x, 0, mobileVector2.y));
　
        if (CheckGrounded())
        {

            if (movingFlg)
            {
                gameManager.isStart = true;
                SetTrigger("run");
            }
            else
            {
                SetTrigger("idle");
            }
        }
        else
        {
            SetTrigger("jump");
        }


        if (transform.position.y < -10)
        {
            gameManager.JustStop();
        }
    }

    /// <summary>
    /// 移動処理
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    private void Moving(Vector3 moveDirection)
    {
        moveDirection *=　0.1f;

        moveDirection *= walkSpeed;


        if (moveDirection != Vector3.zero)
        {
            var lookrotation2 = Quaternion.LookRotation(moveDirection, Vector3.up);
            lookrotation2.x = 0;
            lookrotation2.z = 0;
            transform.rotation = Quaternion.Lerp(transform.rotation, lookrotation2, Time.deltaTime * 1);
        }

        //moveDirection.y += Physics.gravity.y * Time.deltaTime;

        openCharacterController.Move(moveDirection);        
    }

    public void Jump()
    {
        if (openCharacterController.isGrounded)
        {
         
            Debug.Log("ジャンプするよ");
            rigidbody.AddForce(Vector3.up * jumpPower, ForceMode.VelocityChange);
        }
    }


    public void MobileSet(Vector2 vector2)
    {
        mobileVector2 = vector2;
        //Debug.Log("mobileVector2:" + mobileVector2);
    }


    public void Reset()
    {
        transform.localPosition = initPostion;
        base.Reset();
    }
}
