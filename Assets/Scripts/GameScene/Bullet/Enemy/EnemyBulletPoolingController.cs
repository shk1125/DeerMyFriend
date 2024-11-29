using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class EnemyBulletPoolingController : MonoBehaviour //적 총알 오브젝트의 풀링을 관리하는 클래스
{
	public Dictionary<int, IObjectPool<GameObject>> bulletPoolDictionary; //적 총알 풀링 딕셔너리 : 적 오브젝트마다 총알이 다르기 때문에 똑같은 풀링을 호출할 수 없다. 따라서 딕셔너리로 분리해놓음.
	public IObjectPool<GameObject> bulletPool { get; private set; } //적 총알 컨트롤러 스크립트에 전달할 풀링 변수

	[Header("Pool Size Variables")]
	[SerializeField] private int defaultPoolSize = 10; //기본 풀링 사이즈 변수
	[SerializeField]  private int maxPoolSize = 20; //최대 풀링 사이즈 변수

	private GameObject enemyBulletPrefab; //적 총알 오브젝트 프리팹 : 생성되는 적마다 다른 프리팹을 가지고 있기 때문에 전역변수로 선언해두고 GenerateBulletPool 메소드에서 지정
	private double enemyBulletSpeed; //적 총알 속도 변수 : double인 이유는 Litjson이 float을 지원하지 않기 때문
	private float enemyDamage; //적 총알 데미지 변수 : 총알 속도 변수가 double인데 총알 데미지 변수가 float인 이유는 총알 데미지 값은 적 오브젝트와 부딪혔을 때의 데미지 값과 동일하기 때문에 그 전 단계에서 형변환이 끝났기 때문

	

	public void GenerateBulletPool(GameObject enemyBulletPrefab, double enemyBulletSpeed, float enemyDamange, int enemyNum) //적 총알 오브젝트 풀링 생성 메소드
	{
		bulletPool = new ObjectPool<GameObject>(CreatePooledItem, OnTakeFromPool, OnReturnedToPool, OnDestroyPoolObject, true, defaultPoolSize, maxPoolSize); //적 총알 오브젝트 풀링 생성
		this.enemyBulletPrefab = enemyBulletPrefab; //적 총알 프리팹 저장
		this.enemyBulletSpeed = enemyBulletSpeed; //적 총알 속도 저장
		this.enemyDamage = enemyDamange; //적 총알 데미지 저장
		for(int i = 0; i < defaultPoolSize; i++)
		{
			EnemyBulletController enemyBulletController = CreatePooledItem().GetComponent<EnemyBulletController>(); //기본 풀링 사이즈만큼 총알 오브젝트 생성 후 풀링에 저장
			enemyBulletController.Pool.Release(enemyBulletController.gameObject); //총알 오브젝트들은 비활성화 상태로 대기
		}

		if (bulletPoolDictionary == null) //딕셔너리가 생성되지 않았을 경우 : 총알을 요구하는 적 오브젝트들마다 각자 GenerateBulletPool 메소드를 호출하기 때문에 딕셔너리가 없거나
										  //여러번 생성되는 상황을 방지해야 함. 정적 변수와 비슷한 알고리즘
		{
			bulletPoolDictionary = new Dictionary<int, IObjectPool<GameObject>>(); //딕셔너리 생성
		}
		

		if (!bulletPoolDictionary.ContainsKey(enemyNum)) //딕셔너리에 총알 풀링이 준비되지 않은 경우 : 똑같은 번호의 적이 여러마리 존재할 경우 풀링을 여러 번 생성할 필요 없이 기존의 풀링을 쓴다
		{
			bulletPoolDictionary.Add(enemyNum, bulletPool); //딕셔너리에 적 번호를 key로, 총알 풀링을 value로 추가
		}
	}

	public void ReleaseBulletPool() //총알 풀링을 회수하는 메소드 : 메인 메뉴로 돌아가기를 선택했을 시 로딩 화면에서 오브젝트들을 치워야 하고 따라서 풀링을 수동적으로 회수해야 하기 때문에 만듦
	{
		EnemyBulletController enemyBulletController; //적 총알 컨트롤러 스크립트 선언
		for (int i = 0; i < transform.childCount; i++) //적 총알 오브젝트 풀링은 자식 오브젝트로 등록되어 있기 때문에 수동적으로 회수하려면 자식에 접근하면 된다.
		{
			enemyBulletController = transform.GetChild(i).GetComponent<EnemyBulletController>(); //자식 오브젝트의 적 총알 컨트롤러 스크립트 저장
			if(enemyBulletController.gameObject.activeSelf) //Pool.Release에서 이미 Release가 된 것을 다시 Release했다는 오류가 발생. 검색 결과 유니티 엔진의 버그로 추정. 조건문으로 방지한 상태.
			{
				enemyBulletController.Pool.Release(enemyBulletController.gameObject); //적 총알 컨트롤러 스크립트에 저장된 풀링 변수로 회수
			}
		}
	}


	private GameObject CreatePooledItem() //적 총알 오브젝트 생성 메소드
	{
		EnemyBulletController enemyBulletController = Instantiate(enemyBulletPrefab, transform).GetComponent<EnemyBulletController>(); //적 총알 오브젝트 생성 후 컨트롤러 스크립트 저장
		enemyBulletController.SetEnemyBulletData(enemyBulletSpeed, enemyDamage, bulletPool); //적 총알 컨트롤러 스크립트의 세팅 메소드 호출

		return enemyBulletController.gameObject; //적 총알 오브젝트를 풀링에 추가함
	}


	private void OnTakeFromPool(GameObject bulletPool) // 풀링 사용 메소드
	{
		bulletPool.SetActive(true);
	}


	private void OnReturnedToPool(GameObject bulletPool) // 풀링 반환 메소드
	{
		bulletPool.SetActive(false);
	}


	private void OnDestroyPoolObject(GameObject bulletPool) // 풀링 삭제 메소드
	{
		Destroy(bulletPool);
	}
}
