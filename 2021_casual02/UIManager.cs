using System.Collections;
using System.Collections.Generic;
using MalbersAnimations;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] Camera _camera;
    [SerializeField] MobileJoystick mobileJoystick;

    Vector3 tapPosi = Vector3.zero;

    public void SetMove()
    {
        mobileJoystick.transform.localPosition = tapPosi;
        //mobileJoystick.OnXAxisChange.Invoke();
    }

    void Update()
    {
        // マウスのワールド座標
        tapPosi = _camera.ScreenToWorldPoint(Input.mousePosition + _camera.transform.forward * 50);
        tapPosi = tapPosi * 200;
        tapPosi = new Vector3(tapPosi.x, tapPosi.y, -1000);
    }

}
