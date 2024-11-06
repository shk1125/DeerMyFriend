using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class ItemController : MonoBehaviour //������ ���� Ŭ����
{
    [Header("Item Variable")]
    [SerializeField] protected float itemHealth; //�������� �÷��̾�� �����ϴ� ü�� ����

    private PlayerController playerController; //�÷��̾� ��Ʈ�ѷ� ��ũ��Ʈ

    [Header("Audio Objects and Variables")]
    public AudioClip takeItemAudioClip; //������ ���� ����� Ŭ��

    public void SetPlayerController(PlayerController playerController) //�÷��̾� ��Ʈ�ѷ� ��ũ��Ʈ�� �����ϴ� �޼ҵ�
    {
        this.playerController = playerController; //�÷��̾� ��Ʈ�ѷ� ��ũ��Ʈ ����
    }

    public abstract void SetSpecialFunction(); //Ư�� ��� ���� �޼ҵ� : �����۸��� Ư���� ����� ���� �� �����ϴ� �޼ҵ��̴�.

	private void FixedUpdate()
	{
        if(!GameManager.instance.pause && !GameManager.instance.countdown) //�Ͻ������� ī��Ʈ�ٿ� ���°� �ƴ� ��
        {
			transform.Translate(transform.right * -1f * Time.deltaTime); //����(x�� ����)���� �̵�
		}
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if(collision.tag == "Player") //Ʈ���Ŵ� �±׷� ����
        {
            playerController.TakeItem(itemHealth, takeItemAudioClip); //�÷��̾� ��Ʈ�ѷ� ��ũ��Ʈ�� ������ �޼ҵ� ȣ��
            gameObject.SetActive(false); //������Ʈ ��Ȱ��ȭ
        }
        else if(collision.tag == "DeathWall_Enemy") //�����۰� �Ѿ��� DeathWall_Enemy�� �����ϰ� ����
		{
            gameObject.SetActive(false); //������Ʈ ��Ȱ��ȭ
        }
	}

  

}
