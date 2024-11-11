using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;
using LitJson;
using System.IO;
using TMPro;
using System;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.SceneManagement;


public class MainMenuManager : MonoBehaviour //메인 메뉴 매니저 클래스
{
	#region MainMenuPanel Objects
	[Header("MainMenuPanel Objects")]
	public GameObject mainMenuPanel; //메인 메뉴 패널

	#endregion

	#region SelectStagePanel Objects and Variables
	[Header("SelectStagePanel Objects")]
	public GameObject selectStagePanel; //스테이지 선택 패널
	public Transform selectStageScrollViewTransform; //스테이지 선택 스크롤뷰 Transform
	public Button stageButtonPrefab; //스테이지 목록 버튼 프리팹 : 인스펙터에서 등록

	[Space(10f)]
	public TextMeshProUGUI stageTitle; //스테이지 제목 텍스트
	public TextMeshProUGUI stageDescription; //스테이지 설명 텍스트
	public TextMeshProUGUI stageScore; //스테이지 점수 텍스트

	private Button currentStageButton; //현재 스테이지 버튼 : 이미 선택한 스테이지 버튼의 클릭을 비활성화하기 위해 사용

	#endregion

	#region SkinStorePanel Objects and Variables

	[Header("SkinStorePanel Objects and Variables")]
	public GameObject skinStorePanel; //스킨 상점 패널
	public TextMeshProUGUI moneyText; //가진 돈 텍스트
	public GameObject leftButton; //이전 스킨 버튼
	public GameObject rightButton; //다음 스킨 버튼

	[Space(10f)]
	public TextMeshProUGUI skinName; //스킨 이름 텍스트
	public Image skinImage; //스킨 이미지
	public TextMeshProUGUI skinDescription; //스킨 설명 텍스트
	public TextMeshProUGUI skinPrice; //스킨 가격 텍스트
	public Button buySkinButton; //스킨 구매 버튼
	public TextMeshProUGUI buySkinButtonText; //스킨 구매 버튼 텍스트

	private Sprite[] skinSpriteArray; //스킨 스프라이트 배열
	private int currentSkinNum; //현재 스킨 번호 변수
	private bool equipOrBuy; //구매 혹은 장비 여부 변수
	#endregion

	#region HowToPlayPanel Objects
	[Header("HowToPlayPanel Objects")]
	public GameObject howToPlayPanel; //플레이 방법 패널
	public GameObject howToPlayText_PC; //PC 플레이 방법 오브젝트
	public GameObject howToPlayText_Mobile; //모바일 플레이 방법 오브젝트

	#endregion

	#region QuitGamePanel Objects
	[Header("QuitGamePanel Objects")]
	public GameObject quitGamePanel; //게임 종료 패널

	#endregion

	#region StageLoadingPanel Objects and Variables

	[Header("StageLoadingPanel Objects")]
	public GameObject stageLoadingPanel; //스테이지 로딩 패널
	public TextMeshProUGUI loadingText; //로딩 텍스트
	public Image loadingBarImage; //로딩 바 이미지

	#endregion

	#region OptionPanel Objects and Variables

	[Header("OptionPanel Objects and Variables")]
	public GameObject optionButton; //옵션 버튼
	public GameObject optionPanel; //옵션 패널
	public GameObject option; //옵션 화면
	public GameObject deletePlayerData; //플레이어 데이터 삭제 화면

	#endregion

	#region Background Objects and Variables
	[Header("Background Objects and Variables")]
	public Renderer backgroundRenderer; //Quad 방식으로 구현된 배경화면 Renderer 컴포넌트 : 인스펙터에서 등록
	[SerializeField] private float backgroundSpeed; //배경 화면 이동 속도 변수

	[Space(10f)]
	public Rigidbody2D deerRigidbody; //배경에서 움직이는 사슴 rigidbody
	public Animator deerAnimator; //배경에서 움직이는 사슴 애니메이터
	[SerializeField] private float deerJumpForce; //배경에서 움직이는 사슴 점프 강도

	[Space(10f)]
	public AudioSource backgroundMuisicAudioSource; //배경음악 Audio Source

	#endregion

	#region Save/Load Variables


	private string playerDataLocation; //플레이어 데이터 위치 변수
	private PlayerData playerData; //플레이어 데이터 클래스

	private Dictionary<string, StageData> stageDataDictionary; //스테이지 데이터 딕셔너리
	private SkinData[] skinDataArray; //스킨 데이터 배열
	#endregion



	private void Awake()
	{
		if (Application.genuine == false && Application.genuineCheckAvailable == false) //무결성 검사
		{
			Debug.Log("무결성 검사 실패");
#if UNITY_EDITOR

			UnityEditor.EditorApplication.isPlaying = false; //실패했을 시 에디터는 종료
#endif

			Application.Quit(); //애플리케이션 종료
		}


		Application.targetFrameRate = 60; //타겟 프레임은 60
		DirectoryInfo dataFolder = new DirectoryInfo(Application.persistentDataPath + "/Data/"); //데이터 폴더 정보 저장
		if (!dataFolder.Exists) //데이터 폴더가 없을 경우
		{
			dataFolder.Create(); //데이터 폴더 생성
		}
		playerDataLocation = Application.persistentDataPath + "/Data/playerData.json"; //플레이어 데이터 위치 저장
		LoadData(); //데이터 불러오기
	}

	private void Start()
	{
		backgroundSpeed = 0.02f; //배경화면 이동 속도 저장
		deerJumpForce = 6.05f; //배경에서 움직이는 사슴 점프 강도 저장

#if UNITY_ANDROID

howToPlayText_Mobile.SetActive(true); //모바일의 경우 모바일용 플레이 방법 오브젝트 활성화

#else

		howToPlayText_PC.SetActive(true); //PC일 경우 PC용 플레이 방법 오브젝트 활성화

#endif


		StartCoroutine(JumpDeer()); //배경에서 움직이는 사슴 코루틴 실행
	}

	private void Update()
	{
#if UNITY_ANDROID
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OpenQuitGamePanel(); //안드로이드 기기의 경우 뒤로 가기 버튼이 있음. 버튼 클릭 시 게임 종료 패널 활성화
        }
#endif

	}

	private void FixedUpdate()
	{
		MoveBackground(); //배경화면 이동
	}

	#region SelectStagePanel Methods

	public void OpenSelectStagePanel() //스테이지 선택 패널 활성화 메소드
	{
		mainMenuPanel.SetActive(false); //메인 메뉴 패널 비활성화
		optionButton.SetActive(false); //옵션 버튼 비활성화
		selectStagePanel.SetActive(true); //스테이지 선택 패널 활성화
	}

	public void CloseSelectStagePanel() //스테이지 선택 패널 비활성화 메소드
	{
		selectStagePanel.SetActive(false); //스테이지 선택 패널 비활성화
		mainMenuPanel.SetActive(true); //메인 메뉴 패널 활성화
		optionButton.SetActive(true); //옵션 버튼 활성화
	}

	private void GenerateSelectStage() //스테이지 선택 패널 생성 메소드
	{
		if (playerData.stageScoreList.Count < stageDataDictionary.Count) //플레이어 데이터에 있는 스테이지 점수 목록 개수가 부족할 경우
																		 //: 스테이지가 업데이트로 추가되어도 플레이어의 json에는 영향이 없기 때문
		{
			for (int i = 0; i < stageDataDictionary.Count - playerData.stageScoreList.Count; i++)
			{
				playerData.AddStageScoreList(); //스테이지 점수 목록 추가 : 0점으로 초기화
			}
			SavePlayerData(); //플레이어 데이터 저장
		}

		List<Button> stageButtonList = new List<Button>(); //스테이지 버튼 리스트 저장
		Button stageButton; //스테이지 버튼 선언
		foreach (KeyValuePair<string, StageData> items in stageDataDictionary) //스테이지 데이터 딕셔너리를 순서대로 접근
		{
			stageButton = Instantiate(stageButtonPrefab, selectStageScrollViewTransform); //스테이지 버튼 생성
			stageButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = items.Key; //스테이지 버튼의 텍스트 저장
			stageButton.gameObject.name = items.Key; //스테이지 버튼 오브젝트 이름 저장

			if (playerData.unlockedStageCount < items.Value.stageNum) //만약 플레이어가 해금한 스테이지가 아닐 경우
			{
				stageButton.interactable = false; //스테이지 버튼 비활성화 : 목록에 표시만 되고 누르진 못함
			}

			stageButton.onClick.AddListener(SelectStage); //스테이지 버튼에 메소드 연결

			stageButtonList.Add(stageButton); //스테이지 버튼 리스트 추가
		}


		//stageButtonList[0].onClick.Invoke();
		currentStageButton = stageButtonList[0]; //첫 번째 스테이지 버튼을 누른 것으로 초기화
		SelectStage(); //스테이지 선택
					   //TODO : Invoke가 null exception을 반환하고 있음, currentStageButton에 0번째 버튼을 할당하고 메소드를 강제로 호출하는 방법으로 우회함
	}

	public void SelectStage() //스테이지 선택 메소드
	{
		if (currentStageButton != null)
		{
			currentStageButton.interactable = true; //스테이지 버튼 활성화
		}
		Button stageButton; //스테이지 버튼 선언

		//OnClick Invoke가 null exception을 반환해서 임시로 우회한 방법
		if (EventSystem.current.currentSelectedGameObject != null)
		{
			stageButton = EventSystem.current.currentSelectedGameObject.GetComponent<Button>(); //현재 선택한 오브젝트의 버튼 컴포넌트 저장
		}
		else
		{
			stageButton = currentStageButton; //선택한 스테이지 버튼 저장
		}

		stageTitle.text = stageButton.gameObject.name; //스테이지 제목 텍스트 저장
		stageDescription.text = stageDataDictionary[stageButton.gameObject.name].stageDescription; //스테이지 설명 텍스트 저장
		stageScore.text = $"점수 : {playerData.stageScoreList[stageDataDictionary[stageButton.gameObject.name].stageNum]}점"; //스테이지 점수 텍스트 저장

		currentStageButton = stageButton; //현재 선택한 스테이지 버튼 저장
		currentStageButton.interactable = false; //스테이지 버튼 비활성화 : 이미 선택한 스테이지 버튼의 클릭을 비활성화하기 위해 사용

	}

	#endregion

	#region SkinStorePanel Methods

	public void OpenSkinStorePanel() //스킨 상점 패널 활성화 메소드
	{
		mainMenuPanel.SetActive(false); //메인 메뉴 패널 비활성화
		optionButton.SetActive(false); //옵션 버튼 비활성화
		skinStorePanel.SetActive(true); //스킨 상점 패널 활성화
	}


	public void CloseSkinStorePanel() //스킨 상점 패널 비활성화 메소드
	{
		skinStorePanel.SetActive(false); //스킨 상점 패널 비활성화 
		mainMenuPanel.SetActive(true); //메인 메뉴 패널 활성화
		optionButton.SetActive(true); //옵션 버튼 활성화
	}

	private void GenerateSkinStore() //스킨 상점 패널 생성 메소드
	{
		skinSpriteArray = new Sprite[skinDataArray.Length]; //스킨 스프라이트 배열 저장

		for (int i = 0; i < skinSpriteArray.Length; i++)
		{
			skinSpriteArray[i] = Resources.Load<Sprite>(skinDataArray[i].skinSpriteLocation); //스킨 스프라이트 배열을 Resource에서 불러와 저장
		}
		moneyText.text = $"가진 돈 : {playerData.money} 골드"; //가진 돈 텍스트 저장
		currentSkinNum = 0; //스킨 상점 화면을 0번 스킨으로 초기화
		ChangeSkinStore(); //스킨 상점 변경

	}

	public void PressLeftButton() //스킨 상점 이전 버튼 메소드
	{
		currentSkinNum--; //현재 스킨 번호 감소
		if (currentSkinNum == 0) //스킨 번호가 0이면 : 이전 스킨이 없을 시
		{
			leftButton.SetActive(false); //이전 버튼 비활성화
		}
		if (!rightButton.activeSelf) //다음 버튼이 비활성화되어있을 시 : 마지막 스킨 번호에 접근하면 다음 버튼이 비활성화되는데 이전 버튼을 누르면 다음 스킨이 존재해야 하기 때문
		{
			rightButton.SetActive(true); //다음 버튼 활성화
		}
		ChangeSkinStore(); //스킨 상점 변경


	}

	public void PressRightButton() //스킨 상점 다음 버튼 메소드
	{
		currentSkinNum++;  //현재 스킨 번호 증가
		if (currentSkinNum == (skinDataArray.Length - 1)) //마지막 스킨일 경우 : 길이에서 -1을 하는 이유는 0번 스킨이 존재하기 때문
		{
			rightButton.SetActive(false); //다음 버튼 비활성화
		}
		if (!leftButton.activeSelf) //이전 버튼이 비활성화되어있을 시 : 처음 스킨 번호에 접근하면 이전 버튼이 비활성화되는데 다음 버튼을 누르면 이전 스킨이 존재해야 하기 때문
		{
			leftButton.SetActive(true); //이전 버튼 활성화
		}
		ChangeSkinStore(); //스킨 상점 변경
	}

	private void ChangeSkinStore() //스킨 상점 변경 메소드
	{
		skinName.text = skinDataArray[currentSkinNum].skinName; //스킨 이름 저장
		skinImage.sprite = skinSpriteArray[currentSkinNum]; //스킨 이미지 저장
		skinDescription.text = skinDataArray[currentSkinNum].skinDescription; //스킨 설명 저장
		skinPrice.text = $"{skinDataArray[currentSkinNum].skinPrice} 골드"; //스킨 가격 저장

		if (playerData.skinList.Contains(currentSkinNum)) //플레이어가 이미 구매한 스킨일 경우
		{
			equipOrBuy = true; //장비 혹은 구매 변수 저장
			skinPrice.gameObject.SetActive(false); //스킨 가격 텍스트 오브젝트 비활성화
			if (playerData.currentSkinNum == currentSkinNum) //이미 장비한 스킨일 경우
			{
				buySkinButtonText.text = "장비됨"; //스킨 장비 텍스트 저장
				buySkinButton.interactable = false; //스킨 장비 버튼 비활성화
			}
			else
			{
				buySkinButtonText.text = "장비"; //스킨 장비 텍스트 저장
				buySkinButton.interactable = true; //스킨 장비 버튼 활성화
			}
		}
		else
		{
			equipOrBuy = false; //장비 혹은 구매 변수 저장
			skinPrice.gameObject.SetActive(true); //스킨 가격 텍스트 오브젝트 활성화
			buySkinButtonText.text = "구매"; //스킨 구매 텍스트 저장
			if (playerData.money < skinDataArray[currentSkinNum].skinPrice) //가진 돈이 부족할 경우
			{
				buySkinButton.interactable = false; //구매 버튼 비활성화
			}
			else
			{
				buySkinButton.interactable = true; //구매 버튼 활성화
			}
		}
	}

	public void BuyOrEquipSkin() //스킨 구매 혹은 장비 메소드
	{
		if (equipOrBuy) //장비일 경우
		{
			playerData.currentSkinNum = currentSkinNum; //플레이어 데이터에 현재 스킨 번호 저장
			buySkinButtonText.text = "장비됨"; //장비 텍스트 저장
			buySkinButton.interactable = false; //장비 버튼 비활성화
		}
		else
		{
			playerData.money -= skinDataArray[currentSkinNum].skinPrice; //가진 돈에서 가격만큼 감소
			moneyText.text = $"가진 돈 : {playerData.money} 골드"; //가진 돈 텍스트 저장
			playerData.AddSkinNum(currentSkinNum); //플레이어 데이터에 현재 스킨 번호 추가
			buySkinButtonText.text = "장비"; //장비 텍스트 저장
			equipOrBuy = !equipOrBuy; //구매 상태를 장비 상태로 변경
			skinPrice.gameObject.SetActive(false); //스킨 가격 텍스트 오브젝트 비활성화
		}
		SavePlayerData(); //플레이어 데이터 저장
	}


	#endregion

	#region HowToPlayPanel Methods

	public void OpenHowToPlayPanel() //플레이 방법 패널 활성화 메소드
	{
		mainMenuPanel.SetActive(false); //메인 메뉴 패널 비활성화
		optionButton.SetActive(false); //옵션 버튼 비활성화
		howToPlayPanel.SetActive(true); //플레이 방법 패널 활성화
	}

	public void CloseHowToPlayPanel() //플레이 방법 패널 비활성화 메소드
	{
		howToPlayPanel.SetActive(false); //플레이 방법 패널 비활성화
		mainMenuPanel.SetActive(true); //메인 메뉴 패널 활성화
		optionButton.SetActive(true); //옵션 버튼 활성화
	}

	#endregion

	#region QuitGamePanel Methods

	public void OpenQuitGamePanel() //게임 종료 패널 활성화 메소드
	{
		mainMenuPanel.SetActive(false); //메인 메뉴 패널 비활성화
		optionButton.SetActive(false); //옵션 버튼 비활성화
		quitGamePanel.SetActive(true); //게임 종료 패널 활성화
	}

	public void CloseQuitGamePanel() //게임 종료 패널 비활성화 메소드
	{
		quitGamePanel.SetActive(false); //게임 종료 패널 비활성화
		mainMenuPanel.SetActive(true); //메인 메뉴 패널 활성화
		optionButton.SetActive(true); //옵션 버튼 활성화
	}

	public void QuitGame() //게임 종료 메소드
	{
		SavePlayerData(); //플레이어 데이터 저장

#if UNITY_EDITOR

		UnityEditor.EditorApplication.isPlaying = false; //에디터일 경우 종료
#endif

		Application.Quit(); //애플리케이션 종료
	}


	private void OnApplicationPause(bool pause) //일시정지 상태일 경우 : 모바일은 탭아웃으로 앱을 나가버릴 수 있음
	{
		if (pause == true)
		{
			SavePlayerData(); //플레이어 데이터 저장
		}
	}

	#endregion

	#region OptionPanel Methods

	public void OpenOrCloseOptionPanel() //옵션 패널 활성화 메소드
	{
		mainMenuPanel.SetActive(!mainMenuPanel.activeSelf); //메인 메뉴 패널 비활성화
		optionPanel.SetActive(!optionPanel.activeSelf); //옵션 패널 활성화
	}

	public void OpenDeletePlayerData() //플레이어 데이터 삭제 화면 활성화 메소드
	{
		option.SetActive(false); //옵션 화면 비활성화
		optionButton.SetActive(false); //옵션 버튼 비활성화
		deletePlayerData.SetActive(true); //플레이어 데이터 삭제 화면 활성화
	}

	public void CloseDeletePlayerData() //플레이어 데이터 삭제 화면 비활성화 메소드
	{
		deletePlayerData.SetActive(false); //플레이어 데이터 삭제 화면 비활성화
		option.SetActive(true); //옵션 화면 활성화
		optionButton.SetActive(true); //옵션 버튼 활성화
	}

	public void DeletePlayerData() //플레이어 데이터 삭제 메소드
	{
		File.Delete(playerDataLocation); //플레이어 데이터 json 삭제
		deletePlayerData.SetActive(false); //플레이어 데이터 삭제 화면 비활성화
		stageLoadingPanel.SetActive(true); //스테이지 로딩 패널 활성화
		StartCoroutine(LoadScene("MainMenuScene")); //LoadScene 코루틴 실행
	}

	#endregion

	#region Save/Load Methods

	public void SavePlayerData() //플레이어 데이터 저장 메소드
	{
		File.WriteAllText(playerDataLocation, JsonMapper.ToJson(this.playerData)); //플레이어 데이터 저장
	}

	public void LoadData() //데이터 불러오기 메소드
	{
		#region PlayerData
		if (File.Exists(playerDataLocation))
		{
			string playerDataJson = File.ReadAllText(playerDataLocation); //플레이어 데이터 json 불러오기
			playerData = JsonMapper.ToObject<PlayerData>(playerDataJson); //JsonMapper로 플레이어 데이터 저장
		}
		else
		{
			playerData = new PlayerData(); //플레이어가 처음 플레이할 경우 데이터 초기화
		}
		#endregion

		#region StageData

		TextAsset stageDataJson = Resources.Load<TextAsset>("Stage/stageData"); //스테이지 데이터 json 불러오기
		stageDataDictionary = JsonMapper.ToObject<Dictionary<string, StageData>>(stageDataJson.text); //JsonMapper로 스테이지 데이터 저장
		GenerateSelectStage(); //스테이지 선택 패널 생성

		#endregion

		#region SkinData

		TextAsset skinDataJson = Resources.Load<TextAsset>("Skin/skinData"); //스킨 데이터 json 불러오기
		skinDataArray = JsonMapper.ToObject<SkinData[]>(skinDataJson.text); //JsonMapper로 스킨 데이터 저장
		GenerateSkinStore(); //스킨 상점 패널 생성

		#endregion

	}
	#endregion

	#region Background Methods

	private void MoveBackground() //배경화면 이동 메소드
	{
		backgroundRenderer.material.mainTextureOffset += Vector2.right * backgroundSpeed * Time.deltaTime; //배경화면 이동
	}

	private IEnumerator JumpDeer() //배경화면에서 이동하는 사슴 메소드
	{
		while (true)
		{
			deerAnimator.Rebind(); //점프 애니메이션 실행을 위해 애니메이터 초기화

			deerRigidbody.velocity = Vector2.zero; //rigidbody velocity 초기화
			deerRigidbody.AddForce(Vector2.up * deerJumpForce, ForceMode2D.Impulse); //Impulse로 점프

			yield return new WaitForSeconds(1.2f); //1.2f초 후 다시 점프
		}
	}

	#endregion

	#region StageLoadingPanel Methods

	public void StartStage() //스테이지 실행 메소드
	{
		backgroundMuisicAudioSource.Stop(); //배경음악 정지

		selectStagePanel.SetActive(false); //스테이지 선택 패널 비활성화
		optionButton.SetActive(false); //옵션 버튼 비활성화
		stageLoadingPanel.SetActive(true); //스테이지 로딩 패널 활성화

		DataHolder.currentSkinNum = playerData.currentSkinNum; //DataHolder에 현재 스킨 번호 저장
		DataHolder.stageData_Enemy = stageDataDictionary[currentStageButton.name].stageData_Enemy; //DataHolder에 스테이지 적 데이터 저장
		DataHolder.stageData_Item = stageDataDictionary[currentStageButton.name].stageData_Item; //DataHolder에 스테이지 아이템 데이터 저장
		DataHolder.stageData_Bullet = stageDataDictionary[currentStageButton.name].stageData_Bullet; //DataHolder에 스테이지 총알 데이터 저장
		DataHolder.stageNum = stageDataDictionary[currentStageButton.name].stageNum; //DataHolder에 스테이지 번호 저장
		DataHolder.playerData = playerData; //DataHolder에 플레이어 데이터 저장

		StartCoroutine(LoadScene("GameScene")); //LoadScene 코루틴 실행
	}

	IEnumerator LoadScene(string scene) //Scene 변경 메소드
	{
		yield return null;
		AsyncOperation op = SceneManager.LoadSceneAsync(scene); //AsyncOperation 변수에 GameScene 연결
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

	#endregion
}


public static class DataHolder //GameScene으로 가져갈 DataHolder 클래스
{
	public static int currentSkinNum; //현재 스킨 번호 변수
	public static StageData_Enemy[] stageData_Enemy; //스테이지 적 데이터 struct
	public static StageData_Item[] stageData_Item; //스테이지 아이템 데이터 struct
	public static StageData_Bullet[] stageData_Bullet; //스테이지 총알 데이터 struct
	public static int stageNum; //스테이지 번호 변수
	public static PlayerData playerData; //플레이어 데이터 변수
}