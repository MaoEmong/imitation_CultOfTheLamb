using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Enemy_HPbar : MonoBehaviour
{
    public Slider MainBar;
    public Slider backBar;

	Enemy enemyHp;
	Boss_Logic BossHp;

	float per;

	bool nowHit;

	private void Start()
	{
		if (_type == HpType.Enemy)
		{
			enemyHp = transform.parent.gameObject.GetComponent<Enemy>();
		}
		else if (_type == HpType.Boss)
		{
			BossHp = transform.parent.gameObject.GetComponent<Boss_Logic>();
		}
	}

	private void Update()
	{
		transform.forward = Camera.main.transform.forward;
	}

	public enum HpType
	{
		Enemy,
		Boss
	}

	public HpType _type;

	public void HPbar()
	{
		if (_type == HpType.Enemy)
		{
			per = enemyHp.getHPpersent();
			MainBar.value = per;

			Invoke("BackHP", 0.5f);
		}
		else if (_type == HpType.Boss)
		{
			per = BossHp.getHPpersent();
			MainBar.value = per;

			Invoke("BackHP", 0.5f);
		}
	}

	void BackHP()
	{
		if (!nowHit && per > 0)
			StartCoroutine(hpBar());
		else if (!nowHit && per <= 0)
			backBar.value = 0;
	}

	IEnumerator hpBar()
	{
		nowHit = true;

		while(backBar.value > per)
		{
			backBar.value -= Time.deltaTime;

			yield return null;
		}

		nowHit = false;
	}
}
