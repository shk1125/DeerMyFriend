using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class ItemController : MonoBehaviour //아이템 관리 클래스
{
    [Header("Item Variable")]
    [SerializeField] protected float itemHealth; //아이템이 플레이어에게 전달하는 체력 변수

    private PlayerController playerController; //플레이어 컨트롤러 스크립트

    [Header("Audio Objects and Variables")]
    public AudioClip takeItemAudioClip; //아이템 접촉 오디오 클립

    public void SetPlayerController(PlayerController playerController) //플레이어 컨트롤러 스크립트를 저장하는 메소드
    {
        this.playerController = playerController; //플레이어 컨트롤러 스크립트 저장
    }

    public abstract void SetSpecialFunction(); //특수 기능 세팅 메소드 : 아이템마다 특정한 기능이 있을 시 세팅하는 메소드이다.

	private void FixedUpdate()
	{
        if(!GameManager.instance.pause && !GameManager.instance.countdown) //일시정지와 카운트다운 상태가 아닐 시
        {
			transform.Translate(transform.right * -1f * Time.deltaTime); //왼쪽(x축 기준)으로 이동
		}
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if(collision.tag == "Player") //트리거는 태그로 구분
        {
            playerController.TakeItem(itemHealth, takeItemAudioClip); //플레이어 컨트롤러 스크립트의 아이템 메소드 호출
            gameObject.SetActive(false); //오브젝트 비활성화
        }
        else if(collision.tag == "DeathWall_Enemy") //아이템과 총알은 DeathWall_Enemy를 공유하고 있음
		{
            gameObject.SetActive(false); //오브젝트 비활성화
        }
	}

  

}
