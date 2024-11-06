using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

public class StageData //스테이지 데이터 클래스
{
	public int stageNum; //스테이지 번호 변수
	public string stageDescription; //스테이지 설명 변수
	public StageData_Enemy[] stageData_Enemy; //스테이지 적 데이터 struct
	public StageData_Item[] stageData_Item; //스테이지 아이템 데이터 struct
	public StageData_Bullet[] stageData_Bullet; //스테이지 총알 데이터 struct
}


public struct StageData_Enemy //스테이지 적 데이터 struct
{
	public int enemyNum; //적 번호 변수
	public double enemyPositionX; //적 x좌표 변수
	public double enemyPositionY; //적 y좌표 변수
}

public struct StageData_Item //스테이지 아이템 데이터 struct
{
	public int itemNum; //아이템 번호 변수
	public double itemPositionX; //아이템 x좌표 변수
	public double itemPositionY; //아이템 y좌표 변수
}

public struct StageData_Bullet //스테이지 총알 데이터 struct
{
	public int bulletNum; //총알 번호 변수
	public double bulletPositionX; //총알 x좌표 변수
	public double bulletPositionY; //총알 y좌표 변수
}

