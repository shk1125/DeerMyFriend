using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public readonly struct EnemyData  //json���� ������ ��ȯ�� ���� ������ struct
{
    public readonly string enemyName; //�� �̸� ���� : ���� �������� ������ �ʴ� �����̳� �� ���� ����� �߰��� �� ���� �� ����.
									  //enemyDescription ����
	public readonly string enemyDescription; //�� ���� ���� 
	public readonly double enemyHealth; //�� ü�� ���� : double�� ������ Litjson�� float�� �������� ����, ���� ����
	public readonly double enemySpeed; //�� �ӵ� ����
    public readonly double enemyDamage; //�� ������ ����
    public readonly bool enemyBullet; //�� �Ѿ� �߻� ���� ����
	public readonly double enemyBulletSpeed; //�� �Ѿ� �ӵ� ���� : ���� �Ѿ��� �߻����� ���� ��� �ӵ��� 0���� ����
	public readonly int enemyScore; //�� ���� ����
}
