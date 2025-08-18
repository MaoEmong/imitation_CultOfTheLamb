using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class Boss_Logic : MonoBehaviour
{
	// �÷��̾�
	Transform player;

	Vector3 dir;

	// �ִϸ��̼�
	SkeletonAnimation bossskel;
	// �ִϸ��̼�
	public GameObject tunnelskel;
	// �ִϸ��̼� ���� ��ũ��Ʈ
	Boss_AnimScript anim;
	// ���� HP
	public GameObject hp;

	// ���� Ÿ��
	float patternTime = 3;
	float curPattern;

	// ü��
	public float Hp;
	public float maxHp = 100;

	// ������ ����
	public bool bossBattle;

	// Ȱ��
	bool active;

	// ���� ����
	public bool Dead;

	// ���ݹ���
	public Collider[] smesh;
	public Collider[] attack;

	public GameObject enemyPaf;
	public List<GameObject> wormList = new List<GameObject>();
	GameObject[] wormLive = new GameObject[20];

	public GameObject hit;

	public enum BossPattern
	{
		IDLE = -1,
		JUMP,
		SUMMON,
		INOUT,
		SHOOT,
		BITE,
		Dive
	}

	// ���� ���� ����
	public BossPattern nowPattern;

	// ���� �̵� ����ĳ��Ʈ
	RaycastHit rayHit;
	Vector3 oriPos;

	private void Start()
	{
		player = GameObject.Find("Player").transform;

		// �ʿ� ������Ʈ ����
		bossskel = GetComponentInChildren<SkeletonAnimation>();
		anim = GetComponentInChildren<Boss_AnimScript>();

		bossBattle = false;

		// ü�� �ʱ�ȭ
		Hp = maxHp;

		for (int i = 0; i < 20; i++)
		{
			GameObject enemy = Instantiate(enemyPaf);

			wormList.Add(enemy);
			wormLive[i] = enemy;
			
			enemy.transform.parent = transform.parent.transform.GetChild(0).transform;
			enemy.transform.localScale = Vector3.one;

			enemy.SetActive(false);
		}
	}

	void Update()
    {
		if (Dead)
			return;

		if (bossBattle && !active)
		{
			if (anim.animState != Boss_AnimScript.AnimState.INTRO)
			{
				SoundManager.Sound.Play("Enemy/Boss/Master.assets [405]");
			}
			anim.animState = Boss_AnimScript.AnimState.INTRO;
		}
			

		// ��Ʈ�� ���� ����� Ȱ�� ����
		if (!active)
		{
			if (anim.currentAnimation == "intro" &&
				bossskel.state.GetCurrent(0).AnimationTime == bossskel.state.GetCurrent(0).AnimationEnd)
			{
				active = true;
				hp.SetActive(true);
			}
			return;
		}

		if (Hp < 0)
		{
			nowPattern = BossPattern.IDLE;
			anim.animState = Boss_AnimScript.AnimState.DIE;
		}

		// ���� ���°� ��� ������ ���� �ð� ����
		if (nowPattern == BossPattern.IDLE)
			// ���Ͻð� ����
			curPattern += Time.deltaTime;

		// �ẹ ���� �� �÷��̾������� �̵�
		if (nowPattern == BossPattern.INOUT && bossskel.state.GetCurrent(0).AnimationTime > 0.6f)
		{
			BossRay();
			transform.position += dir.normalized * 10 * Time.deltaTime;
		}

		if ((nowPattern == BossPattern.SUMMON || nowPattern == BossPattern.SHOOT) &&
			bossskel.state.GetCurrent(0).AnimationTime == bossskel.state.GetCurrent(0).AnimationEnd)
		{
			nowPattern = BossPattern.IDLE;
		}

		// ȭ������ ���� �� ���� �ݶ��̴� ����
		if (nowPattern == BossPattern.Dive)
		{
			attack[0].enabled = false;
			nowPattern = BossPattern.IDLE;
		}

		// ���� ���� ���Ͽ� ���� ���������� �ݶ��̴� ����
		if (nowPattern == BossPattern.BITE)
		{
			// ���� ����
			if (0.2f < bossskel.state.GetCurrent(0).AnimationTime && bossskel.state.GetCurrent(0).AnimationTime < 0.5f)
			{
				smesh[0].enabled = true;
			}
			// ������ ����
			if (0.9f < bossskel.state.GetCurrent(0).AnimationTime && bossskel.state.GetCurrent(0).AnimationTime < 1.2f)
			{
				smesh[1].enabled = true;
				smesh[0].enabled = false;
			}
			// �߾� ����
			if (1.8f < bossskel.state.GetCurrent(0).AnimationTime && bossskel.state.GetCurrent(0).AnimationTime < 2.2f)
			{
				smesh[2].enabled = true;
				smesh[1].enabled = false;
			}
			// ���� ��
			if (bossskel.state.GetCurrent(0).AnimationTime > 2.3f)
			{
				smesh[2].enabled = false;
				nowPattern = BossPattern.IDLE;
			}
				
		}

		// ���� �ð��� �����ϸ�
		if (curPattern > patternTime)
		{
			patternSet();
			curPattern = 0;
		}
	}

	void BossRay()
	{
		oriPos = new Vector3(transform.position.x, 1, transform.position.z);

		rayHit = new RaycastHit();

		Ray ray = new Ray(oriPos, dir.normalized);

		if (Physics.Raycast(ray, out rayHit, 2.5f, LayerMask.GetMask("Wall")))
		{
			if (rayHit.collider.gameObject.layer.Equals(15))
			{
				dir.x *= -1;
				dir.z *= -1;
			}
		}

		Debug.DrawRay(oriPos, dir.normalized * 2.5f, Color.green);
	}

	// ���� ����
	void patternSet()
	{
		// ü�¿� ���� ���� ��ȭ
		if (Hp > 70)
		{
			int p = Random.RandomRange(0, 2);
			switch(p)
			{
				case (int)BossPattern.JUMP:
					Jump();
					break;
				case (int)BossPattern.SUMMON:
					Summon();
					break;
			}
		}
		// ���� ü���� 35 ~ 70 ������ ��
		else if (Hp > 35)
		{
			int p = Random.RandomRange(0, 5);
			switch (p)
			{
				case (int)BossPattern.JUMP:
					Jump();
					break;
				case (int)BossPattern.SUMMON:
					Summon();
					break;
				case (int)BossPattern.INOUT:
					In();
					break;
				case (int)BossPattern.SHOOT:
					Shoot();
					break;
			}
		}
		// ���� ü���� 35 ���� �϶�
		else if (Hp < 35)
		{
			int p = Random.RandomRange(0, 6);
			switch (p)
			{
				case (int)BossPattern.JUMP:
					Jump();
					break;
				case (int)BossPattern.SUMMON:
					Summon();
					break;
				case (int)BossPattern.INOUT:
					In();
					break;
				case (int)BossPattern.SHOOT:
					Shoot();
					break;
				case (int)BossPattern.BITE:
					Bite();
					break;
			}
		}
	}

	// ���� ���̺� ����
	void Jump()
	{
		anim.animState = Boss_AnimScript.AnimState.JUMP;

		SoundManager.Sound.Play("Enemy/Normal/Attack/Master.assets [864]", SoundManager.SoundType.Enemy);

		Invoke("Dive", 2f);

		nowPattern = BossPattern.JUMP;
	}

	// ���̺�
	void Dive()
	{
		// �÷��̾� �������� �̵�
		transform.position = new Vector3(player.transform.position.x, 
										 player.transform.position.y, 
										 player.transform.position.z + 0.5f);

		anim.animState = Boss_AnimScript.AnimState.GROUND;

		Invoke("DiveHit", 0.3f);
	}

	// ���̺�
	void DiveHit()
	{
		// ������ ���� ���� ����
		attack[0].enabled = true;

		outSound();

		Invoke("DiveUp", 0.8f);
	}

	// ���̺�
	void DiveUp()
	{
		nowPattern = BossPattern.Dive;
	}

	// ��ȯ ����
	void Summon()
	{
		for (int i = 0; i < 20; i++)
		{
			if (!wormLive[i].activeSelf)
			{
				wormLive[i].GetComponent<Enemy>().state = Enemy.EnemyState.WAIT;

				Enemy_AnimScript wormAnim = wormLive[i].GetComponentInChildren<Enemy_AnimScript>();
				if (wormAnim.Dead)
				{
					wormList.Add(wormLive[i]);
					Enemy enemy = wormLive[i].GetComponent<Enemy>();
					enemy.setHP();
				}
			}
		}

		anim.animState = Boss_AnimScript.AnimState.SUMMON;

		nowPattern = BossPattern.SUMMON;

		if (wormList.Count <= 0)
		{
			Jump();
		}
		else if (wormList.Count > 0)
		{
			Invoke("summonBaby", 1.5f);
		}
	}

	void summonBaby()
	{
		SoundManager.Sound.Play("Enemy/Boss/Master.assets [413]");

		for (int i = 0; i < 5; i++)
		{

			GameObject enemy = wormList[0];
			enemy.SetActive(true);
			Enemy_AnimScript wormAnim = enemy.GetComponentInChildren<Enemy_AnimScript>();
			enemy.transform.position = new Vector3(Random.RandomRange(1330, 1370), 0, Random.RandomRange(230, 250));
			wormAnim.animState = Enemy_AnimScript.AnimState.Bone;
			enemy.GetComponent<Enemy>().state = Enemy.EnemyState.IDLE;

			wormList.Remove(enemy);
		}
	}

	// �ẹ ����
	void In()
	{
		anim.animState = Boss_AnimScript.AnimState.IN;

		Invoke("Out", 5f);

		Invoke("tunnelSound", 0.5f);

		// �̵����� ����
		dir = player.position - transform.position;

		// �ẹ �� ���� ���� Ȱ��ȭ
		attack[1].enabled = true;

		tunnelskel.SetActive(true);

		nowPattern = BossPattern.INOUT;
	}

	void tunnelSound()
	{
		SoundManager.Sound.Play("Enemy/Boss/Master.assets [699]");
	}

	// �ẹ ����
	void Out()
	{
		// �ẹ ���� ���ÿ� ���� ���� ��Ȱ��ȭ
		attack[1].enabled = false;

		anim.animState = Boss_AnimScript.AnimState.OUT;

		nowPattern = BossPattern.IDLE;

		outSound();
		Invoke("outSound", 0.8f);

		tunnelskel.GetComponent<Boss_Tunnel>().NowEnd();
	}

	void outSound()
	{
		SoundManager.Sound.Play("Enemy/Boss/Master.assets [671]");
	}

	// �� ����
	void Shoot()
	{
		if (ObjectPool_BossBullet.instance.getBullet() == null)
			In();
		else
		{
			Invoke("ShootSound", 0.7f);

			anim.animState = Boss_AnimScript.AnimState.SHOOT;

			nowPattern = BossPattern.SHOOT;
		}
	}

	void ShootSound()
	{
		SoundManager.Sound.Play("Enemy/Boss/Master.assets [491]");

		// �Ѿ˻����� ���� for��
		for (int i = 0; i < 16; i++)
		{
			// ���� ������Ʈ ����
			GameObject _bullet = ObjectPool_BossBullet.instance.getBullet();

			if (_bullet != null)
			{
				// �Ѿ��� ��ġ�� �ڽ��� ��ġ�� ����
				_bullet.transform.position = new Vector3(
					transform.position.x, 3f, transform.position.z);
			}

			Vector3 dir = new Vector3(
				player.transform.position.x + UnityEngine.Random.RandomRange(-5, 5),
				player.transform.position.y,
				player.transform.position.z + UnityEngine.Random.RandomRange(-5, 5));

			// �Ѿ��� ������ �÷��̾������� 
			_bullet.GetComponent<Boss_Bullet>().shootTarget(dir);
		}
	}

	// ���� ����
	void Bite()
	{
		SoundManager.Sound.Play("Enemy/Boss/Master.assets [709]");

		anim.animState = Boss_AnimScript.AnimState.BITE;

		nowPattern = BossPattern.BITE;
	}

	public float getHPpersent()
	{
		return Hp / maxHp;
	}

	void Hit(float damage)
	{
		Hp -= damage;

		anim.animState = Boss_AnimScript.AnimState.HIT;

		Enemy_HPbar hpbar = GetComponentInChildren<Enemy_HPbar>();
		hpbar.HPbar();

		if (Hp < 0)
		{
			Dead = true;
			anim.animState = Boss_AnimScript.AnimState.DIE;

			SoundManager.Sound.Play("Enemy/Boss/Master.assets [1278]");
		}

		hit.SetActive(true);
		hit.GetComponent<ksjHitImpact>().AwakeHitImpact();

		SoundManager.Sound.Play("Enemy/Normal/Hit/Master.assets [1031]", SoundManager.SoundType.Enemy);
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.layer.Equals(20) && nowPattern == BossPattern.IDLE && curPattern < 2.2f && !Dead)
		{
			Hit(ksjPlayerManager.Instance.CurPlayerWeaponDamage);
		}
	}
}
