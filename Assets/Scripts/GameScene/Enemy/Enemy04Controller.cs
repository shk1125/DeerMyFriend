using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy04Controller : EnemyController //04�� �� ������Ʈ�� �����ϴ� Ŭ����
{

	public GameObject enemyBulletPrefab; //�� �Ѿ� ������ : �ν����Ϳ��� ���

	private bool moveUp; //�̵� ������ �����ϴ� ����

	private void Start()
	{
		moveUp = true; //�̵� ������ �����ϴ� ���� ����
	}

	#region Setting Methods

	protected override void SetEnemyBullet(double enemyBulletSpeed) //�� �Ѿ� ���� �޼ҵ� 
	{
		GameManager.instance.enemyBulletPoolingController.GenerateBulletPool(enemyBulletPrefab, enemyBulletSpeed, enemyDamange, enemyNum); //�� �Ѿ� Ǯ�� ����
	}

	#endregion

	#region Action Methods

	protected override void MoveInside() //ȭ�� �ȿ��� �̵��ϴ� �޼ҵ�
	{
		if (transform.position.x > 6.0f) //ȭ�� ���̶�� ������ DeathWall ������Ʈ�� �������� �ϴµ� 04�� ���� �� �� �������� ���� �˰����� �۵����Ѿ� �Ѵ�. ���� x�� �������� 6.0f���� �⺻ �̵� ���¸� �����Ѵ�.
		{
			MoveInsideDefault(); //�⺻ �̵� ���� ����
		}
		else //�˰��� ���� ���������� �̵����� ��
		{
			if (transform.position.y > 3.0f) //3.0f���� ����(y�� ����)���� �̵����� ��
			{
				moveUp = false; //�̵� ������ �Ʒ�(y�� ����)�� ����
			}
			else if (transform.position.y < -3.0f) //-3.0f���� �Ʒ���(y�� ����)���� �̵����� ��
			{
				moveUp = true; //�̵� ������ ��(y�� ����)�� ����
			}

			if (moveUp) 
			{
				transform.Translate(Vector3.up * enemySpeed * Time.deltaTime); //��(y�� ����)�� �̵�
			}
			else
			{
				transform.Translate(Vector3.down * enemySpeed * Time.deltaTime); //�Ʒ�(y�� ����)�� �̵�
			}
		}
	}


	public void Attack() //���� �ִϸ��̼� ���� �޼ҵ� : �Ѿ� �߻� �ֱⰡ �ִϸ��̼ǿ� �̺�Ʈ�� ��ϵǾ� ����, �ʹ� ������ �߻� �ֱ⸦ �и��ؼ� ���� ���� ���ɼ� ����
	{
		if (arrivedToStage) //ȭ�� �ȿ��� �̵��� ���� �Ѿ� �߻� �ִϸ��̼� �۵�
		{
			enemyAnimator.SetTrigger("Attack"); //�ִϸ����Ϳ� Attack Ʈ���� ȣ��
		}
	}

	public void Shoot() //�Ѿ� ������Ʈ�� �߻��ϴ� �޼ҵ� : �ִϸ��̼��� �̺�Ʈ�� ȣ���
	{
		ChangeAudioClip(shootAudioClip); //�Ѿ� �߻� ȿ���� ����

		var bullet = GameManager.instance.enemyBulletPoolingController.bulletPoolDictionary[enemyNum].Get(); //�Ѿ� ������Ʈ Ǯ�� ȣ��
		bullet.transform.position = this.transform.position; //�Ѿ� position�� ���� �� ������Ʈ�� position���� �̵�
	}


	#endregion



}
