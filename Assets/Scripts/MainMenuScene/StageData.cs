using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

public readonly struct StageData //�������� ������ struct
{
	public readonly int stageNum; //�������� ��ȣ ����
	public readonly string stageDescription; //�������� ���� ����
	public readonly StageData_Enemy[] stageData_Enemy; //�������� �� ������ struct
	public readonly StageData_Item[] stageData_Item; //�������� ������ ������ struct
	public readonly StageData_Bullet[] stageData_Bullet; //�������� �Ѿ� ������ struct
}


public readonly struct StageData_Enemy //�������� �� ������ struct
{
	public readonly int enemyNum; //�� ��ȣ ����
	public readonly double enemyPositionX; //�� x��ǥ ����
	public readonly double enemyPositionY; //�� y��ǥ ����
}

public readonly struct StageData_Item //�������� ������ ������ struct
{
	public readonly int itemNum; //������ ��ȣ ����
	public readonly double itemPositionX; //������ x��ǥ ����
	public readonly double itemPositionY; //������ y��ǥ ����
}

public readonly struct StageData_Bullet //�������� �Ѿ� ������ struct
{
	public readonly int bulletNum; //�Ѿ� ��ȣ ����
	public readonly double bulletPositionX; //�Ѿ� x��ǥ ����
	public readonly double bulletPositionY; //�Ѿ� y��ǥ ����
}

