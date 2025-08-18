using Spine.Unity;
using System.Collections;
using System;
using UnityEngine;
using UnityEngine.Networking.Types;

public class Enemy : MonoBehaviour
{
	// 적 상태 제어를 위한 enum
	#region 적 상태 제어 enum
	public enum EnemyState
	{
		WAIT,
		IDLE,   // 대기
		MOVE,   // 움직임 
		CHARGE, // 때릴준비
		IMPACT, // 공격
		SHOOT,  // (터렛)공격
		HIT,    // 맞음
		DIE     // 죽음
	}

	public enum EnemyType
	{
		Human,  // 인간형
		bat, // 곤충형
		Insect, // 곤충형
		Turret  // 터렛형   
	}

	public enum turretType
	{
		None,
		Arrow,  // 인간형 궁수 
		normal, // 기본(일직선)형
		round   // 강화(전방향)형
	}

	public enum HumanType
	{
		None,
		Large,
		Normal
	}

	// 적 상태 변수
	public EnemyState state;

	// 적 타입 설정
	public EnemyType Type;

	// 터렛 타입
	public turretType t_type;

	public HumanType H_Type;
	#endregion

	// 스파인 애니메이션    
	SkeletonAnimation skel;
	Enemy_AnimScript anim;
	Enemy_HumanAnimScript human;
	Enemy_TurretAnimScript turret;

	// 터렛 총알 오브젝트
	public GameObject bullet;

	// 타겟 오브젝트
	GameObject player;

	// 플레이어 스크립트를 불러온다.
	PlayerMove p_Move;

	// 이동 방향
	public Vector3 dir;

	#region 에너미제어
	// 적 현재 체력
	public float m_HP;

	//적 최대 체력
	public float m_MaxHP;

	// 적 공격력
	public int m_Power;

	// 적 속도
	public float m_Speed;

	// 적 감지 거리
	public float m_find;

	// 적 공격 거리
	public float m_AtkDis;

	// 누적 시간
	public float m_curTime = 0;

	// 적 공격 대기 시간
	public float m_AtkDeley = 3.0f;

	// 터렛의 총알 수
	public int bulletCount = 8;

	// 총알간 거리
	public float bulletSpread = 3.0f;

	// 적 Move 기준 위치
	Vector3 OriginPos;

	// Idle 상태일때 움직임 제어
	bool isMove;

	// 죽음 상태 제어
	bool nowDead;

	// Idle 상태일때 움직임 딜레이
	float deleyTime = 3;
	// Idle 상태일때 누적시간
	float Timer = 0;
	#endregion

	// 에너미 무브 레이 체크
	RaycastHit rayHit;
	Vector3 oriPos;

	// 벽 탈출 움직임
	bool escape;
	public Field field;

	// 몬스터 어택 판정
	public Collider atkRange;

	// 히트 이펙트
	public GameObject hit;
	// 데드 이펙트
	public GameObject dead;

	// Enemy가 Impect 상태일때
	bool Sound = false;

	// Hit 상태 일때
	bool H_Sound = false;

	// Die 상태 일때
	bool D_Sound = false;

	// Player가 Enemy의 범위 안에 접근했을 때
	bool Search = false;

	public void Init(Field _field)
	{
		field = _field;
	}

	private void Start()
	{
		// 최초 시작은 대기 상태로
		//state = EnemyState.WAIT;

		// 적 애니메이션을 받아온다.
		skel = GetComponentInChildren<SkeletonAnimation>();
		anim = GetComponentInChildren<Enemy_AnimScript>();
		human = GetComponentInChildren<Enemy_HumanAnimScript>();
		turret = GetComponentInChildren<Enemy_TurretAnimScript>();

		// 플레이어 오브젝트를 받아온다.
		player = GameObject.Find("Player");

		// 플레이어 스크립트를 받아온다.
		p_Move = GameObject.Find("Player").GetComponent<PlayerMove>();

		// 자신과 플레이어와의 거리
		dir = transform.position - player.transform.position;

		dir.Normalize();

		ksjPlayerManager pm = GameObject.Find("PlayerManager").GetComponent<ksjPlayerManager>();

		// 현재 체력을 최대체력으로
		m_MaxHP += (pm.CurStage * 6);

		m_HP = m_MaxHP;

		// Idle상태 기준점을 현재 자신의 자리로
		OriginPos = transform.position;
	}

	private void Update()
	{
		if (state == EnemyState.WAIT)
			return;

		if (gameObject.transform.parent.gameObject.name == "EnemyBox")
		{
			if (GameObject.Find("Boss").gameObject.GetComponent<Boss_Logic>().Dead)
			{
				state = EnemyState.DIE;
			}
		}

		// 적 상태 제어
		switch (state)
		{
			case EnemyState.IDLE:
				Idle();
				if (t_type == turretType.None)
					atkRange.enabled = false;
				break;

			case EnemyState.MOVE:
				Move();
				if (t_type == turretType.None)
					atkRange.enabled = false;
				break;

			case EnemyState.IMPACT:
				Impect();
				break;

			case EnemyState.CHARGE:
				Charge();
				break;

			case EnemyState.SHOOT:
				Shoot();
				if (t_type == turretType.None)
					atkRange.enabled = false;
				break;

			case EnemyState.HIT:
				Hit();
				if (t_type == turretType.None)
					atkRange.enabled = false;
				break;

			case EnemyState.DIE:
				Die();
				if (t_type == turretType.None)
					atkRange.enabled = false;
				break;
		}

		// 이미지 방향전환
		if (dir.x < 0)
			transform.rotation = Quaternion.Euler(0, 0, 0);
		else if (dir.x > 0)
			transform.rotation = Quaternion.Euler(0, 180, 0);
	}

	// 레이캐스트 움직임
	void rayMove()
	{
		oriPos = new Vector3(transform.position.x, 1, transform.position.z);

		rayHit = new RaycastHit();

		Ray ray = new Ray(oriPos, dir.normalized);

		if (Physics.Raycast(ray, out rayHit, 2f, LayerMask.GetMask("Wall")))
		{
			if (rayHit.collider.gameObject.layer.Equals(15))
			{
				escape = true;

				if (escape)
				{
					dir.x = UnityEngine.Random.RandomRange(-1, 2);
					dir.z = UnityEngine.Random.RandomRange(-1, 2);
				}
			}
		}

		Debug.DrawRay(oriPos, new Vector3(dir.x, 0, dir.z).normalized * 2f, Color.green);
	}

	// 레이캐스트 스탑
	void rayStop()
	{
		oriPos = new Vector3(transform.position.x, 1, transform.position.z);

		rayHit = new RaycastHit();

		Ray ray = new Ray(oriPos, dir.normalized);

		if (Physics.Raycast(ray, out rayHit, 2f, LayerMask.GetMask("Wall")))
		{
			if (rayHit.collider.gameObject.layer.Equals(15))
			{
				dir.x = 0;
				dir.z = 0;
			}
		}

		Debug.DrawRay(oriPos, new Vector3(dir.x, 0, dir.z).normalized * 2f, Color.green);
	}

	void Idle()
	{
		// 플레이어 발견시
		if (Vector3.Distance(transform.position, player.transform.position) < m_find)
		{
			Search = true;
			switch (Type)
			{
				case EnemyType.Human:
					{
						StartCoroutine(FindSound());
						// 애니메이션을 Ture로 변경
						human.Find = true;
						// 적 상태를 이동으로 변경
						state = EnemyState.MOVE;
						break;
					}
				case EnemyType.bat:
					{
						StartCoroutine(FindSound());
						// 애니메이션을 Ture로 변경
						anim.Find = true;
						// 적 상태를 이동으로 변경
						state = EnemyState.MOVE;
						Timer = 5;
						deleyTime = 1;
						break;
					}
				case EnemyType.Insect:
					{
						StartCoroutine(FindSound());
						// 애니메이션을 Ture로 변경
						anim.Find = true;
						// 적 상태를 이동으로 변경
						state = EnemyState.MOVE;
						Timer = 5;
						deleyTime = 1;
						break;
					}
				case EnemyType.Turret:
					{
						StartCoroutine(FindSound());
						// 상태를 SHOOT으로 변경
						state = EnemyState.SHOOT;
						break;
					}
			}
		}
		// 발견 못할시
		else
		{
			if (Type != EnemyType.Human)
			{
				// Idle 이동 누적 시간에 Time.deltaTime을 더해준다.
				Timer += Time.deltaTime;

				// 누적시간이 딜레이시간보다 커졌을때
				if (Timer > deleyTime)
				{
					// 움직임 제어 검사
					if (!isMove)
					{
						// 움직이는 방향을 기준 위치의 5만큼 랜덤으로 받아서 적용
						dir = new Vector3(UnityEngine.Random.Range(OriginPos.x - 3, OriginPos.x + 3), 0, UnityEngine.Random.Range(OriginPos.z - 3, OriginPos.z + 3));

						// 이동 거리 조정
						if (dir.x < 0)
							dir.x += -2;
						else
							dir.x += 2;

						if (dir.z < 0)
							dir.z += -2;
						else
							dir.z += 2;
					}

					idlemove();
				}
			}
		}

	}

	void idlemove()
	{
		switch (Type)
		{
			case EnemyType.bat:
				{
					// false일때
					if (!isMove)
						// true로 변경
						isMove = true;
					anim.Find = true;

					rayMove();

					// 적용된 위치와 현재위치를 구하고 정규화
					Vector3 _vec = (dir - transform.position).normalized;

					// 현재 위치에서 적용된 위치까지 이동
					transform.position += _vec * m_Speed * Time.deltaTime;

					// 적용된 위치까지 이동하면
					if ((int)transform.position.x == (int)dir.x && (int)transform.position.z == (int)dir.z)
					{
						// 누적 시간을 0으로 초기화
						Timer = 0;
						// 움직이는  딜레이 시간을 랜덤한 시간으로 변경
						deleyTime = UnityEngine.Random.Range(0.5f, 1.5f);
						// 움직이지 못하게 다시 false로 변경
						isMove = false;
						anim.Find = false;
					}
					break;
				}
			case EnemyType.Insect:
				{
					// false일때
					if (!isMove)
						// true로 변경
						isMove = true;
					anim.Find = true;

					rayMove();

					// 적용된 위치와 현재위치를 구하고 정규화
					Vector3 _vec = (dir - transform.position).normalized;

					// 현재 위치에서 적용된 위치까지 이동
					transform.position += _vec * m_Speed * Time.deltaTime;

					// 적용된 위치까지 이동하면
					if ((int)transform.position.x == (int)dir.x && (int)transform.position.z == (int)dir.z)
					{
						// 누적 시간을 0으로 초기화
						Timer = 0;
						// 움직이는  딜레이 시간을 랜덤한 시간으로 변경
						deleyTime = UnityEngine.Random.Range(0.5f, 1.5f);
						// 움직이지 못하게 다시 false로 변경
						isMove = false;
						anim.Find = false;
					}
					break;
				}
		}


	}

	void Move()
	{
		StopCoroutine(FindSound());
		// 플레이어와 적 사이가 공격거리보다 멀 때
		if (Vector3.Distance(transform.position, player.transform.position) > m_AtkDis)
		{

			if (Type != EnemyType.Human)
			{
				m_curTime += Time.deltaTime;

				// Idle 이동 누적 시간에 Time.deltaTime을 더해준다.
				Timer += Time.deltaTime;

				// 누적시간이 딜레이시간보다 커졌을때
				if (Timer > deleyTime)
				{
					dir = (player.transform.position - transform.position).normalized;

					Timer = 0;
				}

				rayMove();

				// 플레이어쪽으로 이동
				transform.position += dir * m_Speed * Time.deltaTime;
			}
			else
			{
				if (!escape)
					dir = (player.transform.position - transform.position).normalized;
				else
					m_curTime += Time.deltaTime;

				// 벽 탈출
				if (m_curTime > m_AtkDeley)
				{
					escape = false;
					m_curTime = 0;
				}

				rayMove();

				// 플레이어쪽으로 이동
				transform.position += dir * m_Speed * Time.deltaTime;
			}
		}
		// 플레이어와 적사이가 공격거리보다 가까울 때
		else if (Vector3.Distance(transform.position, player.transform.position) < m_AtkDis)
		{

			if (Type == EnemyType.Human)
			{
				// 애니메이션 상태를 CHARGE로 변경
				human.animState = Enemy_HumanAnimScript.AnimState.CHARGE;

				// 적 상태를 CHARGE로 변경
				state = EnemyState.CHARGE;
			}
			else
			{
				m_curTime += Time.deltaTime;

				if (m_curTime > m_AtkDeley)
				{
					// 애니메이션 상태를 CHARGE로 변경
					anim.animState = Enemy_AnimScript.AnimState.CHARGE;

					// 적 상태를 CHARGE로 변경
					state = EnemyState.CHARGE;

					m_curTime = 0;
				}
			}
		}
	}

	void Charge()
	{
		if (Type == EnemyType.Insect)
		{
			if (0.4f < anim.wormCharge && anim.wormCharge < 0.43f)
			{
				// 플레이어와 적 캐릭터 간의 방향 벡터 계산
				dir = (player.transform.position - transform.position).normalized;
			}
			if (anim.wormCharge > 0.5f)
			{
				Sound = true;
				// 적 상태를 IMPACT로 변경
				state = EnemyState.IMPACT;

				if (t_type == turretType.None)
					atkRange.enabled = true;
			}
		}
		// 애니메이션이 끝났을때 
		else
		{
			if (0.4f < skel.state.GetCurrent(0).AnimationTime &&
				skel.state.GetCurrent(0).AnimationTime < 0.42f)
			{
				// 플레이어와 적 캐릭터 간의 방향 벡터 계산
				dir = (player.transform.position - transform.position).normalized;
			}
			if (skel.state.GetCurrent(0).AnimationTime ==
				skel.state.GetCurrent(0).AnimationEnd)
			{
				Sound = true;
				// 적 상태를 IMPACT로 변경
				state = EnemyState.IMPACT;
				if (t_type == turretType.None)
					atkRange.enabled = true;
			}
		}
	}

	void Impect()
	{
		switch (Type)
		{
			case EnemyType.Human:
				{
					if (t_type == turretType.Arrow)
					{
						if (human.currentAnimation.Contains("charge") &&
							skel.state.GetCurrent(0).AnimationTime ==
							skel.state.GetCurrent(0).AnimationEnd)
						{
							SoundManager.Sound.Play("Enemy/Normal/Hit/Master.assets [313]", SoundManager.SoundType.Effect);
							// 누적시간이 딜레이시간보다 클때
							{
								// 누적시간을 0으로 초기화
								m_curTime = 0;
							}
						}

					}
					else
					{
						// 현재 애니메이션 이름이 charge가 아닐때, 임펙트와 동시에
						if (!human.currentAnimation.Contains("charge") &&
							skel.state.GetCurrent(0).AnimationTime < 0.25f)
						{
							StartCoroutine(AttackSound());

							rayStop();

							// 적 캐릭터를 플레이어 방향으로 이동
							transform.position += dir.normalized * m_Speed * 3 * Time.deltaTime;

							StopCoroutine(AttackSound());
						}
					}

					// 현재 애니메이션 이름이 charge가 아닐때, 현재 애니메이션이 끝나면
					if (!human.currentAnimation.Contains("charge") &&
						skel.state.GetCurrent(0).AnimationTime >
						0.25f)
					{
						if (t_type == turretType.None)
							atkRange.enabled = false;
					}

					// 현재 애니메이션 이름이 charge가 아닐때, 현재 애니메이션이 끝나면
					if (!human.currentAnimation.Contains("charge") &&
					skel.state.GetCurrent(0).AnimationTime ==
					skel.state.GetCurrent(0).AnimationEnd)
					{
						// 다시 플레이어와 적의 위치를 검사하고 공격거리보다 가까울때
						if (Vector3.Distance(transform.position, player.transform.position) <= m_AtkDis)
						{
							// 애니메이션 상태를 CHARGE로 변경
							human.animState = Enemy_HumanAnimScript.AnimState.CHARGE;
						}
						// 공격거리보다 멀면
						else
						{
							// 애니메이션 상태를 CHARGE로 변경
							human.animState = Enemy_HumanAnimScript.AnimState.MOVE;
						}

						// 상태를 이동으로 변경
						state = EnemyState.MOVE;
					}
					break;
				};

			case EnemyType.Insect:

				StartCoroutine(AttackSound());

				rayStop();

				// 적 캐릭터를 플레이어 방향으로 이동
				transform.position += dir.normalized * m_Speed * 1.3f * Time.deltaTime;

				// 현재 애니메이션 이름이 charge가 아닐때, 현재 애니메이션이 끝나면
				if (!anim.currentAnimation.Contains("charge") &&
					skel.state.GetCurrent(0).AnimationTime >
					0.3f)
				{
					if (t_type == turretType.None)
						atkRange.enabled = false;
				}

				// 현재 애니메이션 이름이 charge가 아닐때, 현재 애니메이션이 끝나면
				if (!anim.currentAnimation.Contains("charge") &&
				   skel.state.GetCurrent(0).AnimationTime ==
				   skel.state.GetCurrent(0).AnimationEnd)
				{
					if (t_type == turretType.None)
						atkRange.enabled = false;

					StopCoroutine(AttackSound());

					// 다시 플레이어와 적의 위치를 검사하고 공격거리보다 가까울때
					if (Vector3.Distance(transform.position, player.transform.position) <= m_AtkDis)
					{
						// 애니메이션 상태를 CHARGE로 변경
						anim.animState = Enemy_AnimScript.AnimState.CHARGE;
					}
					// 공격거리보다 멀면
					else
					{
						// 애니메이션 상태를 CHARGE로 변경
						anim.animState = Enemy_AnimScript.AnimState.MOVE;
					}
					// 상태를 이동으로 변경
					state = EnemyState.MOVE;
				}
				break;
			case EnemyType.bat:

				StartCoroutine(AttackSound());

				rayStop();

				// 현재 애니메이션 이름이 charge가 아닐때, 임펙트와 동시에
				if (!anim.currentAnimation.Contains("charge") &&
					skel.state.GetCurrent(0).AnimationTime < 0.25f)
				{
					// 적 캐릭터를 플레이어 방향으로 이동
					transform.position += dir.normalized * m_Speed * 3 * Time.deltaTime;
				}

				// 현재 애니메이션 이름이 charge가 아닐때, 현재 애니메이션이 끝나면
				if (!anim.currentAnimation.Contains("charge") &&
					skel.state.GetCurrent(0).AnimationTime >
					0.25f)
				{
					if (t_type == turretType.None)
						atkRange.enabled = false;
				}

				// 현재 애니메이션 이름이 charge가 아닐때, 현재 애니메이션이 끝나면
				if (!anim.currentAnimation.Contains("charge") && skel.state.GetCurrent(0).AnimationTime ==
				   skel.state.GetCurrent(0).AnimationEnd)
				{
					if (t_type == turretType.None)
						atkRange.enabled = false;

					StopCoroutine(AttackSound());

					// 다시 플레이어와 적의 위치를 검사하고 공격거리보다 가까울때
					if (Vector3.Distance(transform.position, player.transform.position) <= m_AtkDis)
					{
						// 애니메이션 상태를 CHARGE로 변경
						anim.animState = Enemy_AnimScript.AnimState.CHARGE;
					}
					// 공격거리보다 멀면
					else
					{
						// 애니메이션 상태를 CHARGE로 변경
						anim.animState = Enemy_AnimScript.AnimState.MOVE;
					}
					// 상태를 이동으로 변경
					state = EnemyState.MOVE;
				}
				break;
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.layer.Equals(20))
		{
			if (state != EnemyState.IMPACT && state != EnemyState.HIT)
				hitEnemy(ksjPlayerManager.Instance.CurPlayerWeaponDamage);
		}
	}

	void Shoot()
	{
		turret.find = true;

		// 애니메이션 진행시간 == 애니메이션 종료시간 <= 애니메이션 상태 검사 후 종료 시 특정 상태 끝
		if (!turret.currentAnimation.Contains("shoot"))
		{
			if (!Sound)
			{
				Sound = true;
			}
			// 적 누적시간에 Time.deltaTime을 더해준다.
			m_curTime += Time.deltaTime;
		}

		else
		{
			// 터렛타입을 검사
			switch (t_type)
			{
				// 터렛 타입이 normal일때
				case turretType.normal:
					if (m_curTime > 2f)
					{
						// 게임 오브젝트 생성
						GameObject _bullet1 = ObjectPool_Bullet.instance.getBullet();

						StartCoroutine(AttackSound());

						if (_bullet1 != null)
						{
							// 총알의 위치를 자신의 위치로 변경
							_bullet1.transform.position = new Vector3(transform.position.x, 1.4f, transform.position.z);
							// 총알의 방향을 플레이어쪽으로 
							_bullet1.GetComponent<Enemy_Bullet>().shootTarget(player.transform.position);
						}
						StopCoroutine(AttackSound());
						// 누적시간을 0으로 초기화
						m_curTime = 0;
					}
					break;
				// 터렛타입이 round일때
				case turretType.round:
					// 누적시간이 딜레이시간보다 클때
					if (m_curTime > 2)
					{
						StartCoroutine(AttackSound());
						// 총알생성을 위한 for문
						for (int i = 0; i < bulletCount; i++)
						{
							// 게임 오브젝트 생성
							GameObject _bullet2 = ObjectPool_Bullet.instance.getBullet();

							if (_bullet2 != null)
							{
								// 총알의 위치를 자신의 위치로 변경
								_bullet2.transform.position = new Vector3(
									transform.position.x, 1.4f, transform.position.z);
							}

							Vector3 dir = new Vector3(
								player.transform.position.x + UnityEngine.Random.RandomRange(-2, 2),
								player.transform.position.y,
								player.transform.position.z + UnityEngine.Random.RandomRange(-2, 2));

							// 총알의 방향을 플레이어쪽으로 
							_bullet2.GetComponent<Enemy_Bullet>().shootTarget(dir);

							StopCoroutine(AttackSound());

							// 누적시간을 0으로 초기화
							m_curTime = 0;
						}
					}
					break;
			}
		}
	}

	public void hitEnemy(float atkpow)
	{
		if (t_type == turretType.None)
			atkRange.enabled = false;

		switch (Type)
		{
			case EnemyType.Human:
				{
					// 현재 체력에 맞은데미지 빼기
					m_HP -= atkpow;

					// 적 캐릭터의 HP가 0보다 크면 피격 상태 전환
					if (m_HP > 0)
					{
						H_Sound = true;
						human.animState = Enemy_HumanAnimScript.AnimState.HIT;
						state = EnemyState.HIT;
						StartCoroutine(HitSound());
					}
					// 그렇지 않다면 사망 상태로 전환
					else
					{
						D_Sound = true;
						human.animState = Enemy_HumanAnimScript.AnimState.DIE;
						state = EnemyState.DIE;
						StartCoroutine(DieSound());
					}
					break;
				}
			case EnemyType.Insect:
				{
					// 현재 체력에 맞은데미지 빼기
					m_HP -= atkpow;

					// 적 캐릭터의 HP가 0보다 크면 피격 상태 전환
					if (m_HP > 0)
					{
						H_Sound = true;
						anim.animState = Enemy_AnimScript.AnimState.HIT;
						state = EnemyState.HIT;
						StartCoroutine(HitSound());
					}
					// 그렇지 않다면 사망 상태로 전환
					else
					{
						D_Sound = true;
						anim.animState = Enemy_AnimScript.AnimState.DIE;
						state = EnemyState.DIE;
						StartCoroutine(DieSound());
					}
					break;
				}
			case EnemyType.bat:
				{
					// 현재 체력에 맞은데미지 빼기
					m_HP -= atkpow;

					// 적 캐릭터의 HP가 0보다 크면 피격 상태 전환
					if (m_HP > 0)
					{
						H_Sound = true;
						anim.animState = Enemy_AnimScript.AnimState.HIT;
						state = EnemyState.HIT;
						StartCoroutine(HitSound());
					}
					// 그렇지 않다면 사망 상태로 전환
					else
					{
						D_Sound = true;
						anim.animState = Enemy_AnimScript.AnimState.DIE;
						state = EnemyState.DIE;
						StartCoroutine(DieSound());
					}
					break;
				}
			case EnemyType.Turret:
				// 애니메이션 진행시간 == 애니메이션 종료시간 <= 애니메이션 상태 검사 후 종료 시 특정 상태 끝
				if (!turret.currentAnimation.Contains("shoot"))
				{
					// 적 누적시간에 Time.deltaTime을 더해준다.
					m_curTime += Time.deltaTime;
				}

				// 현재 체력에 맞은데미지 빼기
				m_HP -= atkpow;

				H_Sound = true;

				StartCoroutine(HitSound());

				// 적 캐릭터의 HP가 0보다 크면 피격 상태 전환
				if (m_HP < 0)
				{
					D_Sound = true;
					turret.animState = Enemy_TurretAnimScript.AnimState.DIE;
					state = EnemyState.DIE;
					StartCoroutine(DieSound());
				}
				break;
		}

		Enemy_HPbar hpbar = GetComponentInChildren<Enemy_HPbar>();
		hpbar.HPbar();

		hit.SetActive(true);
		hit.GetComponent<ksjHitImpact>().AwakeHitImpact();
	}

	public float getHPpersent()
	{
		return m_HP / m_MaxHP;
	}

	public void setHP()
	{
		m_HP = m_MaxHP;
	}

	void Hit()
	{
		switch (Type)
		{
			case EnemyType.Human:
				{
					// 애니메이션이 끝날때
					if (skel.state.GetCurrent(0).AnimationTime ==
						skel.state.GetCurrent(0).AnimationEnd)
					{
						// 상태를 다시 Move로 변경
						human.animState = Enemy_HumanAnimScript.AnimState.MOVE;
						state = EnemyState.MOVE;
						StopCoroutine(HitSound());
					}
					break;
				}

			case EnemyType.Insect:
				{
					// 애니메이션이 끝날때
					if (skel.state.GetCurrent(0).AnimationTime ==
						skel.state.GetCurrent(0).AnimationEnd)
					{
						// 상태를 다시 Move로 변경
						anim.animState = Enemy_AnimScript.AnimState.MOVE;
						state = EnemyState.MOVE;
						StopCoroutine(HitSound());
					}
					break;
				}
			case EnemyType.bat:
				{
					// 애니메이션이 끝날때
					if (skel.state.GetCurrent(0).AnimationTime ==
						skel.state.GetCurrent(0).AnimationEnd)
					{
						// 상태를 다시 Move로 변경
						anim.animState = Enemy_AnimScript.AnimState.MOVE;
						state = EnemyState.MOVE;
						StopCoroutine(HitSound());
					}
					break;
				}
		}
	}

	void Die()
	{
		if (nowDead)
			return;

		gameObject.GetComponent<Collider>().enabled = false;

		switch (Type)
		{
			case EnemyType.Human:
				{
					// 사망 애니메이션을 위해 True 로 변경
					human.Dead = true;

					// 적 상태도 Die로 변경
					human.animState = Enemy_HumanAnimScript.AnimState.DIE;
					state = EnemyState.DIE;

					StopCoroutine(HitSound());

					// 사망 이펙트
					dead.SetActive(true);
					StartCoroutine(dead.GetComponent<Dead_AnimScript>().startEff());

					// 스킨을 숨김
					gameObject.transform.GetChild(1).gameObject.SetActive(false);
					gameObject.transform.GetChild(2).gameObject.SetActive(false);

					field.MinusCurEnemyCount();

					nowDead = true;
					break;
				}

			case EnemyType.Insect:
				{
					// 사망 애니메이션을 위해 True 로 변경
					anim.Dead = true;

					// 적 상태도 Die로 변경
					anim.animState = Enemy_AnimScript.AnimState.DIE;
					state = EnemyState.DIE;

					StopCoroutine(HitSound());

					// 사망 이펙트
					dead.SetActive(true);
					StartCoroutine(dead.GetComponent<Dead_AnimScript>().startEff());

					// 스킨을 숨김
					gameObject.transform.GetChild(1).gameObject.SetActive(false);
					gameObject.transform.GetChild(2).gameObject.SetActive(false);

					if (gameObject.transform.parent.name != "EnemyBox")
						field.MinusCurEnemyCount();
					
					nowDead = true;
					break;
				}
			case EnemyType.bat:
				{
					// 사망 애니메이션을 위해 True 로 변경
					anim.Dead = true;

					// 적 상태도 Die로 변경
					anim.animState = Enemy_AnimScript.AnimState.DIE;
					state = EnemyState.DIE;

					StopCoroutine(HitSound());

					// 사망 이펙트
					dead.SetActive(true);
					StartCoroutine(dead.GetComponent<Dead_AnimScript>().startEff());

					// 스킨을 숨김
					gameObject.transform.GetChild(1).gameObject.SetActive(false);
					gameObject.transform.GetChild(2).gameObject.SetActive(false);

					field.MinusCurEnemyCount();
					nowDead = true;
					break;
				}
			case EnemyType.Turret:
				{
					// 적 상태도 Die로 변경
					turret.animState = Enemy_TurretAnimScript.AnimState.DIE;
					state = EnemyState.DIE;

					StopCoroutine(HitSound());

					// 사망 이펙트
					dead.SetActive(true);
					StartCoroutine(dead.GetComponent<Dead_AnimScript>().startEff());

					// 스킨을 숨김
					gameObject.transform.GetChild(0).gameObject.SetActive(false);
					gameObject.transform.GetChild(1).gameObject.SetActive(false);

					field.MinusCurEnemyCount();
					nowDead = true;
					break;
				}
		}
	}

	// 적이 플레이어를 발견했을 때 
	IEnumerator FindSound()
    {		
		// 적 타입이 Human이라면
		if (Type == EnemyType.Human)
		{
			// Human중 Human타입이 Large이고
			if (H_Type == HumanType.Large)
			{
				// 플레이어가 범위 내 라면
				if (Search == true)
				{					
					// SoundManager 에서 
					SoundManager.Sound.Play("Enemy/Normal/Warning/Master.assets [746]", SoundManager.SoundType.Enemy);
					Search = false;
				}
			}
			// Human중 Human타입이 Normal이라면
			else if (H_Type == HumanType.Normal)
			{
				if (Search == true)
				{					
					SoundManager.Sound.Play("Enemy/Normal/Warning/Master.assets [421]", SoundManager.SoundType.Enemy);
					Search = false;
				}
			}
		}
		// Human타입이 아니라면
		else
		{
			if (Search == true)
			{
				SoundManager.Sound.Play("Enemy/Normal/Warning/Master.assets [916]", SoundManager.SoundType.Enemy);
				Search = false;
			}
		}

		yield return new WaitForSeconds(0.2f);
	}

	// 적이 플레이어를 공격할 때
	IEnumerator AttackSound()
    {
		if(Type == EnemyType.Human)
        {
			if(H_Type == HumanType.Large)
            {
				if (Sound == true)
				{
					SoundManager.Sound.Play("Enemy/Normal/Attack/Master.assets [864]", SoundManager.SoundType.Enemy);
					Sound = false;
				}
			}
			else if(H_Type == HumanType.Normal)
            {
				if (Sound == true)
				{
					SoundManager.Sound.Play("Enemy/Normal/Attack/Master.assets [278]", SoundManager.SoundType.Enemy);
					Sound = false;
				}
			}
        }

		if(Type == EnemyType.Insect)
        {			
			if(Sound == true)
            {
				SoundManager.Sound.Play("Enemy/Normal/Attack/Master.assets [762]", SoundManager.SoundType.Enemy);
				Sound = false;
            }
        }

		else if(Type==EnemyType.bat)
        {
			if (Sound == true)
			{
				SoundManager.Sound.Play("Enemy/Normal/Attack/Master.assets [775]", SoundManager.SoundType.Enemy);
				Sound = false;
			}
		}

		else if (Type == EnemyType.Turret)
        {
			if (Sound == true)
			{
				SoundManager.Sound.Play("Enemy/Normal/Attack/Master.assets [927]", SoundManager.SoundType.Enemy);
				Sound = false;
			}
		}
		
		yield return new WaitForSeconds(0.2f);
    }

	// 플레이어에게 공격 당했을 때
	IEnumerator HitSound()
    {
		if(Type == EnemyType.Human)
        {
            if(H_Type == HumanType.Large)
            {
				if(H_Sound == true)
                {
					SoundManager.Sound.Play("Enemy/Normal/Hit/Master.assets [373]", SoundManager.SoundType.Enemy);
					H_Sound = false;
				}
			}
			else if (H_Type == HumanType.Normal)
            {
				if (H_Sound == true)
				{
					SoundManager.Sound.Play("Enemy/Normal/Hit/Master.assets [957]", SoundManager.SoundType.Enemy);
					H_Sound = false;
				}
			}			
        }
		if(Type == EnemyType.Insect)
        {
			if (H_Sound == true)
			{
				SoundManager.Sound.Play("Enemy/Normal/Hit/Master.assets [1031]", SoundManager.SoundType.Enemy);
				H_Sound = false;
			}
		}

		if(Type == EnemyType.bat)
        {
			if (H_Sound == true)
			{
				SoundManager.Sound.Play("Enemy/Normal/Hit/Master.assets [724]", SoundManager.SoundType.Enemy);
				H_Sound = false;
			}
		}

		if(Type == EnemyType.Turret)
        {
			if (H_Sound == true)
			{
				SoundManager.Sound.Play("Enemy/Normal/Hit/Master.assets [830]", SoundManager.SoundType.Enemy);
				H_Sound = false;
			}
		}
		
		yield return new WaitForSeconds(0.2f);
    }

	// 적이 죽을때
	IEnumerator DieSound()
    {
		Debug.Log("in");
		if (Type == EnemyType.Human)
		{
			if (H_Type == HumanType.Large)
			{
				if (D_Sound == true)
				{
					Debug.Log("Large");
					SoundManager.Sound.Play("Enemy/Normal/Hit/death/Master.assets [919]", SoundManager.SoundType.Enemy);
					D_Sound = false;
				}
			}
			else if (H_Type == HumanType.Normal)
			{
				if (D_Sound == true)
				{
					Debug.Log("Normal");
					SoundManager.Sound.Play("Enemy/Normal/Hit/death/Master.assets [218]", SoundManager.SoundType.Enemy);
					D_Sound = false;
				}
			}
		}

        else
        {
			if (D_Sound == true)
			{
				SoundManager.Sound.Play("Enemy/Normal/Hit/death/Master.assets [223]", SoundManager.SoundType.Enemy);
				D_Sound = false;
			}
		}
		yield return new WaitForSeconds(0.2f);
    }
}


