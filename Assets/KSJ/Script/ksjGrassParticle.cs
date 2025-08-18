using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ksjGrassParticle : MonoBehaviour
{
    [SerializeField]
    private GameObject[] GrassParticle;
    public void StartGrassParticle()
    {
		for (int i = 0; i < GrassParticle.Length; i++)
		{
			GrassParticle[i].SetActive(true);

			var rigid = GrassParticle[i].transform.GetComponent<Rigidbody>();

			float randX = Random.Range(-1, 1);
			float randY = Random.Range(6, 12);
			float randZ = Random.Range(-1, 1);

			rigid.AddForce(new Vector3(randX, randY, randZ), ForceMode.Impulse);
		}
		Invoke("DestroySelf", 3f);
	}
    private void DestroySelf()
    {
        Destroy(gameObject);
    }
}
