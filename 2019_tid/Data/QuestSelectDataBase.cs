using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestSelectDataBase {

    public int questId;
    public string name;
    public string limitTime;
    public List<QuestSelectDetailDataBase> questDetailList = new List<QuestSelectDetailDataBase>();

}
