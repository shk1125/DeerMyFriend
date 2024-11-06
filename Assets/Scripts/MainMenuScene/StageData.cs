using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

public class StageData //�������� ������ Ŭ����
{
	public int stageNum; //�������� ��ȣ ����
	public string stageDescription; //�������� ���� ����
	public StageData_Enemy[] stageData_Enemy; //�������� �� ������ struct
	public StageData_Item[] stageData_Item; //�������� ������ ������ struct
	public StageData_Bullet[] stageData_Bullet; //�������� �Ѿ� ������ struct
}


public struct StageData_Enemy //�������� �� ������ struct
{
	public int enemyNum; //�� ��ȣ ����
	public double enemyPositionX; //�� x��ǥ ����
	public double enemyPositionY; //�� y��ǥ ����
}

public struct StageData_Item //�������� ������ ������ struct
{
	public int itemNum; //������ ��ȣ ����
	public double itemPositionX; //������ x��ǥ ����
	public double itemPositionY; //������ y��ǥ ����
}

public struct StageData_Bullet //�������� �Ѿ� ������ struct
{
	public int bulletNum; //�Ѿ� ��ȣ ����
	public double bulletPositionX; //�Ѿ� x��ǥ ����
	public double bulletPositionY; //�Ѿ� y��ǥ ����
}

