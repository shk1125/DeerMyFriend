using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class EnemyController : MonoBehaviour //�� ������Ʈ�� �����ϴ� Ŭ����
{
	[Header("Audio Objects and Variables")]
	public AudioSource enemyAudioSource; //�� ������Ʈ ����� �ҽ� ������Ʈ : �ν����Ϳ��� ���
	public AudioClip deathAudioClip; //���� �׾��� �� ����� ����� Ŭ�� ���� : �ν����Ϳ��� ���
	public AudioClip shootAudioClip; //���� �Ѿ��� �߻��� �� ����� ����� Ŭ�� ���� : �ν����Ϳ��� ���

	[Header("Enemy Variables")]
	[SerializeField] private float enemyHealth; //�� ü�� ����
	[SerializeField] protected float enemySpeed; //�� �ӵ� ����
	[SerializeField] protected float enemyDamange; //�� ������ ����
	[SerializeField] private int enemyScore; //�� ���� ����
	[SerializeField] protected int enemyNum; //�� ��ȣ ����

	protected PlayerController playerController; //�÷��̾� ��Ʈ�ѷ� ��ũ��Ʈ
	protected Animator enemyAnimator; //�� �ִϸ����� ������Ʈ
	protected bool arrivedToStage; //ȭ�� �� ���� Ȯ�� ����
	protected bool targeted; //�÷��̾� ������Ʈ �߰� ����

	private BoxCollider2D enemyCollider; //�� �ݶ��̴� ������Ʈ
	private bool death; //���� ���� ����
	private List<PlayerBulletDetectController> playerBulletDetectControllerList; //�÷��̾� �Ѿ� ���� ��Ʈ�ѷ� ��ũ��Ʈ
	private bool canGiveDamage = false; //���� ���� ���� ����
	private float giveDamageInterval = 0.0f; //���� �� ����


	private void Awake()
	{
		enemyAnimator = GetComponent<Animator>(); //�� �ִϸ����� ������Ʈ ����
		enemyCollider = GetComponent<BoxCollider2D>(); //�� �ݶ��̴� ������Ʈ ����
	}

	private void Start()
	{
		death = false; //���� ���� ���� ����
		arrivedToStage = false; //ȭ�� �� ���� Ȯ�� ���� ����
	}


	#region Setting Methods


	public void SetEnemyData(double enemyHealth, double enemySpeed, double enemyDamage, bool enemyBullet, double enemyBulletSpeed, int score, int enemyNum, PlayerController playerController) //�� ������ ���� �޼ҵ�
	{
		this.enemyHealth = (float)enemyHealth; //�� ü�� ���� ���� : double�� ���޹��� ������ Litjson�� float�� �������� �ʱ� ����. ���� ����
		this.enemySpeed = (float)enemySpeed; //�� �ӵ� ���� ����
		enemyDamange = (float)enemyDamage; //�� ������ ���� ����
		this.enemyScore = score; //�� ���� ���� ����
		this.enemyNum = enemyNum; //�� ��ȣ ���� ����
		this.playerController = playerController; //�÷��̾� ��Ʈ�ѷ� ��ũ��Ʈ ����
		if (enemyBullet) //�� ������Ʈ�� �Ѿ��� �߻��ϴ� �˰����� ������ ���� ��
		{
			SetEnemyBullet(enemyBulletSpeed); //�� �Ѿ� �ӵ� ������ �����Ͽ� ���� �޼ҵ� ȣ��
		}
	}

	protected abstract void SetEnemyBullet(double enemyBulletSpeed); //�� �Ѿ� ���� �޼ҵ� : �Ѿ��� �߻��ϴ� �˰����� �ִ� ��ũ��Ʈ�� ����



	#endregion

	private void FixedUpdate()
	{
		if (!GameManager.instance.pause && !death && !GameManager.instance.countdown) //������ �Ͻ������� ī��Ʈ�ٿ� ���°� �ƴ� ��� + ���� ���°� �ƴ� ���
		{
			if (arrivedToStage) //ȭ�� ������ ������ ��
			{
				MoveInside(); //ȭ�� �� �̵� �޼ҵ� ȣ��
			}
			else
			{
				MoveOutSide(); //ȭ�� �� �̵� �޼ҵ� ȣ��
			}
		}
		canGiveDamage = false; //���� ���� ���� ���� ����
	}

	private void OnTriggerStay2D(Collider2D collision)
	{
		if (collision.tag == "Player")
		{
			canGiveDamage = true; //�÷��̾� ������Ʈ�� �� ������Ʈ�� �浹�ϰ� �ִ� ���ȿ��� ���� ����
		}
	}


	private void Update()
	{
		if (0.0f < giveDamageInterval && !death) //���� �󵵰� 0���� ũ�� ���� �ʾ��� �� : death ������ ���� �ִϸ��̼ǿ� �����ϴµ�
												 //�� �ִϸ��̼��� �۵��ϴ� ���ȿ� �÷��̾ �������� ������ �ȵ�
		{
			giveDamageInterval -= Time.deltaTime; //���� �󵵸� deltaTime��ŭ ����
		}


		if (canGiveDamage == true) //������ ������ �� : OnTriggerStay2D �޼ҵ尡 ȣ��� ��
		{
			if (giveDamageInterval <= 0.0f) //���� �󵵰� 0 ���ϰ� �Ǹ�
			{
				playerController.TakeDamage(enemyDamange); //�÷��̾� ��Ʈ�ѷ� ��ũ��Ʈ�� ������ ����

				giveDamageInterval = 2.0f; //���� �󵵸� 2.0f�� �ʱ�ȭ
			}
		}
	}


	#region Action Methods
	protected abstract void MoveInside(); //ȭ�� �� �̵� �޼ҵ�

	protected void MoveInsideDefault() //ȭ�� �� �⺻ �̵� �޼ҵ� : �ܼ��� ���� �������θ� �̵��ϴ� �˰����� �� �޼ҵ带 ����ϸ� ��
	{
		transform.Translate((transform.right * -1f) * enemySpeed * Time.deltaTime); //����(�� ������Ʈ�� transform ����)���� �̵�
	}

	private void MoveOutSide() //ȭ�� �� �̵� �޼ҵ� : ȭ�� ���� �������� �˰����� ���۵Ǳ� ���̱� ������ ��� �� ������Ʈ�� �����ϰ� �̵�
	{
		transform.Translate((transform.right * -1f) * Time.deltaTime); //����(�� ������Ʈ�� transform ����)���� �̵�
	}


	public void TakeDamage(float damage) //������ ���� �޼ҵ�
	{
		enemyHealth -= damage; //�� ü���� ����������ŭ ����
		if (enemyHealth <= 0f) //�� ü���� 0 ���ϰ� �Ǹ� ���� ������ ����
		{
			ChangeAudioClip(deathAudioClip); //�״� ȿ���� ����
			enemyAnimator.SetTrigger("Death"); //�ִϸ����Ϳ� Death Ʈ���� ȣ��
			enemyCollider.enabled = false; //�״� �ִϸ��̼��� ����Ǵ� ���ȿ� �Ѿ��� �ε����� �ȵǱ� ������ �ݶ��̴��� ��Ȱ��ȭ
			death = true; //���� ���� ���� ����
			GameManager.instance.AddScore(enemyScore); //�������� ������ �� ���� �߰�
		}
	}

	public void Death() //���� �޼ҵ� : ���� �ִϸ��̼��� ������ ������ �̺�Ʈ�� ȣ���
	{
		gameObject.SetActive(false); //������Ʈ ��Ȱ��ȭ

		if (playerBulletDetectControllerList != null)
		{
			ReleaseTarget(playerBulletDetectControllerList); //�Ѿ��� �� ���� ���¸� ����
		}


		GameManager.instance.AddEnemyDeaths(); //���� �� ī��Ʈ : ���������� ��� ���� �׾��� ��� ���� Ŭ���� �˰��� �۵�
	}

	private IEnumerator GiveDamage() //�÷��̾ �������� �ִ� �޼ҵ� : ���� �ٸ� �˰������� ��ü��
	{
		while (true)
		{
			playerController.TakeDamage(enemyDamange); //�÷��̾� ��Ʈ�ѷ� ��ũ��Ʈ�� ������ ����
			yield return new WaitForSeconds(2.0f); //2.0f�� �ڿ��� �÷��̾� ������Ʈ�� �� ������Ʈ�� �������� ��� �ٽ� ������ ����
		}
	}
	
	public void PauseEnemy() //���� �Ͻ����� ���� ���ο� ���� �ִϸ��̼� ������ �����ϴ� �޼ҵ�
	{
		if (gameObject.activeSelf)
		{
			enemyAnimator.enabled = !enemyAnimator.enabled; //�ִϸ��̼� ���� Ȥ�� �����
		}
	}

	public void Targeted(PlayerBulletDetectController playerBulletDetectController) //�÷��̾� �Ѿ��� ���� �˰��� ����� �޼ҵ�
	{
		if (playerBulletDetectControllerList == null)
		{
			playerBulletDetectControllerList = new List<PlayerBulletDetectController>(); //�Ѿ� ��Ʈ�ѷ� ��ũ��Ʈ ����Ʈ�� ���� ��� ����
		}

		this.playerBulletDetectControllerList.Add(playerBulletDetectController); //�÷��̾��� �Ѿ� ��Ʈ�ѷ� ��ũ��Ʈ ����Ʈ�� ���� ������ �Ѿ� ��ũ��Ʈ �߰� : �Ѿ� ���� ���� ���ÿ� ������ �� �ֱ� ����
	}

	public void ReleaseTarget(List<PlayerBulletDetectController> playerBulletDetectControllerList)
	{
		for (int i = 0; i < playerBulletDetectControllerList.Count; i++)
		{
			playerBulletDetectControllerList[i].ReleaseTarget(); //�÷��̾� �Ѿ��� �� ���� ���¸� ����
		}
		playerBulletDetectControllerList.Clear(); //�÷��̾� �Ѿ� ��Ʈ�ѷ� ��ũ��Ʈ ����Ʈ �ʱ�ȭ
	}



	protected void ChangeAudioClip(AudioClip audioClip) //����� Ŭ���� �����ϴ� �޼ҵ�
	{
		enemyAudioSource.clip = audioClip; //����� �ҽ� ������Ʈ�� ����� Ŭ�� ����
		enemyAudioSource.Play(); //����� ����
	}


	#endregion

	private void OnTriggerEnter2D(Collider2D collision)
	{

		if (collision.tag == "DeathWall") //Ʈ���Ŵ� �±׷� ������
		{
			arrivedToStage = true; //�÷��̾�� DeathWall�� �����ϸ� ȭ�� ������ ���� ������ ����
		}
		else if (collision.tag == "DeathWall_Enemy")
		{
			//StopCoroutine(GiveDamage()); //���� DeathWall�� �����ϸ� ȭ�� ������ ���� ������ ����. �÷��̾�� �������� �ִ� �ڷ�ƾ �ߴ�
			death = true; //���� ���� ���� ����
			canGiveDamage = false;
			Death(); //�״� �޼ҵ� ȣ�� : ȭ�� ������ ������ TakeDamage �޼ҵ��� �˰����� ���� �۵��� �ʿ� ����
		}

	}


	private void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		if (enemyCollider != null)
		{
			Gizmos.DrawWireCube(transform.position, enemyCollider.size); //�ݶ��̴��� ��������  Gizmos �ۼ�
		}
	}





}
