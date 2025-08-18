using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.UIElements;
using static Cinemachine.DocumentationSortingAttribute;

public class PlayerMove : MonoBehaviour
{
	// 스파인 애니메이션
	SkeletonAnimation skeletonAnimation;

	Player_AnimScript playerData;

	public GameObject atkRange;

	public Player_AttackScript weaponData;

	// 조작 관련 함수
	public float h;
	public float v;

	public bool isFlip;

	bool nowAttack;

	bool nowHit;
	float hitTime;

	// 대쉬 구현 함수들
	public bool isdash;
	public float defaultSpeed = 10f;
	public float dashSpeed;

	// 플레이어 체력
	public float minHp = 0;
	public float maxHp = 4;

	RaycastHit rayHit;
	Vector3 oriPos;

	void Start()
	{
		// 스파인 애니메이션
		skeletonAnimation = GetComponentInChildren<SkeletonAnimation>();
		// 플레이어 정보
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
		// 벽에 충돌 하였을 때 캐릭터의 움직임을 제한한다.
		if (!isdash)
		{
			h = Input.GetAxisRaw("Horizontal");

			v = Input.GetAxisRaw("Vertical");

			//Debug.Log("isDash : "+isdash+"isPlayed :" + isPlayed + ", cTime : " + cTime);
			//Debug.Log("h :" + h + "v : " + v);

			// 플레이어가 공격 중일 때, 이동 소리를 제어한다.
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

		// 플레이어가 대쉬 중일 때, 이동 소리를 제어한다.
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
			Debug.Log("원거리 공격 모으는 중...");
		}

		if (Input.GetKeyDown(KeyCode.F))
		{
			if (isPlayed == false && cTime <= 0 && !isdash)
			{
				if (!(h == 0 && v == 0))
					StartSound(SoundType.Walk);
			}
			Debug.Log("상호작용");
		}
		if (Input.GetKeyDown(KeyCode.E))
		{
			Debug.Log("취소");
		}
	}

	#region _Move세팅

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

		//Transform Move로 추가할 것. (AddForce 사용 x)

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
		// 무기와 상호작용
		if (other.gameObject.layer.Equals(10) && Input.GetKeyDown(KeyCode.F))
		{
			var triggerWeaponData = other.GetComponent<Item_WeaponDrop>();
			var pm = ksjPlayerManager.Instance;
			pm.ChangePlayerWeapon(triggerWeaponData.type, triggerWeaponData.level, triggerWeaponData.atk, triggerWeaponData.weaponSpeed);


			// 현재 무기 타입 저장
			WeaponType oldType;

			oldType = playerData.myWeapon;
			int oldLevel = weaponData.level;

			Item_WeaponDrop weapon = other.GetComponent<Item_WeaponDrop>();
			weaponData.SetAtkDamae(weapon.changeLevel(), weapon.changeAtk());
			weapon.setLevel(oldLevel);

			// 상호작용한 무기타입에 따라 무기 변경
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

			// 이전 무기와 무기 드롭의 타입을 변경
			other.gameObject.GetComponent<Item_WeaponDrop>().setWeapon(oldType);

			// 무기의 위치 변경
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


	// ====// ====// ====// ==== 아래부턴 코루틴 관련 // ====// ====// ====// ====


	bool isCompleted = false;
	bool isPlayed = false;

	// 코루틴 값을 제어하기 위한 float 함수
	float cTime = 0;

	SoundType curSoundType;

	// 각 사운드를 enum으로 관리
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

			// 각 움직임마다 switch 문으로 다르게 소리를 설정해 준다.
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

	// float에서 void 수정
	// if (pm.isOpendMenu)
	//return; 추가
	private void CallWeaponSound()
	{
		//플레이 매니저 컴포넌트
		var pm = ksjPlayerManager.Instance;

		//메뉴를 열었을 때 공격 안되 게 설정
		if (pm.IsOpenedMenu)
			return;

		//플레이어 애니메이션 컴포넌트
		var pa = GetComponentInChildren<Player_AnimScript>();



		// 각 무기마다 딜레이 조정 해주기
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

