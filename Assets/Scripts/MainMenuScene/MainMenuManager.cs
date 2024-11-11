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


public class MainMenuManager : MonoBehaviour //���� �޴� �Ŵ��� Ŭ����
{
	#region MainMenuPanel Objects
	[Header("MainMenuPanel Objects")]
	public GameObject mainMenuPanel; //���� �޴� �г�

	#endregion

	#region SelectStagePanel Objects and Variables
	[Header("SelectStagePanel Objects")]
	public GameObject selectStagePanel; //�������� ���� �г�
	public Transform selectStageScrollViewTransform; //�������� ���� ��ũ�Ѻ� Transform
	public Button stageButtonPrefab; //�������� ��� ��ư ������ : �ν����Ϳ��� ���

	[Space(10f)]
	public TextMeshProUGUI stageTitle; //�������� ���� �ؽ�Ʈ
	public TextMeshProUGUI stageDescription; //�������� ���� �ؽ�Ʈ
	public TextMeshProUGUI stageScore; //�������� ���� �ؽ�Ʈ

	private Button currentStageButton; //���� �������� ��ư : �̹� ������ �������� ��ư�� Ŭ���� ��Ȱ��ȭ�ϱ� ���� ���

	#endregion

	#region SkinStorePanel Objects and Variables

	[Header("SkinStorePanel Objects and Variables")]
	public GameObject skinStorePanel; //��Ų ���� �г�
	public TextMeshProUGUI moneyText; //���� �� �ؽ�Ʈ
	public GameObject leftButton; //���� ��Ų ��ư
	public GameObject rightButton; //���� ��Ų ��ư

	[Space(10f)]
	public TextMeshProUGUI skinName; //��Ų �̸� �ؽ�Ʈ
	public Image skinImage; //��Ų �̹���
	public TextMeshProUGUI skinDescription; //��Ų ���� �ؽ�Ʈ
	public TextMeshProUGUI skinPrice; //��Ų ���� �ؽ�Ʈ
	public Button buySkinButton; //��Ų ���� ��ư
	public TextMeshProUGUI buySkinButtonText; //��Ų ���� ��ư �ؽ�Ʈ

	private Sprite[] skinSpriteArray; //��Ų ��������Ʈ �迭
	private int currentSkinNum; //���� ��Ų ��ȣ ����
	private bool equipOrBuy; //���� Ȥ�� ��� ���� ����
	#endregion

	#region HowToPlayPanel Objects
	[Header("HowToPlayPanel Objects")]
	public GameObject howToPlayPanel; //�÷��� ��� �г�
	public GameObject howToPlayText_PC; //PC �÷��� ��� ������Ʈ
	public GameObject howToPlayText_Mobile; //����� �÷��� ��� ������Ʈ

	#endregion

	#region QuitGamePanel Objects
	[Header("QuitGamePanel Objects")]
	public GameObject quitGamePanel; //���� ���� �г�

	#endregion

	#region StageLoadingPanel Objects and Variables

	[Header("StageLoadingPanel Objects")]
	public GameObject stageLoadingPanel; //�������� �ε� �г�
	public TextMeshProUGUI loadingText; //�ε� �ؽ�Ʈ
	public Image loadingBarImage; //�ε� �� �̹���

	#endregion

	#region OptionPanel Objects and Variables

	[Header("OptionPanel Objects and Variables")]
	public GameObject optionButton; //�ɼ� ��ư
	public GameObject optionPanel; //�ɼ� �г�
	public GameObject option; //�ɼ� ȭ��
	public GameObject deletePlayerData; //�÷��̾� ������ ���� ȭ��

	#endregion

	#region Background Objects and Variables
	[Header("Background Objects and Variables")]
	public Renderer backgroundRenderer; //Quad ������� ������ ���ȭ�� Renderer ������Ʈ : �ν����Ϳ��� ���
	[SerializeField] private float backgroundSpeed; //��� ȭ�� �̵� �ӵ� ����

	[Space(10f)]
	public Rigidbody2D deerRigidbody; //��濡�� �����̴� �罿 rigidbody
	public Animator deerAnimator; //��濡�� �����̴� �罿 �ִϸ�����
	[SerializeField] private float deerJumpForce; //��濡�� �����̴� �罿 ���� ����

	[Space(10f)]
	public AudioSource backgroundMuisicAudioSource; //������� Audio Source

	#endregion

	#region Save/Load Variables


	private string playerDataLocation; //�÷��̾� ������ ��ġ ����
	private PlayerData playerData; //�÷��̾� ������ Ŭ����

	private Dictionary<string, StageData> stageDataDictionary; //�������� ������ ��ųʸ�
	private SkinData[] skinDataArray; //��Ų ������ �迭
	#endregion



	private void Awake()
	{
		if (Application.genuine == false && Application.genuineCheckAvailable == false) //���Ἲ �˻�
		{
			Debug.Log("���Ἲ �˻� ����");
#if UNITY_EDITOR

			UnityEditor.EditorApplication.isPlaying = false; //�������� �� �����ʹ� ����
#endif

			Application.Quit(); //���ø����̼� ����
		}


		Application.targetFrameRate = 60; //Ÿ�� �������� 60
		DirectoryInfo dataFolder = new DirectoryInfo(Application.persistentDataPath + "/Data/"); //������ ���� ���� ����
		if (!dataFolder.Exists) //������ ������ ���� ���
		{
			dataFolder.Create(); //������ ���� ����
		}
		playerDataLocation = Application.persistentDataPath + "/Data/playerData.json"; //�÷��̾� ������ ��ġ ����
		LoadData(); //������ �ҷ�����
	}

	private void Start()
	{
		backgroundSpeed = 0.02f; //���ȭ�� �̵� �ӵ� ����
		deerJumpForce = 6.05f; //��濡�� �����̴� �罿 ���� ���� ����

#if UNITY_ANDROID

howToPlayText_Mobile.SetActive(true); //������� ��� ����Ͽ� �÷��� ��� ������Ʈ Ȱ��ȭ

#else

		howToPlayText_PC.SetActive(true); //PC�� ��� PC�� �÷��� ��� ������Ʈ Ȱ��ȭ

#endif


		StartCoroutine(JumpDeer()); //��濡�� �����̴� �罿 �ڷ�ƾ ����
	}

	private void Update()
	{
#if UNITY_ANDROID
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OpenQuitGamePanel(); //�ȵ���̵� ����� ��� �ڷ� ���� ��ư�� ����. ��ư Ŭ�� �� ���� ���� �г� Ȱ��ȭ
        }
#endif

	}

	private void FixedUpdate()
	{
		MoveBackground(); //���ȭ�� �̵�
	}

	#region SelectStagePanel Methods

	public void OpenSelectStagePanel() //�������� ���� �г� Ȱ��ȭ �޼ҵ�
	{
		mainMenuPanel.SetActive(false); //���� �޴� �г� ��Ȱ��ȭ
		optionButton.SetActive(false); //�ɼ� ��ư ��Ȱ��ȭ
		selectStagePanel.SetActive(true); //�������� ���� �г� Ȱ��ȭ
	}

	public void CloseSelectStagePanel() //�������� ���� �г� ��Ȱ��ȭ �޼ҵ�
	{
		selectStagePanel.SetActive(false); //�������� ���� �г� ��Ȱ��ȭ
		mainMenuPanel.SetActive(true); //���� �޴� �г� Ȱ��ȭ
		optionButton.SetActive(true); //�ɼ� ��ư Ȱ��ȭ
	}

	private void GenerateSelectStage() //�������� ���� �г� ���� �޼ҵ�
	{
		if (playerData.stageScoreList.Count < stageDataDictionary.Count) //�÷��̾� �����Ϳ� �ִ� �������� ���� ��� ������ ������ ���
																		 //: ���������� ������Ʈ�� �߰��Ǿ �÷��̾��� json���� ������ ���� ����
		{
			for (int i = 0; i < stageDataDictionary.Count - playerData.stageScoreList.Count; i++)
			{
				playerData.AddStageScoreList(); //�������� ���� ��� �߰� : 0������ �ʱ�ȭ
			}
			SavePlayerData(); //�÷��̾� ������ ����
		}

		List<Button> stageButtonList = new List<Button>(); //�������� ��ư ����Ʈ ����
		Button stageButton; //�������� ��ư ����
		foreach (KeyValuePair<string, StageData> items in stageDataDictionary) //�������� ������ ��ųʸ��� ������� ����
		{
			stageButton = Instantiate(stageButtonPrefab, selectStageScrollViewTransform); //�������� ��ư ����
			stageButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = items.Key; //�������� ��ư�� �ؽ�Ʈ ����
			stageButton.gameObject.name = items.Key; //�������� ��ư ������Ʈ �̸� ����

			if (playerData.unlockedStageCount < items.Value.stageNum) //���� �÷��̾ �ر��� ���������� �ƴ� ���
			{
				stageButton.interactable = false; //�������� ��ư ��Ȱ��ȭ : ��Ͽ� ǥ�ø� �ǰ� ������ ����
			}

			stageButton.onClick.AddListener(SelectStage); //�������� ��ư�� �޼ҵ� ����

			stageButtonList.Add(stageButton); //�������� ��ư ����Ʈ �߰�
		}


		//stageButtonList[0].onClick.Invoke();
		currentStageButton = stageButtonList[0]; //ù ��° �������� ��ư�� ���� ������ �ʱ�ȭ
		SelectStage(); //�������� ����
					   //TODO : Invoke�� null exception�� ��ȯ�ϰ� ����, currentStageButton�� 0��° ��ư�� �Ҵ��ϰ� �޼ҵ带 ������ ȣ���ϴ� ������� ��ȸ��
	}

	public void SelectStage() //�������� ���� �޼ҵ�
	{
		if (currentStageButton != null)
		{
			currentStageButton.interactable = true; //�������� ��ư Ȱ��ȭ
		}
		Button stageButton; //�������� ��ư ����

		//OnClick Invoke�� null exception�� ��ȯ�ؼ� �ӽ÷� ��ȸ�� ���
		if (EventSystem.current.currentSelectedGameObject != null)
		{
			stageButton = EventSystem.current.currentSelectedGameObject.GetComponent<Button>(); //���� ������ ������Ʈ�� ��ư ������Ʈ ����
		}
		else
		{
			stageButton = currentStageButton; //������ �������� ��ư ����
		}

		stageTitle.text = stageButton.gameObject.name; //�������� ���� �ؽ�Ʈ ����
		stageDescription.text = stageDataDictionary[stageButton.gameObject.name].stageDescription; //�������� ���� �ؽ�Ʈ ����
		stageScore.text = $"���� : {playerData.stageScoreList[stageDataDictionary[stageButton.gameObject.name].stageNum]}��"; //�������� ���� �ؽ�Ʈ ����

		currentStageButton = stageButton; //���� ������ �������� ��ư ����
		currentStageButton.interactable = false; //�������� ��ư ��Ȱ��ȭ : �̹� ������ �������� ��ư�� Ŭ���� ��Ȱ��ȭ�ϱ� ���� ���

	}

	#endregion

	#region SkinStorePanel Methods

	public void OpenSkinStorePanel() //��Ų ���� �г� Ȱ��ȭ �޼ҵ�
	{
		mainMenuPanel.SetActive(false); //���� �޴� �г� ��Ȱ��ȭ
		optionButton.SetActive(false); //�ɼ� ��ư ��Ȱ��ȭ
		skinStorePanel.SetActive(true); //��Ų ���� �г� Ȱ��ȭ
	}


	public void CloseSkinStorePanel() //��Ų ���� �г� ��Ȱ��ȭ �޼ҵ�
	{
		skinStorePanel.SetActive(false); //��Ų ���� �г� ��Ȱ��ȭ 
		mainMenuPanel.SetActive(true); //���� �޴� �г� Ȱ��ȭ
		optionButton.SetActive(true); //�ɼ� ��ư Ȱ��ȭ
	}

	private void GenerateSkinStore() //��Ų ���� �г� ���� �޼ҵ�
	{
		skinSpriteArray = new Sprite[skinDataArray.Length]; //��Ų ��������Ʈ �迭 ����

		for (int i = 0; i < skinSpriteArray.Length; i++)
		{
			skinSpriteArray[i] = Resources.Load<Sprite>(skinDataArray[i].skinSpriteLocation); //��Ų ��������Ʈ �迭�� Resource���� �ҷ��� ����
		}
		moneyText.text = $"���� �� : {playerData.money} ���"; //���� �� �ؽ�Ʈ ����
		currentSkinNum = 0; //��Ų ���� ȭ���� 0�� ��Ų���� �ʱ�ȭ
		ChangeSkinStore(); //��Ų ���� ����

	}

	public void PressLeftButton() //��Ų ���� ���� ��ư �޼ҵ�
	{
		currentSkinNum--; //���� ��Ų ��ȣ ����
		if (currentSkinNum == 0) //��Ų ��ȣ�� 0�̸� : ���� ��Ų�� ���� ��
		{
			leftButton.SetActive(false); //���� ��ư ��Ȱ��ȭ
		}
		if (!rightButton.activeSelf) //���� ��ư�� ��Ȱ��ȭ�Ǿ����� �� : ������ ��Ų ��ȣ�� �����ϸ� ���� ��ư�� ��Ȱ��ȭ�Ǵµ� ���� ��ư�� ������ ���� ��Ų�� �����ؾ� �ϱ� ����
		{
			rightButton.SetActive(true); //���� ��ư Ȱ��ȭ
		}
		ChangeSkinStore(); //��Ų ���� ����


	}

	public void PressRightButton() //��Ų ���� ���� ��ư �޼ҵ�
	{
		currentSkinNum++;  //���� ��Ų ��ȣ ����
		if (currentSkinNum == (skinDataArray.Length - 1)) //������ ��Ų�� ��� : ���̿��� -1�� �ϴ� ������ 0�� ��Ų�� �����ϱ� ����
		{
			rightButton.SetActive(false); //���� ��ư ��Ȱ��ȭ
		}
		if (!leftButton.activeSelf) //���� ��ư�� ��Ȱ��ȭ�Ǿ����� �� : ó�� ��Ų ��ȣ�� �����ϸ� ���� ��ư�� ��Ȱ��ȭ�Ǵµ� ���� ��ư�� ������ ���� ��Ų�� �����ؾ� �ϱ� ����
		{
			leftButton.SetActive(true); //���� ��ư Ȱ��ȭ
		}
		ChangeSkinStore(); //��Ų ���� ����
	}

	private void ChangeSkinStore() //��Ų ���� ���� �޼ҵ�
	{
		skinName.text = skinDataArray[currentSkinNum].skinName; //��Ų �̸� ����
		skinImage.sprite = skinSpriteArray[currentSkinNum]; //��Ų �̹��� ����
		skinDescription.text = skinDataArray[currentSkinNum].skinDescription; //��Ų ���� ����
		skinPrice.text = $"{skinDataArray[currentSkinNum].skinPrice} ���"; //��Ų ���� ����

		if (playerData.skinList.Contains(currentSkinNum)) //�÷��̾ �̹� ������ ��Ų�� ���
		{
			equipOrBuy = true; //��� Ȥ�� ���� ���� ����
			skinPrice.gameObject.SetActive(false); //��Ų ���� �ؽ�Ʈ ������Ʈ ��Ȱ��ȭ
			if (playerData.currentSkinNum == currentSkinNum) //�̹� ����� ��Ų�� ���
			{
				buySkinButtonText.text = "����"; //��Ų ��� �ؽ�Ʈ ����
				buySkinButton.interactable = false; //��Ų ��� ��ư ��Ȱ��ȭ
			}
			else
			{
				buySkinButtonText.text = "���"; //��Ų ��� �ؽ�Ʈ ����
				buySkinButton.interactable = true; //��Ų ��� ��ư Ȱ��ȭ
			}
		}
		else
		{
			equipOrBuy = false; //��� Ȥ�� ���� ���� ����
			skinPrice.gameObject.SetActive(true); //��Ų ���� �ؽ�Ʈ ������Ʈ Ȱ��ȭ
			buySkinButtonText.text = "����"; //��Ų ���� �ؽ�Ʈ ����
			if (playerData.money < skinDataArray[currentSkinNum].skinPrice) //���� ���� ������ ���
			{
				buySkinButton.interactable = false; //���� ��ư ��Ȱ��ȭ
			}
			else
			{
				buySkinButton.interactable = true; //���� ��ư Ȱ��ȭ
			}
		}
	}

	public void BuyOrEquipSkin() //��Ų ���� Ȥ�� ��� �޼ҵ�
	{
		if (equipOrBuy) //����� ���
		{
			playerData.currentSkinNum = currentSkinNum; //�÷��̾� �����Ϳ� ���� ��Ų ��ȣ ����
			buySkinButtonText.text = "����"; //��� �ؽ�Ʈ ����
			buySkinButton.interactable = false; //��� ��ư ��Ȱ��ȭ
		}
		else
		{
			playerData.money -= skinDataArray[currentSkinNum].skinPrice; //���� ������ ���ݸ�ŭ ����
			moneyText.text = $"���� �� : {playerData.money} ���"; //���� �� �ؽ�Ʈ ����
			playerData.AddSkinNum(currentSkinNum); //�÷��̾� �����Ϳ� ���� ��Ų ��ȣ �߰�
			buySkinButtonText.text = "���"; //��� �ؽ�Ʈ ����
			equipOrBuy = !equipOrBuy; //���� ���¸� ��� ���·� ����
			skinPrice.gameObject.SetActive(false); //��Ų ���� �ؽ�Ʈ ������Ʈ ��Ȱ��ȭ
		}
		SavePlayerData(); //�÷��̾� ������ ����
	}


	#endregion

	#region HowToPlayPanel Methods

	public void OpenHowToPlayPanel() //�÷��� ��� �г� Ȱ��ȭ �޼ҵ�
	{
		mainMenuPanel.SetActive(false); //���� �޴� �г� ��Ȱ��ȭ
		optionButton.SetActive(false); //�ɼ� ��ư ��Ȱ��ȭ
		howToPlayPanel.SetActive(true); //�÷��� ��� �г� Ȱ��ȭ
	}

	public void CloseHowToPlayPanel() //�÷��� ��� �г� ��Ȱ��ȭ �޼ҵ�
	{
		howToPlayPanel.SetActive(false); //�÷��� ��� �г� ��Ȱ��ȭ
		mainMenuPanel.SetActive(true); //���� �޴� �г� Ȱ��ȭ
		optionButton.SetActive(true); //�ɼ� ��ư Ȱ��ȭ
	}

	#endregion

	#region QuitGamePanel Methods

	public void OpenQuitGamePanel() //���� ���� �г� Ȱ��ȭ �޼ҵ�
	{
		mainMenuPanel.SetActive(false); //���� �޴� �г� ��Ȱ��ȭ
		optionButton.SetActive(false); //�ɼ� ��ư ��Ȱ��ȭ
		quitGamePanel.SetActive(true); //���� ���� �г� Ȱ��ȭ
	}

	public void CloseQuitGamePanel() //���� ���� �г� ��Ȱ��ȭ �޼ҵ�
	{
		quitGamePanel.SetActive(false); //���� ���� �г� ��Ȱ��ȭ
		mainMenuPanel.SetActive(true); //���� �޴� �г� Ȱ��ȭ
		optionButton.SetActive(true); //�ɼ� ��ư Ȱ��ȭ
	}

	public void QuitGame() //���� ���� �޼ҵ�
	{
		SavePlayerData(); //�÷��̾� ������ ����

#if UNITY_EDITOR

		UnityEditor.EditorApplication.isPlaying = false; //�������� ��� ����
#endif

		Application.Quit(); //���ø����̼� ����
	}


	private void OnApplicationPause(bool pause) //�Ͻ����� ������ ��� : ������� �Ǿƿ����� ���� �������� �� ����
	{
		if (pause == true)
		{
			SavePlayerData(); //�÷��̾� ������ ����
		}
	}

	#endregion

	#region OptionPanel Methods

	public void OpenOrCloseOptionPanel() //�ɼ� �г� Ȱ��ȭ �޼ҵ�
	{
		mainMenuPanel.SetActive(!mainMenuPanel.activeSelf); //���� �޴� �г� ��Ȱ��ȭ
		optionPanel.SetActive(!optionPanel.activeSelf); //�ɼ� �г� Ȱ��ȭ
	}

	public void OpenDeletePlayerData() //�÷��̾� ������ ���� ȭ�� Ȱ��ȭ �޼ҵ�
	{
		option.SetActive(false); //�ɼ� ȭ�� ��Ȱ��ȭ
		optionButton.SetActive(false); //�ɼ� ��ư ��Ȱ��ȭ
		deletePlayerData.SetActive(true); //�÷��̾� ������ ���� ȭ�� Ȱ��ȭ
	}

	public void CloseDeletePlayerData() //�÷��̾� ������ ���� ȭ�� ��Ȱ��ȭ �޼ҵ�
	{
		deletePlayerData.SetActive(false); //�÷��̾� ������ ���� ȭ�� ��Ȱ��ȭ
		option.SetActive(true); //�ɼ� ȭ�� Ȱ��ȭ
		optionButton.SetActive(true); //�ɼ� ��ư Ȱ��ȭ
	}

	public void DeletePlayerData() //�÷��̾� ������ ���� �޼ҵ�
	{
		File.Delete(playerDataLocation); //�÷��̾� ������ json ����
		deletePlayerData.SetActive(false); //�÷��̾� ������ ���� ȭ�� ��Ȱ��ȭ
		stageLoadingPanel.SetActive(true); //�������� �ε� �г� Ȱ��ȭ
		StartCoroutine(LoadScene("MainMenuScene")); //LoadScene �ڷ�ƾ ����
	}

	#endregion

	#region Save/Load Methods

	public void SavePlayerData() //�÷��̾� ������ ���� �޼ҵ�
	{
		File.WriteAllText(playerDataLocation, JsonMapper.ToJson(this.playerData)); //�÷��̾� ������ ����
	}

	public void LoadData() //������ �ҷ����� �޼ҵ�
	{
		#region PlayerData
		if (File.Exists(playerDataLocation))
		{
			string playerDataJson = File.ReadAllText(playerDataLocation); //�÷��̾� ������ json �ҷ�����
			playerData = JsonMapper.ToObject<PlayerData>(playerDataJson); //JsonMapper�� �÷��̾� ������ ����
		}
		else
		{
			playerData = new PlayerData(); //�÷��̾ ó�� �÷����� ��� ������ �ʱ�ȭ
		}
		#endregion

		#region StageData

		TextAsset stageDataJson = Resources.Load<TextAsset>("Stage/stageData"); //�������� ������ json �ҷ�����
		stageDataDictionary = JsonMapper.ToObject<Dictionary<string, StageData>>(stageDataJson.text); //JsonMapper�� �������� ������ ����
		GenerateSelectStage(); //�������� ���� �г� ����

		#endregion

		#region SkinData

		TextAsset skinDataJson = Resources.Load<TextAsset>("Skin/skinData"); //��Ų ������ json �ҷ�����
		skinDataArray = JsonMapper.ToObject<SkinData[]>(skinDataJson.text); //JsonMapper�� ��Ų ������ ����
		GenerateSkinStore(); //��Ų ���� �г� ����

		#endregion

	}
	#endregion

	#region Background Methods

	private void MoveBackground() //���ȭ�� �̵� �޼ҵ�
	{
		backgroundRenderer.material.mainTextureOffset += Vector2.right * backgroundSpeed * Time.deltaTime; //���ȭ�� �̵�
	}

	private IEnumerator JumpDeer() //���ȭ�鿡�� �̵��ϴ� �罿 �޼ҵ�
	{
		while (true)
		{
			deerAnimator.Rebind(); //���� �ִϸ��̼� ������ ���� �ִϸ����� �ʱ�ȭ

			deerRigidbody.velocity = Vector2.zero; //rigidbody velocity �ʱ�ȭ
			deerRigidbody.AddForce(Vector2.up * deerJumpForce, ForceMode2D.Impulse); //Impulse�� ����

			yield return new WaitForSeconds(1.2f); //1.2f�� �� �ٽ� ����
		}
	}

	#endregion

	#region StageLoadingPanel Methods

	public void StartStage() //�������� ���� �޼ҵ�
	{
		backgroundMuisicAudioSource.Stop(); //������� ����

		selectStagePanel.SetActive(false); //�������� ���� �г� ��Ȱ��ȭ
		optionButton.SetActive(false); //�ɼ� ��ư ��Ȱ��ȭ
		stageLoadingPanel.SetActive(true); //�������� �ε� �г� Ȱ��ȭ

		DataHolder.currentSkinNum = playerData.currentSkinNum; //DataHolder�� ���� ��Ų ��ȣ ����
		DataHolder.stageData_Enemy = stageDataDictionary[currentStageButton.name].stageData_Enemy; //DataHolder�� �������� �� ������ ����
		DataHolder.stageData_Item = stageDataDictionary[currentStageButton.name].stageData_Item; //DataHolder�� �������� ������ ������ ����
		DataHolder.stageData_Bullet = stageDataDictionary[currentStageButton.name].stageData_Bullet; //DataHolder�� �������� �Ѿ� ������ ����
		DataHolder.stageNum = stageDataDictionary[currentStageButton.name].stageNum; //DataHolder�� �������� ��ȣ ����
		DataHolder.playerData = playerData; //DataHolder�� �÷��̾� ������ ����

		StartCoroutine(LoadScene("GameScene")); //LoadScene �ڷ�ƾ ����
	}

	IEnumerator LoadScene(string scene) //Scene ���� �޼ҵ�
	{
		yield return null;
		AsyncOperation op = SceneManager.LoadSceneAsync(scene); //AsyncOperation ������ GameScene ����
		op.allowSceneActivation = false; //�ε� �������� Scene�� �ε����� �ʵ��� ����
		float timer = 0.0f; //�ð� ���� ����
		while (!op.isDone) //�ε��� ������ �ʾ��� ���
		{
			yield return null;
			timer += Time.deltaTime; //�ð� ���� ����
			if (op.progress < 0.9f) //0.9f ������ �ε��� �Ϸ�� ������ ����
			{
				loadingBarImage.fillAmount = Mathf.Lerp(loadingBarImage.fillAmount, op.progress, timer); //�ε� ������ Ÿ�̸� ������ ������ �ε� ���� fillAmount�� ǥ��
				if (loadingBarImage.fillAmount >= op.progress) //�ε� ���� fillAmount�� �ε� �������� ���� ���
				{
					timer = 0f; //�ð� ���� �ʱ�ȭ
				}
			}
			else
			{
				loadingBarImage.fillAmount = Mathf.Lerp(loadingBarImage.fillAmount, 1f, timer); //0.9f �ð� ������ �ε��� �Ϸ�� ������ ���ֵǱ� ������ 1f���� �������� ����
				if (loadingBarImage.fillAmount == 1.0f) //�ε��� �Ϸ�Ǿ��� ��
				{
					op.allowSceneActivation = true; //Scene�� �ε��ϵ��� �㰡
					yield break;
				}
			}
		}
	}

	#endregion
}


public static class DataHolder //GameScene���� ������ DataHolder Ŭ����
{
	public static int currentSkinNum; //���� ��Ų ��ȣ ����
	public static StageData_Enemy[] stageData_Enemy; //�������� �� ������ struct
	public static StageData_Item[] stageData_Item; //�������� ������ ������ struct
	public static StageData_Bullet[] stageData_Bullet; //�������� �Ѿ� ������ struct
	public static int stageNum; //�������� ��ȣ ����
	public static PlayerData playerData; //�÷��̾� ������ ����
}