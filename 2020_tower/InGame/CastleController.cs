using System;
using System.Collections;
using System.Collections.Generic;
using TW.GameSetting;
using UnityEngine;

public class CastleController : CharacterBase
{
    public void Initialized(Action _resultCheack)
    {
        resultCheck = _resultCheack;
        charaAttribute = CharaAttribute.Castel;
        base.Initialize(false);
        //https://loumo.jp/archives/6404
        rigidbody.constraints = RigidbodyConstraints.FreezeAll;

        DebugSet();
    }

    private void Update()
    {
        //if(characterController.isGrounded)
        //{

        //}
    }

    public void DebugSet()
    {
        charaInfo = new CharaInfo(DataManager.Instance.GetCharaMaster(1),1){};
        charaInfo.maxHP = charaInfo.base_hp;
    }


}
