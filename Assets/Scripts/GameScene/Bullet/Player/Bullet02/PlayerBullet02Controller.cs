using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBullet02Controller : PlayerBulletController //플레이어의 02번째 총알을 관리하는 클래스
{
	public PlayerBullet02ChildController[] bulletChildArray; //발사된 총알들을 각자 관리하는 자식 오브젝트의 컨트롤러 스크립트 배열 : 인스펙터에서 등록



	private void Start()
	{
		for (int i = 0; i < bulletChildArray.Length; i++)
		{
			bulletChildArray[i].SetBulletData(bulletSpeed, bulletDamage); //각각의 총알 오브젝트의 컨트롤러 스크립트에 데이터 전달
		}
	}

	private void OnEnable() //풀링에서 총알 오브젝트를 가져와 활성화했을 때 작동하는 메소드
	{
		for (int i = 0; i < bulletChildArray.Length; i++)
		{
			bulletChildArray[i].transform.position = transform.position; //총알이 발사되는 시점에서 적 오브젝트의 위치로 총알 오브젝트들을 이동
			bulletChildArray[i].gameObject.SetActive(true); //각각의 총알 오브젝트 활성화 : 총알은 적에게 부딪히거나 DeathWall에 부딪힐 경우 비활성화된 채로 풀링에 돌아가기 때문
		}
	}

	public override void ReleaseTarget() //타겟팅한 적 변수 초기화 : 적을 타겟팅하는 알고리즘이 있는 총알(예시 : 01번째 총알) 스크립트를 위한 메소드. 추상 클래스에서 선언했기 때문에 상속만 받고 구현하지 않음
	{

	}
	 
	#region Action Methods

	protected override void Move() //총알 이동 메소드
	{
		for (int i = 0; i < bulletChildArray.Length; i++)
		{
			bulletChildArray[i].Move(); //각각의 총알 오브젝트 이동을 호출
		}
	}


	public void CountBullet() //비활성화된 총알 오브젝트들의 개수를 확인하고 전부 비활성화되었을 경우 풀링에서 회수하는 메소드 : 총알 오브젝트들 각각이 따로 날아가서 비활성화되기 때문에 매번 체크해야 함
	{
		for (int i = 0; i < bulletChildArray.Length; i++)
		{
			if (bulletChildArray[i].gameObject.activeSelf)
			{
				return; //아직 활성화된 총알이 남아있을 경우 풀링이 회수하면 안되기 때문에 return
			}
		}
		if (gameObject.activeSelf) //Pool.Release에서 이미 Release가 된 것을 다시 Release했다는 오류가 발생. 검색 결과 유니티 엔진의 버그로 추정. 조건문으로 방지한 상태.
		{
			Pool.Release(gameObject); //풀링이 오브젝트 회수
		}
	}

	#endregion
}
