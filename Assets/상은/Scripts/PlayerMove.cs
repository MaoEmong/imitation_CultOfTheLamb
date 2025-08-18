using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.UIElements;
using static Cinemachine.DocumentationSortingAttribute;

public class PlayerMove : MonoBehaviour
{
	// ������ �ִϸ��̼�
	SkeletonAnimation skeletonAnimation;

	Player_AnimScript playerData;

	public GameObject atkRange;

	public Player_AttackScript weaponData;

	// ���� ���� �Լ�
	public float h;
	public float v;

	public bool isFlip;

	bool nowAttack;

	bool nowHit;
	float hitTime;

	// �뽬 ���� �Լ���
	public bool isdash;
	public float defaultSpeed = 10f;
	public float dashSpeed;

	// �÷��̾� ü��
	public float minHp = 0;
	public float maxHp = 4;

	RaycastHit rayHit;
	Vector3 oriPos;

	void Start()
	{
		// ������ �ִϸ��̼�
		skeletonAnimation = GetComponentInChildren<SkeletonAnimation>();
		// �÷��̾� ����
		playerData = GetComponentInChildren<Player_AnimScript>();

		var pm = ksjPlayerManager.Instance;
		minHp = pm.HP;
	}

	void Update()
	{
		if (!ksjPlayerManager.Instance.IsVideoEnded)
			return;
		
		if (cTime >= 0)
		{
			cTime -= Time.deltaTime;
		}

		isDash();
		InputMethod();

		rayMove();

		if (!isdash)
			Attack();

		if (hitTime > 0)
			hitTime -= Time.deltaTime;
		else
			nowHit = false;

		if (!nowAttack && !playerData.nowAction)
			Move();
		if (playerData.nowAction && playerData.animState == Player_AnimScript.AnimState.HIT)
			Move();
	}

	void rayMove()
	{
		oriPos = new Vector3(transform.position.x, 1, transform.position.z);

		rayHit = new RaycastHit();

		Ray ray = new Ray(oriPos, new Vector3(h, 0, v).normalized);


		if (Physics.Raycast(ray, out rayHit, 1.5f, LayerMask.GetMask("Wall")))
		{
			if (rayHit.collider.gameObject.layer.Equals(15))
			{
				if (isdash)
				{
					h = 0;
					v = 0;
				}
				else
				{
					if (Input.GetAxisRaw("Horizontal") != 0)
					{
						h = 0;
					}
					if (Input.GetAxisRaw("Vertical") != 0)
					{
						v = 0;
					}
				}
			}
		}
		if (Physics.SphereCast(transform.position, 0.5f, new Vector3(h, 0, v).normalized, out rayHit, 0.5f, LayerMask.GetMask("BreakableObject")))
		{
			Debug.Log("HitBreakableObject");
			if (rayHit.collider.gameObject.tag == "Stone" ||
				rayHit.collider.gameObject.tag == "Stump" ||
				rayHit.collider.gameObject.tag == "Tent" ||
				rayHit.collider.gameObject.tag == "WoodenRedStick")
			{
				if (isdash)
				{
					h = 0;
					v = 0;
				}
				else
				{
					if (Input.GetAxisRaw("Horizontal") != 0)
					{
						h = 0;
					}
					if (Input.GetAxisRaw("Vertical") != 0)
					{
						v = 0;
					}
				}
			}
		}

		Debug.DrawRay(oriPos, new Vector3(h, 0, v).normalized * 1.5f, Color.green);
	}

	void InputMethod()
	{
		// ���� �浹 �Ͽ��� �� ĳ������ �������� �����Ѵ�.
		if (!isdash)
		{
			h = Input.GetAxisRaw("Horizontal");

			v = Input.GetAxisRaw("Vertical");

			//Debug.Log("isDash : "+isdash+"isPlayed :" + isPlayed + ", cTime : " + cTime);
			//Debug.Log("h :" + h + "v : " + v);

			// �÷��̾ ���� ���� ��, �̵� �Ҹ��� �����Ѵ�.
			if (isPlayed == false && cTime <= 0 && !nowAttack && !playerData.nowAction)
			{
				if (!(h == 0 && v == 0))
					StartSound(SoundType.Walk);
			}

			//if (h == 0 && v == 0)
			//{
			//    isCompleted = true;
			//}
		}

		// �÷��̾ �뽬 ���� ��, �̵� �Ҹ��� �����Ѵ�.
		if (Input.GetKeyDown(KeyCode.Space) && (h != 0 || v != 0) && !nowHit && !nowAttack)
		{
			if (isPlayed && curSoundType == SoundType.Walk)
			{
				StopSound();
				cTime = 0;
			}

			isdash = true;

			if (!isPlayed && cTime <= 0)
			{
				Debug.Log(isPlayed + "," + cTime);
				StartSound(SoundType.Dash);
			}
		}


		if (Input.GetMouseButtonDown(0) && !isdash)
		{
			if (!isPlayed && cTime <= 0)
			{
				var PA = GetComponentInChildren<Player_AnimScript>();

				switch (PA.nowCombo)
				{
					case 0:
						StartSound(SoundType.Attack0);
						break;
					case 1:
						StartSound(SoundType.Attack1);
						break;
					case 2:
						StartSound(SoundType.Attack2);
						break;
					case 3:
						StartSound(SoundType.Attack2);
						break;
				}

			}
		}
		if (Input.GetMouseButton(1))
		{
			Debug.Log("���Ÿ� ���� ������ ��...");
		}

		if (Input.GetKeyDown(KeyCode.F))
		{
			if (isPlayed == false && cTime <= 0 && !isdash)
			{
				if (!(h == 0 && v == 0))
					StartSound(SoundType.Walk);
			}
			Debug.Log("��ȣ�ۿ�");
		}
		if (Input.GetKeyDown(KeyCode.E))
		{
			Debug.Log("���");
		}
	}

	#region _Move����

	void Move()
	{
		Vector3 dir = new Vector3(h, 0, v);

		if (h > 0)
		{
			transform.rotation = Quaternion.Euler(0, 180, 0);
			isFlip = true;
		}
		else if (h < 0)
		{
			transform.rotation = Quaternion.Euler(0, 0, 0);
			isFlip = false;
		}


		if (isdash)
		{
			transform.position += dir.normalized * dashSpeed * Time.deltaTime;
		}
		else
		{
			transform.position += dir.normalized * defaultSpeed * Time.deltaTime;
		}

		//Transform Move�� �߰��� ��. (AddForce ��� x)

		// rigid.AddForce(new Vector3 (h,0, v), ForceMode.Impulse);		

	}

	void isDash()
	{
		if (isdash)
		{
			if (skeletonAnimation.state.GetCurrent(0).AnimationTime ==
				skeletonAnimation.state.GetCurrent(0).AnimationEnd)
				isdash = false;
		}
	}

	#endregion

	void Attack()
	{
		if (Input.GetMouseButton(0))
		{
			nowAttack = true;
		}

		if (playerData.animState == Player_AnimScript.AnimState.IDLE)
		{
			nowAttack = false;
		}

		if (nowAttack && playerData.animState == Player_AnimScript.AnimState.MOVE)
		{
			nowAttack = false;

		}

		if (nowAttack && skeletonAnimation.state.GetCurrent(0).AnimationTime ==
						 skeletonAnimation.state.GetCurrent(0).AnimationEnd)
		{
			nowAttack = false;
		}
	}

	void Hit()
	{
		if (nowHit)
			return;

		var pm = ksjPlayerManager.Instance;

		hitTime = 1.5f;

		nowHit = true;


		pm.PlayerDamagedProcess(-0.5f);
		minHp = pm.HP;

		if (!nowAttack)
			playerData.animState = Player_AnimScript.AnimState.HIT;
		if (nowAttack)
			playerData.StartHitFlash();

		nowAttack = false;

		if (minHp <= 0)
		{
			playerData.animState = Player_AnimScript.AnimState.DIE;
		}
	}

	private void OnTriggerStay(Collider other)
	{
		// ����� ��ȣ�ۿ�
		if (other.gameObject.layer.Equals(10) && Input.GetKeyDown(KeyCode.F))
		{
			var triggerWeaponData = other.GetComponent<Item_WeaponDrop>();
			var pm = ksjPlayerManager.Instance;
			pm.ChangePlayerWeapon(triggerWeaponData.type, triggerWeaponData.level, triggerWeaponData.atk, triggerWeaponData.weaponSpeed);


			// ���� ���� Ÿ�� ����
			WeaponType oldType;

			oldType = playerData.myWeapon;
			int oldLevel = weaponData.level;

			Item_WeaponDrop weapon = other.GetComponent<Item_WeaponDrop>();
			weaponData.SetAtkDamae(weapon.changeLevel(), weapon.changeAtk());
			weapon.setLevel(oldLevel);

			// ��ȣ�ۿ��� ����Ÿ�Կ� ���� ���� ����
			switch (other.gameObject.GetComponent<Item_WeaponDrop>().type)
			{
				case WeaponType.SWORD:
					playerData.getWeapon(WeaponType.SWORD);
					break;
				case WeaponType.AXE:
					playerData.getWeapon(WeaponType.AXE);
					break;
				case WeaponType.DAGGER:
					playerData.getWeapon(WeaponType.DAGGER);
					break;
			}

			// ���� ����� ���� ����� Ÿ���� ����
			other.gameObject.GetComponent<Item_WeaponDrop>().setWeapon(oldType);

			// ������ ��ġ ����
			other.transform.position = new Vector3(transform.position.x - 1, 1, transform.position.z + 0.5f);


			pm.SetWeaponDataInUI(triggerWeaponData.type, triggerWeaponData.level, triggerWeaponData.atk, triggerWeaponData.weaponSpeed);
		}

		if (other.gameObject.layer.Equals(21))
		{
			if (!isdash)
				Hit();
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.layer == LayerMask.NameToLayer("Weapon"))
		{
			Debug.Log("findWeapon");
			var triggerWeaponData = other.GetComponent<Item_WeaponDrop>();

			var pm = ksjPlayerManager.Instance;
			var weapon = other.GetComponent<Item_WeaponDrop>();

			pm.SetWeaponDataInUI(triggerWeaponData.type, triggerWeaponData.level, triggerWeaponData.atk, triggerWeaponData.weaponSpeed);
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.gameObject.layer == LayerMask.NameToLayer("Weapon"))
		{

			var pm = ksjPlayerManager.Instance;
			pm.SleepWeaponDataUI();
		}
	}


	// ====// ====// ====// ==== �Ʒ����� �ڷ�ƾ ���� // ====// ====// ====// ====


	bool isCompleted = false;
	bool isPlayed = false;

	// �ڷ�ƾ ���� �����ϱ� ���� float �Լ�
	float cTime = 0;

	SoundType curSoundType;

	// �� ���带 enum���� ����
	private enum SoundType
	{
		Walk,
		Attack0,
		Attack1,
		Attack2,
		Attack3,
		Dash,
		Hit,
		Default
	}

	private void StartSound(SoundType sd)
	{
		isPlayed = true;
		isCompleted = false;
		curSoundType = sd;
		StartCoroutine(PlaySound(sd));
	}

	private void StopSound()
	{
		StopCoroutine(PlaySound());
		isPlayed = false;
	}

	IEnumerator PlaySound(SoundType sd = SoundType.Default)
	{
		while (true)
		{
			if (isCompleted == true)
			{
				StopSound();
				break;
			}

			// �� �����Ӹ��� switch ������ �ٸ��� �Ҹ��� ������ �ش�.
			switch (sd)
			{
				case SoundType.Walk:
					cTime = 0.2f;
					SoundManager.Sound.Play("Player/FootStep/Master.assets [40]");
					isCompleted = true;
					break;
				case SoundType.Dash:
					cTime = 0.53f;
					SoundManager.Sound.Play("Player/Dash/Master.assets [640]");
					isCompleted = true;
					break;
				case SoundType.Attack0:
					CallWeaponSound();
					//SoundManager.Sound.Play("Player/Attack/Master.assets [751]");
					isCompleted = true;
					break;
				case SoundType.Attack1:
					CallWeaponSound();
					//SoundManager.Sound.Play("Player/Attack/Master.assets [232]");
					isCompleted = true;
					break;
				case SoundType.Attack2:
					CallWeaponSound();
					//SoundManager.Sound.Play("Player/Attack/Master.assets [771]");
					isCompleted = true;
					break;
				case SoundType.Attack3:
					CallWeaponSound();
					//SoundManager.Sound.Play("Player/Attack/Master.assets [771]");
					isCompleted = true;
					break;
				case SoundType.Default:
					break;

			}
			yield return new WaitForSeconds(0.2f);
		}
	}

	// float���� void ����
	// if (pm.isOpendMenu)
	//return; �߰�
	private void CallWeaponSound()
	{
		//�÷��� �Ŵ��� ������Ʈ
		var pm = ksjPlayerManager.Instance;

		//�޴��� ������ �� ���� �ȵ� �� ����
		if (pm.IsOpenedMenu)
			return;

		//�÷��̾� �ִϸ��̼� ������Ʈ
		var pa = GetComponentInChildren<Player_AnimScript>();



		// �� ���⸶�� ������ ���� ���ֱ�
		switch (pm.CurPlayerWeapon)
		{
			case WeaponType.SWORD:
				switch (pa.nowCombo)
				{
					case 0:
						cTime = 0.5f;
						SoundManager.Sound.Play("Player/Attack/Master.assets [810]");
						break;
					case 1:
						cTime = 0.5f;
						SoundManager.Sound.Play("Player/Attack/Master.assets [891]");
						break;
					case 2:
						cTime = 0.5f;
						SoundManager.Sound.Play("Player/Attack/Master.assets [865]");
						break;
				}
				break;
			case WeaponType.AXE:
				switch (pa.nowCombo)
				{
					case 0:
						cTime = 0.6f;
						SoundManager.Sound.Play("Player/Attack/Master.assets [978]");
						break;
					case 1:
						cTime = 0.6f;
						SoundManager.Sound.Play("Player/Attack/Master.assets [986]");
						break;
					case 2:
						cTime = 0.6f;
						SoundManager.Sound.Play("Player/Attack/Master.assets [907]");
						break;
				}
				break;
			case WeaponType.DAGGER:
				switch (pa.nowCombo)
				{
					case 0:
						cTime = 0.43f;
						SoundManager.Sound.Play("Player/Attack/Master.assets [751]");
						break;
					case 1:
						cTime = 0.43f;
						SoundManager.Sound.Play("Player/Attack/Master.assets [771]");
						break;
					case 2:
						cTime = 0.58f;
						SoundManager.Sound.Play("Player/Attack/Master.assets [248]");
						break;
					case 3:
						cTime = 0.58f;
						SoundManager.Sound.Play("Player/Attack/Master.assets [806]");
						break;
				}
				break;
		}
		//return cTime = Delay;
	}
}

