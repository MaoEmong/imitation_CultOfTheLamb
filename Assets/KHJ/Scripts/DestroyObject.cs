using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

// 파괴 가능한 오브젝트
public class DestroyObject : MonoBehaviour
{
    // 파괴하기 위한 체력 수치
    public int Hp = 1;

	// 파괴시 바로 사라질 오브젝트를 자식으로 갖는 빈오브젝트
	public GameObject VisibleObjects;

    // 파티클 부모객체(빈 오브젝트)
	public GameObject ParticleEffect;
	// 부모객체에서 뽑아올 자식객체 리스트
    public List<GameObject> Particles;

	private void Start()
	{
		// 자식객체 가져오기
		for(int i = 0; i < ParticleEffect.transform.childCount; i++) 
		{
			GameObject obj = ParticleEffect.transform.GetChild(i).gameObject;
			obj.SetActive(false);
			Particles.Add(obj);
		}

	}

	// 충돌검사
	private void OnTriggerEnter(Collider other)
	{
		/*
		// 충돌한 물체의 태그가 플레이어 공격 일때
		if(other.CompareTag("PlayerAttack"))
		{
			// 체력--
			Hp--;
			// 체력이 0보다 작아질 때
			if(Hp <= 0)
			{
				// 파티클용 오브젝트 뿌리기
				StartCoroutine(ParticleObjectEffect());

				// 오브젝트 비활성화
				VisibleObjects.SetActive(false);
			}
		}
		*/
	}

	// 파티클 이펙트 효과
	IEnumerator ParticleObjectEffect()
	{
		ParticleEffect.SetActive(true);

		foreach (var n in Particles)
		{
			n.SetActive(true);

			// 좌우 힘
			float randx = Random.Range(-5.0f, 5.0f);
			// 상하 힘
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
