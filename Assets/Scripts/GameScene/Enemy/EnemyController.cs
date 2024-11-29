using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class EnemyController : MonoBehaviour //적 오브젝트를 관리하는 클래스
{
	[Header("Audio Objects and Variables")]
	public AudioSource enemyAudioSource; //적 오브젝트 오디오 소스 컴포넌트 : 인스펙터에서 등록
	public AudioClip deathAudioClip; //적이 죽었을 시 실행될 오디오 클립 변수 : 인스펙터에서 등록
	public AudioClip shootAudioClip; //적이 총알을 발사할 시 실행될 오디오 클립 변수 : 인스펙터에서 등록

	[Header("Enemy Variables")]
	[SerializeField] private float enemyHealth; //적 체력 변수
	[SerializeField] protected float enemySpeed; //적 속도 변수
	[SerializeField] protected float enemyDamange; //적 데미지 변수
	[SerializeField] private int enemyScore; //적 점수 변수
	[SerializeField] protected int enemyNum; //적 번호 변수

	protected PlayerController playerController; //플레이어 컨트롤러 스크립트
	protected Animator enemyAnimator; //적 애니메이터 컴포넌트
	protected bool arrivedToStage; //화면 안 도착 확인 변수
	protected bool targeted; //플레이어 오브젝트 발견 변수

	private BoxCollider2D enemyCollider; //적 콜라이더 컴포넌트
	private bool death; //죽음 상태 변수
	private List<PlayerBulletDetectController> playerBulletDetectControllerList; //플레이어 총알 감지 컨트롤러 스크립트
	private bool canGiveDamage = false; //공격 가능 여부 변수
	private float giveDamageInterval = 0.0f; //공격 빈도 변수


	private void Awake()
	{
		enemyAnimator = GetComponent<Animator>(); //적 애니메이터 컴포넌트 저장
		enemyCollider = GetComponent<BoxCollider2D>(); //적 콜라이더 컴포넌트 저장
	}

	private void Start()
	{
		death = false; //죽음 상태 변수 저장
		arrivedToStage = false; //화면 안 도착 확인 변수 저장
	}


	#region Setting Methods


	public void SetEnemyData(double enemyHealth, double enemySpeed, double enemyDamage, bool enemyBullet, double enemyBulletSpeed, int score, int enemyNum, PlayerController playerController) //적 데이터 세팅 메소드
	{
		this.enemyHealth = (float)enemyHealth; //적 체력 변수 저장 : double로 전달받은 이유는 Litjson이 float을 지원하지 않기 때문. 이하 동일
		this.enemySpeed = (float)enemySpeed; //적 속도 변수 저장
		enemyDamange = (float)enemyDamage; //적 데미지 변수 저장
		this.enemyScore = score; //적 점수 변수 저장
		this.enemyNum = enemyNum; //적 번호 변수 저장
		this.playerController = playerController; //플레이어 컨트롤러 스크립트 저장
		if (enemyBullet) //적 오브젝트가 총알을 발사하는 알고리즘을 가지고 있을 시
		{
			SetEnemyBullet(enemyBulletSpeed); //적 총알 속도 변수를 전달하여 세팅 메소드 호출
		}
	}

	protected abstract void SetEnemyBullet(double enemyBulletSpeed); //적 총알 세팅 메소드 : 총알을 발사하는 알고리즘이 있는 스크립트만 구현



	#endregion

	private void FixedUpdate()
	{
		if (!GameManager.instance.pause && !death && !GameManager.instance.countdown) //게임이 일시정지와 카운트다운 상태가 아닐 경우 + 죽은 상태가 아닐 경우
		{
			if (arrivedToStage) //화면 안으로 들어왔을 시
			{
				MoveInside(); //화면 안 이동 메소드 호출
			}
			else
			{
				MoveOutSide(); //화면 밖 이동 메소드 호출
			}
		}
		canGiveDamage = false; //공격 가능 여부 변수 저장
	}

	private void OnTriggerStay2D(Collider2D collision)
	{
		if (collision.tag == "Player")
		{
			canGiveDamage = true; //플레이어 오브젝트가 적 오브젝트와 충돌하고 있는 동안에만 공격 가능
		}
	}


	private void Update()
	{
		if (0.0f < giveDamageInterval && !death) //공격 빈도가 0보다 크고 죽지 않았을 때 : death 변수는 죽음 애니메이션에 관여하는데
												 //그 애니메이션이 작동하는 동안에 플레이어가 데미지를 받으면 안됨
		{
			giveDamageInterval -= Time.deltaTime; //공격 빈도를 deltaTime만큼 감소
		}


		if (canGiveDamage == true) //공격이 가능할 때 : OnTriggerStay2D 메소드가 호출될 때
		{
			if (giveDamageInterval <= 0.0f) //공격 빈도가 0 이하가 되면
			{
				playerController.TakeDamage(enemyDamange); //플레이어 컨트롤러 스크립트에 데미지 전달

				giveDamageInterval = 2.0f; //공격 빈도를 2.0f로 초기화
			}
		}
	}


	#region Action Methods
	protected abstract void MoveInside(); //화면 안 이동 메소드

	protected void MoveInsideDefault() //화면 안 기본 이동 메소드 : 단순히 한쪽 방향으로만 이동하는 알고리즘은 이 메소드를 사용하면 됨
	{
		transform.Translate((transform.right * -1f) * enemySpeed * Time.deltaTime); //왼쪽(적 오브젝트의 transform 기준)으로 이동
	}

	private void MoveOutSide() //화면 밖 이동 메소드 : 화면 밖은 독자적인 알고리즘이 시작되기 전이기 때문에 모든 적 오브젝트가 동일하게 이동
	{
		transform.Translate((transform.right * -1f) * Time.deltaTime); //왼쪽(적 오브젝트의 transform 기준)으로 이동
	}


	public void TakeDamage(float damage) //데미지 받음 메소드
	{
		enemyHealth -= damage; //적 체력이 데미지값만큼 감소
		if (enemyHealth <= 0f) //적 체력이 0 이하가 되면 죽은 것으로 간주
		{
			ChangeAudioClip(deathAudioClip); //죽는 효과음 실행
			enemyAnimator.SetTrigger("Death"); //애니메이터에 Death 트리거 호출
			enemyCollider.enabled = false; //죽는 애니메이션이 재생되는 동안에 총알이 부딪히면 안되기 때문에 콜라이더를 비활성화
			death = true; //죽음 상태 변수 저장
			GameManager.instance.AddScore(enemyScore); //스테이지 점수에 적 점수 추가
		}
	}

	public void Death() //죽음 메소드 : 죽음 애니메이션이 끝나는 순간에 이벤트로 호출됨
	{
		gameObject.SetActive(false); //오브젝트 비활성화

		if (playerBulletDetectControllerList != null)
		{
			ReleaseTarget(playerBulletDetectControllerList); //총알의 적 감지 상태를 해제
		}


		GameManager.instance.AddEnemyDeaths(); //죽은 적 카운트 : 스테이지의 모든 적이 죽었을 경우 게임 클리어 알고리즘 작동
	}

	private IEnumerator GiveDamage() //플레이어에 데미지를 주는 메소드 : 현재 다른 알고리즘으로 대체됨
	{
		while (true)
		{
			playerController.TakeDamage(enemyDamange); //플레이어 컨트롤러 스크립트에 데미지 전달
			yield return new WaitForSeconds(2.0f); //2.0f초 뒤에도 플레이어 오브젝트가 적 오브젝트와 겹쳐있을 경우 다시 데미지 전달
		}
	}
	
	public void PauseEnemy() //게임 일시정지 상태 여부에 따라 애니메이션 실행을 변경하는 메소드
	{
		if (gameObject.activeSelf)
		{
			enemyAnimator.enabled = !enemyAnimator.enabled; //애니메이션 정지 혹은 재실행
		}
	}

	public void Targeted(PlayerBulletDetectController playerBulletDetectController) //플레이어 총알의 감지 알고리즘에 연결된 메소드
	{
		if (playerBulletDetectControllerList == null)
		{
			playerBulletDetectControllerList = new List<PlayerBulletDetectController>(); //총알 컨트롤러 스크립트 리스트가 없을 경우 생성
		}

		this.playerBulletDetectControllerList.Add(playerBulletDetectController); //플레이어의 총알 컨트롤러 스크립트 리스트에 적을 감지한 총알 스크립트 추가 : 총알 여러 개가 동시에 감지할 수 있기 때문
	}

	public void ReleaseTarget(List<PlayerBulletDetectController> playerBulletDetectControllerList)
	{
		for (int i = 0; i < playerBulletDetectControllerList.Count; i++)
		{
			playerBulletDetectControllerList[i].ReleaseTarget(); //플레이어 총알의 적 감지 상태를 해제
		}
		playerBulletDetectControllerList.Clear(); //플레이어 총알 컨트롤러 스크립트 리스트 초기화
	}



	protected void ChangeAudioClip(AudioClip audioClip) //오디오 클립을 변경하는 메소드
	{
		enemyAudioSource.clip = audioClip; //오디오 소스 컴포넌트의 오디오 클립 변경
		enemyAudioSource.Play(); //오디오 실행
	}


	#endregion

	private void OnTriggerEnter2D(Collider2D collision)
	{

		if (collision.tag == "DeathWall") //트리거는 태그로 구분함
		{
			arrivedToStage = true; //플레이어용 DeathWall에 접촉하면 화면 안으로 들어온 것으로 간주
		}
		else if (collision.tag == "DeathWall_Enemy")
		{
			//StopCoroutine(GiveDamage()); //적용 DeathWall에 접촉하면 화면 밖으로 나간 것으로 간주. 플레이어에게 데미지를 주는 코루틴 중단
			death = true; //죽음 상태 변수 저장
			canGiveDamage = false;
			Death(); //죽는 메소드 호출 : 화면 밖으로 나가면 TakeDamage 메소드의 알고리즘이 따로 작동할 필요 없음
		}

	}


	private void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		if (enemyCollider != null)
		{
			Gizmos.DrawWireCube(transform.position, enemyCollider.size); //콜라이더를 기준으로  Gizmos 작성
		}
	}





}
