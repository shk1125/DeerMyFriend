using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageBulletController : MonoBehaviour //�������� �Ѿ� ������Ʈ�� �����ϴ� Ŭ����
{
	[Header("Audio Objects and Variables")]
	public AudioClip stageBulletAudioClip; //�Ѿ� ������Ʈ�� �������� �� ���� ����� Ŭ�� ���� : �ν����Ϳ��� ���

	private PlayerController playerController; //�÷��̾� ��Ʈ�ѷ� ��ũ��Ʈ
	private PlayerBulletPoolingController playerBulletPoolingController; //�÷��̾� �Ѿ� Ǯ�� ��Ʈ�ѷ� ��ũ��Ʈ
	private int bulletNum; //�Ѿ� ��ȣ ����

	#region Setting Methods
	public void SetStageBulletData(int bulletNum, PlayerBulletPoolingController playerBulletPoolingController, PlayerController playerController) //�������� �Ѿ� ������Ʈ ������ ���� �޼ҵ�
	{
		this.bulletNum = bulletNum; //�Ѿ� ��ȣ ���� ����
		this.playerBulletPoolingController = playerBulletPoolingController; //�Ѿ� Ǯ�� ��Ʈ�ѷ� ��ũ��Ʈ ����
		this.playerController = playerController; //�÷��̾� ��Ʈ�ѷ� ��ũ���� ����
	}

	#endregion

	private void FixedUpdate()
	{
		if (!GameManager.instance.pause && !GameManager.instance.countdown) //���� ���� ���°� �Ͻ������� ī��Ʈ�ٿ��� �ƴ� ���
		{
			transform.Translate(transform.right * -1f * Time.deltaTime); //ȭ�� �̵�
		}
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if(collision.tag == "Player") //Ʈ���Ŵ� �±׷� ������
		{
			playerController.TakeBullet(stageBulletAudioClip); //�Ѿ� ���� ȿ���� ����
			playerBulletPoolingController.ChangeBulletPool(bulletNum); //�÷��̾� �Ѿ� Ǯ�� ��Ʈ�ѷ� ��ũ��Ʈ���� ���� �Ѿ� Ǯ�� ����
			GameManager.instance.SetBulletImage(bulletNum); //UI�� ǥ�õ� ���� �Ѿ� ��������Ʈ ����
			gameObject.SetActive(false); //������Ʈ ��Ȱ��ȭ
		}
		if(collision.tag == "DeathWall_Enemy") //�����۰� �Ѿ��� DeathWall_Enemy�� �����ϰ� ����
		{
			gameObject.SetActive(false); //������Ʈ ��Ȱ��ȭ
		}
	}
}
