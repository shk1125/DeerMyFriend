using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct PlayerBulletData //json���� ������ ��ȯ�� �÷��̾� �Ѿ��� ������ struct
{
    public double bulletDamage; //�Ѿ� ������ ���� : double�� ������ Litjson�� float�� �������� ����, ���� ����
    public double bulletSpeed; //�Ѿ� �ӵ� ����
    public double bulletFireRate; //�Ѿ� �߻� �ֱ� ����
    public string bulletSpriteLocation; //UI�� ǥ�õ� �Ѿ� ��������Ʈ ��ġ ����
}
