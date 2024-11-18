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

public class GameManager : MonoBehaviour //���ӸŴ��� Ŭ����
{
	#region Stage Objects and Variables

	public static GameManager instance = null; //���ӸŴ��� �ν��Ͻ�

	[Header("Stage Objects and Variables")]
	public bool pause; //�Ͻ����� ���� ����
	public bool countdown = true; //ī��Ʈ�ٿ� ���� ����

	[Space(10f)]
	public AudioClip gameOverAudioClip; //���� ���� ����� Ŭ�� ���� : �ν����Ϳ��� ���
	public AudioClip gameFinishedAudioClip; //���� Ŭ���� ����� Ŭ�� ���� : �ν����Ϳ��� ���


	#endregion

	#region Player Objects and Variables
	[Header("Player Objects and Variables")]
	public GameObject playerPrefab; //�÷��̾� ������Ʈ ������ : �ν����Ϳ��� ���

	private SpriteLibrary playerSpriteLibrary; //�÷��̾� ��������Ʈ ���̺귯�� : ��Ų ���濡 ����
	private PlayerController playerController; //�÷��̾� ��Ʈ�ѷ� ��ũ��Ʈ
	private Rigidbody2D playerRigidbody2D; //�÷��̾� rigidbody
	#endregion

	#region Enemy Objects and Variables
	[Header("Enemy Objects and Variables")]
	public GameObject[] enemyPrefabArray; //�� ������ �迭 : �ν����Ϳ��� ���

	private List<EnemyController> enemyControllerList; //�� ��Ʈ�ѷ� ��ũ��Ʈ ����Ʈ
	private int score; //�������� ���� ����
	private int enemyDeaths; //�� ���� ī��Ʈ ���� : Scene���� ���� ���� ������ ���� Ŭ���� �˰��� �۵�
	#endregion

	#region Item Objects and Variables
	[Header("Item Objects and Variables")]
	public GameObject[] itemPrefabArray; //������ ������ �迭 : �ν����Ϳ��� ���

	private List<GameObject> itemGameObjectList; //������ ������Ʈ ����Ʈ

	#endregion

	#region	Bullet Objects and Variables

	[Header("Bullet Objects and Variables")]
	#region Player Bullet Objects and Variables
	[Header("Player Bullet Objects and Variables")]
	public Transform playerBulletPoolTransform; //�÷��̾� �Ѿ� Ǯ�� Transform : �ν����Ϳ��� ���. �÷��̾��� �Ѿ� ������Ʈ�� ���� �ڽ� ������Ʈ�� ��ϵ�
	public GameObject[] stageBulletPrefabArray; //�������� �Ѿ� ������ �迭 : �ν����Ϳ��� ���

	private PlayerBulletData[] playerBulletDataArray; //�÷��̾� �Ѿ� ������ �迭
	private List<GameObject> stageBulletGameObjectList; //�������� �Ѿ� ������Ʈ ����Ʈ

	#endregion

	#region Enemy Bullet Objects and Variables

	[Header("Enemy Bullet Objects and Variables")]

	public EnemyBulletPoolingController enemyBulletPoolingController; //�� �Ѿ� Ǯ�� ��Ʈ�ѷ� ��ũ��Ʈ : �ν����Ϳ��� ���

	#endregion

	#endregion

	#region DeathWall Objects and Variables
	[Header("DeathWall Objects and Variables")]
	public GameObject deathWall3; //DeathWall 3�� : �ν����Ϳ��� ���. DeathWall 3���� ����(y�� ����)�� �ִ� DeathWall�ε� ī��Ʈ�ٿ� ���� �÷��̾� ������Ʈ�� �Ʒ��� �������� ������ �ֱ� ������ ��Ȱ��ȭ ���·� ����ϴ� ī��Ʈ�ٿ��� ������ �ٽ� Ȱ��ȭ��

	public Transform[] deathWallTransformArray; //DeathWall Transform �迭
	#endregion

	#region Background Objects and Variables
	[Header("Background Objects and Variables")]
	public Renderer backgroundRenderer; //Quad ������� ������ ���ȭ�� Renderer ������Ʈ : �ν����Ϳ��� ���
	[SerializeField] private float backgroundSpeed; //���ȭ�� �̵� �ӵ�

	[Space(10f)]
	public AudioSource backgroundMusicAudioSource; //������� AudioSource ������Ʈ : �ν����Ϳ��� ���
	#endregion

	#region UI Objects
	[Header("UI Objects")]
	//UI ������Ʈ�� playerBulletSpriteArray�� ������ ��� �ν����Ϳ��� ���
	public GameObject gameOverPanel; //���� ���� �г�
	public GameObject playerPanel; //�÷��̾� �г�
	public GameObject pausePanel; //�Ͻ����� �г�
	public GameObject SceneLoadingPanel; //�ε� �г�
	public GameObject gameFinishedPanel; //���� Ŭ���� �г�

	[Space(10f)]
	public TextMeshProUGUI gameFinishedScoreText; //���� Ŭ���� ���� �ؽ�Ʈ
	public TextMeshProUGUI pauseScoreText; //���� �Ͻ����� ���� �ؽ�Ʈ
	public TextMeshProUGUI countDownText; //ī��Ʈ�ٿ� �ؽ�Ʈ
	public TextMeshProUGUI healthText; //ü�� �ؽ�Ʈ
	public JoystickController joystickController; //���̽�ƽ ��Ʈ�ѷ� ��ũ��Ʈ
	public Image loadingBarImage; //�ε� �� �̹���
	public Image bulletImage; //UI�� ǥ�õǴ� ���� �÷��̾� �Ѿ� �̹���
	public Sprite[] playerBulletSpriteArray; //UI�� ǥ�õǴ� ���� �÷��̾� �Ѿ� ��������Ʈ �迭

	#region Mobile Objects
	[Header("Mobile Objects")]
	public GameObject mobilePanel; //����� �г�
	#endregion

	#endregion

	private void Awake()
	{
		if (instance == null)
		{
			instance = this; //���ӸŴ��� �ν��Ͻ� ���
		}
		else
		{
			if (instance != this)
				Destroy(this.gameObject); //�̹� �ν��Ͻ��� ���� ��� ����
		}
		Application.targetFrameRate = 60; //Ÿ�� �������� 60

#if UNITY_ANDROID
mobilePanel.SetActive(true); //����� �г� Ȱ��ȭ
SetDeathWallPosition(); //����� �ػ� ���� DeathWall position ����
#else
Screen.SetResolution(1920, 1080, true); //PC���� �ػ� Full HD�� ����
#endif


		GenerateBackground(); //���ȭ�� ���� �޼ҵ� ȣ��
		GenerateStage(); //�������� ���� �޼ҵ� ȣ��

	}

	void Start()
	{
		score = 0; //���� ���� ����
		enemyDeaths = 0; //�� ���� ī��Ʈ ���� ����
		backgroundSpeed = 0.02f; //���ȭ�� �̵� �ӵ� ���� ����
		pause = false; //�Ͻ����� ���� ����
	}


	

	private void FixedUpdate()
	{
		if (!pause) //�Ͻ����� ���°� �ƴ� ��
		{
			MoveBackground(); //���ȭ�� �̵� �޼ҵ� ȣ��
		}
	}


	#region Stage Methods

	private void GenerateStage() //�������� ���� �޼ҵ�
	{
		StartCoroutine(StartCountDown()); //StartCountDown �ڷ�ƾ ����

		GeneratePlayer(); //�÷��̾� ���� �޼ҵ� ȣ��
		GenerateEnemies(); //�� ���� �޼ҵ� ȣ��
		GenerateItems(); //������ ���� �޼ҵ� ȣ��
		GenerateBullets(); //�Ѿ� ���� �޼ҵ� ȣ��
	}
	private IEnumerator StartCountDown() //ī��Ʈ�ٿ� �޼ҵ�
	{

		float countDownTime = 3.0f; //ī��Ʈ�ٿ� �ð� ����


		while (countDownTime > 0)
		{
			countDownText.text = Mathf.FloorToInt(countDownTime).ToString(); //ī��Ʈ�ٿ� �ð� �ؽ�Ʈ ǥ��

			yield return new WaitForSeconds(1.0f); //1�� ���

			countDownTime--; //ī��Ʈ�ٿ� �ð� ����

			if (countDownTime <= 1.0f && countDownTime > 0f)
			{
				playerRigidbody2D.gravityScale = 1.0f; //ī��Ʈ�ٿ��� 1.0f�� ������ �� �÷��̾� ������Ʈ�� gravityScale�� 1.0f�� ������ ������ �������� ����
				playerController.SetGameOver(true); //�÷��̾� ��Ʈ�ѷ� ��ũ��Ʈ�� ���� ���� ���� ���� ���� : ī��Ʈ�ٿ��� ����Ǵ� ���ȿ��� �÷��̾ ������ �� ���� ������ ���� ���� ���� ������ ��Ȱ��
													//playerController ��ũ��Ʈ�� ���� �˰���� ���������� ����Ǿ� ���� �ʱ⿡ null reference ����� ������ 2.0f���� ī��Ʈ�ٿ� �ð��̸� ������ �Ϸ�Ǳ� ����� �ð��̶� ������
			}
		}

		countDownText.gameObject.SetActive(false); //ī��Ʈ�ٿ��� ������ ī��Ʈ�ٿ� �ؽ�Ʈ ��Ȱ��ȭ
		countdown = false; //ī��Ʈ�ٿ� ���� ���� ����
		deathWall3.SetActive(true); //DeathWall 3�� Ȱ��ȭ : DeathWall 3���� ����(y�� ����)�� �ִ� DeathWall�ε� ī��Ʈ�ٿ� ���� �÷��̾� ������Ʈ�� �Ʒ��� �������� ������ �ֱ� ������ �÷��̾ ȭ�� ������ ������ Ȱ��ȭ
		playerController.SetGameOver(false); //�÷��̾� ��Ʈ�ѷ� ��ũ��Ʈ�� ���� ���� ���� ���� ���� : ī��Ʈ�ٿ��� �������� �÷��̾��� ������ ���������� ��
		backgroundMusicAudioSource.Play(); //������� ����



	}

	public void SetHealthText(float currentHealth, float maxHealth) //ü�� �ؽ�Ʈ ǥ�� �޼ҵ�
	{
		healthText.text = $"{currentHealth}/{maxHealth}"; //ü�� �ؽ�Ʈ ����
	}

	public void SetBulletImage(int bulletNum) //UI�� ǥ�õǴ� ���� �Ѿ� �̹��� ǥ�� �޼ҵ�
	{
		bulletImage.sprite = playerBulletSpriteArray[bulletNum]; //UI�� ǥ�õǴ� ���� �Ѿ� ��������Ʈ�� �÷��̾� �Ѿ� ��������Ʈ �迭���� �޾ƿ�
	}

	private void MoveBackground() //���ȭ�� �̵� �޼ҵ�
	{
		backgroundRenderer.material.mainTextureOffset += Vector2.right * backgroundSpeed * Time.deltaTime; //Quad ������� ������ ���ȭ�� �̵�
	}

	public void GameOver() //���� ���� �޼ҵ�
	{
		pause = true; //�Ͻ����� ���� ���� ���� : �Ͻ����� �˰����� ������
		PauseEnemies(); //�� ������Ʈ �ִϸ��̼� �Ͻ�����
		GameFinished(gameOverAudioClip); //���� ���� ����� Ŭ�� ���

		gameOverPanel.SetActive(true); //���� ���� �г� Ȱ��ȭ
#if UNITY_ANDROID
mobilePanel.SetActive(false); //����� �г� ��Ȱ��ȭ
#endif
	}

	public void Restart() //����� �޼ҵ�
	{
		LoadScene(); //Scene �ε� �� ȭ�鿡�� ������Ʈ���� �����ϴ� �޼ҵ� ȣ��
		StartCoroutine(LoadScene("GameScene")); //GameScene �ٽ� �ε�
	}

	public void ReturnToMainMenu() //���� �޴� Scene���� ���ư��� �޼ҵ�
	{
		backgroundMusicAudioSource.Stop(); //������� ����
		LoadScene(); //Scene �ε� �� ȭ�鿡�� ������Ʈ���� �����ϴ� �޼ҵ� ȣ��
		StartCoroutine(LoadScene("MainMenuScene")); //MainMenuScene �ε�
	}

	private void LoadScene() //Scene �ε� �� ȭ�鿡�� ������Ʈ���� �����ϴ� �޼ҵ�
	{
		playerController.ReleaseBulletPool(); //�÷��̾� �Ѿ� Ǯ�� ���� Release
		playerController.gameObject.SetActive(false); //�÷��̾� ������Ʈ ��Ȱ��ȭ
		enemyBulletPoolingController.ReleaseBulletPool(); //�� �Ѿ� Ǯ�� ���� Release
		for (int i = 0; i < enemyControllerList.Count; i++)
		{
			enemyControllerList[i].gameObject.SetActive(false); //�� ������Ʈ ��Ȱ��ȭ
		}
		for (int i = 0; i < itemGameObjectList.Count; i++)
		{
			itemGameObjectList[i].SetActive(false); //������ ������Ʈ ��Ȱ��ȭ
		}
		for (int I = 0; I < stageBulletGameObjectList.Count; I++)
		{
			stageBulletGameObjectList[I].gameObject.SetActive(false); //�Ѿ� ������Ʈ ��Ȱ��ȭ
		}

		gameFinishedPanel.SetActive(false); //���� Ŭ���� �г� ��Ȱ��ȭ
		gameOverPanel.SetActive(false); //���� ���� �г� ��Ȱ��ȭ
		playerPanel.SetActive(false); //�÷��̾� �г� ��Ȱ��ȭ
		mobilePanel.SetActive(false); //����� �г� ��Ȱ��ȭ
		pausePanel.SetActive(false); //�Ͻ����� �г� ��Ȱ��ȭ
		SceneLoadingPanel.SetActive(true); //Scene �ε� �г� Ȱ��ȭ
	}

	IEnumerator LoadScene(string sceneName) //Scene �ε� �޼ҵ�
	{
		yield return null;
		AsyncOperation op = SceneManager.LoadSceneAsync(sceneName); //AsyncOperation ������ �䱸�ϴ� Scene ����
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

	public void OpenPausePanel() //�Ͻ����� ȭ�� Ȱ��ȭ �޼ҵ�
	{
		if (countdown)
		{
			return; //ī��Ʈ�ٿ� ���¿��� �Ͻ����� ��ư�� �۵��ϸ� �ȵ�
		}
		pause = true; //�Ͻ����� ���� ���� ����
		pauseScoreText.text = $"���� ���� : {score}"; //���� ���� �ؽ�Ʈ ����
		playerController.PausePlayer(); //�÷��̾� ������Ʈ �Ͻ�����
		PauseEnemies(); //�� ������Ʈ �ִϸ��̼� �Ͻ�����
		pausePanel.SetActive(true); //�Ͻ����� �г� Ȱ��ȭ
		backgroundMusicAudioSource.Pause(); //������� �Ͻ�����

	}
	public void ClosePausePanel() //�Ͻ����� ȭ�� ��Ȱ��ȭ �޼ҵ�
	{
		pause = false; //�Ͻ����� ���� ���� ����
		pausePanel.SetActive(false); //�Ͻ����� ȭ�� ��Ȱ��ȭ
		playerController.PausePlayer(); //�÷��̾� �Ͻ����� ����
		PauseEnemies(); //�� ������Ʈ �ִϸ��̼� �Ͻ����� ����
		backgroundMusicAudioSource.Play(); //������� ����
	}

	private IEnumerator GameFinished() //���� Ŭ���� �޼ҵ�
	{
		yield return new WaitForSeconds(3.0f); //���� ���� �׾��� �� 3.0f�ʸ�ŭ ����� �Ŀ� �˰��� ����

		GameFinished(gameFinishedAudioClip); //���� Ŭ���� ����� Ŭ�� ���

		pause = true; //�Ͻ����� ���� ���� ���� : �Ͻ����� �˰����� ����
		playerController.PausePlayer(); //�÷��̾� ������Ʈ �Ͻ�����

		gameFinishedScoreText.text = $"���� : {score}��"; //���� Ŭ���� ���� �ؽ�Ʈ ����
		gameFinishedPanel.SetActive(true); //���� Ŭ���� �г� Ȱ��ȭ
		DataHolder.playerData.AddUnlockedStageCount(); //DataHolder�� ���� ���������� ���� �������� ����
		DataHolder.playerData.SetStageScore(DataHolder.stageNum, score); //DataHolder�� ���� �������� ������ ����
		DataHolder.playerData.AddMoney(score / 10); //DataHolder�� �������� ������ 10���� ���� ���� ���� ������ �߰�
		File.WriteAllText(Application.persistentDataPath + "/Data/playerData.json", JsonMapper.ToJson(DataHolder.playerData));
		//json���� ����
#if UNITY_ANDROID
mobilePanel.SetActive(false); //����� �г� ��Ȱ��ȭ
#endif
	}

	private void GameFinished(AudioClip audioClip) //���� ����(Ŭ����, ���� ����) ����� Ŭ�� ��� �޼ҵ�
	{
		backgroundMusicAudioSource.Stop(); //��� ���� ����
		backgroundMusicAudioSource.clip = audioClip; //����� Ŭ�� ����
		backgroundMusicAudioSource.loop = false; //��� ������ loop �����̱� ������ ����� Ŭ���� �� ���� ����� �� �ֵ��� ���� : ����� ȿ�����̱� ������ loop�� ��Ȱ��ȭ ������ �������� ����ȴٸ� �� �κ��� ���� ����
		backgroundMusicAudioSource.Play(); //����� Ŭ�� ���
	}

	private void GenerateBackground() //��� ȭ�� ���� �޼ҵ�
	{
		backgroundRenderer.material = Resources.Load<Material>($"Stage/Background/BackgroundMaterial{DataHolder.stageNum}"); 
		//DataHolder�� ���� �������� ��� ȭ������ material ����
	}

	private void SetDeathWallPosition() //����� �ػ� ���� DeathWall position ���� �޼ҵ�
	{
		float systemHeight = Display.main.systemHeight; //��� ��� Height ����
		float systemWidth = Display.main.systemWidth; //��� ��� Width ����

		for(int i = 0; i < deathWallTransformArray.Length; i++)
		{
			deathWallTransformArray[i].transform.position = new Vector3((systemWidth * deathWallTransformArray[i].transform.position.x) / 1920f,
																		(systemHeight * deathWallTransformArray[i].transform.position.y) / 1080f, 0); //��ʽ����� DeathWall Position ����
		}
	}

	#endregion

	#region Player Methods
	private void GeneratePlayer() //�÷��̾� ���� �޼ҵ�
	{
		Transform playerTransform = Instantiate(playerPrefab).transform; //�÷��̾� ���� �� Transform ����
		playerSpriteLibrary = playerTransform.GetComponent<SpriteLibrary>(); //�÷��̾� ��������Ʈ ���̺귯�� ������Ʈ ����
		playerRigidbody2D = playerTransform.GetComponent<Rigidbody2D>(); //�÷��̾� rigidbody ������Ʈ ����
		playerController = playerTransform.GetComponent<PlayerController>(); //�÷��̾� ��Ʈ�ѷ� ��ũ��Ʈ ����

		playerSpriteLibrary.spriteLibraryAsset = Resources.Load<SpriteLibraryAsset>($"Player/PlayerSpriteLibraryAsset{DataHolder.currentSkinNum}"); //DataHolder�� ���� ��Ų ��ȣ��
																																					//��������Ʈ ���̺귯�� ����

		joystickController.SetPlayerController(playerController); //���̽�ƽ ��Ʈ�ѷ� ��ũ��Ʈ�� �÷��̾� ��� :
																  //������� �ƴ� ��� �۵��� �ʿ䰡 ������ �����Ϳ����� Ȯ���� ���� ���� ����. ��ó����� �и� ����
	}


	#region Mobile Player Methods

	public void MobileJump() //����� ���� �޼ҵ�
	{
		if (!countdown && !pause)
		{
			playerController.MobileJump(); //�÷��̾� ���� ����
		}

	}


	public void MobileShoot() //����� �Ѿ� �߻� �޼ҵ�
	{
		if (!countdown && !pause)
		{
			playerController.MobileShoot(); //����� �Ѿ� �߻� ����
		}
	}

	#endregion

	#endregion

	#region Enemy Mehtods

	private void GenerateEnemies() //�� ���� �޼ҵ�
	{

		TextAsset enemyDataJson = Resources.Load<TextAsset>("Enemy/enemyData"); //�� ������ json �ҷ�����
		EnemyData[] enemyDataArray = JsonMapper.ToObject<EnemyData[]>(enemyDataJson.text); //JsonMapper�� struct ����
		enemyControllerList = new List<EnemyController>(); //�� ��Ʈ�ѷ� ����Ʈ ����
		for (int i = 0; i < DataHolder.stageData_Enemy.Length; i++) //���������� ��ϵ� �� ������Ʈ ���� ��ŭ
		{
			Transform enemyTransform = Instantiate(enemyPrefabArray[DataHolder.stageData_Enemy[i].enemyNum - 1]).transform; //�� ������Ʈ ���� �� Transform ���� : enemyNum���� -1�� �ϴ� ������ json���� 1�� ���� �迭������ 0������ ���ֵǱ� ����
			enemyTransform.position = new Vector2((float)DataHolder.stageData_Enemy[i].enemyPositionX, (float)DataHolder.stageData_Enemy[i].enemyPositionY); //�� ������Ʈ ��ġ ����
			EnemyData enemyData = enemyDataArray[DataHolder.stageData_Enemy[i].enemyNum - 1]; //�� ������ �迭���� 1���� ��� ����
			EnemyController enemyController = enemyTransform.GetComponent<EnemyController>();  //�� ��Ʈ�ѷ� ��ũ��Ʈ ����
			enemyController.SetEnemyData(enemyData.enemyHealth, enemyData.enemySpeed, enemyData.enemyDamage, enemyData.enemyBullet, enemyData.enemyBulletSpeed, enemyData.enemyScore, DataHolder.stageData_Enemy[i].enemyNum, playerController); //�� ������ ���� �޼ҵ� ȣ��
			enemyControllerList.Add(enemyController); //�� ��Ʈ�ѷ� ��ũ��Ʈ ����Ʈ �߰�
		}


	}

	public void AddScore(int score) //���� �׾��� �� ������ �߰��Ǵ� �޼ҵ�
	{
		this.score += score; //���� �߰�
	}

	public void AddEnemyDeaths() //���� �׾��� �� ī��Ʈ�ϴ� �޼ҵ�
	{
		enemyDeaths++; //�� ���� ī��Ʈ
		if (enemyDeaths == enemyControllerList.Count) //���������� ��� ���� �׾��� ��
		{
			StartCoroutine(GameFinished()); //GameFinished �ڷ�ƾ ����
		}
	}

	private void PauseEnemies() //�Ͻ������� �� �ִϸ��̼� ���� �޼ҵ�
	{
		for (int i = 0; i < enemyControllerList.Count; i++)
		{
			enemyControllerList[i].PauseEnemy(); //�� ��Ʈ�ѷ� ��ũ��Ʈ�� �Ͻ����� �޼ҵ带 ���� ȣ��
		}
	}

	#endregion

	#region Item Methods

	private void GenerateItems() //������ ���� �޼ҵ�
	{
		itemGameObjectList = new List<GameObject>(); //������ ������Ʈ ����Ʈ ����
		if (DataHolder.stageData_Item != null) //���������� ������ ������ ������Ʈ�� ��õǾ� ���� ��
		{
			for (int i = 0; i < DataHolder.stageData_Item.Length; i++) //���������� ��ϵ� ������ ������ŭ
			{
				Transform itemTransform = Instantiate(itemPrefabArray[DataHolder.stageData_Item[i].itemNum - 1]).transform; //������ ���� �� Transform ���� : itemNum���� -1�� �ϴ� ������ json���� 1�� �������� �迭������ 0������ ���ֵǱ� ����
				itemTransform.position = new Vector2((float)DataHolder.stageData_Item[i].itemPositionX, (float)DataHolder.stageData_Item[i].itemPositionY); //������ ������Ʈ ��ġ ����
				itemTransform.GetComponent<ItemController>().SetPlayerController(playerController); //������ ��Ʈ�ѷ� ��ũ��Ʈ�� �÷��̾� ���
				itemGameObjectList.Add(itemTransform.gameObject); //������ ������Ʈ ����Ʈ �߰�
			}
		}
	}

	#endregion

	#region Bullet Methods

	private void GenerateBullets() //�Ѿ� ���� �޼ҵ�
	{
		PlayerBulletPoolingController playerBulletPoolingController = null; //�÷��̾� �Ѿ� Ǯ�� ��Ʈ�ѷ� ��ũ��Ʈ ���� : null�� �ʱ�ȭ�ϴ� ������ �޸� ������ �Ҵ��ϴ� �ڵ尡 if�� �ȿ� �����ִµ� �������� �Ѿ� ���� �˰��򿡼� ������ �����ؾ� �ϱ� ����

		TextAsset playerBulletDataJson = Resources.Load<TextAsset>("Bullet/Player/playerBulletData"); //�������� �Ѿ� ������ json �ҷ�����
		playerBulletDataArray = JsonMapper.ToObject<PlayerBulletData[]>(playerBulletDataJson.text); //JsonMapper�� Struct ����
		playerBulletPoolingController = playerController.GetComponent<PlayerBulletPoolingController>(); //�÷��̾� �Ѿ� Ǯ�� ��Ʈ�ѷ� ��ũ��Ʈ ����
		playerBulletPoolingController.GenerateBulletPool(playerBulletDataArray, playerBulletPoolTransform); //�÷��̾� �Ѿ� Ǯ�� ���� �޼ҵ� ȣ��

		playerBulletSpriteArray = new Sprite[playerBulletDataArray.Length]; //ȭ�鿡 ǥ�õ� �÷��̾� �Ѿ� ��������Ʈ �迭 ����
		for (int i = 0; i < playerBulletSpriteArray.Length; i++) //ȭ�鿡 ǥ�õ� �÷��̾� �Ѿ� ��������Ʈ �迭 ���̸�ŭ
		{
			playerBulletSpriteArray[i] = Resources.Load<Sprite>(playerBulletDataArray[i].bulletSpriteLocation); //ȭ�鿡 ǥ�õ� �÷��̾� �Ѿ� ��������Ʈ �迭�� Resource�� �غ�Ǿ� �ִ� ��������Ʈ ���� ����
		}
		SetBulletImage(0); //0�� �Ѿ� �̹����� ���� �޼ҵ� ȣ��



		stageBulletGameObjectList = new List<GameObject>(); //�������� �Ѿ� ������Ʈ ����Ʈ ����
		if (DataHolder.stageData_Bullet != null) //���������� ������ �Ѿ� ������Ʈ�� ��õǾ� ���� ��
		{
			for (int i = 0; i < DataHolder.stageData_Bullet.Length; i++) //���������� ��ϵ� �Ѿ� ������ŭ
			{
				Transform stageBulletTransform = Instantiate(stageBulletPrefabArray[DataHolder.stageData_Bullet[i].bulletNum]).transform; //�Ѿ� ���� �� Transform ����
				stageBulletTransform.position = new Vector2((float)DataHolder.stageData_Bullet[i].bulletPositionX, (float)DataHolder.stageData_Bullet[i].bulletPositionY); //�Ѿ� ������Ʈ ��ġ ����
				stageBulletTransform.GetComponent<StageBulletController>().SetStageBulletData(DataHolder.stageData_Bullet[i].bulletNum, playerBulletPoolingController, playerController); //�������� �Ѿ� ������ ���� �޼ҵ� ȣ��
				stageBulletGameObjectList.Add(stageBulletTransform.gameObject); //�������� �Ѿ� ������Ʈ ����Ʈ �߰�
			}
		}


	}



	#endregion

}

