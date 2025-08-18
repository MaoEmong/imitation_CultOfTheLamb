using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_HumanAnimScript : MonoBehaviour
{
	// ������ �ִϸ��̼�
	SkeletonAnimation skeletonAnimation;
	// �ִϸ��̼� ����� ���� Ŭ��
	public AnimationReferenceAsset[] moveCilp;
	public AnimationReferenceAsset[] attackCilp;

	public enum HumanType
	{
		Scamp1,
		Scamp2,
		Archer,
		Elite
	}

	public HumanType type;

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

	// �ִϸ��̼� Ÿ��
	public enum AnimState
	{
		IDLE,
		MOVE,
		CHARGE,
		IMPACT,
		HIT,
		DIE,
	}

	private void Start()
	{
		skeletonAnimation = GetComponent<SkeletonAnimation>();
	}

	private void Update()
	{
		if (Dead)
		{
			nowAction = false;
			nowCharge = false;
			nowImpact = false;
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
		// ���� ������ �ִϸ��̼� ��� ���� ��
		if (animState != AnimState.HIT && (nowCharge || nowImpact))
		{
			// �ִϸ��̼� ����ð� == �ִϸ��̼� ����ð� <= �ִϸ��̼� ���� �˻� �� ���� �� Ư�� ���� ��
			if (skeletonAnimation.state.GetCurrent(0).AnimationTime ==
				skeletonAnimation.state.GetCurrent(0).AnimationEnd)
			{
				nowCharge = false;
				nowImpact = false;
			}
			// ���� �������̶�� �ִϸ��̼� ��ȯ X
			return;
		}
		if (nowAction)
		{
			nowCharge = false;
			nowImpact = false;

			// �ִϸ��̼� ����ð� == �ִϸ��̼� ����ð� <= �ִϸ��̼� ���� �˻� �� ���� �� Ư�� ���� ��
			if (skeletonAnimation.state.GetCurrent(0).AnimationTime ==
				skeletonAnimation.state.GetCurrent(0).AnimationEnd)
			{
				nowAction = false;
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

		if (type == HumanType.Archer)
		{
			if (currentAnimation == "archer-attack-impact")
				nowImpact = true;
		}
		else
		{
			if (currentAnimation == "grunt-attack-impact")
				nowImpact = true;
		}

		if (currentAnimation == "hurt-front")
			nowAction = true;
	}

	// ���� ���¿� ���� �ִϸ��̼� ��� ��ȯ
	void _setCurrentAnimation(AnimState state)
	{
		// ���� ����
		switch (state)
		{
			// IDLE
			case AnimState.IDLE:
				// IDLE �ִϸ��̼�, ���� O, �ִϸ��̼� ��� �ð� 1���
				_AsyncAnimation(moveCilp[0], true, 1);
				break;
			case AnimState.MOVE:
				// �̵� ���⿡ ���� �ٸ� �ִϸ��̼� ���
				moveDir();
				break;
			case AnimState.CHARGE:
				// CHARGE �ִϸ��̼�, ���� X, �ִϸ��̼� ��� �ð� 1���
				if (type == HumanType.Archer)
					_AsyncAnimation(attackCilp[2], false, 1);
				else
					_AsyncAnimation(attackCilp[0], false, 1);

				if (skeletonAnimation.state.GetCurrent(0).AnimationTime !=
				skeletonAnimation.state.GetCurrent(0).AnimationEnd)
					nowCharge = true;
				break;
			case AnimState.IMPACT:
				// IMPACT �ִϸ��̼�, ���� X, �ִϸ��̼� ��� �ð� 1���
				if (type == HumanType.Archer)
					_AsyncAnimation(attackCilp[3], false, 1);
				else
					_AsyncAnimation(attackCilp[1], false, 1);
				break;
			case AnimState.HIT:
				// HIT �ִϸ��̼�, ���� X, �ִϸ��̼� ��� �ð� 1���
				_AsyncAnimation(moveCilp[3], false, 1);
				break;
			case AnimState.DIE:
				// DIE �ִϸ��̼�, ���� X, �ִϸ��̼� ��� �ð� 1���
				_AsyncAnimation(moveCilp[4], false, 1);
				Dead = true;
				break;
		}
	}

	// MOVE �ִϸ��̼� ��� �޼ҵ�
	void moveDir()
	{
		float v = 0;
		// �� �̵�
		if (v > 0)
			// Move �ִϸ��̼�, ���� O, �ִϸ��̼� ��� �ð� 1���
			_AsyncAnimation(moveCilp[2], true, 1);
		// �⺻ �̵�
		else
			// Move �ִϸ��̼�, ���� O, �ִϸ��̼� ��� �ð� 1���
			_AsyncAnimation(moveCilp[1], true, 1);
	}
}
