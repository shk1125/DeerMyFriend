using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

public readonly struct StageData //스테이지 데이터 struct
{
	public readonly int stageNum; //스테이지 번호 변수
	public readonly string stageDescription; //스테이지 설명 변수
	public readonly StageData_Enemy[] stageData_Enemy; //스테이지 적 데이터 struct
	public readonly StageData_Item[] stageData_Item; //스테이지 아이템 데이터 struct
	public readonly StageData_Bullet[] stageData_Bullet; //스테이지 총알 데이터 struct
}


public readonly struct StageData_Enemy //스테이지 적 데이터 struct
{
	public readonly int enemyNum; //적 번호 변수
	public readonly double enemyPositionX; //적 x좌표 변수
	public readonly double enemyPositionY; //적 y좌표 변수
}

public readonly struct StageData_Item //스테이지 아이템 데이터 struct
{
	public readonly int itemNum; //아이템 번호 변수
	public readonly double itemPositionX; //아이템 x좌표 변수
	public readonly double itemPositionY; //아이템 y좌표 변수
}

public readonly struct StageData_Bullet //스테이지 총알 데이터 struct
{
	public readonly int bulletNum; //총알 번호 변수
	public readonly double bulletPositionX; //총알 x좌표 변수
	public readonly double bulletPositionY; //총알 y좌표 변수
}

