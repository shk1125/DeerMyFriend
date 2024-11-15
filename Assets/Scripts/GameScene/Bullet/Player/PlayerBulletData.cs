using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public readonly struct PlayerBulletData //json���� ������ ��ȯ�� �÷��̾� �Ѿ��� ������ struct
{
    public readonly double bulletDamage; //�Ѿ� ������ ���� : double�� ������ Litjson�� float�� �������� ����, ���� ����
    public readonly double bulletSpeed; //�Ѿ� �ӵ� ����
    public readonly double bulletFireRate; //�Ѿ� �߻� �ֱ� ����
    public readonly string bulletSpriteLocation; //UI�� ǥ�õ� �Ѿ� ��������Ʈ ��ġ ����
}
