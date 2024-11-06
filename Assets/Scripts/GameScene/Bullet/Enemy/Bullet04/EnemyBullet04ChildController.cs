using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet04ChildController : MonoBehaviour //4번 적 오브젝트로부터 발사된 각각의 총알을 관리하는 클래스
{
	[SerializeField] private float enemyBulletSpeed; //총알 속도 변수
	[SerializeField] private float enemyDamage; //총알 데미지 변수 : 적 오브젝트가 플레이어와 충돌했을 시의 데미지 값과 동일함

	private EnemyBullet04Controller enemyBullet04Controller; //부모 오브젝트의 컨트롤러 스크립트 : CountBullet 메소드 호출에 쓰임

	#region Setting Methods

	public void SetBulletData(EnemyBullet04Controller enemyBullet04Controller, float enemyBulletSpeed, float enemyDamage) //총알 정보를 세팅하는 메소드
	{
		this.enemyBullet04Controller = enemyBullet04Controller; //부모 오브젝트의 컨트롤러 스크립트 저장
		this.enemyBulletSpeed = enemyBulletSpeed; //총알 속도 변수 저장
		this.enemyDamage = enemyDamage; //총알 데미지 변수 저장
	}

	#endregion


	#region Action Methods

	public void Move() //총알 이동 메소드
	{
		transform.Translate(Vector3.up * enemyBulletSpeed * Time.deltaTime); //총알 이동 : 각각의 총알이 Vector3.up을 향하여 산탄처럼 흩어짐
	}

	#endregion

	private void OnTriggerEnter2D(Collider2D collision)
	{
		switch (collision.tag) //트리거는 태그로 구분함
		{
			case "Player": //플레이어 오브젝트와 부딪혔을 경우
				{
					gameObject.SetActive(false); //총알 오브젝트 비활성화
					enemyBullet04Controller.CountBullet(); //부모 오브젝트의 컨트롤러 스크립트 CountBullet 메소드 호출
					collision.transform.GetComponent<PlayerController>().TakeDamage(enemyDamage); //플레이어 컨트롤러 스크립트에 데미지 전달
					break;
				}
			case "DeathWall": //화면 바깥으로 나간 경우 : 플레이어는 상하좌우 모두 나가면 안되지만 적은 우측 방향에서 진행하기 때문에 오브젝트와 태그를 분리해놓음
			case "DeathWall_Enemy": //하지만 총알은 각자 다른 알고리즘이 있기 때문에 우측으로 나가는 상황이 발생해도 비활성화되야함
				{
					gameObject.SetActive(false); //총알 오브젝트 비활성화
					enemyBullet04Controller.CountBullet(); //부모 오브젝트의 컨트롤러 스크립트 CountBullet 메소드 호출
					break;
				}

		}
	}

	
}
