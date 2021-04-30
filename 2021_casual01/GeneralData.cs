using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GeneralData : SerializedMonoBehaviour
{
    [SerializeField]
    public Dictionary<int, int> numbers;
    public Dictionary<int, GameObject> gameObejcets;
    public Dictionary<int, Image> images;
    public Dictionary<int, TextMeshProUGUI> textMeshProUGUIs;
    public Dictionary<int, Animator> animators;
    public Dictionary<int, Transform> tfs;
    public Dictionary<int, Button> buttons;
    public Action<int> action;
    public Action generalAction;
    public bool flg;

    public void ButtonAction(int num)
    {
        action(num);
    }
}
