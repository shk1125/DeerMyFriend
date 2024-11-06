using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageBulletController : MonoBehaviour //스테이지 총알 오브젝트를 관리하는 클래스
{
	[Header("Audio Objects and Variables")]
	public AudioClip stageBulletAudioClip; //총알 오브젝트에 접촉했을 때 사용될 오디오 클립 변수 : 인스펙터에서 등록

	private PlayerController playerController; //플레이어 컨트롤러 스크립트
	private PlayerBulletPoolingController playerBulletPoolingController; //플레이어 총알 풀링 컨트롤러 스크립트
	private int bulletNum; //총알 번호 변수

	#region Setting Methods
	public void SetStageBulletData(int bulletNum, PlayerBulletPoolingController playerBulletPoolingController, PlayerController playerController) //스테이지 총알 오브젝트 데이터 세팅 메소드
	{
		this.bulletNum = bulletNum; //총알 번호 변수 저장
		this.playerBulletPoolingController = playerBulletPoolingController; //총알 풀링 컨트롤러 스크립트 저장
		this.playerController = playerController; //플레이어 컨트롤러 스크립터 저장
	}

	#endregion

	private void FixedUpdate()
	{
		if (!GameManager.instance.pause && !GameManager.instance.countdown) //현재 게임 상태가 일시정지와 카운트다운이 아닐 경우
		{
			transform.Translate(transform.right * -1f * Time.deltaTime); //화면 이동
		}
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if(collision.tag == "Player") //트리거는 태그로 구분함
		{
			playerController.TakeBullet(stageBulletAudioClip); //총알 접촉 효과음 실행
			playerBulletPoolingController.ChangeBulletPool(bulletNum); //플레이어 총알 풀링 컨트롤러 스크립트에서 현재 총알 풀링 변경
			GameManager.instance.SetBulletImage(bulletNum); //UI에 표시될 현재 총알 스프라이트 변경
			gameObject.SetActive(false); //오브젝트 비활성화
		}
		if(collision.tag == "DeathWall_Enemy") //아이템과 총알은 DeathWall_Enemy를 공유하고 있음
		{
			gameObject.SetActive(false); //오브젝트 비활성화
		}
	}
}
