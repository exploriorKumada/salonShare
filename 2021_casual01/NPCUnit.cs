using System.Collections;
using System.Collections.Generic;
using StandardAssets.Characters.Physics;
using Timers;
using UnityEngine;
using UnityEngine.AI;
using UniRx;

public class NPCUnit : CharacterBase
{
    StickManManager stickManManager;
    NavMeshAgent agent;

    public int id;
    public float lifeSecond;
    int hani = 50;

    public void Init(StickManManager _stickManManager,int _id, GameManager _gameManager)
    {
        id = _id;
        gameManager = _gameManager;
        agent = GetComponent<NavMeshAgent>();
        stickManManager = _stickManManager;
        openCharacterController = GetComponent<OpenCharacterController>();
        charaAttribute = TW.GameSetting.CharaAttribute.Enemy;
        animator = GetComponent<Animator>();

        colEventFunction.enterAction = () =>
        {
            if (isTouch) return;

            isTouch = true;
            gameManager.JustStop();
        };

        transform.localPosition = new Vector3(transform.localPosition.x, 0, transform.localPosition.z);
        agent.enabled = true;
        BaseInit();

        this.ObserveEveryValueChanged( x => isStart)
            .Subscribe(_ => LifeSet());

    }


    public void BaseInit()
    {
        SetPosition();
        base.BaseInit();
    }

    private void Update()
    {
        if (openCharacterController == null || isTouch || !isStart) return;

        if (agent.pathStatus != NavMeshPathStatus.PathInvalid)
        {
            agent.destination = stickManManager.transform.position;
            SetTrigger("run");
        }

        if (transform.position.y < -200)
        {
            Reset();
        }
    }

    public void LifeSet()
    {
        lifeSecond = Random.RandomRange(5f, 20f);
        TimersManager.SetTimer(this, lifeSecond, Reset);
    }

    public void Reset()
    {
        base.Reset();
        SetPosition();
        LifeSet();
    }

    public void SetPosition()
    {
        var playerPosi = gameManager.stickManManager.transform.localPosition;
        var setPosi = new Vector3(Random.RandomRange(-hani, hani), 50, Random.RandomRange(-hani, hani));
        float dis = Vector3.Distance(setPosi, playerPosi);
        //Debug.Log("dis:" + dis);
        if (dis < 30)
        {
            Debug.Log("位置再版");
            SetPosition();
            return;
        }

        gameObject.SetActive(true);
        transform.localPosition = setPosi;
    }

}
