using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct EnemyData  //json���� ������ ��ȯ�� ���� ������ struct
{
    public string enemyName; //�� �̸� ���� : ���� �������� ������ �ʴ� �����̳� �� ���� ����� �߰��� �� ���� �� ����. enemyDescription ����
	public string enemyDescription; //�� ���� ����
	public double enemyHealth; //�� ü�� ����
    public double enemySpeed; //�� �ӵ� ����
    public double enemyDamage; //�� ������ ����
    public bool enemyBullet; //�� �Ѿ� �߻� ���� ����
	public double enemyBulletSpeed; //�� �Ѿ� �ӵ� ���� : ���� �Ѿ��� �߻����� ���� ��� �ӵ��� 0���� ����
	public int enemyScore; //�� ���� ����
}
