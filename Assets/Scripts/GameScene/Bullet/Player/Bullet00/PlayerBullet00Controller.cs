using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBullet00Controller : PlayerBulletController //�÷��̾��� 00��° �Ѿ��� �����ϴ� Ŭ����
{

	#region Action Methods
	protected override void Move() //�Ѿ� �̵� �޼ҵ�
	{
		transform.Translate(Vector3.up * bulletSpeed * Time.deltaTime); //Vector3.up �������� �Ѿ� �̵�
	}

	public override void ReleaseTarget() //Ÿ������ �� ���� �ʱ�ȭ : ���� Ÿ�����ϴ� �˰����� �ִ� �Ѿ�(���� : 01��° �Ѿ�) ��ũ��Ʈ�� ���� �޼ҵ�. �߻� Ŭ�������� �����߱� ������ ��Ӹ� �ް� �������� ����
	{
		
	}
	#endregion
}
