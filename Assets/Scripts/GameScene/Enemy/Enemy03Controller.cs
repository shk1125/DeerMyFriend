using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy03Controller : EnemyController //03번 적 오브젝트를 관리하는 클래스
{
	private Rigidbody2D enemyRigidbody; //적 오브젝트 rigidbody 변수
	
	private void Start()
	{
		enemyRigidbody = GetComponent<Rigidbody2D>(); //적 오브젝트 rigidbody 변수 저장
		targeted = false; //플레이어 오브젝트 발견 변수 저장
	}

	#region Setting Methods

	protected override void SetEnemyBullet(double enemyBulletSpeed) //적 총알 세팅 메소드 : 총알을 발사하는 알고리즘이 있는 적 스크립트(예시 : 04번 적 오브젝트)를 위한 메소드. 추상 클래스에서 선언했기 때문에 상속만 받고 구현하지 않음
	{

	}

	#endregion

	#region Action Methods

	protected override void MoveInside() //화면 안에서 이동하는 메소드
	{
		if (!targeted)
		{
			MoveInsideDefault(); //03번 적 오브젝트는 플레이어 오브젝트 발견 전 기본 이동 메소드를 그대로 이용함
		}
		if (transform.position.x <= playerController.transform.position.x) //03번 적은 플레이어 오브젝트의 위(y축 기준)에서 rigidbody의 gravity를 기준으로 추락하는 알고리즘임
		{
			targeted = true; //플레이어 오브젝트 발견 변수 저장 : 이동을 멈추고 아래(y축 기준)으로 추락함
			enemyRigidbody.gravityScale = 1f; //0이었던 rigidbody 컴포넌트의 gravityScale을 1로 변경
		}

	}

	#endregion

	
}
