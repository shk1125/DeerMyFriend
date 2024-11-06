using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class JoystickController : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler //모바일용 조이스틱 관리 클래스
{
	[Header("Lever Object and Variables")]
	public RectTransform leverRectTransform; //조이스틱의 레버를 담당하는 오브젝트의 Transform : 인스펙터에서 등록
	[SerializeField]
	private float leverRange; //레버가 이동 가능한 범위 변수

	[Header("Position Anchor Variables")]
	[SerializeField] private float xPositivePositionAnchor; //x 우측 좌표 제한 지점 변수
	[SerializeField] private float xNegativePositionAnchor; //x 좌측 좌표 제한 지점 변수
	[SerializeField] private float yPositivePositionAnchor; //y 위측 좌표 제한 지점 변수
	[SerializeField] private float yNegativePositionAnchor; //y 아래측 좌표 제한 지점 변수

	private RectTransform rectTransform; //조이스틱의 Transform
	private PlayerController playerController; //플레이어 컨트롤러 스크립트
	private Vector2 clampedDir; //레버의 이동 위치를 계산하는 Vector2 변수
	private bool dragging; //조이스틱 사용 여부 변수


	private void Awake()
	{
		rectTransform = GetComponent<RectTransform>(); //조이스틱 transform 저장


		//이하 조이스틱의 Anchor 변경 : 해상도에 따라 위치 비율을 조절하기 위해 Anchor를 좌하단으로 맞춰놓았으나 조이스틱이 정상적으로 작동하기 위해선 Anchor가 중앙에 놓여야 함.
		#region Change Rect Transform Anchor
		var rectTransformPosition = rectTransform.localPosition;
		var rectTransformWidth = rectTransform.rect.width;
		var rectTransformHeight = rectTransform.rect.height;
		rectTransform.anchorMin = Vector2.zero;
		rectTransform.anchorMax = Vector2.zero;
		rectTransform.localPosition = rectTransformPosition;
		rectTransform.sizeDelta = new Vector2(rectTransformWidth, rectTransformHeight);
		#endregion
	}

	private void Start()
	{
		leverRange = 100f; //레버의 이동 범위 변수 저장
		dragging = false; //조이스틱 사용 여부 변수 저장

		xPositivePositionAnchor = 1820f; //x 우측 좌표 제한 지점 변수
		xNegativePositionAnchor = 100f; //x 좌측 좌표 제한 지점 변수
		yPositivePositionAnchor = 740f; //y 위측 좌표 제한 지점 변수
		yNegativePositionAnchor = 340f; //y 아래측 좌표 제한 지점 변수

	}

	private void FixedUpdate()
	{
		if (!GameManager.instance.pause && !GameManager.instance.countdown ) //일시정지와 카운트다운이 아닐 때
		{
			if (Input.GetMouseButtonDown(0)) //모바일에선 기본 클릭으로 취급
			{
				if(Input.mousePosition.x <= xPositivePositionAnchor && Input.mousePosition.x >= xNegativePositionAnchor
					&& Input.mousePosition.y <= yPositivePositionAnchor && Input.mousePosition.y >= yNegativePositionAnchor) //조이스틱이 다른 UI에 겹치지 않게 좌표 제한 지점을 만듬
				{
					rectTransform.position = Input.mousePosition; //조이스틱의 위치를 클릭한 곳으로 이동
				}
			}

			if (dragging) //드래그중일 때 : 조이스틱의 위치 이동과 레버 이동을 구분해야 하기 때문에 조건문으로 분리
			{
				if (clampedDir.x > 0)
				{
					playerController.JoystickMove(true); //플레이어 오브젝트가 우측으로 이동
				}
				else if (clampedDir.x < 0)
				{
					playerController.JoystickMove(false); //플레이어 오브젝트가 좌측으로 이동
				}
			}
		}
	}

	public void SetPlayerController(PlayerController playerController) //플레이어 컨트롤러 스크립트 저장 메소드
	{
		this.playerController = playerController; //플레이어 컨트롤러 스크립트 저장
	}

	#region Drag Methods

	public void OnBeginDrag(PointerEventData eventData) //드래그 시작 메소드
	{
		dragging = true; //조이스틱 사용 여부 변수 저장
		MoveLever(eventData); //레버 이동
	}

	public void OnDrag(PointerEventData eventData) //드래그 진행 메소드
	{
		MoveLever(eventData); //레버 이동
	}

	private void MoveLever(PointerEventData eventData)
	{
		var inputDir = eventData.position - rectTransform.anchoredPosition; //드래그하는 위치와 조이스틱 위치로 벡터 계산
		clampedDir = inputDir.magnitude < leverRange ? inputDir : inputDir.normalized * leverRange; //드래그하는 위치가 이동 가능 범위를 벗어날 경우 정규화
		leverRectTransform.anchoredPosition = clampedDir; //레버 위치 변경

	}

	public void OnEndDrag(PointerEventData eventData) //드래그 종료 메소드
	{
		dragging = false; //조이스틱 사용 여부 변수 저장
		leverRectTransform.anchoredPosition = Vector2.zero; //레버의 위치를 조이스틱 중심으로 이동
	}

	#endregion
}
