using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// �޴� UI�� ���� �̹��� ��ũ��Ʈ
// ���콺 Ŭ�� �̺�Ʈ�� ���� IPointerClickHandler �������̽� ���
public class GetMenuNumber : MonoBehaviour, IPointerClickHandler
{
    // �޴� ����
    public Menu MyParant;
    // �ش� ��ũ��Ʈ�� ���� ��ü�� ��ȯ�� �ѹ��� ��
    public int MenuNumber;

    // �̹����� �ؽ�Ʈ
    public Image spriteImage;
    public Text menuText;

    // �ʱ�ȭ
    public void Init(Menu parant)
    {
        MyParant = parant;
    }

    // ���콺 Ŭ�� �̺�Ʈ
    public void OnPointerClick(PointerEventData eventData)
    {
        MyParant.MouseInput(MenuNumber);
    }


}
