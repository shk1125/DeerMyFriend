using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBullet01Controller : PlayerBulletController //플레이어의 01번째 총알을 관리하는 클래스
{
	public PlayerBullet01DetectionAreaController playerBullet01DetectionAreaController; //적 오브젝트 감지 영역을 관리하는 스크립트 변수 : 인스펙터에서 등록

	[SerializeField] float directionWeight; //총알 방향 전환 강도

	private bool enemyTargeted; //적이 타겟팅 되었는지 확인하는 변수
	private Transform enemyTransform; //타겟팅된 적의 Transform 변수
	private Vector2 bulletDirectionVector; //총알 방향 벡터 : 적이 타겟팅되면 적을 향해야 함
	
	
	private void Start()
	{
		enemyTargeted = false; //적 타겟팅 확인 변수 초기화
		directionWeight = 6f; //총알 방향 전환 강도 초기화
	}

	#region Action Methods

	public void TargetEnemy(bool enemyDetected, Transform enemyTransform) //타겟팅된 적 변수를 저장하는 메소드
	{
		this.enemyTransform = enemyTransform; //적 오브젝트의 Transform 저장
		this.enemyTargeted = enemyDetected; //타겟팅된 상태 변수 저장 : 그냥 true를 주지 않는 이유는 ReleaseTarget 메소드에서도 TargetEnemy 메소드를 호출하기 때문에 null, false가 전달될 수도 있음
	}

	protected override void Move() //총알 이동 메소드
	{
		if (enemyTargeted) //적이 타겟팅되었을 시
		{
			MoveWithTarget(); //타겟팅된 적을 향해 이동하는 메소드 호출
		}
		else
		{
			MoveWithoutTarget(); //평소 상태로 이동하는 메소드 호출
		}
	}


	private void MoveWithoutTarget() //평소 상태로 이동하는 메소드
	{
		transform.Translate(Vector3.up * bulletSpeed * Time.deltaTime); //Vector3.up 방향으로 이동
	}

	private void MoveWithTarget() //타겟팅된 적을 향해 이동하는 메소드
	{
		transform.Translate(Vector3.up * bulletSpeed * Time.deltaTime); //Vector3.up 방향으로 이동
		bulletDirectionVector = (enemyTransform.position - transform.position).normalized; //적 방향으로 벡터 변수 저장
		transform.up = Vector3.Slerp(transform.up.normalized, bulletDirectionVector, directionWeight * Time.deltaTime); //총알 방향이 적을 향하도록 변경. directionWeight 변수로 강도 조절
	}

	public override void ReleaseTarget() //타겟팅된 적을 해제하는 메소드
	{
		playerBullet01DetectionAreaController.ReleaseTarget(); //적 오브젝트 감지 영역을 관리하는 스크립트의 ReleaseTarget 메소드 호출
		//두 메소드가 하나로 관리되지 못하는 이유는 총알 오브젝트가 풀링에 회수될 때 감지 영역을 관리하는 스크립트에 접근하지 않고 현 스크립트의 ReleaseTarget 메소드에 접근하기 때문
		//감지 영역을 관리하는 스크립트에도 타겟팅된 적을 해제하는 알고리즘이 따로 필요함. 따라서 현 스크립트에 쓰일 알고리즘의 호출을 감지 영역을 관리하는 스크립트에 작성하여 중복 방지
		//또한 적 오브젝트의 경우 감지 영역을 관리하는 스크립트의 ReleaseTarget 메소드를 호출
	}

	#endregion

}
