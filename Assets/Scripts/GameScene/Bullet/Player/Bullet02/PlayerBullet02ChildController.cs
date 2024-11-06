using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBullet02ChildController : MonoBehaviour //플레이어의 02번째 총알들을 관리하는 클래스
{
	public PlayerBullet02Controller playerBullet02Controller; //부모 오브젝트의 컨트롤러 스크립트 : CountBullet 메소드 호출에 쓰임. 인스펙터에서 등록
	[SerializeField] private float bulletSpeed; //총알 속도 변수
	[SerializeField] private float bulletDamage; //총알 데미지 변수

	#region Setting Methods
	public void SetBulletData(float bulletSpeed, float bulletDamage) //총알 정보를 세팅하는 메소드
	{
		this.bulletSpeed = bulletSpeed; //총알 속도 변수 저장
		this.bulletDamage = bulletDamage; //총알 데미지 변수 저장
	}
	#endregion

	#region Action Methods
	public void Move() //총알 이동 메소드
	{
		transform.Translate(Vector3.up * bulletSpeed * Time.deltaTime); //총알 이동 : 각각의 총알이 Vector3.up을 향하여 산탄처럼 흩어짐
	}
	#endregion


	private void OnTriggerEnter2D(Collider2D collision)
	{
		switch(collision.tag) //트리거는 태그로 구분함
		{
			case "Enemy": //적에게 부딪혔을 경우
				{
					gameObject.SetActive(false); //총알 오브젝트 비활성화
					playerBullet02Controller.CountBullet(); //부모 오브젝트의 컨트롤러 스크립트 CountBullet 메소드 호출
					collision.transform.GetComponent<EnemyController>().TakeDamage(bulletDamage); //적 오브젝트에 데미지 전달
					break;
				}
			case "DeathWall": //화면 바깥으로 나간 경우 : 플레이어는 상하좌우 모두 나가면 안되지만 적은 우측 방향에서 진행하기 때문에 오브젝트와 태그를 분리해놓음
			case "DeathWall_Enemy": //하지만 총알은 각자 다른 알고리즘이 있기 때문에 모든 방향에서 비활성화되야함
				{
					gameObject.SetActive(false); //총알 오브젝트 비활성화
					playerBullet02Controller.CountBullet(); //부모 오브젝트의 컨트롤러 스크립트 CountBullet 메소드 호출
					break;
				}
		}
	}
}
