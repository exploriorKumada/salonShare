using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class GlobalText : SerializedMonoBehaviour
{
    [SerializeField] List<GlobalTextData> globalTextDatas = new List<GlobalTextData>();
}

public class GlobalTextData
{
    public int id;
    public string jpn;
    public string eng;
}
