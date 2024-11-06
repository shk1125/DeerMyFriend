using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;

public abstract class PlayerBulletController : MonoBehaviour //플레이어의 총알 관리 클래스
{

    public IObjectPool<GameObject> Pool { get; set; } //플레이어가 발사하는 총알 오브젝트를 관리하는 풀 변수

	[SerializeField] protected float bulletSpeed; //총알 속도 변수
    [SerializeField] protected float bulletDamage; //총알 데미지 변수

	#region Setting Methods
	public void SetPlayerBulletData(double bulletSpeed, double bulletDamage, IObjectPool<GameObject> currentBulletPool) //총알 정보를 세팅하는 메소드
	{
        this.bulletSpeed = (float)bulletSpeed; //총알 속도 변수 저장 : 전달받은 변수가 double인 이유는 Litjson이 float을 지원하지 않기 때문. 이하 동일
		this.bulletDamage = (float)bulletDamage; //총알 데미지 변수 저장
		this.Pool = currentBulletPool; //풀링 변수 저장
    }
	#endregion


	private void FixedUpdate()
	{
		if (!GameManager.instance.pause) //현재 게임의 상태가 일시정지가 아닌 경우
		{
            Move(); //총알 이동 메소드 호출
		}
	}

	#region Action Methods
	protected abstract void Move(); //총알 이동 메소드

    public abstract void ReleaseTarget(); //적 타겟팅 해제 메소드 : 적을 타겟팅해야 하는 총알 스크립트에서 상속받아 구현

	#endregion

	private void OnTriggerEnter2D(Collider2D collision)
    {
        switch(collision.tag) //트리거는 태그로 구분함
        {
            case "Enemy": //적 오브젝트와 부딪혔을 경우
                {
					if (gameObject.activeSelf) //Pool.Release에서 이미 Release가 된 것을 다시 Release했다는 오류가 발생. 검색 결과 유니티 엔진의 버그로 추정. 조건문으로 방지한 상태.
					{
						Pool.Release(this.gameObject); //풀링이 오브젝트 회수
					}
					collision.transform.GetComponent<EnemyController>().TakeDamage(bulletDamage); //적 오브젝트에 데미지 전달
					ReleaseTarget(); //타겟팅 알고리즘이 있는 총알의 경우 적 타겟팅 해제 메소드 호출
					break;
                }
            case "DeathWall": //화면 바깥으로 나간 경우 : 플레이어는 상하좌우 모두 나가면 안되지만 적은 우측 방향에서 진행하기 때문에 오브젝트와 태그를 분리해놓음
			case "DeathWall_Enemy": //하지만 총알은 각자 다른 알고리즘이 있기 때문에 모든 방향에서 비활성화되야함
				{
                    if(gameObject.activeSelf) //Pool.Release에서 이미 Release가 된 것을 다시 Release했다는 오류가 발생. 검색 결과 유니티 엔진의 버그로 추정. 조건문으로 방지한 상태.
					{
						Pool.Release(this.gameObject); //풀링이 오브젝트 회수
					}
					ReleaseTarget(); //타겟팅 알고리즘이 있는 총알의 경우 적 타겟팅 해제 메소드 호출
					break;
                }

		}
    }

}
