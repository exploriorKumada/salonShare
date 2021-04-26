using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bttale : ScenePrefab {

	private string acitonValueText;
	private string acitonTargetText;
	private string acitonNumberText;

	int selfAttack =60;
	int selfHP =2200;
	string selfType ="red";

	int enemyAttack =37;
	int enemyHP =800;
	string enemyType ="red";

	// Use this for initialization
	List<string> actionNumber = new List<string>
	{
		"repaire,blue,1.2",
		"damage,red,1.2",
		"repaire,all,0.7",
		"damage,blue,1",
		"repaire,all,0.5",
		"miss",


	};
	void Start() 
	{
		//ClickEventStart ("Button", () => {
		//	DiceSet();
		//});
			
	}


	private void DiceSet()
	{
		SetAttack( Random.Range(0,6) );
	}


	private void SetAttack( int number )
	{	
		Debug.Log("この出目" + number );

		//Debug.Log( actionNumber[number] );

		var splited = actionNumber[number].Split( ',' );
		
		if( splited[0] == "damage" )
		{
			AttackAction( splited );
		}

		if( splited[0] == "repaire" )
		{
			RepaireAction( splited );
		}


	}


	private void AttackAction( string[] splited )
	{

		int damage = (int)( selfAttack * float.Parse( splited[2])) ;

		Debug.Log( ConvertTarget( splited[1] ) + "に攻撃力の" + damage + "のダメージ" );

		if( splited[1] == "all" || splited[1] == enemyType )
		{
			enemyHP = enemyHP - damage;

			if( enemyHP >=0 )
			{
				enemyHP=0;
				WinProsess();
				return;
			}
		}

		Debug.Log("敵のHPは" + enemyHP );
	

	}


	private void RepaireAction( string[] splited )
	{
		Debug.Log( ConvertTarget( splited[1]) + "に" + ( 100 * ( float.Parse( splited[2]) ) ) + "%の回復" );

		if( splited[1] == "all" || splited[1] == selfType )
		{	
			selfHP =(int)( selfHP * ( float.Parse( splited[2]) + 1f ) );	
		}

		Debug.Log("自分のHPは" + selfHP);


	}

	private string ConvertValue( string valueText )
	{
		if( valueText == "damage")
		{
			return "攻撃";
		}else if( valueText == "repaire")
		{
			return "回復";
		}else if( valueText == "miss")
		{
			return "ミス";
		}

		return "エラー";

	}


	private string ConvertTarget( string targetText )
	{
		if( targetText == "blue")
		{
			return "青";
		}else if( targetText == "red")
		{
			return "赤";
		}else if( targetText == "yellow")
		{
			return "黄";
		}else if( targetText == "all" )
		{
			return "全員";
		}

		return "エラー";

	}


	private void WinProsess()
	{
		Debug.Log("勝ちました");

	}


}
