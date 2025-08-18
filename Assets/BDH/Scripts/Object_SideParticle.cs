using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Object_SideParticle : MonoBehaviour
{
	[SerializeField]
	private GameObject[] GrassParticle;

	public void StartGrassParticle(bool onRight)
	{
		if (onRight)
		{
			for (int i = 0; i < GrassParticle.Length; i++)
			{
				GrassParticle[i].SetActive(true);

				var rigid = GrassParticle[i].transform.GetComponent<Rigidbody>();

				float randX = Random.Range(6, 3);
				float randY = Random.Range(6, 12);
				float randZ = Random.Range(-1, 1);

				rigid.AddForce(new Vector3(randX, randY, randZ), ForceMode.Impulse);
			}
			Invoke("DestroySelf", 3f);
		}
		else
		{
			for (int i = 0; i < GrassParticle.Length; i++)
			{
				GrassParticle[i].SetActive(true);

				var rigid = GrassParticle[i].transform.GetComponent<Rigidbody>();

				float randX = Random.Range(-3, -6);
				float randY = Random.Range(6, 12);
				float randZ = Random.Range(-1, 1);

				rigid.AddForce(new Vector3(randX, randY, randZ), ForceMode.Impulse);
			}
			Invoke("DestroySelf", 3f);
		}
	}
	private void DestroySelf()
	{
		Destroy(gameObject);
	}
}
