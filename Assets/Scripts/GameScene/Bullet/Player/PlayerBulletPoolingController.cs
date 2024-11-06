using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class PlayerBulletPoolingController : MonoBehaviour //�÷��̾��� �Ѿ� Ǯ���� �����ϴ� Ŭ����
{
    public GameObject[] bulletPrefabArray; //�÷��̾� �Ѿ� ������ �迭 : �ν����Ϳ��� ���
	public IObjectPool<GameObject> currentBulletPool { get; private set; } //���� �߻��ϴ� �Ѿ� ������Ʈ Ǯ�� ����
	public float bulletFireRate; //�Ѿ� �߻� �ֱ�

	[Header("Pool Size Variables")]
	[SerializeField] private int defaultPoolSize = 10; //�⺻ Ǯ�� ������ ����
	[SerializeField] private int maxPoolSize = 20; //�ִ� Ǯ�� ������ ����

    private Dictionary<int, IObjectPool<GameObject>> bulletPoolDictionary; //�÷��̾� �Ѿ� Ǯ�� ��ųʸ� : �Ѿ��� ����� ������ ��ųʸ����� Ǯ�� ȣ��

	private Transform playerBulletPoolTransform; //������ �Ѿ� ������Ʈ�� �ڽ����� �� Transform
	private PlayerBulletData[] playerBulletDataArray; //�÷��̾� �Ѿ� ������ struct �迭

	
    private int bulletNum; //�Ѿ� ��ȣ ���� : �Ѿ� Ǯ�� ��ųʸ��� key�� ����


	public void ChangeBulletPool(int bulletNum) //�÷��̾��� �Ѿ� ������Ʈ�� �����ϴ� �޼ҵ�
    {
        this.bulletNum = bulletNum; //�Ѿ� ��ȣ ���� ����
        currentBulletPool = bulletPoolDictionary[this.bulletNum]; //�Ѿ� Ǯ�� ��ųʸ����� �Ѿ� ��ȣ�� key�� ������ ���� �߻��ϴ� �Ѿ� ������Ʈ Ǯ�� ������ ����
		bulletFireRate = (float)playerBulletDataArray[bulletNum].bulletFireRate; //�Ѿ� �߻� �ֱ� ������ �Ѿ� ������ �迭���� ������ ���� : �Ѿ� ������ �迭�� ������ �Ѿ� ��ȣ�� ȣȯ�Ǳ� ������ �״�� ȣ���� ������
	}
  

	public void GenerateBulletPool(PlayerBulletData[] playerBulletDataArray, Transform playerBulletPoolTransform) //�÷��̾� �Ѿ� ������Ʈ Ǯ�� ���� �޼ҵ�
	{
        this.playerBulletDataArray = playerBulletDataArray; //�÷��̾� �Ѿ� ������ struct �迭 ����
        this.playerBulletPoolTransform = playerBulletPoolTransform; //������ �Ѿ� ������Ʈ�� �ڽ����� �� Transform ����
		bulletPoolDictionary = new Dictionary<int, IObjectPool<GameObject>>(0); //��ųʸ� ����
        List<int> bulletList = new List<int>(); //�Ѿ� ��ȣ ����Ʈ ���� : ���������� ���� �Ѿ��� ������ �ʿ䰡 ���� ������ �Ѿ� ��ȣ ����Ʈ�� ���� �غ�

		
		bulletList.Add(0); //�Ѿ� ��ȣ ����Ʈ�� 0 �߰� : 0�� ����� �⺻ �����̸� �⺻ ����� DataHolder�� ���� ���� �ֱ� ������ Default�� ���� �����س���
		IObjectPool<GameObject> bulletPool = new ObjectPool<GameObject>(CreatePooledItem, OnTakeFromPool, OnReturnedToPool, OnDestroyPoolObject, true, defaultPoolSize, maxPoolSize); //�÷��̾� �Ѿ� ������Ʈ Ǯ�� ����
        bulletPoolDictionary.Add(0, bulletPool); //�Ѿ� Ǯ�� ��ųʸ��� 0�� key�� Ǯ�� 1�� �߰�

		for (int i = 0; i < DataHolder.stageData_Bullet.Length; i++)
        {
            if (!bulletList.Contains(DataHolder.stageData_Bullet[i].bulletNum)) //�Ѿ� ��ȣ ����Ʈ�� ��ȣ�� ������� �ʴٸ� : �̹� ������ ��ȣ�� �߰��� �ʿ䰡 ���� ����
            {
                bulletList.Add(DataHolder.stageData_Bullet[i].bulletNum); //DataHolder���� �Ѿ� ��ȣ�� ȣ���� �Ѿ� ��ȣ ����Ʈ�� �߰�
            }
        }
		bulletList.Sort(); //�Ѿ� ��ȣ ����Ʈ ���� ����


        

        for (int i = 1; i < bulletList.Count; i++) //i�� 1���� �����ϴ� ������ 0�� �Ѿ��� �̹� ��ųʸ��� �߰��Ǿ��� ����
        {
            bulletPool = new ObjectPool<GameObject>(CreatePooledItem, OnTakeFromPool, OnReturnedToPool, OnDestroyPoolObject, true, defaultPoolSize, maxPoolSize); //�÷��̾� �Ѿ� ������Ʈ Ǯ�� ����
			bulletPoolDictionary.Add(bulletList[i], bulletPool); //�Ѿ� ��ȣ ����Ʈ�� ������ key�� ��ųʸ��� Ǯ�� �߰�
        }

        
        for(int i = 0; i < bulletList.Count; i++)
        {
            for(int j = 0; j < defaultPoolSize; j++)
            {
                bulletNum = bulletList[i]; //�Ѿ� ��ȣ ������ �Ѿ� ��ȣ ����Ʈ�� ���� ����
                currentBulletPool = bulletPoolDictionary[bulletNum]; //���� Ǯ�� ������ �Ѿ� ��ȣ�� key�� ���� ��ųʸ��� Ǯ�� ���� ����
				PlayerBulletController playerBulletController = CreatePooledItem().GetComponent<PlayerBulletController>(); ///�⺻ Ǯ�� �����ŭ �Ѿ� ������Ʈ ���� �� Ǯ���� ����
				playerBulletController.Pool.Release(playerBulletController.gameObject); //�Ѿ� ������Ʈ���� ��Ȱ��ȭ ���·� ���
			}
        }

		
		bulletNum = 0; //ó�� �����ϴ� �Ѿ��� 0�� �Ѿ˷� ����
        currentBulletPool = bulletPoolDictionary[bulletNum]; //���� �Ѿ� Ǯ���� ��ųʸ����� �ҷ���
        bulletFireRate = (float)playerBulletDataArray[bulletNum].bulletFireRate; //�Ѿ� �߻� �ֱ⸦ �Ѿ� ������ �迭���� ȣ���� ����
	}

    public void ReleaseBulletPool() //�Ѿ� Ǯ���� ȸ���ϴ� �޼ҵ� : ���� �޴��� ���ư��⸦ �������� �� �ε� ȭ�鿡�� ������Ʈ���� ġ���� �ϰ� ���� Ǯ���� ���������� ȸ���ؾ� �ϱ� ������ ����
	{
        PlayerBulletController playerBulletController; //�÷��̾� �Ѿ� ��Ʈ�ѷ� ��ũ��Ʈ ����
		for (int i = 0; i < playerBulletPoolTransform.childCount; i++)
        {
            playerBulletController = playerBulletPoolTransform.GetChild(i).GetComponent<PlayerBulletController>();  //�÷��̾� �Ѿ� ������Ʈ Ǯ���� �ڽ� ������Ʈ�� ��ϵǾ� �ֱ� ������ ���������� ȸ���Ϸ��� �ڽĿ� �����ϸ� �ȴ�.
			if (playerBulletController.gameObject.activeSelf) //Pool.Release���� �̹� Release�� �� ���� �ٽ� Release�ߴٴ� ������ �߻�. �˻� ��� ����Ƽ ������ ���׷� ����. ���ǹ����� ������ ����.
			{
                playerBulletController.Pool.Release(playerBulletController.gameObject);  //�÷��̾� �Ѿ� ��Ʈ�ѷ� ��ũ��Ʈ�� ����� Ǯ�� ������ ȸ��
			}
		}
    }


	private GameObject CreatePooledItem() //�÷��̾� �Ѿ� ������Ʈ ���� �޼ҵ�
	{
		PlayerBulletController playerBulletController  = Instantiate(bulletPrefabArray[bulletNum], playerBulletPoolTransform).GetComponent<PlayerBulletController>();  //�÷��̾� �Ѿ� ������Ʈ ���� �� ��Ʈ�ѷ� ��ũ��Ʈ ����
		playerBulletController.SetPlayerBulletData(playerBulletDataArray[bulletNum].bulletSpeed, playerBulletDataArray[bulletNum].bulletDamage, currentBulletPool); //�÷��̾� �Ѿ� ��Ʈ�ѷ� ��ũ��Ʈ�� ���� �޼ҵ� ȣ��
		
        return playerBulletController.gameObject;  //�Ѿ� ������Ʈ�� Ǯ���� �߰���
	}

    
    private void OnTakeFromPool(GameObject bulletPool) // Ǯ�� ���
    {
        bulletPool.SetActive(true);
    }

    
    private void OnReturnedToPool(GameObject bulletPool) // Ǯ�� ��ȯ
    {
        bulletPool.SetActive(false);
    }

    
    private void OnDestroyPoolObject(GameObject bulletPool) // Ǯ�� ����
    {
        Destroy(bulletPool);
    }

	
}
