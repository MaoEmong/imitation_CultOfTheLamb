using System.Collections;
using Spine.Unity;
using UnityEditor.Rendering;
using UnityEngine;

public class Player_AnimScript : MonoBehaviour
{

	//
	[SerializeField]
	private Material ksjCusMat;

	// 스파인 애니메이션
	SkeletonAnimation skeletonAnimation;

	// 플레이어 무브 상태 확인
	PlayerMove move;

	// 애니매이션 재생을 위한 클립
	public AnimationReferenceAsset[] moveCilp;
	public AnimationReferenceAsset[] attackCilp;
	public AnimationReferenceAsset[] actionCilp;
	public AnimationReferenceAsset[] getCilp;

	// 현재 Attack 상태 여부
	public bool nowAttack;
	// 현재 Action 상태 여부
	public bool nowAction;

	bool Die;

	// 애니메이션 타입
	public enum AnimState
	{
		IDLE,
		MOVE,
		ROLL,
		ATTACK,
		COMBO1,
		COMBO2,
		COMBO3,
		COMBO4,
		CHARGE,
		IMPACT,
		ACTION,
		HIT,
		DIE,
		GETCARD,
		GETSWORD,
		GETAXE,
		GETDAGGER
	}

	#region _이동제어 변수

	// 구르기 상태
	bool nowRoll;

	// 이동 방향
	float h;
	float v;

	#endregion

	#region _공격제어 변수

	// 콤보 타임
	public float combotime;
	// 콤보 진행
	public int nowCombo;

	// public enum WeaponType
	// {  
	// 	  SWORD,
	// 	  AXE,
	// 	  KNIFE,
	// 	  GAUNTLETS
	// }  

	public WeaponType myWeapon;

	#endregion

	#region _액션제어 변수

	public bool getCard;

	#endregion

	// 현재 애니메이션 처리 상태
	public AnimState animState;

	// 현재 진행 중인 애니메이션
	string currentAnimation;

	private void Start()
	{
		// 필요 컴포넌트들 가져오기
		skeletonAnimation = GetComponent<SkeletonAnimation>();
        orignMat = skeletonAnimation.skeletonDataAsset.atlasAssets[0].PrimaryMaterial;
		move = GetComponentInParent<PlayerMove>();
	}

	private void Update()
	{
		if (Die)
			return;

		var pm = ksjPlayerManager.Instance;

		ActionAnim();

		// 애니메이션 상태 검사
		if (!nowAction && pm.Menu.activeSelf != true && !move.isdash)
			AttackAnim();

		// 애니메이션 상태 검사
		if (!nowAction && !nowAttack && !getCard)
			MoveAnim();
	}

	void MoveAnim()
	{
		// 현재 이동키의 입력 상태를 float값으로 저장받는다
		h = Input.GetAxisRaw("Horizontal");
		v = Input.GetAxisRaw("Vertical");

		// 이동 중이 아닐 때
		if (h == 0 && v == 0)
		{
			// 현재 애니메이션 상태를 IDLE
			animState = AnimState.IDLE;
		}
		// 구르기(회피)를 사용했을 때
		else if (Input.GetKeyDown(KeyCode.Space) && (h != 0 || v != 0))
		{
			// 현재 애니메이션 상태를 ROLL
			animState = AnimState.ROLL;
		}
		// 이동 중일 때
		else if (!Input.GetKeyDown(KeyCode.Space) && (h != 0 || v != 0))
		{
			// 현재 애니메이션 상태를 MOVE
			animState = AnimState.MOVE;
		}

		move_setCurrentAnimation(animState);
	}

	void AttackAnim()
	{
        var pm = ksjPlayerManager.Instance;
        myWeapon = pm.CurPlayerWeapon;

        // 콤보 1 공격
        if (nowCombo == 0 && Input.GetMouseButtonDown(0) && combotime < 0.5f)
		{
			nowCombo = 1;
			animState = AnimState.COMBO1;
			transform.parent.GetComponent<ksjPlayerAttackScan>().AttackRay();           
        }
		// 콤보 2 공격
		else if (nowCombo == 1 && Input.GetMouseButtonDown(0) && combotime < 0.5f)
		{
			nowCombo = 2;
			animState = AnimState.COMBO2;
			transform.parent.GetComponent<ksjPlayerAttackScan>().AttackRay();
		}
		// 콤보 3 공격
		else if (nowCombo == 2 && Input.GetMouseButtonDown(0) && combotime < 0.5f)
		{
			nowCombo = 3;
			animState = AnimState.COMBO3;
			transform.parent.GetComponent<ksjPlayerAttackScan>().AttackRay();
		}
		// 콤보 4 공격(나이프 한정)
		else if (nowCombo == 3 && Input.GetMouseButtonDown(0) && myWeapon == WeaponType.DAGGER
			&& combotime < 0.5f)
		{
			nowCombo = 4;
			animState = AnimState.COMBO4;
			transform.parent.GetComponent<ksjPlayerAttackScan>().AttackRay();
		}

		/*

		// 첫 공격 중 마우스 좌클릭 중일 때
		if (nowCombo == 0 && Input.GetMouseButton(0))
		{
			// 콤보 스택 0
			nowCombo = 0;
			// 차지
			animState = AnimState.CHARGE;
		}

		// 차지 중 마우스 좌클릭을 떼면
		if (animState == AnimState.CHARGE && Input.GetMouseButtonUp(0))
		{
			// 임팩트
			animState = AnimState.IMPACT;
		}

		*/

		attack_setCurrentAnimation(animState);
    }

	void ActionAnim()
	{
		action_setCurrentAnimation(animState);
	}

	#region _무브 세팅

	// 애니메이션 재생 메소드
	void move_AsyncAnimation(AnimationReferenceAsset Clip, bool loop, float timeScale)
	{
		// 현재 구르기 애니메이션 출력 중일 때
		if (nowRoll)
		{
			// 현재 재생중인 애니메이션 클립이 구르기가 아닐 때
			if (currentAnimation == "idle")
				nowRoll = false;

			// 애니메이션 진행시간 == 애니메이션 종료시간 <= 애니메이션 상태 검사 후 종료 시 구르기 상태 끝
			if (skeletonAnimation.state.GetCurrent(0).AnimationTime ==
				skeletonAnimation.state.GetCurrent(0).AnimationEnd)
			{
				nowRoll = false;
			}
			// 아직 진행중이라면 애니메이션 전환 X
			return;
		}

		combotime += Time.deltaTime;

		if (combotime > 0.5f)
		{
			combotime = 0;
			nowCombo = 0;
		}	

		// 현재 재생중인 애니메이션이 다시 재생될 때
		if (Clip.name.Equals(currentAnimation))
			// 애니메이션 전환 X
			return;

		// 해당 애니메이션 재생
		skeletonAnimation.state.SetAnimation(0, Clip, loop).TimeScale = timeScale;

		// 현재 애니메이션의 이름을 저장
		currentAnimation = Clip.name;
	}

	// 현재 상태에 따라 애니메이션 재생 전환
	void move_setCurrentAnimation(AnimState state)
	{
		// 현재 상태
		switch (state)
		{
			// IDLE
			case AnimState.IDLE:
				// IDLE 애니메이션, 루프 O, 애니메이션 재생 시간 1배속
				move_AsyncAnimation(moveCilp[0], true, 1);
				break;
			case AnimState.MOVE:
				// 이동 방향에 따라 다른 애니메이션 출력
				moveDir();
				break;
			case AnimState.ROLL:
				// 구르기 방향에 따라 다른 애니메이션 출력
				rollDir();
				nowRoll = true;
				break;
		}
	}

	// MOVE 애니메이션 출력 메소드
	void moveDir()
	{
		if (h != 0)
		{
			// 대각 아래 이동
			if (v < 0)
				// Move 애니메이션, 루프 O, 애니메이션 재생 시간 1배속
				move_AsyncAnimation(moveCilp[4], true, 1);
			// 대각 위 이동
			else if (v > 0)
				// Move 애니메이션, 루프 O, 애니메이션 재생 시간 1배속
				move_AsyncAnimation(moveCilp[2], true, 1);
			// 좌우 아래 이동
			else if (v == 0)
				// Move 애니메이션, 루프 O, 애니메이션 재생 시간 1배속
				move_AsyncAnimation(moveCilp[3], true, 1);
		}
		else
		{
			// 아래 이동
			if (v < 0)
				// Move 애니메이션, 루프 O, 애니메이션 재생 시간 1배속
				move_AsyncAnimation(moveCilp[5], true, 1);
			// 위 이동
			else if (v > 0)
				// Move 애니메이션, 루프 O, 애니메이션 재생 시간 1배속
				move_AsyncAnimation(moveCilp[1], true, 1);
		}
	}

	// ROLL 애니메이션 출력 메소드
	void rollDir()
	{
		// 아래 구르기
		if (v < 0)
			// ROLL 애니메이션, 루프 X, 애니메이션 재생 시간 1배속
			move_AsyncAnimation(moveCilp[8], false, 1);
		// 위 구르기
		else if (v > 0)
			// ROLL 애니메이션, 루프 X, 애니메이션 재생 시간 1배속
			move_AsyncAnimation(moveCilp[6], false, 1);
		// 옆 구르기
		else
			// ROLL 애니메이션, 루프 X, 애니메이션 재생 시간 1배속
			move_AsyncAnimation(moveCilp[7], false, 1);
	}

	#endregion

	#region _어택 세팅

	// 애니메이션 재생 메소드
	void attack_AsyncAnimation(AnimationReferenceAsset Clip, bool loop, float timeScale)
	{
		// 현재 공격 애니메이션 출력 중일 때
		if (nowAttack)
		{
			nowAction = false;

			combotime = 1;

            // 애니메이션 진행시간 == 애니메이션 종료시간 <= 애니메이션 상태 검사 후 종료 시 공격 상태 끝
            if (skeletonAnimation.state.GetCurrent(0).AnimationTime ==
				skeletonAnimation.state.GetCurrent(0).AnimationEnd)
			{
				animState = AnimState.ATTACK;
				nowAttack = false;
				combotime = 0;
			}
			// 아직 진행중이라면 애니메이션 전환 X
			return;
		}

		// 현재 재생중인 애니메이션이 다시 재생될 때
		if (Clip.name.Equals(currentAnimation))
			// 애니메이션 전환 X
			return;

        // 해당 애니메이션 재생
        skeletonAnimation.state.SetAnimation(0, Clip, loop).TimeScale = timeScale;
	
		// 현재 애니메이션의 이름을 저장
		currentAnimation = Clip.name;

		if (myWeapon != WeaponType.DAGGER && nowCombo > 2)
			nowCombo = 0;
		else if (myWeapon == WeaponType.DAGGER && nowCombo > 3)
			nowCombo = 0;
	}

	// 현재 상태에 따라 애니메이션 재생 전환
	void attack_setCurrentAnimation(AnimState state)
	{
		// 현재 상태
		switch (state)
		{
			case AnimState.ATTACK:
				attack_AsyncAnimation(moveCilp[0], true, 1);
				return;
			case AnimState.COMBO1:
				comboOne();
				break;
			case AnimState.COMBO2:
				comboTwo();
				break;
			case AnimState.COMBO3:
				comboThree();
				break;
			case AnimState.COMBO4:
				attack_AsyncAnimation(attackCilp[9], false, 1.5f);
				nowAttack = true;
				break;
		}
	}

	// 무기에 따라 다른 콤보 발동
	void comboOne()
	{
       
        switch (myWeapon)
		{
			case WeaponType.SWORD:
				attack_AsyncAnimation(attackCilp[0], false, 1.5f);
				nowAttack = true;
				break;
			case WeaponType.AXE:
				attack_AsyncAnimation(attackCilp[3], false, 1.5f);
                Debug.Log("myWeapon : " + myWeapon);				
                nowAttack = true;
				break;
			case WeaponType.DAGGER:
				attack_AsyncAnimation(attackCilp[6], false, 1.5f);
				nowAttack = true;
                Debug.Log("myWeapon : " + myWeapon);
				break;
			case WeaponType.GAUNTLETS:
				attack_AsyncAnimation(attackCilp[10], false, 1.5f);
				nowAttack = true;
				break;
		}
	}

	// 무기에 따라 다른 콤보 발동
	void comboTwo()
	{
		switch (myWeapon)
		{
			case WeaponType.SWORD:
				attack_AsyncAnimation(attackCilp[1], false, 1.5f);
				nowAttack = true;
				break;
			case WeaponType.AXE:
				attack_AsyncAnimation(attackCilp[4], false, 1.5f);
				nowAttack = true;
				break;
			case WeaponType.DAGGER:
				attack_AsyncAnimation(attackCilp[7], false, 1.5f);
				nowAttack = true;
				break;
			case WeaponType.GAUNTLETS:
				attack_AsyncAnimation(attackCilp[11], false, 1.5f);
				nowAttack = true;
				break;
		}
	}

	// 무기에 따라 다른 콤보 발동
	void comboThree()
	{
		switch (myWeapon)
		{
			case WeaponType.SWORD:
				attack_AsyncAnimation(attackCilp[2], false, 1.5f);
				nowAttack = true;
				break;
			case WeaponType.AXE:
				attack_AsyncAnimation(attackCilp[5], false, 1.5f);
				nowAttack = true;
				break;
			case WeaponType.DAGGER:
				attack_AsyncAnimation(attackCilp[8], false, 1.5f);
				nowAttack = true;
				break;
			case WeaponType.GAUNTLETS:
				attack_AsyncAnimation(attackCilp[12], false, 1.5f);
				nowAttack = true;
				break;
		}
	}

	#endregion

	#region _액션 세팅

	// 애니메이션 재생 메소드
	void action_AsyncAnimation(AnimationReferenceAsset Clip, bool loop, float timeScale)
	{
		// 현재 구르기 애니메이션 출력 중일 때
		if (nowAction)
		{
			nowAttack = false;

			// 애니메이션 진행시간 == 애니메이션 종료시간 <= 애니메이션 상태 검사 후 종료 시 구르기 상태 끝
			if (skeletonAnimation.state.GetCurrent(0).AnimationTime ==
				skeletonAnimation.state.GetCurrent(0).AnimationEnd)
			{
				if (animState == AnimState.GETCARD)
				{
					animState = AnimState.ACTION;
					nowAction = false;
					getCard = true;
				}
				else
				{
					animState = AnimState.ACTION;
					nowAction = false;
				}	
			}
			// 아직 진행중이라면 애니메이션 전환 X
			return;
		}

		// 히트상태일때
		if(Clip == actionCilp[0])
		{
			if(!isStartedCoroutine)
			StartHitFlash();
		}	
		
		// 카드 선택 루프 중 카드를 선택하고 난 후
		if (currentAnimation == "cards_cards-loop" && !getCard)
		{
			skeletonAnimation.state.SetAnimation(0, actionCilp[4], false).TimeScale = 1;
			nowAction = true;
			return;
		}

		// 현재 재생중인 애니메이션이 다시 재생될 때
		if (Clip.name.Equals(currentAnimation))
			// 애니메이션 전환 X
			return;

		// 해당 애니메이션 재생
		skeletonAnimation.state.SetAnimation(0, Clip, loop).TimeScale = timeScale;

		// 현재 애니메이션의 이름을 저장
		currentAnimation = Clip.name;
	}

	// 현재 상태에 따라 애니메이션 재생 전환
	void action_setCurrentAnimation(AnimState state)
	{
		if (getCard)
		{
			// IDLE 애니메이션, 루프 O, 애니메이션 재생 시간 1배속
			action_AsyncAnimation(actionCilp[3], true, 1);
			return;
		}

		// 현재 상태
		switch (state)
		{
			// IDLE
			case AnimState.ACTION:
				// IDLE 애니메이션, 루프 O, 애니메이션 재생 시간 1배속
				action_AsyncAnimation(moveCilp[0], true, 1);
				break;
			case AnimState.HIT:
				// IDLE 애니메이션, 루프 O, 애니메이션 재생 시간 1배속
				action_AsyncAnimation(actionCilp[0], false, 1);
				nowAction = true;
				break;
			case AnimState.DIE:
				// IDLE 애니메이션, 루프 O, 애니메이션 재생 시간 1배속
				action_AsyncAnimation(actionCilp[1], false, 1);
				if (skeletonAnimation.state.GetCurrent(0).AnimationTime ==
					skeletonAnimation.state.GetCurrent(0).AnimationEnd)
					Die = true;
				nowAction = true;
				break;
			case AnimState.GETCARD:
				// IDLE 애니메이션, 루프 O, 애니메이션 재생 시간 1배속
				action_AsyncAnimation(actionCilp[2], false, 1);
				nowAction = true;
				break;
			case AnimState.GETSWORD:
				// IDLE 애니메이션, 루프 O, 애니메이션 재생 시간 1배속
				action_AsyncAnimation(getCilp[0], false, 1);
				nowAction = true;
				break;
			case AnimState.GETAXE:
				// IDLE 애니메이션, 루프 O, 애니메이션 재생 시간 1배속
				action_AsyncAnimation(getCilp[1], false, 1);
				nowAction = true;
				break;
			case AnimState.GETDAGGER:
				// IDLE 애니메이션, 루프 O, 애니메이션 재생 시간 1배속
				action_AsyncAnimation(getCilp[2], false, 1);
				nowAction = true;
				break;
		}
	}

	#endregion

	public void getWeapon(WeaponType type)
	{
		switch (type)
		{
			case WeaponType.SWORD:
				myWeapon = WeaponType.SWORD;
				animState = AnimState.GETSWORD;
				break;
			case WeaponType.AXE:
				myWeapon = WeaponType.AXE;
				animState = AnimState.GETAXE;
				break;
			case WeaponType.DAGGER:
				myWeapon = WeaponType.DAGGER;
				animState = AnimState.GETDAGGER;
				break;
		}
	}


	// ↓ 플레이어 히트 관련 필드와 코루틴
	private WaitForSeconds flashDelay = new WaitForSeconds(0.2f);

	private Material orignMat;
	private float curTime;
	private bool isStartedCoroutine = false;


	/// <summary>
	/// 피격시 깜빡이는 연출을 시작하는 메서드.
	/// 관련 필드를 리셋시켜줍니다.
	/// </summary>
    public  void StartHitFlash()
	{
		curTime = 0;
		isStartedCoroutine = true;
		StartCoroutine(HitFlash());

    }

	/// <summary>
	/// 피격시 깜빡이는 연출의 종료를 실행하는 메서드
	/// 관련 필드를 리셋히켜줍니다.
	/// </summary>
	private void StopHitFlash()
	{
		StopCoroutine(HitFlash());
		isStartedCoroutine= false;
		skeletonAnimation.CustomMaterialOverride
					[skeletonAnimation.skeletonDataAsset.atlasAssets[0].PrimaryMaterial] = orignMat;

    }

	/// <summary>
	/// 0.2초 간격으로 깜빡일거읾,,
	/// </summary>
	/// <returns></returns>
	private IEnumerator HitFlash()
	{
		bool flashBoolean = true;

		while (true)
		{
			curTime += 0.2f;

            flashBoolean = !flashBoolean;

            if (flashBoolean)
			{				
                skeletonAnimation.CustomMaterialOverride
					[skeletonAnimation.skeletonDataAsset.atlasAssets[0].PrimaryMaterial] = ksjCusMat;
            }

			if(!flashBoolean)
			{			
				skeletonAnimation.CustomMaterialOverride
					[skeletonAnimation.skeletonDataAsset.atlasAssets[0].PrimaryMaterial] = orignMat;
            }

			if(curTime>=1.5f)
			{
				StopHitFlash();
				break;
			}

            yield return flashDelay;
		}
	}
}
