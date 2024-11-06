using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBullet02Controller : PlayerBulletController //�÷��̾��� 02��° �Ѿ��� �����ϴ� Ŭ����
{
	public PlayerBullet02ChildController[] bulletChildArray; //�߻�� �Ѿ˵��� ���� �����ϴ� �ڽ� ������Ʈ�� ��Ʈ�ѷ� ��ũ��Ʈ �迭 : �ν����Ϳ��� ���



	private void Start()
	{
		for (int i = 0; i < bulletChildArray.Length; i++)
		{
			bulletChildArray[i].SetBulletData(bulletSpeed, bulletDamage); //������ �Ѿ� ������Ʈ�� ��Ʈ�ѷ� ��ũ��Ʈ�� ������ ����
		}
	}

	private void OnEnable() //Ǯ������ �Ѿ� ������Ʈ�� ������ Ȱ��ȭ���� �� �۵��ϴ� �޼ҵ�
	{
		for (int i = 0; i < bulletChildArray.Length; i++)
		{
			bulletChildArray[i].transform.position = transform.position; //�Ѿ��� �߻�Ǵ� �������� �� ������Ʈ�� ��ġ�� �Ѿ� ������Ʈ���� �̵�
			bulletChildArray[i].gameObject.SetActive(true); //������ �Ѿ� ������Ʈ Ȱ��ȭ : �Ѿ��� ������ �ε����ų� DeathWall�� �ε��� ��� ��Ȱ��ȭ�� ä�� Ǯ���� ���ư��� ����
		}
	}

	public override void ReleaseTarget() //Ÿ������ �� ���� �ʱ�ȭ : ���� Ÿ�����ϴ� �˰����� �ִ� �Ѿ�(���� : 01��° �Ѿ�) ��ũ��Ʈ�� ���� �޼ҵ�. �߻� Ŭ�������� �����߱� ������ ��Ӹ� �ް� �������� ����
	{

	}
	 
	#region Action Methods

	protected override void Move() //�Ѿ� �̵� �޼ҵ�
	{
		for (int i = 0; i < bulletChildArray.Length; i++)
		{
			bulletChildArray[i].Move(); //������ �Ѿ� ������Ʈ �̵��� ȣ��
		}
	}


	public void CountBullet() //��Ȱ��ȭ�� �Ѿ� ������Ʈ���� ������ Ȯ���ϰ� ���� ��Ȱ��ȭ�Ǿ��� ��� Ǯ������ ȸ���ϴ� �޼ҵ� : �Ѿ� ������Ʈ�� ������ ���� ���ư��� ��Ȱ��ȭ�Ǳ� ������ �Ź� üũ�ؾ� ��
	{
		for (int i = 0; i < bulletChildArray.Length; i++)
		{
			if (bulletChildArray[i].gameObject.activeSelf)
			{
				return; //���� Ȱ��ȭ�� �Ѿ��� �������� ��� Ǯ���� ȸ���ϸ� �ȵǱ� ������ return
			}
		}
		if (gameObject.activeSelf) //Pool.Release���� �̹� Release�� �� ���� �ٽ� Release�ߴٴ� ������ �߻�. �˻� ��� ����Ƽ ������ ���׷� ����. ���ǹ����� ������ ����.
		{
			Pool.Release(gameObject); //Ǯ���� ������Ʈ ȸ��
		}
	}

	#endregion
}
