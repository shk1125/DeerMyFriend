using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy01Controller : EnemyController //01번 적 오브젝트를 관리하는 클래스
{
	#region Setting Methods

	protected override void SetEnemyBullet(double enemyBulletSpeed) //적 총알 세팅 메소드 : 총알을 발사하는 알고리즘이 있는 적 스크립트(예시 : 04번 적 오브젝트)를 위한 메소드. 추상 클래스에서 선언했기 때문에 상속만 받고 구현하지 않음
	{

	}

	#endregion


	#region Action Methods
	protected override void MoveInside() //화면 안에서 이동하는 메소드
	{
		MoveInsideDefault(); //01번 적 오브젝트는 기본 이동 메소드를 그대로 이용함
	}
	#endregion
	
}
