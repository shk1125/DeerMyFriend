using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;


public class PlayerController : MonoBehaviour //플레이어 관리 클래스
{
	[Header("Audio Objects and Variables")]
	public AudioSource playerAudioSource; //플레이어 Audio Source 컴포넌트
	public AudioClip jumpAudioClip; //점프 오디오 클립
	public AudioClip shootAudioClip; //총알 발사 오디오 클립
	public AudioClip takeDamageAudioClip; //데미지 받음 오디오 클립

	[SerializeField] private float jumpForce; //플레이어 점프 강도 변수
	[SerializeField] private float movementSpeed; //플레이어 이동 속도 변수
	[SerializeField] private float currentHealth; //플레이어 현재 체력 변수
	[SerializeField] private float maxHealth; //플레이어 최대 체력 변수

	private BoxCollider2D playerCollider; //플레이어 콜라이더
	private Rigidbody2D playerRigidbody; //플레이어 rigidbody
	private Animator playerAnimator; //플레이어 점프 애니메이터
	private PlayerBulletPoolingController playerBulletPoolingController; //플레이어 총알 풀링 스크립트

	private bool gameOver; //게임 오버 확인 변수
	private float bulletFireRate; //총알 발사 주기
	private float movementDirection; //플레이어 이동 방향

	private void Awake()
	{
		playerRigidbody = GetComponent<Rigidbody2D>(); //rigidbody 저장
		playerBulletPoolingController = gameObject.GetComponent<PlayerBulletPoolingController>(); //총알 풀링 스크립트 저장
		playerAnimator = gameObject.GetComponent<Animator>(); //애니메이터 저장
		playerCollider = GetComponent<BoxCollider2D>(); //콜라이더 저장
	}

	void Start()
	{
		jumpForce = 6.5f; //점프 강도 변수 저장
		movementSpeed = 3.0f; //이동 속도 변수 저장
		maxHealth = 10f; //최대 체력 변수 저장
		currentHealth = 10.0f; //현재 체력 변수 저장
		bulletFireRate = 0; //총알 발사 빈도 변수 저장
		GameManager.instance.SetHealthText(currentHealth, maxHealth); //UI에 체력 표시
	}


	void Update()
	{
		if (!gameOver && !GameManager.instance.pause && !GameManager.instance.countdown) //게임 오버, 일시 정지, 카운트다운 상태가 아닐 경우
		{
			Move(); //이동 메소드 실행

			Shoot(); //총알 발사 메소드 실행

			if (Input.GetKeyDown(KeyCode.Escape))
			{
				GameManager.instance.OpenPausePanel(); //일시정지 실행
			}
		}
		else if (GameManager.instance.pause)
		{
			if (Input.GetKeyDown(KeyCode.Escape))
			{
				GameManager.instance.ClosePausePanel(); //일시정지 해제
			}
		}

		bulletFireRate += Time.deltaTime; //총알 발사 주기 계산
	}

	#region Action Methods

	void Move() //이동 메소드
	{
		movementDirection = Input.GetAxisRaw("Horizontal"); //이동 방향 입력

		transform.Translate(Vector3.right * movementDirection * movementSpeed * Time.deltaTime); //이동 속도와 이동 방향에 맞추어 이동


		if (Input.GetKeyDown(KeyCode.UpArrow)) //점프 입력
		{
			ChangeAudioClip(jumpAudioClip); //점프 오디오 클립 실행

			playerAnimator.Rebind(); //점프 애니메이션 실행을 위해 애니메이터 초기화
			playerAnimator.SetTrigger("Jump"); //점프 애니메이션 실행

			playerRigidbody.velocity = Vector2.zero; //rigidbody velocity 초기화
			playerRigidbody.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse); //Impulse로 점프
		}
	}

	public void Shoot() //총알 발사 메소드
	{
		if (Input.GetKeyDown(KeyCode.Space) && bulletFireRate >= playerBulletPoolingController.bulletFireRate) //총알 발사 입력 : 발사 주기가 충분할 경우
		{
			ChangeAudioClip(shootAudioClip); //총알 발사 오디오 클립 실행

			var bullet = playerBulletPoolingController.currentBulletPool.Get(); //총알 풀링 호출
			bullet.transform.position = this.transform.position; //총알 오브젝트를 플레이어의 위치로 이동
			bulletFireRate = 0; //총알 발사 주기 초기화
		}
	}


	public void TakeDamage(float damage) //데미지 받는 메소드
	{

		ChangeAudioClip(takeDamageAudioClip); //데미지 받음 오디오 클립 실행

		currentHealth -= damage; //현재 체력에서 데미지만큼 감소
		if (currentHealth <= 0f) //현재 체력이 0 이하일 시
		{
			currentHealth = 0f; //현재 체력을 0으로 표시
			gameOver = true; //게임 오버 변수 저장
			GameManager.instance.GameOver(); //게임 오버 알고리즘 실행
		}
		GameManager.instance.SetHealthText(currentHealth, maxHealth); //UI에 체력 표시
	}

	public void TakeItem(float health, AudioClip takeItemAudioClip) //아이템 획득 메소드
	{
		ChangeAudioClip(takeItemAudioClip); //아이템 획득 오디오 클립 실행
		this.currentHealth += health; //현재 체력에서 아이템 체력만큼 증가
		if (currentHealth > maxHealth) //현재 체력이 최대 체력을 초과할 시
		{
			currentHealth = maxHealth; //현재 체력을 최대 체력으로 고정
		}
		GameManager.instance.SetHealthText(currentHealth, maxHealth);//UI에 체력 표시
	}

	public void TakeBullet(AudioClip stageBulletAudioClip) //스테이지 총알 획득 메소드
	{
		ChangeAudioClip(stageBulletAudioClip); //총알 획득 오디오 클립 실행
	}

	public void SetGameOver(bool gameOver) //게임 오버 상태 변경 메소드
	{
		this.gameOver = gameOver; //게임 오버 상태 저장
	}

	private void ChangeAudioClip(AudioClip audioClip) //오디오 클립 변경 메소드
	{
		if (!gameOver) //플레이어의 죽음 상태에선 효과음이 발생하면 안됨
		{
			playerAudioSource.clip = audioClip; //오디오 클립 변경
			playerAudioSource.Play(); //오디오 클립 실행
		}
	}

	public void PausePlayer() //Rigidbody2D와 애니메이션 일시정지 메소드
	{
		playerRigidbody.simulated = !playerRigidbody.simulated; //gravity에 의해 추락하지 않도록 rigidbody 일시정지/해제
		if (gameObject.activeSelf)
		{
			playerAnimator.enabled = !playerAnimator.enabled; //애니메이션 일시정지 혹은 재실행
		}
	}

	public void ReleaseBulletPool() //플레이어 총알 풀링 전부 Releease 메소드 : Scene 변경 때 화면 안의 총알 오브젝트를 전부 치워야 하기 때문
	{
		playerBulletPoolingController.ReleaseBulletPool(); //플레이어 총알 풀링 전부 Release
	}

	#endregion

	#region Mobile Methods

	public void JoystickMove(bool right) //조이스틱 이동 메소드
	{
		if (right)
		{
			transform.position += Vector3.right * movementSpeed * Time.deltaTime; //오른쪽(x축 기준) 이동
		}
		else
		{
			transform.position += Vector3.left * movementSpeed * Time.deltaTime; //왼쪽(x축 기준) 이동
		}
	}


	public void MobileJump() //모바일 점프 메소드
	{
		ChangeAudioClip(jumpAudioClip); //점프 오디오 클립 실행

		playerAnimator.Rebind(); //점프 애니메이션 실행을 위해 애니메이터 초기화
		playerAnimator.SetTrigger("Jump"); //점프 애니메이션 실행

		playerRigidbody.velocity = Vector2.zero; //rigidbody velocity 초기화
		playerRigidbody.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse); //Impulse로 점프
	}

	public void MobileShoot() //모바일 총알 발사 메소드
	{
		if (bulletFireRate >= playerBulletPoolingController.bulletFireRate) //총알 발사 입력 : 발사 주기가 충분할 경우
		{
			ChangeAudioClip(shootAudioClip); //총알 발사 오디오 클립 실행

			var bullet = playerBulletPoolingController.currentBulletPool.Get(); //총알 풀링 호출
			bullet.transform.position = this.transform.position; //총알 오브젝트를 플레이어의 위치로 이동
			bulletFireRate = 0; //총알 발사 주기 초기화
		}
	}

	#endregion




	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.tag == "DeathWall") //트리거는 태그로 구분 : 플레이어가 화면 밖으로 나갔을 시
		{
			gameObject.SetActive(false); //오브젝트 비활성화
			GameManager.instance.GameOver(); //게임 오버 알고리즘 실행
		}
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		if (playerCollider != null)
		{
			Gizmos.DrawWireCube(transform.position, playerCollider.size); //콜라이더를 기준으로  Gizmos 작성
		}
	}
}
