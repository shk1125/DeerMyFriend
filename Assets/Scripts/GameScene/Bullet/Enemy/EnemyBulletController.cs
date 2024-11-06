using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;

public abstract class EnemyBulletController : MonoBehaviour //적 오브젝트가 발사하는 총알 오브젝트를 관리하는 클래스
{
    public IObjectPool<GameObject> Pool { get; set; } //적 오브젝트가 발사하는 총알 오브젝트를 관리하는 풀링 변수

	[SerializeField]  protected float enemyBulletSpeed; //총알 속도 변수
	[SerializeField]  protected float enemyDamage; //총알 데미지 변수 : 적 오브젝트가 플레이어와 충돌했을 시의 데미지 값과 동일함


	#region Setting Methods
	public void SetEnemyBulletData(double enemyBulletSpeed, float enemyDamage, IObjectPool<GameObject> bulletPool) //총알 정보를 세팅하는 메소드
    {
        this.enemyBulletSpeed = (float)enemyBulletSpeed; //총알 속도 변수 저장 : 전달받은 변수가 double인 이유는 Litjson이 float을 지원하지 않기 때문
        this.enemyDamage = enemyDamage; //총알 데미지 변수 저장 : 총알 데미지는 float인 이유는 총알 데미지 값이 적 오브젝트와 플레이어가 충돌했을 시의 데미지 값과 동일하기 때문에 전 단계에서 형변환이 끝났기 때문
		this.Pool = bulletPool; //풀링 변수 저장
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
	protected abstract void Move(); //총알 이동 메소드 : 상속받은 스크립트에서 구현
	#endregion

	private void OnTriggerEnter2D(Collider2D collision)
	{
		switch (collision.tag) //트리거는 태그로 구분함
		{
			case "Player": //플레이어 오브젝트와 부딪혔을 경우
				{
					if (gameObject.activeSelf) //Pool.Release에서 이미 Release가 된 것을 다시 Release했다는 오류가 발생. 검색 결과 유니티 엔진의 버그로 추정. 조건문으로 방지한 상태.
					{
						Pool.Release(this.gameObject); //풀링이 오브젝트 회수
					}
					collision.transform.GetComponent<PlayerController>().TakeDamage(enemyDamage); //플레이어 컨트롤러 스크립트에 데미지 전달
					break;
				}
			case "DeathWall": //화면 바깥으로 나간 경우 : 플레이어는 상하좌우 모두 나가면 안되지만 적은 우측 방향에서 진행하기 때문에 오브젝트와 태그를 분리해놓음
			case "DeathWall_Enemy": //하지만 총알은 각자 다른 알고리즘이 있기 때문에 우측으로 나가는 상황이 발생해도 비활성화되야함
				{
					if (gameObject.activeSelf) //Pool.Release에서 이미 Release가 된 것을 다시 Release했다는 오류가 발생. 검색 결과 유니티 엔진의 버그로 추정. 조건문으로 방지한 상태.
					{
						Pool.Release(this.gameObject); //풀링이 오브젝트 회수
					}
					break;
				}

		}
	}

}
