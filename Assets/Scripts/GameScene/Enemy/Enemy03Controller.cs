using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy03Controller : EnemyController //03�� �� ������Ʈ�� �����ϴ� Ŭ����
{
	private Rigidbody2D enemyRigidbody; //�� ������Ʈ rigidbody ����
	
	private void Start()
	{
		enemyRigidbody = GetComponent<Rigidbody2D>(); //�� ������Ʈ rigidbody ���� ����
		targeted = false; //�÷��̾� ������Ʈ �߰� ���� ����
	}

	#region Setting Methods

	protected override void SetEnemyBullet(double enemyBulletSpeed) //�� �Ѿ� ���� �޼ҵ� : �Ѿ��� �߻��ϴ� �˰����� �ִ� �� ��ũ��Ʈ(���� : 04�� �� ������Ʈ)�� ���� �޼ҵ�. �߻� Ŭ�������� �����߱� ������ ��Ӹ� �ް� �������� ����
	{

	}

	#endregion

	#region Action Methods

	protected override void MoveInside() //ȭ�� �ȿ��� �̵��ϴ� �޼ҵ�
	{
		if (!targeted)
		{
			MoveInsideDefault(); //03�� �� ������Ʈ�� �÷��̾� ������Ʈ �߰� �� �⺻ �̵� �޼ҵ带 �״�� �̿���
		}
		if (transform.position.x <= playerController.transform.position.x) //03�� ���� �÷��̾� ������Ʈ�� ��(y�� ����)���� rigidbody�� gravity�� �������� �߶��ϴ� �˰�����
		{
			targeted = true; //�÷��̾� ������Ʈ �߰� ���� ���� : �̵��� ���߰� �Ʒ�(y�� ����)���� �߶���
			enemyRigidbody.gravityScale = 1f; //0�̾��� rigidbody ������Ʈ�� gravityScale�� 1�� ����
		}

	}

	#endregion

	
}
