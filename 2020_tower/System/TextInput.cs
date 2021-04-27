using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using TW.GameSetting;
using System;

public class TextInput : DialogSystem
{
    [SerializeField] TextMeshProUGUI title;
    [SerializeField] TextMeshProUGUI inputText;
    [SerializeField] public TextMeshProUGUI discription;

    public Action pushAction;

    public TextMeshProUGUI Initilize(string _title, string _discription, Action _pushAction)
    {
        title.text = _title;
        discription.text = _discription;
        pushAction = _pushAction;
        return inputText;
    }

    public void PushAction()
    {
        pushAction?.Invoke();
        Close();
    }
}
