using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet04ChildController : MonoBehaviour //4�� �� ������Ʈ�κ��� �߻�� ������ �Ѿ��� �����ϴ� Ŭ����
{
	[SerializeField] private float enemyBulletSpeed; //�Ѿ� �ӵ� ����
	[SerializeField] private float enemyDamage; //�Ѿ� ������ ���� : �� ������Ʈ�� �÷��̾�� �浹���� ���� ������ ���� ������

	private EnemyBullet04Controller enemyBullet04Controller; //�θ� ������Ʈ�� ��Ʈ�ѷ� ��ũ��Ʈ : CountBullet �޼ҵ� ȣ�⿡ ����

	#region Setting Methods

	public void SetBulletData(EnemyBullet04Controller enemyBullet04Controller, float enemyBulletSpeed, float enemyDamage) //�Ѿ� ������ �����ϴ� �޼ҵ�
	{
		this.enemyBullet04Controller = enemyBullet04Controller; //�θ� ������Ʈ�� ��Ʈ�ѷ� ��ũ��Ʈ ����
		this.enemyBulletSpeed = enemyBulletSpeed; //�Ѿ� �ӵ� ���� ����
		this.enemyDamage = enemyDamage; //�Ѿ� ������ ���� ����
	}

	#endregion


	#region Action Methods

	public void Move() //�Ѿ� �̵� �޼ҵ�
	{
		transform.Translate(Vector3.up * enemyBulletSpeed * Time.deltaTime); //�Ѿ� �̵� : ������ �Ѿ��� Vector3.up�� ���Ͽ� ��źó�� �����
	}

	#endregion

	private void OnTriggerEnter2D(Collider2D collision)
	{
		switch (collision.tag) //Ʈ���Ŵ� �±׷� ������
		{
			case "Player": //�÷��̾� ������Ʈ�� �ε����� ���
				{
					gameObject.SetActive(false); //�Ѿ� ������Ʈ ��Ȱ��ȭ
					enemyBullet04Controller.CountBullet(); //�θ� ������Ʈ�� ��Ʈ�ѷ� ��ũ��Ʈ CountBullet �޼ҵ� ȣ��
					collision.transform.GetComponent<PlayerController>().TakeDamage(enemyDamage); //�÷��̾� ��Ʈ�ѷ� ��ũ��Ʈ�� ������ ����
					break;
				}
			case "DeathWall": //ȭ�� �ٱ����� ���� ��� : �÷��̾�� �����¿� ��� ������ �ȵ����� ���� ���� ���⿡�� �����ϱ� ������ ������Ʈ�� �±׸� �и��س���
			case "DeathWall_Enemy": //������ �Ѿ��� ���� �ٸ� �˰����� �ֱ� ������ �������� ������ ��Ȳ�� �߻��ص� ��Ȱ��ȭ�Ǿ���
				{
					gameObject.SetActive(false); //�Ѿ� ������Ʈ ��Ȱ��ȭ
					enemyBullet04Controller.CountBullet(); //�θ� ������Ʈ�� ��Ʈ�ѷ� ��ũ��Ʈ CountBullet �޼ҵ� ȣ��
					break;
				}

		}
	}

	
}
