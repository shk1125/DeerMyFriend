using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBullet00Controller : PlayerBulletController //플레이어의 00번째 총알을 관리하는 클래스
{

	#region Action Methods
	protected override void Move() //총알 이동 메소드
	{
		transform.Translate(Vector3.up * bulletSpeed * Time.deltaTime); //Vector3.up 방향으로 총알 이동
	}

	public override void ReleaseTarget() //타겟팅한 적 변수 초기화 : 적을 타겟팅하는 알고리즘이 있는 총알(예시 : 01번째 총알) 스크립트를 위한 메소드. 추상 클래스에서 선언했기 때문에 상속만 받고 구현하지 않음
	{
		
	}
	#endregion
}
