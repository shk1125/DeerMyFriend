using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy01Controller : EnemyController //01�� �� ������Ʈ�� �����ϴ� Ŭ����
{
	#region Setting Methods

	protected override void SetEnemyBullet(double enemyBulletSpeed) //�� �Ѿ� ���� �޼ҵ� : �Ѿ��� �߻��ϴ� �˰����� �ִ� �� ��ũ��Ʈ(���� : 04�� �� ������Ʈ)�� ���� �޼ҵ�. �߻� Ŭ�������� �����߱� ������ ��Ӹ� �ް� �������� ����
	{

	}

	#endregion


	#region Action Methods
	protected override void MoveInside() //ȭ�� �ȿ��� �̵��ϴ� �޼ҵ�
	{
		MoveInsideDefault(); //01�� �� ������Ʈ�� �⺻ �̵� �޼ҵ带 �״�� �̿���
	}
	#endregion
	
}
