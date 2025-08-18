using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool_BossBullet : MonoBehaviour
{
	public static ObjectPool_BossBullet instance = null;

	List<GameObject> bullets;

	public GameObject bulletPrf;

	private void Awake()
	{
		if (instance == null)
		{
			instance = this;
		}
	}

	private void Start()
	{
		bullets = new List<GameObject>();

		for (int i = 0; i < 32; i++)
		{
			GameObject bullet = Instantiate(bulletPrf);

			bullets.Add(bullet);

			bullet.transform.parent = transform;

			bullet.SetActive(false);
		}
	}

	public GameObject getBullet()
	{
		GameObject bullet = null;

		if (bullets.Count > 0)
		{
			bullet = bullets[0];
			bullet.SetActive(true);

			bullets.Remove(bullet);
		}
		else
		{
			return null;
		}

		return bullet;
	}

	public void reloadBullet(GameObject bullet)
	{
		bullets.Add(bullet);
		bullet.SetActive(false);
	}
}
