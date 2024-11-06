using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy04Controller : EnemyController //04번 적 오브젝트를 관리하는 클래스
{

	public GameObject enemyBulletPrefab; //적 총알 프리팹 : 인스펙터에서 등록

	private bool moveUp; //이동 방향을 결정하는 변수

	private void Start()
	{
		moveUp = true; //이동 방향을 결정하는 변수 저장
	}

	#region Setting Methods

	protected override void SetEnemyBullet(double enemyBulletSpeed) //적 총알 세팅 메소드 
	{
		GameManager.instance.enemyBulletPoolingController.GenerateBulletPool(enemyBulletPrefab, enemyBulletSpeed, enemyDamange, enemyNum); //적 총알 풀링 생성
	}

	#endregion

	#region Action Methods

	protected override void MoveInside() //화면 안에서 이동하는 메소드
	{
		if (transform.position.x > 6.0f) //화면 안이라는 개념은 DeathWall 오브젝트를 기준으로 하는데 04번 적은 좀 더 안쪽으로 들어와 알고리즘을 작동시켜야 한다. 따라서 x축 기준으로 6.0f까지 기본 이동 상태를 유지한다.
		{
			MoveInsideDefault(); //기본 이동 상태 유지
		}
		else //알고리즘 시작 기준점까지 이동했을 시
		{
			if (transform.position.y > 3.0f) //3.0f까지 위쪽(y축 기준)으로 이동했을 시
			{
				moveUp = false; //이동 방향을 아래(y축 기준)로 변경
			}
			else if (transform.position.y < -3.0f) //-3.0f까지 아래쪽(y축 기준)으로 이동했을 시
			{
				moveUp = true; //이동 방향을 위(y축 기준)로 변경
			}

			if (moveUp) 
			{
				transform.Translate(Vector3.up * enemySpeed * Time.deltaTime); //위(y축 기준)로 이동
			}
			else
			{
				transform.Translate(Vector3.down * enemySpeed * Time.deltaTime); //아래(y축 기준)로 이동
			}
		}
	}


	public void Attack() //공격 애니메이션 실행 메소드 : 총알 발사 주기가 애니메이션에 이벤트로 등록되어 있음, 너무 빠르면 발사 주기를 분리해서 로직 구성 가능성 있음
	{
		if (arrivedToStage) //화면 안에서 이동할 때만 총알 발사 애니메이션 작동
		{
			enemyAnimator.SetTrigger("Attack"); //애니메이터에 Attack 트리거 호출
		}
	}

	public void Shoot() //총알 오브젝트를 발사하는 메소드 : 애니메이션의 이벤트로 호출됨
	{
		ChangeAudioClip(shootAudioClip); //총알 발사 효과음 실행

		var bullet = GameManager.instance.enemyBulletPoolingController.bulletPoolDictionary[enemyNum].Get(); //총알 오브젝트 풀링 호출
		bullet.transform.position = this.transform.position; //총알 position을 현재 적 오브젝트의 position으로 이동
	}


	#endregion



}
