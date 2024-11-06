using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct PlayerBulletData //json에서 가져와 변환할 플레이어 총알의 데이터 struct
{
    public double bulletDamage; //총알 데미지 변수 : double인 이유는 Litjson이 float을 지원하지 않음, 이하 동일
    public double bulletSpeed; //총알 속도 변수
    public double bulletFireRate; //총알 발사 주기 변수
    public string bulletSpriteLocation; //UI에 표시될 총알 스프라이트 위치 변수
}
