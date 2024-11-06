using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerBullet01DetectionAreaController : PlayerBulletDetectController //플레이어의 01번째 총알의 감지 영역을 관리하는 클래스
{
	public PlayerBullet01Controller playerBullet01Controller; //부모 오브젝트의 컨트롤러 스크립트

	[SerializeField] private CircleCollider2D circleCollider; //감지 영역의 콜라이더 변수


	private Collider2D[] enemyColliderArray; //감지된 적 오브젝트의 콜라이더 변수 배열
	private bool enemyDetected; //적이 감지되었는지 확인하는 변수
	private EnemyController enemyController; //감지된 적 컨트롤러 스크립트



	private void Start()
	{
		enemyDetected = false; //적 감지 확인 변수 초기화
		enemyColliderArray = Physics2D.OverlapCircleAll(transform.position, circleCollider.radius); //총알이 발사되자마자 감지 영역 내부에 적 오브젝트가 있는지 확인
		CalculateEnemyDistance(); //적 오브젝트와의 거리 계산 메소드 호출
	}

	#region Action Methods

	public override void ReleaseTarget() //감지된 적 해제 메소드
	{
		enemyDetected = false; //적 감지 확인 변수 초기화
		playerBullet01Controller.TargetEnemy(false, null); //부모 오브젝트의 컨트롤러 스크립트의 변수들 초기화
		if(!playerBullet01Controller.gameObject.activeSelf) //총알 오브젝트가 비활성화되었을 시
		{
			playerBullet01Controller.transform.rotation = Quaternion.Euler(0, 0, -90f); //타겟팅된 총알은 적을 향해 방향을 바꾸기 때문에 비활성화되어 풀링에 돌아갈 때 방향 초기화
		}
	}

	private void CalculateEnemyDistance() //적 거리 계산 메소드 : 총알 감지 범위에 적 오브젝트가 여러개 있으면 그 중에 가장 가까운 적을 타겟팅
	{
		float enemyDistance = 0; //거리 비교용 지역변수 초기화
		Collider2D targetedEnemyCollider = null; //현재 타겟팅된 적 지역변수 초기화
		for (int i = 0; i < enemyColliderArray.Length; i++)
		{
			if(Vector2.Distance(transform.position, enemyColliderArray[i].transform.position) < enemyDistance && enemyColliderArray[i].tag == "Enemy") //총알과 적의 거리가 현재 지역변수보다 작으면 앞의 적보다 더 가까움
			{
				targetedEnemyCollider = enemyColliderArray[i]; //현재 타겟팅 된 적 변경
			}
		}
		
		if(targetedEnemyCollider != null) //발사하자마자 감지된 적이 존재할 경우
		{
			enemyController = targetedEnemyCollider.GetComponent<EnemyController>(); //최종적으로 타겟팅된 적 스크립트 저장
			enemyController.Targeted(this); //적이 죽으면 타겟팅을 해제해야 하기 때문에 적 스크립트에 변수를 저장해줌
			playerBullet01Controller.TargetEnemy(true, enemyController.transform); //부모 오브젝트의 컨트롤러 스크립트의 변수 저장
			enemyDetected = true; //이미 타겟팅된 적을 쫓아가기 때문에 다른 적을 찾을 필요가 없어짐
		}
	}

	#endregion

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if(collision.tag == "Enemy" && !enemyDetected)
		{
			enemyController = collision.GetComponent<EnemyController>(); //최종적으로 타겟팅된 적 스크립트 저장
			enemyController.Targeted(this); //적이 죽으면 타겟팅을 해제해야 하기 때문에 적 스크립트에 변수를 저장해줌
			playerBullet01Controller.TargetEnemy(true, enemyController.transform); //부모 오브젝트의 컨트롤러 스크립트의 변수 저장
			enemyDetected = true; //이미 타겟팅된 적을 쫓아가기 때문에 다른 적을 찾을 필요가 없어짐
		}
	}

	private void OnTriggerStay2D(Collider2D collision)
	{
		if(collision.tag == "Enemy" && !enemyDetected)
		{
			enemyColliderArray = Physics2D.OverlapCircleAll(transform.position, circleCollider.radius); //적 타겟팅이 해제된 후에 콜라이더 내부의 다음 적 확인
			CalculateEnemyDistance(); //적 오브젝트와의 거리 계산 메소드 호출
		}
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(transform.position, circleCollider.radius); //감지 영역으로 Gizmo 그림
	}
}
