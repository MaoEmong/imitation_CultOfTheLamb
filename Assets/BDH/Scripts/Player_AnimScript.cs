using System.Collections;
using Spine.Unity;
using UnityEditor.Rendering;
using UnityEngine;

public class Player_AnimScript : MonoBehaviour
{

	//
	[SerializeField]
	private Material ksjCusMat;

	// ������ �ִϸ��̼�
	SkeletonAnimation skeletonAnimation;

	// �÷��̾� ���� ���� Ȯ��
	PlayerMove move;

	// �ִϸ��̼� ����� ���� Ŭ��
	public AnimationReferenceAsset[] moveCilp;
	public AnimationReferenceAsset[] attackCilp;
	public AnimationReferenceAsset[] actionCilp;
	public AnimationReferenceAsset[] getCilp;

	// ���� Attack ���� ����
	public bool nowAttack;
	// ���� Action ���� ����
	public bool nowAction;

	bool Die;

	// �ִϸ��̼� Ÿ��
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

	#region _�̵����� ����

	// ������ ����
	bool nowRoll;

	// �̵� ����
	float h;
	float v;

	#endregion

	#region _�������� ����

	// �޺� Ÿ��
	public float combotime;
	// �޺� ����
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

	#region _�׼����� ����

	public bool getCard;

	#endregion

	// ���� �ִϸ��̼� ó�� ����
	public AnimState animState;

	// ���� ���� ���� �ִϸ��̼�
	string currentAnimation;

	private void Start()
	{
		// �ʿ� ������Ʈ�� ��������
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

		// �ִϸ��̼� ���� �˻�
		if (!nowAction && pm.Menu.activeSelf != true && !move.isdash)
			AttackAnim();

		// �ִϸ��̼� ���� �˻�
		if (!nowAction && !nowAttack && !getCard)
			MoveAnim();
	}

	void MoveAnim()
	{
		// ���� �̵�Ű�� �Է� ���¸� float������ ����޴´�
		h = Input.GetAxisRaw("Horizontal");
		v = Input.GetAxisRaw("Vertical");

		// �̵� ���� �ƴ� ��
		if (h == 0 && v == 0)
		{
			// ���� �ִϸ��̼� ���¸� IDLE
			animState = AnimState.IDLE;
		}
		// ������(ȸ��)�� ������� ��
		else if (Input.GetKeyDown(KeyCode.Space) && (h != 0 || v != 0))
		{
			// ���� �ִϸ��̼� ���¸� ROLL
			animState = AnimState.ROLL;
		}
		// �̵� ���� ��
		else if (!Input.GetKeyDown(KeyCode.Space) && (h != 0 || v != 0))
		{
			// ���� �ִϸ��̼� ���¸� MOVE
			animState = AnimState.MOVE;
		}

		move_setCurrentAnimation(animState);
	}

	void AttackAnim()
	{
        var pm = ksjPlayerManager.Instance;
        myWeapon = pm.CurPlayerWeapon;

        // �޺� 1 ����
        if (nowCombo == 0 && Input.GetMouseButtonDown(0) && combotime < 0.5f)
		{
			nowCombo = 1;
			animState = AnimState.COMBO1;
			transform.parent.GetComponent<ksjPlayerAttackScan>().AttackRay();           
        }
		// �޺� 2 ����
		else if (nowCombo == 1 && Input.GetMouseButtonDown(0) && combotime < 0.5f)
		{
			nowCombo = 2;
			animState = AnimState.COMBO2;
			transform.parent.GetComponent<ksjPlayerAttackScan>().AttackRay();
		}
		// �޺� 3 ����
		else if (nowCombo == 2 && Input.GetMouseButtonDown(0) && combotime < 0.5f)
		{
			nowCombo = 3;
			animState = AnimState.COMBO3;
			transform.parent.GetComponent<ksjPlayerAttackScan>().AttackRay();
		}
		// �޺� 4 ����(������ ����)
		else if (nowCombo == 3 && Input.GetMouseButtonDown(0) && myWeapon == WeaponType.DAGGER
			&& combotime < 0.5f)
		{
			nowCombo = 4;
			animState = AnimState.COMBO4;
			transform.parent.GetComponent<ksjPlayerAttackScan>().AttackRay();
		}

		/*

		// ù ���� �� ���콺 ��Ŭ�� ���� ��
		if (nowCombo == 0 && Input.GetMouseButton(0))
		{
			// �޺� ���� 0
			nowCombo = 0;
			// ����
			animState = AnimState.CHARGE;
		}

		// ���� �� ���콺 ��Ŭ���� ����
		if (animState == AnimState.CHARGE && Input.GetMouseButtonUp(0))
		{
			// ����Ʈ
			animState = AnimState.IMPACT;
		}

		*/

		attack_setCurrentAnimation(animState);
    }

	void ActionAnim()
	{
		action_setCurrentAnimation(animState);
	}

	#region _���� ����

	// �ִϸ��̼� ��� �޼ҵ�
	void move_AsyncAnimation(AnimationReferenceAsset Clip, bool loop, float timeScale)
	{
		// ���� ������ �ִϸ��̼� ��� ���� ��
		if (nowRoll)
		{
			// ���� ������� �ִϸ��̼� Ŭ���� �����Ⱑ �ƴ� ��
			if (currentAnimation == "idle")
				nowRoll = false;

			// �ִϸ��̼� ����ð� == �ִϸ��̼� ����ð� <= �ִϸ��̼� ���� �˻� �� ���� �� ������ ���� ��
			if (skeletonAnimation.state.GetCurrent(0).AnimationTime ==
				skeletonAnimation.state.GetCurrent(0).AnimationEnd)
			{
				nowRoll = false;
			}
			// ���� �������̶�� �ִϸ��̼� ��ȯ X
			return;
		}

		combotime += Time.deltaTime;

		if (combotime > 0.5f)
		{
			combotime = 0;
			nowCombo = 0;
		}	

		// ���� ������� �ִϸ��̼��� �ٽ� ����� ��
		if (Clip.name.Equals(currentAnimation))
			// �ִϸ��̼� ��ȯ X
			return;

		// �ش� �ִϸ��̼� ���
		skeletonAnimation.state.SetAnimation(0, Clip, loop).TimeScale = timeScale;

		// ���� �ִϸ��̼��� �̸��� ����
		currentAnimation = Clip.name;
	}

	// ���� ���¿� ���� �ִϸ��̼� ��� ��ȯ
	void move_setCurrentAnimation(AnimState state)
	{
		// ���� ����
		switch (state)
		{
			// IDLE
			case AnimState.IDLE:
				// IDLE �ִϸ��̼�, ���� O, �ִϸ��̼� ��� �ð� 1���
				move_AsyncAnimation(moveCilp[0], true, 1);
				break;
			case AnimState.MOVE:
				// �̵� ���⿡ ���� �ٸ� �ִϸ��̼� ���
				moveDir();
				break;
			case AnimState.ROLL:
				// ������ ���⿡ ���� �ٸ� �ִϸ��̼� ���
				rollDir();
				nowRoll = true;
				break;
		}
	}

	// MOVE �ִϸ��̼� ��� �޼ҵ�
	void moveDir()
	{
		if (h != 0)
		{
			// �밢 �Ʒ� �̵�
			if (v < 0)
				// Move �ִϸ��̼�, ���� O, �ִϸ��̼� ��� �ð� 1���
				move_AsyncAnimation(moveCilp[4], true, 1);
			// �밢 �� �̵�
			else if (v > 0)
				// Move �ִϸ��̼�, ���� O, �ִϸ��̼� ��� �ð� 1���
				move_AsyncAnimation(moveCilp[2], true, 1);
			// �¿� �Ʒ� �̵�
			else if (v == 0)
				// Move �ִϸ��̼�, ���� O, �ִϸ��̼� ��� �ð� 1���
				move_AsyncAnimation(moveCilp[3], true, 1);
		}
		else
		{
			// �Ʒ� �̵�
			if (v < 0)
				// Move �ִϸ��̼�, ���� O, �ִϸ��̼� ��� �ð� 1���
				move_AsyncAnimation(moveCilp[5], true, 1);
			// �� �̵�
			else if (v > 0)
				// Move �ִϸ��̼�, ���� O, �ִϸ��̼� ��� �ð� 1���
				move_AsyncAnimation(moveCilp[1], true, 1);
		}
	}

	// ROLL �ִϸ��̼� ��� �޼ҵ�
	void rollDir()
	{
		// �Ʒ� ������
		if (v < 0)
			// ROLL �ִϸ��̼�, ���� X, �ִϸ��̼� ��� �ð� 1���
			move_AsyncAnimation(moveCilp[8], false, 1);
		// �� ������
		else if (v > 0)
			// ROLL �ִϸ��̼�, ���� X, �ִϸ��̼� ��� �ð� 1���
			move_AsyncAnimation(moveCilp[6], false, 1);
		// �� ������
		else
			// ROLL �ִϸ��̼�, ���� X, �ִϸ��̼� ��� �ð� 1���
			move_AsyncAnimation(moveCilp[7], false, 1);
	}

	#endregion

	#region _���� ����

	// �ִϸ��̼� ��� �޼ҵ�
	void attack_AsyncAnimation(AnimationReferenceAsset Clip, bool loop, float timeScale)
	{
		// ���� ���� �ִϸ��̼� ��� ���� ��
		if (nowAttack)
		{
			nowAction = false;

			combotime = 1;

            // �ִϸ��̼� ����ð� == �ִϸ��̼� ����ð� <= �ִϸ��̼� ���� �˻� �� ���� �� ���� ���� ��
            if (skeletonAnimation.state.GetCurrent(0).AnimationTime ==
				skeletonAnimation.state.GetCurrent(0).AnimationEnd)
			{
				animState = AnimState.ATTACK;
				nowAttack = false;
				combotime = 0;
			}
			// ���� �������̶�� �ִϸ��̼� ��ȯ X
			return;
		}

		// ���� ������� �ִϸ��̼��� �ٽ� ����� ��
		if (Clip.name.Equals(currentAnimation))
			// �ִϸ��̼� ��ȯ X
			return;

        // �ش� �ִϸ��̼� ���
        skeletonAnimation.state.SetAnimation(0, Clip, loop).TimeScale = timeScale;
	
		// ���� �ִϸ��̼��� �̸��� ����
		currentAnimation = Clip.name;

		if (myWeapon != WeaponType.DAGGER && nowCombo > 2)
			nowCombo = 0;
		else if (myWeapon == WeaponType.DAGGER && nowCombo > 3)
			nowCombo = 0;
	}

	// ���� ���¿� ���� �ִϸ��̼� ��� ��ȯ
	void attack_setCurrentAnimation(AnimState state)
	{
		// ���� ����
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

	// ���⿡ ���� �ٸ� �޺� �ߵ�
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

	// ���⿡ ���� �ٸ� �޺� �ߵ�
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

	// ���⿡ ���� �ٸ� �޺� �ߵ�
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

	#region _�׼� ����

	// �ִϸ��̼� ��� �޼ҵ�
	void action_AsyncAnimation(AnimationReferenceAsset Clip, bool loop, float timeScale)
	{
		// ���� ������ �ִϸ��̼� ��� ���� ��
		if (nowAction)
		{
			nowAttack = false;

			// �ִϸ��̼� ����ð� == �ִϸ��̼� ����ð� <= �ִϸ��̼� ���� �˻� �� ���� �� ������ ���� ��
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
			// ���� �������̶�� �ִϸ��̼� ��ȯ X
			return;
		}

		// ��Ʈ�����϶�
		if(Clip == actionCilp[0])
		{
			if(!isStartedCoroutine)
			StartHitFlash();
		}	
		
		// ī�� ���� ���� �� ī�带 �����ϰ� �� ��
		if (currentAnimation == "cards_cards-loop" && !getCard)
		{
			skeletonAnimation.state.SetAnimation(0, actionCilp[4], false).TimeScale = 1;
			nowAction = true;
			return;
		}

		// ���� ������� �ִϸ��̼��� �ٽ� ����� ��
		if (Clip.name.Equals(currentAnimation))
			// �ִϸ��̼� ��ȯ X
			return;

		// �ش� �ִϸ��̼� ���
		skeletonAnimation.state.SetAnimation(0, Clip, loop).TimeScale = timeScale;

		// ���� �ִϸ��̼��� �̸��� ����
		currentAnimation = Clip.name;
	}

	// ���� ���¿� ���� �ִϸ��̼� ��� ��ȯ
	void action_setCurrentAnimation(AnimState state)
	{
		if (getCard)
		{
			// IDLE �ִϸ��̼�, ���� O, �ִϸ��̼� ��� �ð� 1���
			action_AsyncAnimation(actionCilp[3], true, 1);
			return;
		}

		// ���� ����
		switch (state)
		{
			// IDLE
			case AnimState.ACTION:
				// IDLE �ִϸ��̼�, ���� O, �ִϸ��̼� ��� �ð� 1���
				action_AsyncAnimation(moveCilp[0], true, 1);
				break;
			case AnimState.HIT:
				// IDLE �ִϸ��̼�, ���� O, �ִϸ��̼� ��� �ð� 1���
				action_AsyncAnimation(actionCilp[0], false, 1);
				nowAction = true;
				break;
			case AnimState.DIE:
				// IDLE �ִϸ��̼�, ���� O, �ִϸ��̼� ��� �ð� 1���
				action_AsyncAnimation(actionCilp[1], false, 1);
				if (skeletonAnimation.state.GetCurrent(0).AnimationTime ==
					skeletonAnimation.state.GetCurrent(0).AnimationEnd)
					Die = true;
				nowAction = true;
				break;
			case AnimState.GETCARD:
				// IDLE �ִϸ��̼�, ���� O, �ִϸ��̼� ��� �ð� 1���
				action_AsyncAnimation(actionCilp[2], false, 1);
				nowAction = true;
				break;
			case AnimState.GETSWORD:
				// IDLE �ִϸ��̼�, ���� O, �ִϸ��̼� ��� �ð� 1���
				action_AsyncAnimation(getCilp[0], false, 1);
				nowAction = true;
				break;
			case AnimState.GETAXE:
				// IDLE �ִϸ��̼�, ���� O, �ִϸ��̼� ��� �ð� 1���
				action_AsyncAnimation(getCilp[1], false, 1);
				nowAction = true;
				break;
			case AnimState.GETDAGGER:
				// IDLE �ִϸ��̼�, ���� O, �ִϸ��̼� ��� �ð� 1���
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


	// �� �÷��̾� ��Ʈ ���� �ʵ�� �ڷ�ƾ
	private WaitForSeconds flashDelay = new WaitForSeconds(0.2f);

	private Material orignMat;
	private float curTime;
	private bool isStartedCoroutine = false;


	/// <summary>
	/// �ǰݽ� �����̴� ������ �����ϴ� �޼���.
	/// ���� �ʵ带 ���½����ݴϴ�.
	/// </summary>
    public  void StartHitFlash()
	{
		curTime = 0;
		isStartedCoroutine = true;
		StartCoroutine(HitFlash());

    }

	/// <summary>
	/// �ǰݽ� �����̴� ������ ���Ḧ �����ϴ� �޼���
	/// ���� �ʵ带 ���������ݴϴ�.
	/// </summary>
	private void StopHitFlash()
	{
		StopCoroutine(HitFlash());
		isStartedCoroutine= false;
		skeletonAnimation.CustomMaterialOverride
					[skeletonAnimation.skeletonDataAsset.atlasAssets[0].PrimaryMaterial] = orignMat;

    }

	/// <summary>
	/// 0.2�� �������� �����ϰ���,,
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
