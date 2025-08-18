using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_AnimScript : MonoBehaviour
{
	// ������ �ִϸ��̼�
	SkeletonAnimation skeletonAnimation;
	// �ִϸ��̼� ����� ���� Ŭ��
	public AnimationReferenceAsset[] moveCilp;
	public AnimationReferenceAsset[] attackCilp;

	// ���� Ÿ��
	public enum MonsterType
	{
		Bat,
		Worm,
	}

	// ������ ���� Ÿ��
	public MonsterType type;

	// ���� Charge ���� ����
	public bool nowCharge;
	// ���� Impact ���� ����
	bool nowImpact;
	// ���� Action ���� ����
	public bool nowAction;

	// ���� �÷��̾� ���� ����
	public bool Find;
	// ���� ��� ����
	public bool Dead;

	// ���� �ִϸ��̼� ó�� ����
	public AnimState animState;

	// ���� ���� ���� �ִϸ��̼�
	public string currentAnimation;

	Vector3 upPos = new Vector3(0, 0.5f, 0);
	Vector3 downPos = new Vector3(0, -0.5f, 0);

	bool up;

	// �ִϸ��̼� Ÿ��
	public enum AnimState
	{
		Bone,
		IDLE,
		MOVE,
		CHARGE,
		IMPACT,
		HIT,
		DIE,
	}

	// ���� ��Ʈ Ÿ��
	float batHit;
	// ���� ��¡ Ÿ��
	public float wormCharge;

	private void Start()
	{
		skeletonAnimation = GetComponent<SkeletonAnimation>();
		wormCharge = -1;
	}

	private void Update()
	{
		if (Dead)
		{
			nowAction = false;
			nowCharge = false;
			nowImpact = false;
		}

		if (type == MonsterType.Bat && animState == AnimState.IDLE)
		{
			if (up)
			{
				transform.position = Vector3.Lerp(transform.position, transform.position + upPos, 10 * Time.deltaTime);
				if (transform.position.y > 0.5f)
					up = false;
			}
			else
			{
				transform.position = Vector3.Lerp(transform.position, transform.position + downPos, 10 * Time.deltaTime);
				if (transform.position.y < -0.5f)
					up = true;
			}
			
		}

		_setCurrentAnimation(animState);

		if (animState == AnimState.CHARGE && !nowCharge)
		{
			animState = AnimState.IMPACT;
			return;
		}

		if (!Find)
			animState = AnimState.IDLE;
		else
		{
			if (!nowCharge && !nowImpact && !nowAction)
				animState = AnimState.MOVE;
		}
	}

	// �ִϸ��̼� ��� �޼ҵ�
	void _AsyncAnimation(AnimationReferenceAsset Clip, bool loop, float timeScale)
	{
		// ���� ���� �ִϸ��̼� ��� ���� ��(��Ʈ ���̸� �ִϸ��̼� ����)
		if (animState != AnimState.HIT && (nowCharge || nowImpact))
		{
			if (type == MonsterType.Worm)
			{
				// ��¡ Ÿ�� ����
				wormCharge += Time.deltaTime;

				// �ִϸ��̼� ����ð� > �ִϸ��̼� ����ð� <= �ִϸ��̼� ���� �˻� �� ���� �� Ư�� ���� ��
				if (wormCharge > 0.5f)
					nowCharge = false;

				if (skeletonAnimation.state.GetCurrent(0).AnimationTime ==
					skeletonAnimation.state.GetCurrent(0).AnimationEnd)
					nowImpact = false;
			}
			else
			{
				// �ִϸ��̼� ����ð� == �ִϸ��̼� ����ð� <= �ִϸ��̼� ���� �˻� �� ���� �� Ư�� ���� ��
				if (skeletonAnimation.state.GetCurrent(0).AnimationTime ==
					skeletonAnimation.state.GetCurrent(0).AnimationEnd)
				{
					nowCharge = false;
					nowImpact = false;
				}
			}
			// ���� �������̶�� �ִϸ��̼� ��ȯ X
			return;
		}
		// ��Ʈ ���϶�
		if (nowAction)
		{
			// ������ ��
			if (type == MonsterType.Bat)
			{
				nowCharge = false;
				nowImpact = false;

				// ��Ʈ Ÿ�� ����
				batHit += Time.deltaTime;

				// �����ð� ���� ��Ʈ ����
				if (batHit > 0.5f)
				{
					nowAction = false;
				}
			}
			// ������ ��
			else
			{
				// ���� �ִϸ��̼� ����
				nowCharge = false;
				nowImpact = false;

				// �ִϸ��̼� ����ð� == �ִϸ��̼� ����ð� <= �ִϸ��̼� ���� �˻� �� ���� �� Ư�� ���� ��
				if (skeletonAnimation.state.GetCurrent(0).AnimationTime ==
					skeletonAnimation.state.GetCurrent(0).AnimationEnd)
				{
					nowAction = false;
				}
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

		// ���Ͱ� ������ ��
		if (type == MonsterType.Bat)
		{
			// ���� ���� �ִϸ��̼��϶� ����ó��
			if (currentAnimation == "attack")
				nowImpact = true;

			// ���� ��Ʈ �ִϸ��̼��϶� ����ó��
			if (currentAnimation == "hit")
			{
				nowAction = true;
				batHit = 0;
			}
		}
		// ���Ͱ� ������ ��
		else if (type == MonsterType.Worm)
		{
			// ���� ���� �ִϸ��̼��϶� ����ó��
			if (currentAnimation == "attack-impact")
			{
				nowImpact = true;
				wormCharge = -1;
			}

			// ���� ��Ʈ �ִϸ��̼��϶� ����ó��
			if (currentAnimation == "land")
				nowAction = true;
		}

	}

	// ���� ���¿� ���� �ִϸ��̼� ��� ��ȯ
	void _setCurrentAnimation(AnimState state)
	{
		// ���� ����
		switch (state)
		{
			// Bone
			case AnimState.Bone:
				// IDLE �ִϸ��̼�, ���� O, �ִϸ��̼� ��� �ð� 1���
				_AsyncAnimation(moveCilp[4], false, 1);
				nowAction = true;
				break;
			// IDLE
			case AnimState.IDLE:
				// IDLE �ִϸ��̼�, ���� O, �ִϸ��̼� ��� �ð� 1���
				_AsyncAnimation(moveCilp[0], true, 1);
				break;
			case AnimState.MOVE:
				// �̵� ���⿡ ���� �ٸ� �ִϸ��̼� ���
				Move();
				break;
			case AnimState.CHARGE:
				// CHARGE �ִϸ��̼�, ���� X, �ִϸ��̼� ��� �ð� 1���
				if (type == MonsterType.Worm)
				{
					// ������ �� ��¡ �ð� Ư���Ͽ� ���������� ��¡ Ÿ�� ����
					_AsyncAnimation(attackCilp[0], true, 1);
					if (wormCharge < 0)
					{
						wormCharge = 0;
						nowCharge = true;
					}
				}
				else
				{
					_AsyncAnimation(attackCilp[0], false, 1);
					if (skeletonAnimation.state.GetCurrent(0).AnimationTime !=
						skeletonAnimation.state.GetCurrent(0).AnimationEnd)
						nowCharge = true;
				}
				break;
			case AnimState.IMPACT:
				// IMPACT �ִϸ��̼�, ���� X, �ִϸ��̼� ��� �ð� 1���
				_AsyncAnimation(attackCilp[1], false, 1);
				break;
			case AnimState.HIT:
				// HIT �ִϸ��̼�, ���� X, �ִϸ��̼� ��� �ð� 1���
				if (type == MonsterType.Worm)
					_AsyncAnimation(moveCilp[2], false, 1.5f);
				else
					_AsyncAnimation(moveCilp[2], false, 1);

				nowAction = true;
				break;
			case AnimState.DIE:
				// DIE �ִϸ��̼�, ���� X, �ִϸ��̼� ��� �ð� 1���
				_AsyncAnimation(moveCilp[3], false, 1);
				nowAction = true;
				Dead = true;
				break;
		}
	}

	// MOVE �ִϸ��̼� ��� �޼ҵ�
	void Move()
	{
		// ���� �̵�
		if (type == MonsterType.Bat)
			// Move �ִϸ��̼�, ���� O, �ִϸ��̼� ��� �ð� 1���
			_AsyncAnimation(moveCilp[0], true, 1);
		// ���� �̵�
		else
			// Move �ִϸ��̼�, ���� O, �ִϸ��̼� ��� �ð� 1���
			_AsyncAnimation(moveCilp[1], true, 1);
	}
}
