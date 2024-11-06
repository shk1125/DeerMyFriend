using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct EnemyData  //json에서 가져와 변환할 적의 데이터 struct
{
    public string enemyName; //적 이름 변수 : 현재 버전에서 사용되지 않는 상태이나 적 도감 기능이 추가될 시 사용될 수 있음. enemyDescription 동일
	public string enemyDescription; //적 설명 변수
	public double enemyHealth; //적 체력 변수
    public double enemySpeed; //적 속도 변수
    public double enemyDamage; //적 데미지 변수
    public bool enemyBullet; //적 총알 발사 여부 변수
	public double enemyBulletSpeed; //적 총알 속도 변수 : 적이 총알을 발사하지 않을 경우 속도는 0으로 고정
	public int enemyScore; //적 점수 변수
}
