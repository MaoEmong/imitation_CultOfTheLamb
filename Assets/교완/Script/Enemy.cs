using Spine.Unity;
using System.Collections;
using System;
using UnityEngine;
using UnityEngine.Networking.Types;

public class Enemy : MonoBehaviour
{
	// �� ���� ��� ���� enum
	#region �� ���� ���� enum
	public enum EnemyState
	{
		WAIT,
		IDLE,   // ���
		MOVE,   // ������ 
		CHARGE, // �����غ�
		IMPACT, // ����
		SHOOT,  // (�ͷ�)����
		HIT,    // ����
		DIE     // ����
	}

	public enum EnemyType
	{
		Human,  // �ΰ���
		bat, // ������
		Insect, // ������
		Turret  // �ͷ���   
	}

	public enum turretType
	{
		None,
		Arrow,  // �ΰ��� �ü� 
		normal, // �⺻(������)��
		round   // ��ȭ(������)��
	}

	public enum HumanType
	{
		None,
		Large,
		Normal
	}

	// �� ���� ����
	public EnemyState state;

	// �� Ÿ�� ����
	public EnemyType Type;

	// �ͷ� Ÿ��
	public turretType t_type;

	public HumanType H_Type;
	#endregion

	// ������ �ִϸ��̼�    
	SkeletonAnimation skel;
	Enemy_AnimScript anim;
	Enemy_HumanAnimScript human;
	Enemy_TurretAnimScript turret;

	// �ͷ� �Ѿ� ������Ʈ
	public GameObject bullet;

	// Ÿ�� ������Ʈ
	GameObject player;

	// �÷��̾� ��ũ��Ʈ�� �ҷ��´�.
	PlayerMove p_Move;

	// �̵� ����
	public Vector3 dir;

	#region ���ʹ�����
	// �� ���� ü��
	public float m_HP;

	//�� �ִ� ü��
	public float m_MaxHP;

	// �� ���ݷ�
	public int m_Power;

	// �� �ӵ�
	public float m_Speed;

	// �� ���� �Ÿ�
	public float m_find;

	// �� ���� �Ÿ�
	public float m_AtkDis;

	// ���� �ð�
	public float m_curTime = 0;

	// �� ���� ��� �ð�
	public float m_AtkDeley = 3.0f;

	// �ͷ��� �Ѿ� ��
	public int bulletCount = 8;

	// �Ѿ˰� �Ÿ�
	public float bulletSpread = 3.0f;

	// �� Move ���� ��ġ
	Vector3 OriginPos;

	// Idle �����϶� ������ ����
	bool isMove;

	// ���� ���� ����
	bool nowDead;

	// Idle �����϶� ������ ������
	float deleyTime = 3;
	// Idle �����϶� �����ð�
	float Timer = 0;
	#endregion

	// ���ʹ� ���� ���� üũ
	RaycastHit rayHit;
	Vector3 oriPos;

	// �� Ż�� ������
	bool escape;
	public Field field;

	// ���� ���� ����
	public Collider atkRange;

	// ��Ʈ ����Ʈ
	public GameObject hit;
	// ���� ����Ʈ
	public GameObject dead;

	// Enemy�� Impect �����϶�
	bool Sound = false;

	// Hit ���� �϶�
	bool H_Sound = false;

	// Die ���� �϶�
	bool D_Sound = false;

	// Player�� Enemy�� ���� �ȿ� �������� ��
	bool Search = false;

	public void Init(Field _field)
	{
		field = _field;
	}

	private void Start()
	{
		// ���� ������ ��� ���·�
		//state = EnemyState.WAIT;

		// �� �ִϸ��̼��� �޾ƿ´�.
		skel = GetComponentInChildren<SkeletonAnimation>();
		anim = GetComponentInChildren<Enemy_AnimScript>();
		human = GetComponentInChildren<Enemy_HumanAnimScript>();
		turret = GetComponentInChildren<Enemy_TurretAnimScript>();

		// �÷��̾� ������Ʈ�� �޾ƿ´�.
		player = GameObject.Find("Player");

		// �÷��̾� ��ũ��Ʈ�� �޾ƿ´�.
		p_Move = GameObject.Find("Player").GetComponent<PlayerMove>();

		// �ڽŰ� �÷��̾���� �Ÿ�
		dir = transform.position - player.transform.position;

		dir.Normalize();

		ksjPlayerManager pm = GameObject.Find("PlayerManager").GetComponent<ksjPlayerManager>();

		// ���� ü���� �ִ�ü������
		m_MaxHP += (pm.CurStage * 6);

		m_HP = m_MaxHP;

		// Idle���� �������� ���� �ڽ��� �ڸ���
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

		// �� ���� ����
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

		// �̹��� ������ȯ
		if (dir.x < 0)
			transform.rotation = Quaternion.Euler(0, 0, 0);
		else if (dir.x > 0)
			transform.rotation = Quaternion.Euler(0, 180, 0);
	}

	// ����ĳ��Ʈ ������
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

	// ����ĳ��Ʈ ��ž
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
		// �÷��̾� �߽߰�
		if (Vector3.Distance(transform.position, player.transform.position) < m_find)
		{
			Search = true;
			switch (Type)
			{
				case EnemyType.Human:
					{
						StartCoroutine(FindSound());
						// �ִϸ��̼��� Ture�� ����
						human.Find = true;
						// �� ���¸� �̵����� ����
						state = EnemyState.MOVE;
						break;
					}
				case EnemyType.bat:
					{
						StartCoroutine(FindSound());
						// �ִϸ��̼��� Ture�� ����
						anim.Find = true;
						// �� ���¸� �̵����� ����
						state = EnemyState.MOVE;
						Timer = 5;
						deleyTime = 1;
						break;
					}
				case EnemyType.Insect:
					{
						StartCoroutine(FindSound());
						// �ִϸ��̼��� Ture�� ����
						anim.Find = true;
						// �� ���¸� �̵����� ����
						state = EnemyState.MOVE;
						Timer = 5;
						deleyTime = 1;
						break;
					}
				case EnemyType.Turret:
					{
						StartCoroutine(FindSound());
						// ���¸� SHOOT���� ����
						state = EnemyState.SHOOT;
						break;
					}
			}
		}
		// �߰� ���ҽ�
		else
		{
			if (Type != EnemyType.Human)
			{
				// Idle �̵� ���� �ð��� Time.deltaTime�� �����ش�.
				Timer += Time.deltaTime;

				// �����ð��� �����̽ð����� Ŀ������
				if (Timer > deleyTime)
				{
					// ������ ���� �˻�
					if (!isMove)
					{
						// �����̴� ������ ���� ��ġ�� 5��ŭ �������� �޾Ƽ� ����
						dir = new Vector3(UnityEngine.Random.Range(OriginPos.x - 3, OriginPos.x + 3), 0, UnityEngine.Random.Range(OriginPos.z - 3, OriginPos.z + 3));

						// �̵� �Ÿ� ����
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
					// false�϶�
					if (!isMove)
						// true�� ����
						isMove = true;
					anim.Find = true;

					rayMove();

					// ����� ��ġ�� ������ġ�� ���ϰ� ����ȭ
					Vector3 _vec = (dir - transform.position).normalized;

					// ���� ��ġ���� ����� ��ġ���� �̵�
					transform.position += _vec * m_Speed * Time.deltaTime;

					// ����� ��ġ���� �̵��ϸ�
					if ((int)transform.position.x == (int)dir.x && (int)transform.position.z == (int)dir.z)
					{
						// ���� �ð��� 0���� �ʱ�ȭ
						Timer = 0;
						// �����̴�  ������ �ð��� ������ �ð����� ����
						deleyTime = UnityEngine.Random.Range(0.5f, 1.5f);
						// �������� ���ϰ� �ٽ� false�� ����
						isMove = false;
						anim.Find = false;
					}
					break;
				}
			case EnemyType.Insect:
				{
					// false�϶�
					if (!isMove)
						// true�� ����
						isMove = true;
					anim.Find = true;

					rayMove();

					// ����� ��ġ�� ������ġ�� ���ϰ� ����ȭ
					Vector3 _vec = (dir - transform.position).normalized;

					// ���� ��ġ���� ����� ��ġ���� �̵�
					transform.position += _vec * m_Speed * Time.deltaTime;

					// ����� ��ġ���� �̵��ϸ�
					if ((int)transform.position.x == (int)dir.x && (int)transform.position.z == (int)dir.z)
					{
						// ���� �ð��� 0���� �ʱ�ȭ
						Timer = 0;
						// �����̴�  ������ �ð��� ������ �ð����� ����
						deleyTime = UnityEngine.Random.Range(0.5f, 1.5f);
						// �������� ���ϰ� �ٽ� false�� ����
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
		// �÷��̾�� �� ���̰� ���ݰŸ����� �� ��
		if (Vector3.Distance(transform.position, player.transform.position) > m_AtkDis)
		{

			if (Type != EnemyType.Human)
			{
				m_curTime += Time.deltaTime;

				// Idle �̵� ���� �ð��� Time.deltaTime�� �����ش�.
				Timer += Time.deltaTime;

				// �����ð��� �����̽ð����� Ŀ������
				if (Timer > deleyTime)
				{
					dir = (player.transform.position - transform.position).normalized;

					Timer = 0;
				}

				rayMove();

				// �÷��̾������� �̵�
				transform.position += dir * m_Speed * Time.deltaTime;
			}
			else
			{
				if (!escape)
					dir = (player.transform.position - transform.position).normalized;
				else
					m_curTime += Time.deltaTime;

				// �� Ż��
				if (m_curTime > m_AtkDeley)
				{
					escape = false;
					m_curTime = 0;
				}

				rayMove();

				// �÷��̾������� �̵�
				transform.position += dir * m_Speed * Time.deltaTime;
			}
		}
		// �÷��̾�� �����̰� ���ݰŸ����� ����� ��
		else if (Vector3.Distance(transform.position, player.transform.position) < m_AtkDis)
		{

			if (Type == EnemyType.Human)
			{
				// �ִϸ��̼� ���¸� CHARGE�� ����
				human.animState = Enemy_HumanAnimScript.AnimState.CHARGE;

				// �� ���¸� CHARGE�� ����
				state = EnemyState.CHARGE;
			}
			else
			{
				m_curTime += Time.deltaTime;

				if (m_curTime > m_AtkDeley)
				{
					// �ִϸ��̼� ���¸� CHARGE�� ����
					anim.animState = Enemy_AnimScript.AnimState.CHARGE;

					// �� ���¸� CHARGE�� ����
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
				// �÷��̾�� �� ĳ���� ���� ���� ���� ���
				dir = (player.transform.position - transform.position).normalized;
			}
			if (anim.wormCharge > 0.5f)
			{
				Sound = true;
				// �� ���¸� IMPACT�� ����
				state = EnemyState.IMPACT;

				if (t_type == turretType.None)
					atkRange.enabled = true;
			}
		}
		// �ִϸ��̼��� �������� 
		else
		{
			if (0.4f < skel.state.GetCurrent(0).AnimationTime &&
				skel.state.GetCurrent(0).AnimationTime < 0.42f)
			{
				// �÷��̾�� �� ĳ���� ���� ���� ���� ���
				dir = (player.transform.position - transform.position).normalized;
			}
			if (skel.state.GetCurrent(0).AnimationTime ==
				skel.state.GetCurrent(0).AnimationEnd)
			{
				Sound = true;
				// �� ���¸� IMPACT�� ����
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
							// �����ð��� �����̽ð����� Ŭ��
							{
								// �����ð��� 0���� �ʱ�ȭ
								m_curTime = 0;
							}
						}

					}
					else
					{
						// ���� �ִϸ��̼� �̸��� charge�� �ƴҶ�, ����Ʈ�� ���ÿ�
						if (!human.currentAnimation.Contains("charge") &&
							skel.state.GetCurrent(0).AnimationTime < 0.25f)
						{
							StartCoroutine(AttackSound());

							rayStop();

							// �� ĳ���͸� �÷��̾� �������� �̵�
							transform.position += dir.normalized * m_Speed * 3 * Time.deltaTime;

							StopCoroutine(AttackSound());
						}
					}

					// ���� �ִϸ��̼� �̸��� charge�� �ƴҶ�, ���� �ִϸ��̼��� ������
					if (!human.currentAnimation.Contains("charge") &&
						skel.state.GetCurrent(0).AnimationTime >
						0.25f)
					{
						if (t_type == turretType.None)
							atkRange.enabled = false;
					}

					// ���� �ִϸ��̼� �̸��� charge�� �ƴҶ�, ���� �ִϸ��̼��� ������
					if (!human.currentAnimation.Contains("charge") &&
					skel.state.GetCurrent(0).AnimationTime ==
					skel.state.GetCurrent(0).AnimationEnd)
					{
						// �ٽ� �÷��̾�� ���� ��ġ�� �˻��ϰ� ���ݰŸ����� ����ﶧ
						if (Vector3.Distance(transform.position, player.transform.position) <= m_AtkDis)
						{
							// �ִϸ��̼� ���¸� CHARGE�� ����
							human.animState = Enemy_HumanAnimScript.AnimState.CHARGE;
						}
						// ���ݰŸ����� �ָ�
						else
						{
							// �ִϸ��̼� ���¸� CHARGE�� ����
							human.animState = Enemy_HumanAnimScript.AnimState.MOVE;
						}

						// ���¸� �̵����� ����
						state = EnemyState.MOVE;
					}
					break;
				};

			case EnemyType.Insect:

				StartCoroutine(AttackSound());

				rayStop();

				// �� ĳ���͸� �÷��̾� �������� �̵�
				transform.position += dir.normalized * m_Speed * 1.3f * Time.deltaTime;

				// ���� �ִϸ��̼� �̸��� charge�� �ƴҶ�, ���� �ִϸ��̼��� ������
				if (!anim.currentAnimation.Contains("charge") &&
					skel.state.GetCurrent(0).AnimationTime >
					0.3f)
				{
					if (t_type == turretType.None)
						atkRange.enabled = false;
				}

				// ���� �ִϸ��̼� �̸��� charge�� �ƴҶ�, ���� �ִϸ��̼��� ������
				if (!anim.currentAnimation.Contains("charge") &&
				   skel.state.GetCurrent(0).AnimationTime ==
				   skel.state.GetCurrent(0).AnimationEnd)
				{
					if (t_type == turretType.None)
						atkRange.enabled = false;

					StopCoroutine(AttackSound());

					// �ٽ� �÷��̾�� ���� ��ġ�� �˻��ϰ� ���ݰŸ����� ����ﶧ
					if (Vector3.Distance(transform.position, player.transform.position) <= m_AtkDis)
					{
						// �ִϸ��̼� ���¸� CHARGE�� ����
						anim.animState = Enemy_AnimScript.AnimState.CHARGE;
					}
					// ���ݰŸ����� �ָ�
					else
					{
						// �ִϸ��̼� ���¸� CHARGE�� ����
						anim.animState = Enemy_AnimScript.AnimState.MOVE;
					}
					// ���¸� �̵����� ����
					state = EnemyState.MOVE;
				}
				break;
			case EnemyType.bat:

				StartCoroutine(AttackSound());

				rayStop();

				// ���� �ִϸ��̼� �̸��� charge�� �ƴҶ�, ����Ʈ�� ���ÿ�
				if (!anim.currentAnimation.Contains("charge") &&
					skel.state.GetCurrent(0).AnimationTime < 0.25f)
				{
					// �� ĳ���͸� �÷��̾� �������� �̵�
					transform.position += dir.normalized * m_Speed * 3 * Time.deltaTime;
				}

				// ���� �ִϸ��̼� �̸��� charge�� �ƴҶ�, ���� �ִϸ��̼��� ������
				if (!anim.currentAnimation.Contains("charge") &&
					skel.state.GetCurrent(0).AnimationTime >
					0.25f)
				{
					if (t_type == turretType.None)
						atkRange.enabled = false;
				}

				// ���� �ִϸ��̼� �̸��� charge�� �ƴҶ�, ���� �ִϸ��̼��� ������
				if (!anim.currentAnimation.Contains("charge") && skel.state.GetCurrent(0).AnimationTime ==
				   skel.state.GetCurrent(0).AnimationEnd)
				{
					if (t_type == turretType.None)
						atkRange.enabled = false;

					StopCoroutine(AttackSound());

					// �ٽ� �÷��̾�� ���� ��ġ�� �˻��ϰ� ���ݰŸ����� ����ﶧ
					if (Vector3.Distance(transform.position, player.transform.position) <= m_AtkDis)
					{
						// �ִϸ��̼� ���¸� CHARGE�� ����
						anim.animState = Enemy_AnimScript.AnimState.CHARGE;
					}
					// ���ݰŸ����� �ָ�
					else
					{
						// �ִϸ��̼� ���¸� CHARGE�� ����
						anim.animState = Enemy_AnimScript.AnimState.MOVE;
					}
					// ���¸� �̵����� ����
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

		// �ִϸ��̼� ����ð� == �ִϸ��̼� ����ð� <= �ִϸ��̼� ���� �˻� �� ���� �� Ư�� ���� ��
		if (!turret.currentAnimation.Contains("shoot"))
		{
			if (!Sound)
			{
				Sound = true;
			}
			// �� �����ð��� Time.deltaTime�� �����ش�.
			m_curTime += Time.deltaTime;
		}

		else
		{
			// �ͷ�Ÿ���� �˻�
			switch (t_type)
			{
				// �ͷ� Ÿ���� normal�϶�
				case turretType.normal:
					if (m_curTime > 2f)
					{
						// ���� ������Ʈ ����
						GameObject _bullet1 = ObjectPool_Bullet.instance.getBullet();

						StartCoroutine(AttackSound());

						if (_bullet1 != null)
						{
							// �Ѿ��� ��ġ�� �ڽ��� ��ġ�� ����
							_bullet1.transform.position = new Vector3(transform.position.x, 1.4f, transform.position.z);
							// �Ѿ��� ������ �÷��̾������� 
							_bullet1.GetComponent<Enemy_Bullet>().shootTarget(player.transform.position);
						}
						StopCoroutine(AttackSound());
						// �����ð��� 0���� �ʱ�ȭ
						m_curTime = 0;
					}
					break;
				// �ͷ�Ÿ���� round�϶�
				case turretType.round:
					// �����ð��� �����̽ð����� Ŭ��
					if (m_curTime > 2)
					{
						StartCoroutine(AttackSound());
						// �Ѿ˻����� ���� for��
						for (int i = 0; i < bulletCount; i++)
						{
							// ���� ������Ʈ ����
							GameObject _bullet2 = ObjectPool_Bullet.instance.getBullet();

							if (_bullet2 != null)
							{
								// �Ѿ��� ��ġ�� �ڽ��� ��ġ�� ����
								_bullet2.transform.position = new Vector3(
									transform.position.x, 1.4f, transform.position.z);
							}

							Vector3 dir = new Vector3(
								player.transform.position.x + UnityEngine.Random.RandomRange(-2, 2),
								player.transform.position.y,
								player.transform.position.z + UnityEngine.Random.RandomRange(-2, 2));

							// �Ѿ��� ������ �÷��̾������� 
							_bullet2.GetComponent<Enemy_Bullet>().shootTarget(dir);

							StopCoroutine(AttackSound());

							// �����ð��� 0���� �ʱ�ȭ
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
					// ���� ü�¿� ���������� ����
					m_HP -= atkpow;

					// �� ĳ������ HP�� 0���� ũ�� �ǰ� ���� ��ȯ
					if (m_HP > 0)
					{
						H_Sound = true;
						human.animState = Enemy_HumanAnimScript.AnimState.HIT;
						state = EnemyState.HIT;
						StartCoroutine(HitSound());
					}
					// �׷��� �ʴٸ� ��� ���·� ��ȯ
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
					// ���� ü�¿� ���������� ����
					m_HP -= atkpow;

					// �� ĳ������ HP�� 0���� ũ�� �ǰ� ���� ��ȯ
					if (m_HP > 0)
					{
						H_Sound = true;
						anim.animState = Enemy_AnimScript.AnimState.HIT;
						state = EnemyState.HIT;
						StartCoroutine(HitSound());
					}
					// �׷��� �ʴٸ� ��� ���·� ��ȯ
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
					// ���� ü�¿� ���������� ����
					m_HP -= atkpow;

					// �� ĳ������ HP�� 0���� ũ�� �ǰ� ���� ��ȯ
					if (m_HP > 0)
					{
						H_Sound = true;
						anim.animState = Enemy_AnimScript.AnimState.HIT;
						state = EnemyState.HIT;
						StartCoroutine(HitSound());
					}
					// �׷��� �ʴٸ� ��� ���·� ��ȯ
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
				// �ִϸ��̼� ����ð� == �ִϸ��̼� ����ð� <= �ִϸ��̼� ���� �˻� �� ���� �� Ư�� ���� ��
				if (!turret.currentAnimation.Contains("shoot"))
				{
					// �� �����ð��� Time.deltaTime�� �����ش�.
					m_curTime += Time.deltaTime;
				}

				// ���� ü�¿� ���������� ����
				m_HP -= atkpow;

				H_Sound = true;

				StartCoroutine(HitSound());

				// �� ĳ������ HP�� 0���� ũ�� �ǰ� ���� ��ȯ
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
					// �ִϸ��̼��� ������
					if (skel.state.GetCurrent(0).AnimationTime ==
						skel.state.GetCurrent(0).AnimationEnd)
					{
						// ���¸� �ٽ� Move�� ����
						human.animState = Enemy_HumanAnimScript.AnimState.MOVE;
						state = EnemyState.MOVE;
						StopCoroutine(HitSound());
					}
					break;
				}

			case EnemyType.Insect:
				{
					// �ִϸ��̼��� ������
					if (skel.state.GetCurrent(0).AnimationTime ==
						skel.state.GetCurrent(0).AnimationEnd)
					{
						// ���¸� �ٽ� Move�� ����
						anim.animState = Enemy_AnimScript.AnimState.MOVE;
						state = EnemyState.MOVE;
						StopCoroutine(HitSound());
					}
					break;
				}
			case EnemyType.bat:
				{
					// �ִϸ��̼��� ������
					if (skel.state.GetCurrent(0).AnimationTime ==
						skel.state.GetCurrent(0).AnimationEnd)
					{
						// ���¸� �ٽ� Move�� ����
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
					// ��� �ִϸ��̼��� ���� True �� ����
					human.Dead = true;

					// �� ���µ� Die�� ����
					human.animState = Enemy_HumanAnimScript.AnimState.DIE;
					state = EnemyState.DIE;

					StopCoroutine(HitSound());

					// ��� ����Ʈ
					dead.SetActive(true);
					StartCoroutine(dead.GetComponent<Dead_AnimScript>().startEff());

					// ��Ų�� ����
					gameObject.transform.GetChild(1).gameObject.SetActive(false);
					gameObject.transform.GetChild(2).gameObject.SetActive(false);

					field.MinusCurEnemyCount();

					nowDead = true;
					break;
				}

			case EnemyType.Insect:
				{
					// ��� �ִϸ��̼��� ���� True �� ����
					anim.Dead = true;

					// �� ���µ� Die�� ����
					anim.animState = Enemy_AnimScript.AnimState.DIE;
					state = EnemyState.DIE;

					StopCoroutine(HitSound());

					// ��� ����Ʈ
					dead.SetActive(true);
					StartCoroutine(dead.GetComponent<Dead_AnimScript>().startEff());

					// ��Ų�� ����
					gameObject.transform.GetChild(1).gameObject.SetActive(false);
					gameObject.transform.GetChild(2).gameObject.SetActive(false);

					if (gameObject.transform.parent.name != "EnemyBox")
						field.MinusCurEnemyCount();
					
					nowDead = true;
					break;
				}
			case EnemyType.bat:
				{
					// ��� �ִϸ��̼��� ���� True �� ����
					anim.Dead = true;

					// �� ���µ� Die�� ����
					anim.animState = Enemy_AnimScript.AnimState.DIE;
					state = EnemyState.DIE;

					StopCoroutine(HitSound());

					// ��� ����Ʈ
					dead.SetActive(true);
					StartCoroutine(dead.GetComponent<Dead_AnimScript>().startEff());

					// ��Ų�� ����
					gameObject.transform.GetChild(1).gameObject.SetActive(false);
					gameObject.transform.GetChild(2).gameObject.SetActive(false);

					field.MinusCurEnemyCount();
					nowDead = true;
					break;
				}
			case EnemyType.Turret:
				{
					// �� ���µ� Die�� ����
					turret.animState = Enemy_TurretAnimScript.AnimState.DIE;
					state = EnemyState.DIE;

					StopCoroutine(HitSound());

					// ��� ����Ʈ
					dead.SetActive(true);
					StartCoroutine(dead.GetComponent<Dead_AnimScript>().startEff());

					// ��Ų�� ����
					gameObject.transform.GetChild(0).gameObject.SetActive(false);
					gameObject.transform.GetChild(1).gameObject.SetActive(false);

					field.MinusCurEnemyCount();
					nowDead = true;
					break;
				}
		}
	}

	// ���� �÷��̾ �߰����� �� 
	IEnumerator FindSound()
    {		
		// �� Ÿ���� Human�̶��
		if (Type == EnemyType.Human)
		{
			// Human�� HumanŸ���� Large�̰�
			if (H_Type == HumanType.Large)
			{
				// �÷��̾ ���� �� ���
				if (Search == true)
				{					
					// SoundManager ���� 
					SoundManager.Sound.Play("Enemy/Normal/Warning/Master.assets [746]", SoundManager.SoundType.Enemy);
					Search = false;
				}
			}
			// Human�� HumanŸ���� Normal�̶��
			else if (H_Type == HumanType.Normal)
			{
				if (Search == true)
				{					
					SoundManager.Sound.Play("Enemy/Normal/Warning/Master.assets [421]", SoundManager.SoundType.Enemy);
					Search = false;
				}
			}
		}
		// HumanŸ���� �ƴ϶��
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

	// ���� �÷��̾ ������ ��
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

	// �÷��̾�� ���� ������ ��
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

	// ���� ������
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


