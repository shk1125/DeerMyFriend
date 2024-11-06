using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerBulletDetectController : MonoBehaviour //플레이어 총알의 적 감지를 관리하는 클래스
{
    public abstract void ReleaseTarget(); //적 감지 상태 해제 메소드

}
