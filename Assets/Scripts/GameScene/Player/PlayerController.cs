using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;


public class PlayerController : MonoBehaviour //�÷��̾� ���� Ŭ����
{
	[Header("Audio Objects and Variables")]
	public AudioSource playerAudioSource; //�÷��̾� Audio Source ������Ʈ
	public AudioClip jumpAudioClip; //���� ����� Ŭ��
	public AudioClip shootAudioClip; //�Ѿ� �߻� ����� Ŭ��
	public AudioClip takeDamageAudioClip; //������ ���� ����� Ŭ��

	[SerializeField] private float jumpForce; //�÷��̾� ���� ���� ����
	[SerializeField] private float movementSpeed; //�÷��̾� �̵� �ӵ� ����
	[SerializeField] private float currentHealth; //�÷��̾� ���� ü�� ����
	[SerializeField] private float maxHealth; //�÷��̾� �ִ� ü�� ����

	private BoxCollider2D playerCollider; //�÷��̾� �ݶ��̴�
	private Rigidbody2D playerRigidbody; //�÷��̾� rigidbody
	private Animator playerAnimator; //�÷��̾� ���� �ִϸ�����
	private PlayerBulletPoolingController playerBulletPoolingController; //�÷��̾� �Ѿ� Ǯ�� ��ũ��Ʈ

	private bool gameOver; //���� ���� Ȯ�� ����
	private float bulletFireRate; //�Ѿ� �߻� �ֱ�
	private float movementDirection; //�÷��̾� �̵� ����

	private void Awake()
	{
		playerRigidbody = GetComponent<Rigidbody2D>(); //rigidbody ����
		playerBulletPoolingController = gameObject.GetComponent<PlayerBulletPoolingController>(); //�Ѿ� Ǯ�� ��ũ��Ʈ ����
		playerAnimator = gameObject.GetComponent<Animator>(); //�ִϸ����� ����
		playerCollider = GetComponent<BoxCollider2D>(); //�ݶ��̴� ����
	}

	void Start()
	{
		jumpForce = 6.5f; //���� ���� ���� ����
		movementSpeed = 3.0f; //�̵� �ӵ� ���� ����
		maxHealth = 10f; //�ִ� ü�� ���� ����
		currentHealth = 10.0f; //���� ü�� ���� ����
		bulletFireRate = 0; //�Ѿ� �߻� �� ���� ����
		GameManager.instance.SetHealthText(currentHealth, maxHealth); //UI�� ü�� ǥ��
	}


	void Update()
	{
		if (!gameOver && !GameManager.instance.pause && !GameManager.instance.countdown) //���� ����, �Ͻ� ����, ī��Ʈ�ٿ� ���°� �ƴ� ���
		{
			Move(); //�̵� �޼ҵ� ����

			Shoot(); //�Ѿ� �߻� �޼ҵ� ����

			if (Input.GetKeyDown(KeyCode.Escape))
			{
				GameManager.instance.OpenPausePanel(); //�Ͻ����� ����
			}
		}
		else if (GameManager.instance.pause)
		{
			if (Input.GetKeyDown(KeyCode.Escape))
			{
				GameManager.instance.ClosePausePanel(); //�Ͻ����� ����
			}
		}

		bulletFireRate += Time.deltaTime; //�Ѿ� �߻� �ֱ� ���
	}

	#region Action Methods

	void Move() //�̵� �޼ҵ�
	{
		movementDirection = Input.GetAxisRaw("Horizontal"); //�̵� ���� �Է�

		transform.Translate(Vector3.right * movementDirection * movementSpeed * Time.deltaTime); //�̵� �ӵ��� �̵� ���⿡ ���߾� �̵�


		if (Input.GetKeyDown(KeyCode.UpArrow)) //���� �Է�
		{
			ChangeAudioClip(jumpAudioClip); //���� ����� Ŭ�� ����

			playerAnimator.Rebind(); //���� �ִϸ��̼� ������ ���� �ִϸ����� �ʱ�ȭ
			playerAnimator.SetTrigger("Jump"); //���� �ִϸ��̼� ����

			playerRigidbody.velocity = Vector2.zero; //rigidbody velocity �ʱ�ȭ
			playerRigidbody.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse); //Impulse�� ����
		}
	}

	public void Shoot() //�Ѿ� �߻� �޼ҵ�
	{
		if (Input.GetKeyDown(KeyCode.Space) && bulletFireRate >= playerBulletPoolingController.bulletFireRate) //�Ѿ� �߻� �Է� : �߻� �ֱⰡ ����� ���
		{
			ChangeAudioClip(shootAudioClip); //�Ѿ� �߻� ����� Ŭ�� ����

			var bullet = playerBulletPoolingController.currentBulletPool.Get(); //�Ѿ� Ǯ�� ȣ��
			bullet.transform.position = this.transform.position; //�Ѿ� ������Ʈ�� �÷��̾��� ��ġ�� �̵�
			bulletFireRate = 0; //�Ѿ� �߻� �ֱ� �ʱ�ȭ
		}
	}


	public void TakeDamage(float damage) //������ �޴� �޼ҵ�
	{

		ChangeAudioClip(takeDamageAudioClip); //������ ���� ����� Ŭ�� ����

		currentHealth -= damage; //���� ü�¿��� ��������ŭ ����
		if (currentHealth <= 0f) //���� ü���� 0 ������ ��
		{
			currentHealth = 0f; //���� ü���� 0���� ǥ��
			gameOver = true; //���� ���� ���� ����
			GameManager.instance.GameOver(); //���� ���� �˰��� ����
		}
		GameManager.instance.SetHealthText(currentHealth, maxHealth); //UI�� ü�� ǥ��
	}

	public void TakeItem(float health, AudioClip takeItemAudioClip) //������ ȹ�� �޼ҵ�
	{
		ChangeAudioClip(takeItemAudioClip); //������ ȹ�� ����� Ŭ�� ����
		this.currentHealth += health; //���� ü�¿��� ������ ü�¸�ŭ ����
		if (currentHealth > maxHealth) //���� ü���� �ִ� ü���� �ʰ��� ��
		{
			currentHealth = maxHealth; //���� ü���� �ִ� ü������ ����
		}
		GameManager.instance.SetHealthText(currentHealth, maxHealth);//UI�� ü�� ǥ��
	}

	public void TakeBullet(AudioClip stageBulletAudioClip) //�������� �Ѿ� ȹ�� �޼ҵ�
	{
		ChangeAudioClip(stageBulletAudioClip); //�Ѿ� ȹ�� ����� Ŭ�� ����
	}

	public void SetGameOver(bool gameOver) //���� ���� ���� ���� �޼ҵ�
	{
		this.gameOver = gameOver; //���� ���� ���� ����
	}

	private void ChangeAudioClip(AudioClip audioClip) //����� Ŭ�� ���� �޼ҵ�
	{
		if (!gameOver) //�÷��̾��� ���� ���¿��� ȿ������ �߻��ϸ� �ȵ�
		{
			playerAudioSource.clip = audioClip; //����� Ŭ�� ����
			playerAudioSource.Play(); //����� Ŭ�� ����
		}
	}

	public void PausePlayer() //Rigidbody2D�� �ִϸ��̼� �Ͻ����� �޼ҵ�
	{
		playerRigidbody.simulated = !playerRigidbody.simulated; //gravity�� ���� �߶����� �ʵ��� rigidbody �Ͻ�����/����
		if (gameObject.activeSelf)
		{
			playerAnimator.enabled = !playerAnimator.enabled; //�ִϸ��̼� �Ͻ����� Ȥ�� �����
		}
	}

	public void ReleaseBulletPool() //�÷��̾� �Ѿ� Ǯ�� ���� Releease �޼ҵ� : Scene ���� �� ȭ�� ���� �Ѿ� ������Ʈ�� ���� ġ���� �ϱ� ����
	{
		playerBulletPoolingController.ReleaseBulletPool(); //�÷��̾� �Ѿ� Ǯ�� ���� Release
	}

	#endregion

	#region Mobile Methods

	public void JoystickMove(bool right) //���̽�ƽ �̵� �޼ҵ�
	{
		if (right)
		{
			transform.position += Vector3.right * movementSpeed * Time.deltaTime; //������(x�� ����) �̵�
		}
		else
		{
			transform.position += Vector3.left * movementSpeed * Time.deltaTime; //����(x�� ����) �̵�
		}
	}


	public void MobileJump() //����� ���� �޼ҵ�
	{
		ChangeAudioClip(jumpAudioClip); //���� ����� Ŭ�� ����

		playerAnimator.Rebind(); //���� �ִϸ��̼� ������ ���� �ִϸ����� �ʱ�ȭ
		playerAnimator.SetTrigger("Jump"); //���� �ִϸ��̼� ����

		playerRigidbody.velocity = Vector2.zero; //rigidbody velocity �ʱ�ȭ
		playerRigidbody.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse); //Impulse�� ����
	}

	public void MobileShoot() //����� �Ѿ� �߻� �޼ҵ�
	{
		if (bulletFireRate >= playerBulletPoolingController.bulletFireRate) //�Ѿ� �߻� �Է� : �߻� �ֱⰡ ����� ���
		{
			ChangeAudioClip(shootAudioClip); //�Ѿ� �߻� ����� Ŭ�� ����

			var bullet = playerBulletPoolingController.currentBulletPool.Get(); //�Ѿ� Ǯ�� ȣ��
			bullet.transform.position = this.transform.position; //�Ѿ� ������Ʈ�� �÷��̾��� ��ġ�� �̵�
			bulletFireRate = 0; //�Ѿ� �߻� �ֱ� �ʱ�ȭ
		}
	}

	#endregion




	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.tag == "DeathWall") //Ʈ���Ŵ� �±׷� ���� : �÷��̾ ȭ�� ������ ������ ��
		{
			gameObject.SetActive(false); //������Ʈ ��Ȱ��ȭ
			GameManager.instance.GameOver(); //���� ���� �˰��� ����
		}
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		if (playerCollider != null)
		{
			Gizmos.DrawWireCube(transform.position, playerCollider.size); //�ݶ��̴��� ��������  Gizmos �ۼ�
		}
	}
}
