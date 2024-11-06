using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy02Controller : EnemyController //02번 적 오브젝트를 관리하는 클래스
{ 
	private Vector2 enemyDirectionVector; //방향 벡터 변수
	
	private void Start()
	{
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
		if (!targeted) //플레이어 오브젝트를 발견했을 시 : 02번 적은 화면 안에 들어오자마자 플레이어를 향하는 알고리즘임
		{
			enemyDirectionVector = (playerController.transform.position - transform.position).normalized; //플레이어 오브젝트를 향한 벡터 계산
			transform.right = -enemyDirectionVector; //플레이어 오브젝트를 향해 방향 전환
			targeted = true; //플레이어 오브젝트 발견 변수 저장 : 한 번 플레이어를 향해 방향을 고정하면 그대로 직진하는 알고리즘이기 때문에 조건문은 1번만 작동해야함
		}
		if (targeted)
		{
			MoveInsideDefault(); ////02번 적 오브젝트는 플레이어 오브젝트를 향해 방향을 고정한 후 기본 이동 메소드를 그대로 이용함
		}
	}

	#endregion



	
}
