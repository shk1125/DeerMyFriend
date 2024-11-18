using JetBrains.Annotations;
using LitJson;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.U2D.Animation;
using UnityEngine.UI;

public class GameManager : MonoBehaviour //게임매니저 클래스
{
	#region Stage Objects and Variables

	public static GameManager instance = null; //게임매니저 인스턴스

	[Header("Stage Objects and Variables")]
	public bool pause; //일시정지 상태 변수
	public bool countdown = true; //카운트다운 상태 변수

	[Space(10f)]
	public AudioClip gameOverAudioClip; //게임 오버 오디오 클립 변수 : 인스펙터에서 등록
	public AudioClip gameFinishedAudioClip; //게임 클리어 오디오 클립 변수 : 인스펙터에서 등록


	#endregion

	#region Player Objects and Variables
	[Header("Player Objects and Variables")]
	public GameObject playerPrefab; //플레이어 오브젝트 프리팹 : 인스펙터에서 등록

	private SpriteLibrary playerSpriteLibrary; //플레이어 스프라이트 라이브러리 : 스킨 변경에 사용됨
	private PlayerController playerController; //플레이어 컨트롤러 스크립트
	private Rigidbody2D playerRigidbody2D; //플레이어 rigidbody
	#endregion

	#region Enemy Objects and Variables
	[Header("Enemy Objects and Variables")]
	public GameObject[] enemyPrefabArray; //적 프리팹 배열 : 인스펙터에서 등록

	private List<EnemyController> enemyControllerList; //적 컨트롤러 스크립트 리스트
	private int score; //스테이지 점수 변수
	private int enemyDeaths; //적 죽음 카운트 변수 : Scene에서 적이 전부 죽으면 게임 클리어 알고리즘 작동
	#endregion

	#region Item Objects and Variables
	[Header("Item Objects and Variables")]
	public GameObject[] itemPrefabArray; //아이템 프리팹 배열 : 인스펙터에서 등록

	private List<GameObject> itemGameObjectList; //아이템 오브젝트 리스트

	#endregion

	#region	Bullet Objects and Variables

	[Header("Bullet Objects and Variables")]
	#region Player Bullet Objects and Variables
	[Header("Player Bullet Objects and Variables")]
	public Transform playerBulletPoolTransform; //플레이어 총알 풀링 Transform : 인스펙터에서 등록. 플레이어의 총알 오브젝트는 전부 자식 오브젝트로 등록됨
	public GameObject[] stageBulletPrefabArray; //스테이지 총알 프리팹 배열 : 인스펙터에서 등록

	private PlayerBulletData[] playerBulletDataArray; //플레이어 총알 데이터 배열
	private List<GameObject> stageBulletGameObjectList; //스테이지 총알 오브젝트 리스트

	#endregion

	#region Enemy Bullet Objects and Variables

	[Header("Enemy Bullet Objects and Variables")]

	public EnemyBulletPoolingController enemyBulletPoolingController; //적 총알 풀링 컨트롤러 스크립트 : 인스펙터에서 등록

	#endregion

	#endregion

	#region DeathWall Objects and Variables
	[Header("DeathWall Objects and Variables")]
	public GameObject deathWall3; //DeathWall 3번 : 인스펙터에서 등록. DeathWall 3번은 위쪽(y축 기준)에 있는 DeathWall인데 카운트다운 동안 플레이어 오브젝트가 아래로 떨어지는 연출이 있기 때문에 비활성화 상태로 대기하다 카운트다운이 끝나면 다시 활성화됨

	public Transform[] deathWallTransformArray; //DeathWall Transform 배열
	#endregion

	#region Background Objects and Variables
	[Header("Background Objects and Variables")]
	public Renderer backgroundRenderer; //Quad 방식으로 구현된 배경화면 Renderer 컴포넌트 : 인스펙터에서 등록
	[SerializeField] private float backgroundSpeed; //배경화면 이동 속도

	[Space(10f)]
	public AudioSource backgroundMusicAudioSource; //배경음악 AudioSource 컴포넌트 : 인스펙터에서 등록
	#endregion

	#region UI Objects
	[Header("UI Objects")]
	//UI 오브젝트는 playerBulletSpriteArray를 제외한 모두 인스펙터에서 등록
	public GameObject gameOverPanel; //게임 오버 패널
	public GameObject playerPanel; //플레이어 패널
	public GameObject pausePanel; //일시정지 패널
	public GameObject SceneLoadingPanel; //로딩 패널
	public GameObject gameFinishedPanel; //게임 클리어 패널

	[Space(10f)]
	public TextMeshProUGUI gameFinishedScoreText; //게임 클리어 점수 텍스트
	public TextMeshProUGUI pauseScoreText; //게임 일시정지 점수 텍스트
	public TextMeshProUGUI countDownText; //카운트다운 텍스트
	public TextMeshProUGUI healthText; //체력 텍스트
	public JoystickController joystickController; //조이스틱 컨트롤러 스크립트
	public Image loadingBarImage; //로딩 바 이미지
	public Image bulletImage; //UI에 표시되는 현재 플레이어 총알 이미지
	public Sprite[] playerBulletSpriteArray; //UI에 표시되는 현재 플레이어 총알 스프라이트 배열

	#region Mobile Objects
	[Header("Mobile Objects")]
	public GameObject mobilePanel; //모바일 패널
	#endregion

	#endregion

	private void Awake()
	{
		if (instance == null)
		{
			instance = this; //게임매니저 인스턴스 등록
		}
		else
		{
			if (instance != this)
				Destroy(this.gameObject); //이미 인스턴스가 있을 경우 삭제
		}
		Application.targetFrameRate = 60; //타겟 프레임은 60

#if UNITY_ANDROID
mobilePanel.SetActive(true); //모바일 패널 활성화
SetDeathWallPosition(); //모바일 해상도 대응 DeathWall position 변경
#else
Screen.SetResolution(1920, 1080, true); //PC에서 해상도 Full HD로 고정
#endif


		GenerateBackground(); //배경화면 생성 메소드 호출
		GenerateStage(); //스테이지 생성 메소드 호출

	}

	void Start()
	{
		score = 0; //점수 변수 저장
		enemyDeaths = 0; //적 죽음 카운트 변수 저장
		backgroundSpeed = 0.02f; //배경화면 이동 속도 변수 저장
		pause = false; //일시정지 변수 저장
	}


	

	private void FixedUpdate()
	{
		if (!pause) //일시정지 상태가 아닐 시
		{
			MoveBackground(); //배경화면 이동 메소드 호출
		}
	}


	#region Stage Methods

	private void GenerateStage() //스테이지 생성 메소드
	{
		StartCoroutine(StartCountDown()); //StartCountDown 코루틴 실행

		GeneratePlayer(); //플레이어 생성 메소드 호출
		GenerateEnemies(); //적 생성 메소드 호출
		GenerateItems(); //아이템 생성 메소드 호출
		GenerateBullets(); //총알 생성 메소드 호출
	}
	private IEnumerator StartCountDown() //카운트다운 메소드
	{

		float countDownTime = 3.0f; //카운트다운 시간 저장


		while (countDownTime > 0)
		{
			countDownText.text = Mathf.FloorToInt(countDownTime).ToString(); //카운트다운 시간 텍스트 표시

			yield return new WaitForSeconds(1.0f); //1초 대기

			countDownTime--; //카운트다운 시간 감소

			if (countDownTime <= 1.0f && countDownTime > 0f)
			{
				playerRigidbody2D.gravityScale = 1.0f; //카운트다운이 1.0f초 남았을 때 플레이어 오브젝트의 gravityScale을 1.0f로 변경해 위에서 떨어지는 연출
				playerController.SetGameOver(true); //플레이어 컨트롤러 스크립트의 게임 오버 상태 변수 저장 : 카운트다운이 진행되는 동안에는 플레이어가 조작할 수 없기 때문에 게임 오버 상태 변수를 재활용
													//playerController 스크립트의 저장 알고리즘과 유기적으로 연결되어 있지 않기에 null reference 우려가 있으나 2.0f초의 카운트다운 시간이면 저장이 완료되기 충분한 시간이라 간주함
			}
		}

		countDownText.gameObject.SetActive(false); //카운트다운이 끝나면 카운트다운 텍스트 비활성화
		countdown = false; //카운트다운 상태 변수 저장
		deathWall3.SetActive(true); //DeathWall 3번 활성화 : DeathWall 3번은 위쪽(y축 기준)에 있는 DeathWall인데 카운트다운 동안 플레이어 오브젝트가 아래로 떨어지는 연출이 있기 때문에 플레이어가 화면 안으로 들어오면 활성화
		playerController.SetGameOver(false); //플레이어 컨트롤러 스크립트의 게임 오버 상태 변수 저장 : 카운트다운이 끝났으니 플레이어의 조작이 가능해져야 함
		backgroundMusicAudioSource.Play(); //배경음악 실행



	}

	public void SetHealthText(float currentHealth, float maxHealth) //체력 텍스트 표시 메소드
	{
		healthText.text = $"{currentHealth}/{maxHealth}"; //체력 텍스트 변경
	}

	public void SetBulletImage(int bulletNum) //UI에 표시되는 현재 총알 이미지 표시 메소드
	{
		bulletImage.sprite = playerBulletSpriteArray[bulletNum]; //UI에 표시되는 현재 총알 스프라이트를 플레이어 총알 스프라이트 배열에서 받아옴
	}

	private void MoveBackground() //배경화면 이동 메소드
	{
		backgroundRenderer.material.mainTextureOffset += Vector2.right * backgroundSpeed * Time.deltaTime; //Quad 방식으로 구현된 배경화면 이동
	}

	public void GameOver() //게임 오버 메소드
	{
		pause = true; //일시정지 상태 변수 저장 : 일시정지 알고리즘을 공유함
		PauseEnemies(); //적 오브젝트 애니메이션 일시정지
		GameFinished(gameOverAudioClip); //게임 오버 오디오 클립 재생

		gameOverPanel.SetActive(true); //게임 오버 패널 활성화
#if UNITY_ANDROID
mobilePanel.SetActive(false); //모바일 패널 비활성화
#endif
	}

	public void Restart() //재시작 메소드
	{
		LoadScene(); //Scene 로드 전 화면에서 오브젝트들을 제거하는 메소드 호출
		StartCoroutine(LoadScene("GameScene")); //GameScene 다시 로드
	}

	public void ReturnToMainMenu() //메인 메뉴 Scene으로 돌아가는 메소드
	{
		backgroundMusicAudioSource.Stop(); //배경음악 정지
		LoadScene(); //Scene 로드 전 화면에서 오브젝트들을 제거하는 메소드 호출
		StartCoroutine(LoadScene("MainMenuScene")); //MainMenuScene 로드
	}

	private void LoadScene() //Scene 로드 전 화면에서 오브젝트들을 제거하는 메소드
	{
		playerController.ReleaseBulletPool(); //플레이어 총알 풀링 전부 Release
		playerController.gameObject.SetActive(false); //플레이어 오브젝트 비활성화
		enemyBulletPoolingController.ReleaseBulletPool(); //적 총알 풀링 전부 Release
		for (int i = 0; i < enemyControllerList.Count; i++)
		{
			enemyControllerList[i].gameObject.SetActive(false); //적 오브젝트 비활성화
		}
		for (int i = 0; i < itemGameObjectList.Count; i++)
		{
			itemGameObjectList[i].SetActive(false); //아이템 오브젝트 비활성화
		}
		for (int I = 0; I < stageBulletGameObjectList.Count; I++)
		{
			stageBulletGameObjectList[I].gameObject.SetActive(false); //총알 오브젝트 비활성화
		}

		gameFinishedPanel.SetActive(false); //게임 클리어 패널 비활성화
		gameOverPanel.SetActive(false); //게임 오버 패널 비활성화
		playerPanel.SetActive(false); //플레이어 패널 비활성화
		mobilePanel.SetActive(false); //모바일 패널 비활성화
		pausePanel.SetActive(false); //일시정지 패널 비활성화
		SceneLoadingPanel.SetActive(true); //Scene 로딩 패널 활성화
	}

	IEnumerator LoadScene(string sceneName) //Scene 로드 메소드
	{
		yield return null;
		AsyncOperation op = SceneManager.LoadSceneAsync(sceneName); //AsyncOperation 변수에 요구하는 Scene 연결
		op.allowSceneActivation = false; //로딩 이전에는 Scene을 로드하지 않도록 저장
		float timer = 0.0f; //시간 변수 저장
		while (!op.isDone) //로딩이 끝나지 않았을 경우
		{
			yield return null;
			timer += Time.deltaTime; //시간 변수 증가
			if (op.progress < 0.9f) //0.9f 정도에 로딩이 완료된 것으로 간주
			{
				loadingBarImage.fillAmount = Mathf.Lerp(loadingBarImage.fillAmount, op.progress, timer); //로딩 정도와 타이머 변수의 보간을 로딩 바의 fillAmount로 표현
				if (loadingBarImage.fillAmount >= op.progress) //로딩 바의 fillAmount가 로딩 정도보다 높을 경우
				{
					timer = 0f; //시간 변수 초기화
				}
			}
			else
			{
				loadingBarImage.fillAmount = Mathf.Lerp(loadingBarImage.fillAmount, 1f, timer); //0.9f 시간 정도가 로딩이 완료된 것으로 간주되기 때문에 1f까지 수동으로 보간
				if (loadingBarImage.fillAmount == 1.0f) //로딩이 완료되었을 때
				{
					op.allowSceneActivation = true; //Scene을 로드하도록 허가
					yield break;
				}
			}
		}
	}

	public void OpenPausePanel() //일시정지 화면 활성화 메소드
	{
		if (countdown)
		{
			return; //카운트다운 상태에선 일시정지 버튼이 작동하면 안됨
		}
		pause = true; //일시정지 상태 변수 저장
		pauseScoreText.text = $"현재 점수 : {score}"; //현재 점수 텍스트 변경
		playerController.PausePlayer(); //플레이어 오브젝트 일시정지
		PauseEnemies(); //적 오브젝트 애니메이션 일시정지
		pausePanel.SetActive(true); //일시정지 패널 활성화
		backgroundMusicAudioSource.Pause(); //배경음악 일시정지

	}
	public void ClosePausePanel() //일시정지 화면 비활성화 메소드
	{
		pause = false; //일시정지 상태 변수 저장
		pausePanel.SetActive(false); //일시정지 화면 비활성화
		playerController.PausePlayer(); //플레이어 일시정지 해제
		PauseEnemies(); //적 오브젝트 애니메이션 일시정지 해제
		backgroundMusicAudioSource.Play(); //배경음악 실행
	}

	private IEnumerator GameFinished() //게임 클리어 메소드
	{
		yield return new WaitForSeconds(3.0f); //적이 전부 죽었을 시 3.0f초만큼 대기한 후에 알고리즘 실행

		GameFinished(gameFinishedAudioClip); //게임 클리어 오디오 클립 재생

		pause = true; //일시정지 상태 변수 저장 : 일시정지 알고리즘을 공유
		playerController.PausePlayer(); //플레이어 오브젝트 일시정지

		gameFinishedScoreText.text = $"점수 : {score}점"; //게임 클리어 점수 텍스트 변경
		gameFinishedPanel.SetActive(true); //게임 클리어 패널 활성화
		DataHolder.playerData.AddUnlockedStageCount(); //DataHolder에 다음 스테이지를 접근 가능으로 변경
		DataHolder.playerData.SetStageScore(DataHolder.stageNum, score); //DataHolder에 현재 스테이지 점수를 저장
		DataHolder.playerData.AddMoney(score / 10); //DataHolder에 스테이지 점수를 10으로 나눈 값을 가진 돈으로 추가
		File.WriteAllText(Application.persistentDataPath + "/Data/playerData.json", JsonMapper.ToJson(DataHolder.playerData));
		//json으로 저장
#if UNITY_ANDROID
mobilePanel.SetActive(false); //모바일 패널 비활성화
#endif
	}

	private void GameFinished(AudioClip audioClip) //게임 종료(클리어, 게임 오버) 오디오 클립 재생 메소드
	{
		backgroundMusicAudioSource.Stop(); //배경 음악 중지
		backgroundMusicAudioSource.clip = audioClip; //오디오 클립 변경
		backgroundMusicAudioSource.loop = false; //배경 음악은 loop 상태이기 때문에 오디오 클립이 한 번만 재생될 수 있도록 변경 : 현재는 효과음이기 때문에 loop를 비활성화 하지만 음악으로 변경된다면 이 부분을 삭제 가능
		backgroundMusicAudioSource.Play(); //오디오 클립 재생
	}

	private void GenerateBackground() //배경 화면 생성 메소드
	{
		backgroundRenderer.material = Resources.Load<Material>($"Stage/Background/BackgroundMaterial{DataHolder.stageNum}"); 
		//DataHolder의 현재 스테이지 배경 화면으로 material 변경
	}

	private void SetDeathWallPosition() //모바일 해상도 대응 DeathWall position 변경 메소드
	{
		float systemHeight = Display.main.systemHeight; //사용 기기 Height 저장
		float systemWidth = Display.main.systemWidth; //사용 기기 Width 저장

		for(int i = 0; i < deathWallTransformArray.Length; i++)
		{
			deathWallTransformArray[i].transform.position = new Vector3((systemWidth * deathWallTransformArray[i].transform.position.x) / 1920f,
																		(systemHeight * deathWallTransformArray[i].transform.position.y) / 1080f, 0); //비례식으로 DeathWall Position 조절
		}
	}

	#endregion

	#region Player Methods
	private void GeneratePlayer() //플레이어 생성 메소드
	{
		Transform playerTransform = Instantiate(playerPrefab).transform; //플레이어 생성 후 Transform 저장
		playerSpriteLibrary = playerTransform.GetComponent<SpriteLibrary>(); //플레이어 스프라이트 라이브러리 컴포넌트 저장
		playerRigidbody2D = playerTransform.GetComponent<Rigidbody2D>(); //플레이어 rigidbody 컴포넌트 저장
		playerController = playerTransform.GetComponent<PlayerController>(); //플레이어 컨트롤러 스크립트 저장

		playerSpriteLibrary.spriteLibraryAsset = Resources.Load<SpriteLibraryAsset>($"Player/PlayerSpriteLibraryAsset{DataHolder.currentSkinNum}"); //DataHolder의 현재 스킨 번호로
																																					//스프라이트 라이브러리 변경

		joystickController.SetPlayerController(playerController); //조이스틱 컨트롤러 스크립트에 플레이어 등록 :
																  //모바일이 아닐 경우 작동할 필요가 없지만 에디터에서의 확인을 위해 놔둔 상태. 전처리기로 분리 가능
	}


	#region Mobile Player Methods

	public void MobileJump() //모바일 점프 메소드
	{
		if (!countdown && !pause)
		{
			playerController.MobileJump(); //플레이어 점프 실행
		}

	}


	public void MobileShoot() //모바일 총알 발사 메소드
	{
		if (!countdown && !pause)
		{
			playerController.MobileShoot(); //모바일 총알 발사 실행
		}
	}

	#endregion

	#endregion

	#region Enemy Mehtods

	private void GenerateEnemies() //적 생성 메소드
	{

		TextAsset enemyDataJson = Resources.Load<TextAsset>("Enemy/enemyData"); //적 데이터 json 불러오기
		EnemyData[] enemyDataArray = JsonMapper.ToObject<EnemyData[]>(enemyDataJson.text); //JsonMapper로 struct 저장
		enemyControllerList = new List<EnemyController>(); //적 컨트롤러 리스트 생성
		for (int i = 0; i < DataHolder.stageData_Enemy.Length; i++) //스테이지에 등록된 적 오브젝트 개수 만큼
		{
			Transform enemyTransform = Instantiate(enemyPrefabArray[DataHolder.stageData_Enemy[i].enemyNum - 1]).transform; //적 오브젝트 생성 후 Transform 저장 : enemyNum에서 -1을 하는 이유는 json에선 1번 적이 배열상으론 0번으로 간주되기 때문
			enemyTransform.position = new Vector2((float)DataHolder.stageData_Enemy[i].enemyPositionX, (float)DataHolder.stageData_Enemy[i].enemyPositionY); //적 오브젝트 위치 변경
			EnemyData enemyData = enemyDataArray[DataHolder.stageData_Enemy[i].enemyNum - 1]; //적 데이터 배열에서 1개의 요소 추출
			EnemyController enemyController = enemyTransform.GetComponent<EnemyController>();  //적 컨트롤러 스크립트 저장
			enemyController.SetEnemyData(enemyData.enemyHealth, enemyData.enemySpeed, enemyData.enemyDamage, enemyData.enemyBullet, enemyData.enemyBulletSpeed, enemyData.enemyScore, DataHolder.stageData_Enemy[i].enemyNum, playerController); //작 데이터 세팅 메소드 호출
			enemyControllerList.Add(enemyController); //적 컨트롤러 스크립트 리스트 추가
		}


	}

	public void AddScore(int score) //적이 죽었을 때 점수가 추가되는 메소드
	{
		this.score += score; //점수 추가
	}

	public void AddEnemyDeaths() //적이 죽었을 때 카운트하는 메소드
	{
		enemyDeaths++; //적 죽음 카운트
		if (enemyDeaths == enemyControllerList.Count) //스테이지의 모든 적이 죽었을 때
		{
			StartCoroutine(GameFinished()); //GameFinished 코루틴 실행
		}
	}

	private void PauseEnemies() //일시정지때 적 애니메이션 변경 메소드
	{
		for (int i = 0; i < enemyControllerList.Count; i++)
		{
			enemyControllerList[i].PauseEnemy(); //적 컨트롤러 스크립트의 일시정지 메소드를 각각 호출
		}
	}

	#endregion

	#region Item Methods

	private void GenerateItems() //아이템 생성 메소드
	{
		itemGameObjectList = new List<GameObject>(); //아이템 오브젝트 리스트 생성
		if (DataHolder.stageData_Item != null) //스테이지에 생성될 아이템 오브젝트가 명시되어 있을 시
		{
			for (int i = 0; i < DataHolder.stageData_Item.Length; i++) //스테이지에 등록된 아이템 개수만큼
			{
				Transform itemTransform = Instantiate(itemPrefabArray[DataHolder.stageData_Item[i].itemNum - 1]).transform; //아이템 생성 후 Transform 저장 : itemNum에서 -1을 하는 이유는 json에선 1번 아이템이 배열상으론 0번으로 간주되기 때문
				itemTransform.position = new Vector2((float)DataHolder.stageData_Item[i].itemPositionX, (float)DataHolder.stageData_Item[i].itemPositionY); //아이템 오브젝트 위치 변경
				itemTransform.GetComponent<ItemController>().SetPlayerController(playerController); //아이템 컨트롤러 스크립트에 플레이어 등록
				itemGameObjectList.Add(itemTransform.gameObject); //아이템 오브젝트 리스트 추가
			}
		}
	}

	#endregion

	#region Bullet Methods

	private void GenerateBullets() //총알 생성 메소드
	{
		PlayerBulletPoolingController playerBulletPoolingController = null; //플레이어 총알 풀링 컨트롤러 스크립트 선언 : null로 초기화하는 이유는 메모리 공간을 할당하는 코드가 if문 안에 묶여있는데 스테이지 총알 생성 알고리즘에서 변수에 접근해야 하기 때문

		TextAsset playerBulletDataJson = Resources.Load<TextAsset>("Bullet/Player/playerBulletData"); //스테이지 총알 데이터 json 불러오기
		playerBulletDataArray = JsonMapper.ToObject<PlayerBulletData[]>(playerBulletDataJson.text); //JsonMapper로 Struct 저장
		playerBulletPoolingController = playerController.GetComponent<PlayerBulletPoolingController>(); //플레이어 총알 풀링 컨트롤러 스크립트 저장
		playerBulletPoolingController.GenerateBulletPool(playerBulletDataArray, playerBulletPoolTransform); //플레이어 총알 풀링 생성 메소드 호출

		playerBulletSpriteArray = new Sprite[playerBulletDataArray.Length]; //화면에 표시될 플레이어 총알 스프라이트 배열 저장
		for (int i = 0; i < playerBulletSpriteArray.Length; i++) //화면에 표시될 플레이어 총알 스프라이트 배열 길이만큼
		{
			playerBulletSpriteArray[i] = Resources.Load<Sprite>(playerBulletDataArray[i].bulletSpriteLocation); //화면에 표시될 플레이어 총알 스프라이트 배열에 Resource로 준비되어 있는 스프라이트 파일 저장
		}
		SetBulletImage(0); //0번 총알 이미지로 변경 메소드 호출



		stageBulletGameObjectList = new List<GameObject>(); //스테이지 총알 오브젝트 리스트 저장
		if (DataHolder.stageData_Bullet != null) //스테이지에 생성될 총알 오브젝트가 명시되어 있을 시
		{
			for (int i = 0; i < DataHolder.stageData_Bullet.Length; i++) //스테이지에 등록된 총알 개수만큼
			{
				Transform stageBulletTransform = Instantiate(stageBulletPrefabArray[DataHolder.stageData_Bullet[i].bulletNum]).transform; //총알 생성 후 Transform 저장
				stageBulletTransform.position = new Vector2((float)DataHolder.stageData_Bullet[i].bulletPositionX, (float)DataHolder.stageData_Bullet[i].bulletPositionY); //총알 오브젝트 위치 변경
				stageBulletTransform.GetComponent<StageBulletController>().SetStageBulletData(DataHolder.stageData_Bullet[i].bulletNum, playerBulletPoolingController, playerController); //스테이지 총알 데이터 세팅 메소드 호출
				stageBulletGameObjectList.Add(stageBulletTransform.gameObject); //스테이지 총알 오브젝트 리스트 추가
			}
		}


	}



	#endregion

}

