using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// 메뉴 UI의 하위 이미지 스크립트
// 마우스 클릭 이벤트를 위해 IPointerClickHandler 인터페이스 상속
public class GetMenuNumber : MonoBehaviour, IPointerClickHandler
{
    // 메뉴 정보
    public Menu MyParant;
    // 해당 스크립트를 가진 객체가 반환할 넘버링 값
    public int MenuNumber;

    // 이미지와 텍스트
    public Image spriteImage;
    public Text menuText;

    // 초기화
    public void Init(Menu parant)
    {
        MyParant = parant;
    }

    // 마우스 클릭 이벤트
    public void OnPointerClick(PointerEventData eventData)
    {
        MyParant.MouseInput(MenuNumber);
    }


}
