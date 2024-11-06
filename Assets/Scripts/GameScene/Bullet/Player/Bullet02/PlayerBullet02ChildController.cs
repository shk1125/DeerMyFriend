using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBullet02ChildController : MonoBehaviour //�÷��̾��� 02��° �Ѿ˵��� �����ϴ� Ŭ����
{
	public PlayerBullet02Controller playerBullet02Controller; //�θ� ������Ʈ�� ��Ʈ�ѷ� ��ũ��Ʈ : CountBullet �޼ҵ� ȣ�⿡ ����. �ν����Ϳ��� ���
	[SerializeField] private float bulletSpeed; //�Ѿ� �ӵ� ����
	[SerializeField] private float bulletDamage; //�Ѿ� ������ ����

	#region Setting Methods
	public void SetBulletData(float bulletSpeed, float bulletDamage) //�Ѿ� ������ �����ϴ� �޼ҵ�
	{
		this.bulletSpeed = bulletSpeed; //�Ѿ� �ӵ� ���� ����
		this.bulletDamage = bulletDamage; //�Ѿ� ������ ���� ����
	}
	#endregion

	#region Action Methods
	public void Move() //�Ѿ� �̵� �޼ҵ�
	{
		transform.Translate(Vector3.up * bulletSpeed * Time.deltaTime); //�Ѿ� �̵� : ������ �Ѿ��� Vector3.up�� ���Ͽ� ��źó�� �����
	}
	#endregion


	private void OnTriggerEnter2D(Collider2D collision)
	{
		switch(collision.tag) //Ʈ���Ŵ� �±׷� ������
		{
			case "Enemy": //������ �ε����� ���
				{
					gameObject.SetActive(false); //�Ѿ� ������Ʈ ��Ȱ��ȭ
					playerBullet02Controller.CountBullet(); //�θ� ������Ʈ�� ��Ʈ�ѷ� ��ũ��Ʈ CountBullet �޼ҵ� ȣ��
					collision.transform.GetComponent<EnemyController>().TakeDamage(bulletDamage); //�� ������Ʈ�� ������ ����
					break;
				}
			case "DeathWall": //ȭ�� �ٱ����� ���� ��� : �÷��̾�� �����¿� ��� ������ �ȵ����� ���� ���� ���⿡�� �����ϱ� ������ ������Ʈ�� �±׸� �и��س���
			case "DeathWall_Enemy": //������ �Ѿ��� ���� �ٸ� �˰����� �ֱ� ������ ��� ���⿡�� ��Ȱ��ȭ�Ǿ���
				{
					gameObject.SetActive(false); //�Ѿ� ������Ʈ ��Ȱ��ȭ
					playerBullet02Controller.CountBullet(); //�θ� ������Ʈ�� ��Ʈ�ѷ� ��ũ��Ʈ CountBullet �޼ҵ� ȣ��
					break;
				}
		}
	}
}
