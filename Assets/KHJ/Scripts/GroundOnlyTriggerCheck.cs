using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// �÷��̾ �ʵ�� ���Դ��� üũ�ϱ� ���� Trigger
public class GroundOnlyTriggerCheck : MonoBehaviour
{
    public Field Parant;

	private void Start()
	{
		// Field ���� ��������
		Parant = transform.GetComponentInParent<Field>();
	}

	// Field�� �� ����
	void Update()
    {
        //if (Parant.isCleared)
           // Parant.OpenDoor();
    }

	// �浹 �˻�
	private void OnTriggerEnter(Collider other)
	{
		// �÷��̾� �浹 ��
		if (other.CompareTag("Player"))
		{
			Debug.Log($"{Parant.isCleared}");
			// ���� ���� Ŭ���� ���� �ʾҴٸ�
			if (!Parant.isCleared)
			{
				// ���ݱ�
				Parant.CloseDoor();
				Parant.isStartBattle = true;
				// ���� ��ȯ //=========== ���� - Field���� ��ȯ�� ��
				// StartCoroutine(Parant.SummonEnemy());
			}
		}
	}
}
