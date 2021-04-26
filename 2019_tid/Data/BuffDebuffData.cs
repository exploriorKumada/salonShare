using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffDebuffData{
	public string charaId;

    /// <summary>
    /// 完了ターン
    /// </summary>
	public int turn;

    /// <summary>
    /// 影響範囲
    /// </summary>
	public string type;

    /// <summary>
    /// 影響量
    /// </summary>
	public float amount;

    /// <summary>
    /// 何のバフ 1:attack 2:guard 3:damageRepair
    /// </summary>
	public int buffDebuffType;

    /// <summary>
    /// 説明文
    /// </summary>
	public string text;

    /// <summary>
    ///バフかデバフか 3:buff 4;debuff
    /// </summary>
    public int buffDebuffID;


    public string ｄescription;
}
