using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class PlayerBulletPoolingController : MonoBehaviour //플레이어의 총알 풀링을 관리하는 클래스
{
    public GameObject[] bulletPrefabArray; //플레이어 총알 프리팹 배열 : 인스펙터에서 등록
	public IObjectPool<GameObject> currentBulletPool { get; private set; } //현재 발사하는 총알 오브젝트 풀링 변수
	public float bulletFireRate; //총알 발사 주기

	[Header("Pool Size Variables")]
	[SerializeField] private int defaultPoolSize = 10; //기본 풀링 사이즈 변수
	[SerializeField] private int maxPoolSize = 20; //최대 풀링 사이즈 변수

    private Dictionary<int, IObjectPool<GameObject>> bulletPoolDictionary; //플레이어 총알 풀링 딕셔너리 : 총알이 변경될 때마다 딕셔너리에서 풀링 호출

	private Transform playerBulletPoolTransform; //생성된 총알 오브젝트가 자식으로 들어갈 Transform
	private PlayerBulletData[] playerBulletDataArray; //플레이어 총알 데이터 struct 배열

	
    private int bulletNum; //총알 번호 변수 : 총알 풀링 딕셔너리의 key로 사용됨


	public void ChangeBulletPool(int bulletNum) //플레이어의 총알 오브젝트를 변경하는 메소드
    {
        this.bulletNum = bulletNum; //총알 번호 변수 저장
        currentBulletPool = bulletPoolDictionary[this.bulletNum]; //총알 풀링 딕셔너리에서 총알 번호를 key로 가져와 현재 발사하는 총알 오브젝트 풀링 변수에 저장
		bulletFireRate = (float)playerBulletDataArray[bulletNum].bulletFireRate; //총알 발사 주기 변수를 총알 데이터 배열에서 가져와 저장 : 총알 데이터 배열의 순서는 총알 번호와 호환되기 때문에 그대로 호출이 가능함
	}
  

	public void GenerateBulletPool(PlayerBulletData[] playerBulletDataArray, Transform playerBulletPoolTransform) //플레이어 총알 오브젝트 풀링 생성 메소드
	{
        this.playerBulletDataArray = playerBulletDataArray; //플레이어 총알 데이터 struct 배열 저장
        this.playerBulletPoolTransform = playerBulletPoolTransform; //생성된 총알 오브젝트가 자식으로 들어갈 Transform 저장
		bulletPoolDictionary = new Dictionary<int, IObjectPool<GameObject>>(0); //딕셔너리 생성
        List<int> bulletList = new List<int>(); //총알 번호 리스트 생성 : 스테이지에 없는 총알은 생성할 필요가 없기 때문에 총알 번호 리스트를 따로 준비

		
		bulletList.Add(0); //총알 번호 리스트에 0 추가 : 0번 무기는 기본 무기이며 기본 무기는 DataHolder에 없을 수도 있기 때문에 Default로 먼저 생성해놓음
		IObjectPool<GameObject> bulletPool = new ObjectPool<GameObject>(CreatePooledItem, OnTakeFromPool, OnReturnedToPool, OnDestroyPoolObject, true, defaultPoolSize, maxPoolSize); //플레이어 총알 오브젝트 풀링 생성
        bulletPoolDictionary.Add(0, bulletPool); //총알 풀링 딕셔너리에 0번 key로 풀링 1개 추가

		for (int i = 0; i < DataHolder.stageData_Bullet.Length; i++)
        {
            if (!bulletList.Contains(DataHolder.stageData_Bullet[i].bulletNum)) //총알 번호 리스트에 번호가 들어있지 않다면 : 이미 생성된 번호는 추가할 필요가 없기 때문
            {
                bulletList.Add(DataHolder.stageData_Bullet[i].bulletNum); //DataHolder에서 총알 번호를 호출해 총알 번호 리스트에 추가
            }
        }
		bulletList.Sort(); //총알 번호 리스트 순서 정리


        

        for (int i = 1; i < bulletList.Count; i++) //i를 1에서 시작하는 이유는 0번 총알은 이미 딕셔너리에 추가되었기 때문
        {
            bulletPool = new ObjectPool<GameObject>(CreatePooledItem, OnTakeFromPool, OnReturnedToPool, OnDestroyPoolObject, true, defaultPoolSize, maxPoolSize); //플레이어 총알 오브젝트 풀링 생성
			bulletPoolDictionary.Add(bulletList[i], bulletPool); //총알 번호 리스트의 변수를 key로 딕셔너리에 풀링 추가
        }

        
        for(int i = 0; i < bulletList.Count; i++)
        {
            for(int j = 0; j < defaultPoolSize; j++)
            {
                bulletNum = bulletList[i]; //총알 번호 변수에 총알 번호 리스트의 변수 저장
                currentBulletPool = bulletPoolDictionary[bulletNum]; //현재 풀링 변수에 총알 번호를 key로 받은 딕셔너리의 풀링 변수 저장
				PlayerBulletController playerBulletController = CreatePooledItem().GetComponent<PlayerBulletController>(); ///기본 풀링 사이즈만큼 총알 오브젝트 생성 후 풀링에 저장
				playerBulletController.Pool.Release(playerBulletController.gameObject); //총알 오브젝트들은 비활성화 상태로 대기
			}
        }

		
		bulletNum = 0; //처음 시작하는 총알은 0번 총알로 지정
        currentBulletPool = bulletPoolDictionary[bulletNum]; //현재 총알 풀링을 딕셔너리에서 불러옴
        bulletFireRate = (float)playerBulletDataArray[bulletNum].bulletFireRate; //총알 발사 주기를 총알 데이터 배열에서 호출해 저장
	}

    public void ReleaseBulletPool() //총알 풀링을 회수하는 메소드 : 메인 메뉴로 돌아가기를 선택했을 시 로딩 화면에서 오브젝트들을 치워야 하고 따라서 풀링을 수동적으로 회수해야 하기 때문에 만듦
	{
        PlayerBulletController playerBulletController; //플레이어 총알 컨트롤러 스크립트 선언
		for (int i = 0; i < playerBulletPoolTransform.childCount; i++)
        {
            playerBulletController = playerBulletPoolTransform.GetChild(i).GetComponent<PlayerBulletController>();  //플레이어 총알 오브젝트 풀링은 자식 오브젝트로 등록되어 있기 때문에 수동적으로 회수하려면 자식에 접근하면 된다.
			if (playerBulletController.gameObject.activeSelf) //Pool.Release에서 이미 Release가 된 것을 다시 Release했다는 오류가 발생. 검색 결과 유니티 엔진의 버그로 추정. 조건문으로 방지한 상태.
			{
                playerBulletController.Pool.Release(playerBulletController.gameObject);  //플레이어 총알 컨트롤러 스크립트에 저장된 풀링 변수로 회수
			}
		}
    }


	private GameObject CreatePooledItem() //플레이어 총알 오브젝트 생성 메소드
	{
		PlayerBulletController playerBulletController  = Instantiate(bulletPrefabArray[bulletNum], playerBulletPoolTransform).GetComponent<PlayerBulletController>();  //플레이어 총알 오브젝트 생성 후 컨트롤러 스크립트 저장
		playerBulletController.SetPlayerBulletData(playerBulletDataArray[bulletNum].bulletSpeed, playerBulletDataArray[bulletNum].bulletDamage, currentBulletPool); //플레이어 총알 컨트롤러 스크립트의 세팅 메소드 호출
		
        return playerBulletController.gameObject;  //총알 오브젝트를 풀링에 추가함
	}

    
    private void OnTakeFromPool(GameObject bulletPool) // 풀링 사용
    {
        bulletPool.SetActive(true);
    }

    
    private void OnReturnedToPool(GameObject bulletPool) // 풀링 반환
    {
        bulletPool.SetActive(false);
    }

    
    private void OnDestroyPoolObject(GameObject bulletPool) // 풀링 삭제
    {
        Destroy(bulletPool);
    }

	
}
