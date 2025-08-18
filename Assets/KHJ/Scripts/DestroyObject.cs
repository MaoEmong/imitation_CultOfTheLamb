using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

// �ı� ������ ������Ʈ
public class DestroyObject : MonoBehaviour
{
    // �ı��ϱ� ���� ü�� ��ġ
    public int Hp = 1;

	// �ı��� �ٷ� ����� ������Ʈ�� �ڽ����� ���� �������Ʈ
	public GameObject VisibleObjects;

    // ��ƼŬ �θ�ü(�� ������Ʈ)
	public GameObject ParticleEffect;
	// �θ�ü���� �̾ƿ� �ڽİ�ü ����Ʈ
    public List<GameObject> Particles;

	private void Start()
	{
		// �ڽİ�ü ��������
		for(int i = 0; i < ParticleEffect.transform.childCount; i++) 
		{
			GameObject obj = ParticleEffect.transform.GetChild(i).gameObject;
			obj.SetActive(false);
			Particles.Add(obj);
		}

	}

	// �浹�˻�
	private void OnTriggerEnter(Collider other)
	{
		/*
		// �浹�� ��ü�� �±װ� �÷��̾� ���� �϶�
		if(other.CompareTag("PlayerAttack"))
		{
			// ü��--
			Hp--;
			// ü���� 0���� �۾��� ��
			if(Hp <= 0)
			{
				// ��ƼŬ�� ������Ʈ �Ѹ���
				StartCoroutine(ParticleObjectEffect());

				// ������Ʈ ��Ȱ��ȭ
				VisibleObjects.SetActive(false);
			}
		}
		*/
	}

	// ��ƼŬ ����Ʈ ȿ��
	IEnumerator ParticleObjectEffect()
	{
		ParticleEffect.SetActive(true);

		foreach (var n in Particles)
		{
			n.SetActive(true);

			// �¿� ��
			float randx = Random.Range(-5.0f, 5.0f);
			// ���� ��
			float randy = Random.Range(1.0f, 5.0f);

			Rigidbody rigid = n.GetComponent<Rigidbody>();
			rigid.AddForce(new Vector3(randx, randy, 0), ForceMode.Impulse);

		}

		yield return new WaitForSeconds(1.0f);

		foreach (var n in Particles)
			n.transform.position = ParticleEffect.transform.position;

		ParticleEffect.SetActive(false);

	}

}
