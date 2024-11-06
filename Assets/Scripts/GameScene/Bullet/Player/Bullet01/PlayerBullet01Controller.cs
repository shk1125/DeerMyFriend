using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBullet01Controller : PlayerBulletController //�÷��̾��� 01��° �Ѿ��� �����ϴ� Ŭ����
{
	public PlayerBullet01DetectionAreaController playerBullet01DetectionAreaController; //�� ������Ʈ ���� ������ �����ϴ� ��ũ��Ʈ ���� : �ν����Ϳ��� ���

	[SerializeField] float directionWeight; //�Ѿ� ���� ��ȯ ����

	private bool enemyTargeted; //���� Ÿ���� �Ǿ����� Ȯ���ϴ� ����
	private Transform enemyTransform; //Ÿ���õ� ���� Transform ����
	private Vector2 bulletDirectionVector; //�Ѿ� ���� ���� : ���� Ÿ���õǸ� ���� ���ؾ� ��
	
	
	private void Start()
	{
		enemyTargeted = false; //�� Ÿ���� Ȯ�� ���� �ʱ�ȭ
		directionWeight = 6f; //�Ѿ� ���� ��ȯ ���� �ʱ�ȭ
	}

	#region Action Methods

	public void TargetEnemy(bool enemyDetected, Transform enemyTransform) //Ÿ���õ� �� ������ �����ϴ� �޼ҵ�
	{
		this.enemyTransform = enemyTransform; //�� ������Ʈ�� Transform ����
		this.enemyTargeted = enemyDetected; //Ÿ���õ� ���� ���� ���� : �׳� true�� ���� �ʴ� ������ ReleaseTarget �޼ҵ忡���� TargetEnemy �޼ҵ带 ȣ���ϱ� ������ null, false�� ���޵� ���� ����
	}

	protected override void Move() //�Ѿ� �̵� �޼ҵ�
	{
		if (enemyTargeted) //���� Ÿ���õǾ��� ��
		{
			MoveWithTarget(); //Ÿ���õ� ���� ���� �̵��ϴ� �޼ҵ� ȣ��
		}
		else
		{
			MoveWithoutTarget(); //��� ���·� �̵��ϴ� �޼ҵ� ȣ��
		}
	}


	private void MoveWithoutTarget() //��� ���·� �̵��ϴ� �޼ҵ�
	{
		transform.Translate(Vector3.up * bulletSpeed * Time.deltaTime); //Vector3.up �������� �̵�
	}

	private void MoveWithTarget() //Ÿ���õ� ���� ���� �̵��ϴ� �޼ҵ�
	{
		transform.Translate(Vector3.up * bulletSpeed * Time.deltaTime); //Vector3.up �������� �̵�
		bulletDirectionVector = (enemyTransform.position - transform.position).normalized; //�� �������� ���� ���� ����
		transform.up = Vector3.Slerp(transform.up.normalized, bulletDirectionVector, directionWeight * Time.deltaTime); //�Ѿ� ������ ���� ���ϵ��� ����. directionWeight ������ ���� ����
	}

	public override void ReleaseTarget() //Ÿ���õ� ���� �����ϴ� �޼ҵ�
	{
		playerBullet01DetectionAreaController.ReleaseTarget(); //�� ������Ʈ ���� ������ �����ϴ� ��ũ��Ʈ�� ReleaseTarget �޼ҵ� ȣ��
		//�� �޼ҵ尡 �ϳ��� �������� ���ϴ� ������ �Ѿ� ������Ʈ�� Ǯ���� ȸ���� �� ���� ������ �����ϴ� ��ũ��Ʈ�� �������� �ʰ� �� ��ũ��Ʈ�� ReleaseTarget �޼ҵ忡 �����ϱ� ����
		//���� ������ �����ϴ� ��ũ��Ʈ���� Ÿ���õ� ���� �����ϴ� �˰����� ���� �ʿ���. ���� �� ��ũ��Ʈ�� ���� �˰����� ȣ���� ���� ������ �����ϴ� ��ũ��Ʈ�� �ۼ��Ͽ� �ߺ� ����
		//���� �� ������Ʈ�� ��� ���� ������ �����ϴ� ��ũ��Ʈ�� ReleaseTarget �޼ҵ带 ȣ��
	}

	#endregion

}
