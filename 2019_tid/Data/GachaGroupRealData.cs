using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GachaGroupRealData{
	public long gahaID;
	public string name;
    public string finish_date;
    public string limit_time;

    public Dictionary<int, float> rare_per = new Dictionary<int, float>();
    public float rare1_per;
    public float rare2_per;
    public float rare3_per;
    public float rare4_per;
    public float rare5_per;

    public int pickup1_character_id;
    public float pickup1_per;

}
