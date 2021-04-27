using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ButtonImageSetting : MonoBehaviour
{
    [SerializeField] List<TextMeshProUGUI> buttonText;
    [SerializeField] string settingText = string.Empty;


    public void Initialize( string buttonName )
    {
        buttonText.ForEach( x=> x.text = buttonName );
    }

    private void Start()
    {
        if(!string.IsNullOrEmpty(settingText))
        {
            Initialize(settingText);
        }
    }
}
