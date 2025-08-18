using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

// 스테이지 이미지에 적용할 스크립트 / 클릭 인터페이스 상속
public class UI_StageImage : MonoBehaviour,IPointerClickHandler
{
	// 선택시 적용할 스테이지 넘버링
	public int SetStageNumber;
	// 스테이지 변경 스크립트(부모가 가진 스크립트)
	StageSelect select;

	// 부모객체에서 스크립트 정보 가져오기
	void Start()
	{
		select = transform.GetComponentInParent<StageSelect>();
	}

	// 클릭 함수
	public void OnPointerClick(PointerEventData eventData)
	{
		Debug.Log($"Touch!{gameObject.name}");
		// 이미지 클릭 시 클릭한 이미지가 가지고 있는 넘버링 넘겨주기
		select.SetSelectNumber(SetStageNumber);
	}
}
