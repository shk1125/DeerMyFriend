using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;

public abstract class PlayerBulletController : MonoBehaviour //�÷��̾��� �Ѿ� ���� Ŭ����
{

    public IObjectPool<GameObject> Pool { get; set; } //�÷��̾ �߻��ϴ� �Ѿ� ������Ʈ�� �����ϴ� Ǯ ����

	[SerializeField] protected float bulletSpeed; //�Ѿ� �ӵ� ����
    [SerializeField] protected float bulletDamage; //�Ѿ� ������ ����

	#region Setting Methods
	public void SetPlayerBulletData(double bulletSpeed, double bulletDamage, IObjectPool<GameObject> currentBulletPool) //�Ѿ� ������ �����ϴ� �޼ҵ�
	{
        this.bulletSpeed = (float)bulletSpeed; //�Ѿ� �ӵ� ���� ���� : ���޹��� ������ double�� ������ Litjson�� float�� �������� �ʱ� ����. ���� ����
		this.bulletDamage = (float)bulletDamage; //�Ѿ� ������ ���� ����
		this.Pool = currentBulletPool; //Ǯ�� ���� ����
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
	protected abstract void Move(); //�Ѿ� �̵� �޼ҵ�

    public abstract void ReleaseTarget(); //�� Ÿ���� ���� �޼ҵ� : ���� Ÿ�����ؾ� �ϴ� �Ѿ� ��ũ��Ʈ���� ��ӹ޾� ����

	#endregion

	private void OnTriggerEnter2D(Collider2D collision)
    {
        switch(collision.tag) //Ʈ���Ŵ� �±׷� ������
        {
            case "Enemy": //�� ������Ʈ�� �ε����� ���
                {
					if (gameObject.activeSelf) //Pool.Release���� �̹� Release�� �� ���� �ٽ� Release�ߴٴ� ������ �߻�. �˻� ��� ����Ƽ ������ ���׷� ����. ���ǹ����� ������ ����.
					{
						Pool.Release(this.gameObject); //Ǯ���� ������Ʈ ȸ��
					}
					collision.transform.GetComponent<EnemyController>().TakeDamage(bulletDamage); //�� ������Ʈ�� ������ ����
					ReleaseTarget(); //Ÿ���� �˰����� �ִ� �Ѿ��� ��� �� Ÿ���� ���� �޼ҵ� ȣ��
					break;
                }
            case "DeathWall": //ȭ�� �ٱ����� ���� ��� : �÷��̾�� �����¿� ��� ������ �ȵ����� ���� ���� ���⿡�� �����ϱ� ������ ������Ʈ�� �±׸� �и��س���
			case "DeathWall_Enemy": //������ �Ѿ��� ���� �ٸ� �˰����� �ֱ� ������ ��� ���⿡�� ��Ȱ��ȭ�Ǿ���
				{
                    if(gameObject.activeSelf) //Pool.Release���� �̹� Release�� �� ���� �ٽ� Release�ߴٴ� ������ �߻�. �˻� ��� ����Ƽ ������ ���׷� ����. ���ǹ����� ������ ����.
					{
						Pool.Release(this.gameObject); //Ǯ���� ������Ʈ ȸ��
					}
					ReleaseTarget(); //Ÿ���� �˰����� �ִ� �Ѿ��� ��� �� Ÿ���� ���� �޼ҵ� ȣ��
					break;
                }

		}
    }

}
