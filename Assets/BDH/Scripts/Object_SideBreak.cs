using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Object_SideBreak : MonoBehaviour
{
	[SerializeField]
	private int maxGrassHP = 1;
	private int curGrassHP;

	[SerializeField]
	private GameObject mainGrass;
	[SerializeField]
	private GameObject[] bitGrass;

	[SerializeField]
	private GameObject grassParticlePregab;

	void Start()
	{
		curGrassHP = maxGrassHP;
	}

	public void HitGrass(bool onRight)
	{
		if (curGrassHP <= 0)
			return;

		curGrassHP--;
		print("curGrassHP : " + curGrassHP);
		if (curGrassHP <= 0)
		{
			var grassParticle = grassParticlePregab.GetComponent<Object_SideParticle>();
			grassParticle.StartGrassParticle(onRight);
			for (int i = 0; i < bitGrass.Length; i++)
			{
				bitGrass[i].SetActive(true);
			}
			mainGrass.SetActive(false);
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.layer.Equals(20))
		{
			Player_AttackLogic vec = other.transform.parent.GetComponentInChildren<Player_AttackLogic>();

			HitGrass(vec.onRight);
		}
	}
}
