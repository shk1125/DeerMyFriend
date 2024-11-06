using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class JoystickController : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler //����Ͽ� ���̽�ƽ ���� Ŭ����
{
	[Header("Lever Object and Variables")]
	public RectTransform leverRectTransform; //���̽�ƽ�� ������ ����ϴ� ������Ʈ�� Transform : �ν����Ϳ��� ���
	[SerializeField]
	private float leverRange; //������ �̵� ������ ���� ����

	[Header("Position Anchor Variables")]
	[SerializeField] private float xPositivePositionAnchor; //x ���� ��ǥ ���� ���� ����
	[SerializeField] private float xNegativePositionAnchor; //x ���� ��ǥ ���� ���� ����
	[SerializeField] private float yPositivePositionAnchor; //y ���� ��ǥ ���� ���� ����
	[SerializeField] private float yNegativePositionAnchor; //y �Ʒ��� ��ǥ ���� ���� ����

	private RectTransform rectTransform; //���̽�ƽ�� Transform
	private PlayerController playerController; //�÷��̾� ��Ʈ�ѷ� ��ũ��Ʈ
	private Vector2 clampedDir; //������ �̵� ��ġ�� ����ϴ� Vector2 ����
	private bool dragging; //���̽�ƽ ��� ���� ����


	private void Awake()
	{
		rectTransform = GetComponent<RectTransform>(); //���̽�ƽ transform ����


		//���� ���̽�ƽ�� Anchor ���� : �ػ󵵿� ���� ��ġ ������ �����ϱ� ���� Anchor�� ���ϴ����� ����������� ���̽�ƽ�� ���������� �۵��ϱ� ���ؼ� Anchor�� �߾ӿ� ������ ��.
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
		leverRange = 100f; //������ �̵� ���� ���� ����
		dragging = false; //���̽�ƽ ��� ���� ���� ����

		xPositivePositionAnchor = 1820f; //x ���� ��ǥ ���� ���� ����
		xNegativePositionAnchor = 100f; //x ���� ��ǥ ���� ���� ����
		yPositivePositionAnchor = 740f; //y ���� ��ǥ ���� ���� ����
		yNegativePositionAnchor = 340f; //y �Ʒ��� ��ǥ ���� ���� ����

	}

	private void FixedUpdate()
	{
		if (!GameManager.instance.pause && !GameManager.instance.countdown ) //�Ͻ������� ī��Ʈ�ٿ��� �ƴ� ��
		{
			if (Input.GetMouseButtonDown(0)) //����Ͽ��� �⺻ Ŭ������ ���
			{
				if(Input.mousePosition.x <= xPositivePositionAnchor && Input.mousePosition.x >= xNegativePositionAnchor
					&& Input.mousePosition.y <= yPositivePositionAnchor && Input.mousePosition.y >= yNegativePositionAnchor) //���̽�ƽ�� �ٸ� UI�� ��ġ�� �ʰ� ��ǥ ���� ������ ����
				{
					rectTransform.position = Input.mousePosition; //���̽�ƽ�� ��ġ�� Ŭ���� ������ �̵�
				}
			}

			if (dragging) //�巡������ �� : ���̽�ƽ�� ��ġ �̵��� ���� �̵��� �����ؾ� �ϱ� ������ ���ǹ����� �и�
			{
				if (clampedDir.x > 0)
				{
					playerController.JoystickMove(true); //�÷��̾� ������Ʈ�� �������� �̵�
				}
				else if (clampedDir.x < 0)
				{
					playerController.JoystickMove(false); //�÷��̾� ������Ʈ�� �������� �̵�
				}
			}
		}
	}

	public void SetPlayerController(PlayerController playerController) //�÷��̾� ��Ʈ�ѷ� ��ũ��Ʈ ���� �޼ҵ�
	{
		this.playerController = playerController; //�÷��̾� ��Ʈ�ѷ� ��ũ��Ʈ ����
	}

	#region Drag Methods

	public void OnBeginDrag(PointerEventData eventData) //�巡�� ���� �޼ҵ�
	{
		dragging = true; //���̽�ƽ ��� ���� ���� ����
		MoveLever(eventData); //���� �̵�
	}

	public void OnDrag(PointerEventData eventData) //�巡�� ���� �޼ҵ�
	{
		MoveLever(eventData); //���� �̵�
	}

	private void MoveLever(PointerEventData eventData)
	{
		var inputDir = eventData.position - rectTransform.anchoredPosition; //�巡���ϴ� ��ġ�� ���̽�ƽ ��ġ�� ���� ���
		clampedDir = inputDir.magnitude < leverRange ? inputDir : inputDir.normalized * leverRange; //�巡���ϴ� ��ġ�� �̵� ���� ������ ��� ��� ����ȭ
		leverRectTransform.anchoredPosition = clampedDir; //���� ��ġ ����

	}

	public void OnEndDrag(PointerEventData eventData) //�巡�� ���� �޼ҵ�
	{
		dragging = false; //���̽�ƽ ��� ���� ���� ����
		leverRectTransform.anchoredPosition = Vector2.zero; //������ ��ġ�� ���̽�ƽ �߽����� �̵�
	}

	#endregion
}
