using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class EnemyBulletPoolingController : MonoBehaviour //�� �Ѿ� ������Ʈ�� Ǯ���� �����ϴ� Ŭ����
{
	public Dictionary<int, IObjectPool<GameObject>> bulletPoolDictionary; //�� �Ѿ� Ǯ�� ��ųʸ� : �� ������Ʈ���� �Ѿ��� �ٸ��� ������ �Ȱ��� Ǯ���� ȣ���� �� ����. ���� ��ųʸ��� �и��س���.
	public IObjectPool<GameObject> bulletPool { get; private set; } //�� �Ѿ� ��Ʈ�ѷ� ��ũ��Ʈ�� ������ Ǯ�� ����

	[Header("Pool Size Variables")]
	[SerializeField] private int defaultPoolSize = 10; //�⺻ Ǯ�� ������ ����
	[SerializeField]  private int maxPoolSize = 20; //�ִ� Ǯ�� ������ ����

	private GameObject enemyBulletPrefab; //�� �Ѿ� ������Ʈ ������ : �����Ǵ� ������ �ٸ� �������� ������ �ֱ� ������ ���������� �����صΰ� GenerateBulletPool �޼ҵ忡�� ����
	private double enemyBulletSpeed; //�� �Ѿ� �ӵ� ���� : double�� ������ Litjson�� float�� �������� �ʱ� ����
	private float enemyDamage; //�� �Ѿ� ������ ���� : �Ѿ� �ӵ� ������ double�ε� �Ѿ� ������ ������ float�� ������ �Ѿ� ������ ���� �� ������Ʈ�� �ε����� ���� ������ ���� �����ϱ� ������ �� �� �ܰ迡�� ����ȯ�� ������ ����

	

	public void GenerateBulletPool(GameObject enemyBulletPrefab, double enemyBulletSpeed, float enemyDamange, int enemyNum) //�� �Ѿ� ������Ʈ Ǯ�� ���� �޼ҵ�
	{
		bulletPool = new ObjectPool<GameObject>(CreatePooledItem, OnTakeFromPool, OnReturnedToPool, OnDestroyPoolObject, true, defaultPoolSize, maxPoolSize); //�� �Ѿ� ������Ʈ Ǯ�� ����
		this.enemyBulletPrefab = enemyBulletPrefab; //�� �Ѿ� ������ ����
		this.enemyBulletSpeed = enemyBulletSpeed; //�� �Ѿ� �ӵ� ����
		this.enemyDamage = enemyDamange; //�� �Ѿ� ������ ����
		for(int i = 0; i < defaultPoolSize; i++)
		{
			EnemyBulletController enemyBulletController = CreatePooledItem().GetComponent<EnemyBulletController>(); //�⺻ Ǯ�� �����ŭ �Ѿ� ������Ʈ ���� �� Ǯ���� ����
			enemyBulletController.Pool.Release(enemyBulletController.gameObject); //�Ѿ� ������Ʈ���� ��Ȱ��ȭ ���·� ���
		}

		if (bulletPoolDictionary == null) //��ųʸ��� �������� �ʾ��� ��� : �Ѿ��� �䱸�ϴ� �� ������Ʈ�鸶�� ���� GenerateBulletPool �޼ҵ带 ȣ���ϱ� ������ ��ųʸ��� ���ų�
										  //������ �����Ǵ� ��Ȳ�� �����ؾ� ��. ���� ������ ����� �˰���
		{
			bulletPoolDictionary = new Dictionary<int, IObjectPool<GameObject>>(); //��ųʸ� ����
		}
		

		if (!bulletPoolDictionary.ContainsKey(enemyNum)) //��ųʸ��� �Ѿ� Ǯ���� �غ���� ���� ��� : �Ȱ��� ��ȣ�� ���� �������� ������ ��� Ǯ���� ���� �� ������ �ʿ� ���� ������ Ǯ���� ����
		{
			bulletPoolDictionary.Add(enemyNum, bulletPool); //��ųʸ��� �� ��ȣ�� key��, �Ѿ� Ǯ���� value�� �߰�
		}
	}

	public void ReleaseBulletPool() //�Ѿ� Ǯ���� ȸ���ϴ� �޼ҵ� : ���� �޴��� ���ư��⸦ �������� �� �ε� ȭ�鿡�� ������Ʈ���� ġ���� �ϰ� ���� Ǯ���� ���������� ȸ���ؾ� �ϱ� ������ ����
	{
		EnemyBulletController enemyBulletController; //�� �Ѿ� ��Ʈ�ѷ� ��ũ��Ʈ ����
		for (int i = 0; i < transform.childCount; i++) //�� �Ѿ� ������Ʈ Ǯ���� �ڽ� ������Ʈ�� ��ϵǾ� �ֱ� ������ ���������� ȸ���Ϸ��� �ڽĿ� �����ϸ� �ȴ�.
		{
			enemyBulletController = transform.GetChild(i).GetComponent<EnemyBulletController>(); //�ڽ� ������Ʈ�� �� �Ѿ� ��Ʈ�ѷ� ��ũ��Ʈ ����
			if(enemyBulletController.gameObject.activeSelf) //Pool.Release���� �̹� Release�� �� ���� �ٽ� Release�ߴٴ� ������ �߻�. �˻� ��� ����Ƽ ������ ���׷� ����. ���ǹ����� ������ ����.
			{
				enemyBulletController.Pool.Release(enemyBulletController.gameObject); //�� �Ѿ� ��Ʈ�ѷ� ��ũ��Ʈ�� ����� Ǯ�� ������ ȸ��
			}
		}
	}


	private GameObject CreatePooledItem() //�� �Ѿ� ������Ʈ ���� �޼ҵ�
	{
		EnemyBulletController enemyBulletController = Instantiate(enemyBulletPrefab, transform).GetComponent<EnemyBulletController>(); //�� �Ѿ� ������Ʈ ���� �� ��Ʈ�ѷ� ��ũ��Ʈ ����
		enemyBulletController.SetEnemyBulletData(enemyBulletSpeed, enemyDamage, bulletPool); //�� �Ѿ� ��Ʈ�ѷ� ��ũ��Ʈ�� ���� �޼ҵ� ȣ��

		return enemyBulletController.gameObject; //�� �Ѿ� ������Ʈ�� Ǯ���� �߰���
	}


	private void OnTakeFromPool(GameObject bulletPool) // Ǯ�� ��� �޼ҵ�
	{
		bulletPool.SetActive(true);
	}


	private void OnReturnedToPool(GameObject bulletPool) // Ǯ�� ��ȯ �޼ҵ�
	{
		bulletPool.SetActive(false);
	}


	private void OnDestroyPoolObject(GameObject bulletPool) // Ǯ�� ���� �޼ҵ�
	{
		Destroy(bulletPool);
	}
}
