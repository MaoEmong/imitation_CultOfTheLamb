using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour
{
	// ��Ż�� Ÿ��
	public enum PortalType
	{
		Next,
		Back
	}
	// ��ġ ����
	public PortalType type;
	// �� ��Ʈ�ѷ�
	MapController MC;
	// �����̵��� trnasform
	public Transform TargetPos = null;

	// ��Ż�� �����ϴ� �ʵ� �ѹ���
	public int FieldNumber;

	// ��Ż�� �θ�(Field)
	public Field Parant;

	private void Awake()
	{
		// Field ���� ��������
		Parant = transform.GetComponentInParent<Field>();
		FieldNumber = Parant.StageNumber;

		Debug.Log($"{FieldNumber}Portal Start");
	}
	// ���� ���� ��
	public void Init(MapController map)
	{
		MC = map;
		Field[] myfield = MC.TotalField[MC.StageNumber];
		// ��Ż�� Ÿ�Կ� ���� Ÿ�� ����
		switch(type)
		{
			// ���� ��Ż�� Ÿ���� Next�� ��
			case PortalType.Next:
				// ���� ��Ż�� �����ϴ� �ʵ��� �ѹ����� ��ü �ʵ���ڿ� ������
				if (FieldNumber == myfield.Length - 1)
				{
					// �̵��� �������� null
					TargetPos = null;
				}
				// ���� ��Ż�� �����ϴ� �ʵ尡 ������ �ʵ尡 �ƴ϶��
				else
				{
					// �̵��� �������� ���� �ʵ��� ��Ż�� 0��°(Back)�� �ڽİ�ü
					TargetPos = myfield[FieldNumber + 1].Portals[0].transform.GetChild(0).transform;
				}
				break;

			// ���� ��Ż�� Ÿ���� Back�϶�
			case PortalType.Back:
				// ���� ��Ż�� �����ϴ� �ʵ尡 ���� ù��° �� ��
				if(FieldNumber <= 0)
				{
					// �̵��� ��ġ�� null
					TargetPos = null;
				}
				// ���� ù������ �ƴ϶��
				else
				{
					// �̵��� �������� ���� �ʵ��� ��Ż�� 1��°(Next)�� �ڽİ�ü
					TargetPos = myfield[FieldNumber - 1].Portals[1].transform.GetChild(0).transform;
				}
				break;
		}

		Debug.Log($"{FieldNumber} Portal Init");
	}

	// Ʈ���� ����
	private void OnTriggerEnter(Collider other)
	{
		// �浹�� ��ü�� �±װ� �÷��̾��� ���
		if(other.CompareTag("Player"))
		{
			// ȭ�� ������ ȿ��
			MC.FadeInOutImage();
			// �÷��̾� �����ǰ� ����
			StartCoroutine(GoToTarget(other));
		}
	}

	// �÷��̾� �̵� �ڷ�ƾ
	IEnumerator GoToTarget(Collider other)
	{
		// ȭ���� ������ �������� 0.3��
		yield return new WaitForSeconds(0.3f);

		// Ÿ�� �������� ����ִٸ�(������ �ʵ� �����ϰ� �Ѿ ���)
		if (TargetPos == null)
		{
			// ���� �������� Ŭ����� �����ϰ� �ѹ����� ���� ���������� �ѱ�
			// GameManager.CurStage++;

			// �̵��ϴ� ��Ż�� Ÿ���� Next�϶�
			if (type == PortalType.Next)
			{
				GameObject.Find("PlayerManager").GetComponent<ksjPlayerManager>().CurStage++;
								
				// �������� �ѹ����� 3���� ũ�ٸ�(���������������� Ŭ�����ߴٸ� / ������ �������� �ѹ����� 3��)
				if(GameObject.Find("PlayerManager").GetComponent<ksjPlayerManager>().CurStage > 3)
				{
					var pm = ksjPlayerManager.Instance;
					pm.CurStage = 0;

					pm.ChangePlayerWeapon();

					// �������� �̵�
					SceneManager.LoadScene("LoadWaitRoom");
				}
				// �ƴϸ� ����Ʈ������ �̵�
				else
				// �������� ���� ������ ��ȯ
					SceneManager.LoadScene("Select");
			}
		}
		// Ÿ�� �������� ������ �ִٸ�
		else
		{
			// �÷��̾��� ��ġ�� ����
			other.gameObject.transform.position = TargetPos.position;
			// �̵��� �ʵ��� �ѹ��� ����׷� ���
			Debug.Log($"{TargetPos.parent.GetComponent<Portal>().FieldNumber}");
		}
	}

}
