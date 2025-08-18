using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class Effect_LetterChange : MonoBehaviour
{
    public Material[] letters;

    public ParticleSystem particle1;
	public ParticleSystem particle2;
	public ParticleSystem particle3;

	public bool testStart;
	public int start;

	private void Start()
	{
		particle1.Stop();
		particle2.Stop();
		particle3.Stop();
	}

	private void Update()
	{
		if (testStart && start == 0)
		{
			StartEff();
			start = 1;
		}
	}

	public void StartEff()
	{
		particle1.GetComponent<Renderer>().material = letters[Random.RandomRange(0, 2)];
		particle2.GetComponent<Renderer>().material = letters[Random.RandomRange(2, 4)];
		particle3.GetComponent<Renderer>().material = letters[Random.RandomRange(4, 6)];

		particle1.Play();
		particle2.Play();
		particle3.Play();
	}
}
