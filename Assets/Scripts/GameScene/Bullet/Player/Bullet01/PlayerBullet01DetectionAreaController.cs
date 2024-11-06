using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerBullet01DetectionAreaController : PlayerBulletDetectController //�÷��̾��� 01��° �Ѿ��� ���� ������ �����ϴ� Ŭ����
{
	public PlayerBullet01Controller playerBullet01Controller; //�θ� ������Ʈ�� ��Ʈ�ѷ� ��ũ��Ʈ

	[SerializeField] private CircleCollider2D circleCollider; //���� ������ �ݶ��̴� ����


	private Collider2D[] enemyColliderArray; //������ �� ������Ʈ�� �ݶ��̴� ���� �迭
	private bool enemyDetected; //���� �����Ǿ����� Ȯ���ϴ� ����
	private EnemyController enemyController; //������ �� ��Ʈ�ѷ� ��ũ��Ʈ



	private void Start()
	{
		enemyDetected = false; //�� ���� Ȯ�� ���� �ʱ�ȭ
		enemyColliderArray = Physics2D.OverlapCircleAll(transform.position, circleCollider.radius); //�Ѿ��� �߻���ڸ��� ���� ���� ���ο� �� ������Ʈ�� �ִ��� Ȯ��
		CalculateEnemyDistance(); //�� ������Ʈ���� �Ÿ� ��� �޼ҵ� ȣ��
	}

	#region Action Methods

	public override void ReleaseTarget() //������ �� ���� �޼ҵ�
	{
		enemyDetected = false; //�� ���� Ȯ�� ���� �ʱ�ȭ
		playerBullet01Controller.TargetEnemy(false, null); //�θ� ������Ʈ�� ��Ʈ�ѷ� ��ũ��Ʈ�� ������ �ʱ�ȭ
		if(!playerBullet01Controller.gameObject.activeSelf) //�Ѿ� ������Ʈ�� ��Ȱ��ȭ�Ǿ��� ��
		{
			playerBullet01Controller.transform.rotation = Quaternion.Euler(0, 0, -90f); //Ÿ���õ� �Ѿ��� ���� ���� ������ �ٲٱ� ������ ��Ȱ��ȭ�Ǿ� Ǯ���� ���ư� �� ���� �ʱ�ȭ
		}
	}

	private void CalculateEnemyDistance() //�� �Ÿ� ��� �޼ҵ� : �Ѿ� ���� ������ �� ������Ʈ�� ������ ������ �� �߿� ���� ����� ���� Ÿ����
	{
		float enemyDistance = 0; //�Ÿ� �񱳿� �������� �ʱ�ȭ
		Collider2D targetedEnemyCollider = null; //���� Ÿ���õ� �� �������� �ʱ�ȭ
		for (int i = 0; i < enemyColliderArray.Length; i++)
		{
			if(Vector2.Distance(transform.position, enemyColliderArray[i].transform.position) < enemyDistance && enemyColliderArray[i].tag == "Enemy") //�Ѿ˰� ���� �Ÿ��� ���� ������������ ������ ���� ������ �� �����
			{
				targetedEnemyCollider = enemyColliderArray[i]; //���� Ÿ���� �� �� ����
			}
		}
		
		if(targetedEnemyCollider != null) //�߻����ڸ��� ������ ���� ������ ���
		{
			enemyController = targetedEnemyCollider.GetComponent<EnemyController>(); //���������� Ÿ���õ� �� ��ũ��Ʈ ����
			enemyController.Targeted(this); //���� ������ Ÿ������ �����ؾ� �ϱ� ������ �� ��ũ��Ʈ�� ������ ��������
			playerBullet01Controller.TargetEnemy(true, enemyController.transform); //�θ� ������Ʈ�� ��Ʈ�ѷ� ��ũ��Ʈ�� ���� ����
			enemyDetected = true; //�̹� Ÿ���õ� ���� �Ѿư��� ������ �ٸ� ���� ã�� �ʿ䰡 ������
		}
	}

	#endregion

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if(collision.tag == "Enemy" && !enemyDetected)
		{
			enemyController = collision.GetComponent<EnemyController>(); //���������� Ÿ���õ� �� ��ũ��Ʈ ����
			enemyController.Targeted(this); //���� ������ Ÿ������ �����ؾ� �ϱ� ������ �� ��ũ��Ʈ�� ������ ��������
			playerBullet01Controller.TargetEnemy(true, enemyController.transform); //�θ� ������Ʈ�� ��Ʈ�ѷ� ��ũ��Ʈ�� ���� ����
			enemyDetected = true; //�̹� Ÿ���õ� ���� �Ѿư��� ������ �ٸ� ���� ã�� �ʿ䰡 ������
		}
	}

	private void OnTriggerStay2D(Collider2D collision)
	{
		if(collision.tag == "Enemy" && !enemyDetected)
		{
			enemyColliderArray = Physics2D.OverlapCircleAll(transform.position, circleCollider.radius); //�� Ÿ������ ������ �Ŀ� �ݶ��̴� ������ ���� �� Ȯ��
			CalculateEnemyDistance(); //�� ������Ʈ���� �Ÿ� ��� �޼ҵ� ȣ��
		}
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(transform.position, circleCollider.radius); //���� �������� Gizmo �׸�
	}
}
