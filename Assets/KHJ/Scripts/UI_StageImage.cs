using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

// �������� �̹����� ������ ��ũ��Ʈ / Ŭ�� �������̽� ���
public class UI_StageImage : MonoBehaviour,IPointerClickHandler
{
	// ���ý� ������ �������� �ѹ���
	public int SetStageNumber;
	// �������� ���� ��ũ��Ʈ(�θ� ���� ��ũ��Ʈ)
	StageSelect select;

	// �θ�ü���� ��ũ��Ʈ ���� ��������
	void Start()
	{
		select = transform.GetComponentInParent<StageSelect>();
	}

	// Ŭ�� �Լ�
	public void OnPointerClick(PointerEventData eventData)
	{
		Debug.Log($"Touch!{gameObject.name}");
		// �̹��� Ŭ�� �� Ŭ���� �̹����� ������ �ִ� �ѹ��� �Ѱ��ֱ�
		select.SetSelectNumber(SetStageNumber);
	}
}
