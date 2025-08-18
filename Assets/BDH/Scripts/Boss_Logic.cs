using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class Boss_Logic : MonoBehaviour
{
	// 플레이어
	Transform player;

	Vector3 dir;

	// 애니메이션
	SkeletonAnimation bossskel;
	// 애니메이션
	public GameObject tunnelskel;
	// 애니메이션 제어 스크립트
	Boss_AnimScript anim;
	// 보스 HP
	public GameObject hp;

	// 패턴 타임
	float patternTime = 3;
	float curPattern;

	// 체력
	public float Hp;
	public float maxHp = 100;

	// 보스전 시작
	public bool bossBattle;

	// 활동
	bool active;

	// 죽은 상태
	public bool Dead;

	// 공격범위
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

	// 현재 보스 패턴
	public BossPattern nowPattern;

	// 보스 이동 레이캐스트
	RaycastHit rayHit;
	Vector3 oriPos;

	private void Start()
	{
		player = GameObject.Find("Player").transform;

		// 필요 컴포넌트 습득
		bossskel = GetComponentInChildren<SkeletonAnimation>();
		anim = GetComponentInChildren<Boss_AnimScript>();

		bossBattle = false;

		// 체력 초기화
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
			

		// 인트로 진행 종료시 활동 시작
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

		// 현재 상태가 대기 상태일 때만 시간 진행
		if (nowPattern == BossPattern.IDLE)
			// 패턴시간 진행
			curPattern += Time.deltaTime;

		// 잠복 공격 중 플레이어쪽으로 이동
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

		// 화면으로 나올 때 공격 콜라이더 해제
		if (nowPattern == BossPattern.Dive)
		{
			attack[0].enabled = false;
			nowPattern = BossPattern.IDLE;
		}

		// 물기 공격 패턴에 따라 순차적으로 콜라이더 조정
		if (nowPattern == BossPattern.BITE)
		{
			// 왼쪽 물기
			if (0.2f < bossskel.state.GetCurrent(0).AnimationTime && bossskel.state.GetCurrent(0).AnimationTime < 0.5f)
			{
				smesh[0].enabled = true;
			}
			// 오른쪽 물기
			if (0.9f < bossskel.state.GetCurrent(0).AnimationTime && bossskel.state.GetCurrent(0).AnimationTime < 1.2f)
			{
				smesh[1].enabled = true;
				smesh[0].enabled = false;
			}
			// 중앙 물기
			if (1.8f < bossskel.state.GetCurrent(0).AnimationTime && bossskel.state.GetCurrent(0).AnimationTime < 2.2f)
			{
				smesh[2].enabled = true;
				smesh[1].enabled = false;
			}
			// 공격 끝
			if (bossskel.state.GetCurrent(0).AnimationTime > 2.3f)
			{
				smesh[2].enabled = false;
				nowPattern = BossPattern.IDLE;
			}
				
		}

		// 패턴 시간에 도달하면
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

	// 패턴 진행
	void patternSet()
	{
		// 체력에 따라 패턴 변화
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
		// 현재 체력이 35 ~ 70 사이일 때
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
		// 현재 체력이 35 이하 일때
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

	// 점프 다이브 패턴
	void Jump()
	{
		anim.animState = Boss_AnimScript.AnimState.JUMP;

		SoundManager.Sound.Play("Enemy/Normal/Attack/Master.assets [864]", SoundManager.SoundType.Enemy);

		Invoke("Dive", 2f);

		nowPattern = BossPattern.JUMP;
	}

	// 다이브
	void Dive()
	{
		// 플레이어 방향으로 이동
		transform.position = new Vector3(player.transform.position.x, 
										 player.transform.position.y, 
										 player.transform.position.z + 0.5f);

		anim.animState = Boss_AnimScript.AnimState.GROUND;

		Invoke("DiveHit", 0.3f);
	}

	// 다이브
	void DiveHit()
	{
		// 착지의 순간 어택 판정
		attack[0].enabled = true;

		outSound();

		Invoke("DiveUp", 0.8f);
	}

	// 다이브
	void DiveUp()
	{
		nowPattern = BossPattern.Dive;
	}

	// 소환 패턴
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

	// 잠복 패턴
	void In()
	{
		anim.animState = Boss_AnimScript.AnimState.IN;

		Invoke("Out", 5f);

		Invoke("tunnelSound", 0.5f);

		// 이동방향 설정
		dir = player.position - transform.position;

		// 잠복 중 어택 판정 활성화
		attack[1].enabled = true;

		tunnelskel.SetActive(true);

		nowPattern = BossPattern.INOUT;
	}

	void tunnelSound()
	{
		SoundManager.Sound.Play("Enemy/Boss/Master.assets [699]");
	}

	// 잠복 패턴
	void Out()
	{
		// 잠복 끝과 동시에 어택 판정 비활성화
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

	// 슛 패턴
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

		// 총알생성을 위한 for문
		for (int i = 0; i < 16; i++)
		{
			// 게임 오브젝트 생성
			GameObject _bullet = ObjectPool_BossBullet.instance.getBullet();

			if (_bullet != null)
			{
				// 총알의 위치를 자신의 위치로 변경
				_bullet.transform.position = new Vector3(
					transform.position.x, 3f, transform.position.z);
			}

			Vector3 dir = new Vector3(
				player.transform.position.x + UnityEngine.Random.RandomRange(-5, 5),
				player.transform.position.y,
				player.transform.position.z + UnityEngine.Random.RandomRange(-5, 5));

			// 총알의 방향을 플레이어쪽으로 
			_bullet.GetComponent<Boss_Bullet>().shootTarget(dir);
		}
	}

	// 물기 패턴
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
