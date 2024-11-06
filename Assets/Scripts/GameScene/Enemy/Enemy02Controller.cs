using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy02Controller : EnemyController //02�� �� ������Ʈ�� �����ϴ� Ŭ����
{ 
	private Vector2 enemyDirectionVector; //���� ���� ����
	
	private void Start()
	{
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
		if (!targeted) //�÷��̾� ������Ʈ�� �߰����� �� : 02�� ���� ȭ�� �ȿ� �����ڸ��� �÷��̾ ���ϴ� �˰�����
		{
			enemyDirectionVector = (playerController.transform.position - transform.position).normalized; //�÷��̾� ������Ʈ�� ���� ���� ���
			transform.right = -enemyDirectionVector; //�÷��̾� ������Ʈ�� ���� ���� ��ȯ
			targeted = true; //�÷��̾� ������Ʈ �߰� ���� ���� : �� �� �÷��̾ ���� ������ �����ϸ� �״�� �����ϴ� �˰����̱� ������ ���ǹ��� 1���� �۵��ؾ���
		}
		if (targeted)
		{
			MoveInsideDefault(); ////02�� �� ������Ʈ�� �÷��̾� ������Ʈ�� ���� ������ ������ �� �⺻ �̵� �޼ҵ带 �״�� �̿���
		}
	}

	#endregion



	
}
