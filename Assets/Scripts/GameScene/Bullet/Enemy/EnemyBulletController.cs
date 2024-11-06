using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;

public abstract class EnemyBulletController : MonoBehaviour //�� ������Ʈ�� �߻��ϴ� �Ѿ� ������Ʈ�� �����ϴ� Ŭ����
{
    public IObjectPool<GameObject> Pool { get; set; } //�� ������Ʈ�� �߻��ϴ� �Ѿ� ������Ʈ�� �����ϴ� Ǯ�� ����

	[SerializeField]  protected float enemyBulletSpeed; //�Ѿ� �ӵ� ����
	[SerializeField]  protected float enemyDamage; //�Ѿ� ������ ���� : �� ������Ʈ�� �÷��̾�� �浹���� ���� ������ ���� ������


	#region Setting Methods
	public void SetEnemyBulletData(double enemyBulletSpeed, float enemyDamage, IObjectPool<GameObject> bulletPool) //�Ѿ� ������ �����ϴ� �޼ҵ�
    {
        this.enemyBulletSpeed = (float)enemyBulletSpeed; //�Ѿ� �ӵ� ���� ���� : ���޹��� ������ double�� ������ Litjson�� float�� �������� �ʱ� ����
        this.enemyDamage = enemyDamage; //�Ѿ� ������ ���� ���� : �Ѿ� �������� float�� ������ �Ѿ� ������ ���� �� ������Ʈ�� �÷��̾ �浹���� ���� ������ ���� �����ϱ� ������ �� �ܰ迡�� ����ȯ�� ������ ����
		this.Pool = bulletPool; //Ǯ�� ���� ����
	}
	#endregion

	private void FixedUpdate()
	{
		if (!GameManager.instance.pause) //���� ������ ���°� �Ͻ������� �ƴ� ���
		{
			Move(); //�Ѿ� �̵� �޼ҵ� ȣ��
		}
	}

	#region Action Methods
	protected abstract void Move(); //�Ѿ� �̵� �޼ҵ� : ��ӹ��� ��ũ��Ʈ���� ����
	#endregion

	private void OnTriggerEnter2D(Collider2D collision)
	{
		switch (collision.tag) //Ʈ���Ŵ� �±׷� ������
		{
			case "Player": //�÷��̾� ������Ʈ�� �ε����� ���
				{
					if (gameObject.activeSelf) //Pool.Release���� �̹� Release�� �� ���� �ٽ� Release�ߴٴ� ������ �߻�. �˻� ��� ����Ƽ ������ ���׷� ����. ���ǹ����� ������ ����.
					{
						Pool.Release(this.gameObject); //Ǯ���� ������Ʈ ȸ��
					}
					collision.transform.GetComponent<PlayerController>().TakeDamage(enemyDamage); //�÷��̾� ��Ʈ�ѷ� ��ũ��Ʈ�� ������ ����
					break;
				}
			case "DeathWall": //ȭ�� �ٱ����� ���� ��� : �÷��̾�� �����¿� ��� ������ �ȵ����� ���� ���� ���⿡�� �����ϱ� ������ ������Ʈ�� �±׸� �и��س���
			case "DeathWall_Enemy": //������ �Ѿ��� ���� �ٸ� �˰����� �ֱ� ������ �������� ������ ��Ȳ�� �߻��ص� ��Ȱ��ȭ�Ǿ���
				{
					if (gameObject.activeSelf) //Pool.Release���� �̹� Release�� �� ���� �ٽ� Release�ߴٴ� ������ �߻�. �˻� ��� ����Ƽ ������ ���׷� ����. ���ǹ����� ������ ����.
					{
						Pool.Release(this.gameObject); //Ǯ���� ������Ʈ ȸ��
					}
					break;
				}

		}
	}

}
