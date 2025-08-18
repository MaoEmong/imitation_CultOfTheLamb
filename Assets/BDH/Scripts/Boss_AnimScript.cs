using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Boss_AnimScript : MonoBehaviour
{
	// ������ �ִϸ��̼�
	SkeletonAnimation skeletonAnimation;

	// �ִϸ��̼� ����� ���� Ŭ��
	public AnimationReferenceAsset[] animCilp;

	// �ִϸ��̼� Ÿ��
	public enum AnimState
	{
		WAIT,
		INTRO,
		IDLE,
		IN,
		OUT,
		JUMP,
		GROUND,
		BITE,
		SUMMON,
		SHOOT,
		HIT,
		DIE,
	}

	// �׼� ��
	public bool nowAction;
	// ����&����
	public bool outCamera;
	// ��� ����
	bool Dead;

	// ���� �ִϸ��̼� ó�� ����
	public AnimState animState;

	// ���� ���� ���� �ִϸ��̼�
	public string currentAnimation;

	private void Start()
	{
		skeletonAnimation = GetComponent<SkeletonAnimation>();
	}

	private void Update()
	{
		_setCurrentAnimation(animState);
	}

	// �ִϸ��̼� ��� �޼ҵ�
	void _AsyncAnimation(AnimationReferenceAsset Clip, bool loop, float timeScale)
	{
		if (Dead)
			return;

		if (outCamera)
			return;

		// ���� ���� �ִϸ��̼� ��� ���� ��
		if (nowAction)
		{
			if (animState == AnimState.GROUND)
			{
				// �ִϸ��̼� ����ð� == �ִϸ��̼� ����ð� <= �ִϸ��̼� ���� �˻� �� ���� �� ���� ���� ��
				if (skeletonAnimation.state.GetCurrent(0).AnimationTime > 0.7f)
				{
					animState = AnimState.OUT;
					nowAction = false;
				}	
			}
			else
			{
				// �ִϸ��̼� ����ð� == �ִϸ��̼� ����ð� <= �ִϸ��̼� ���� �˻� �� ���� �� ���� ���� ��
				if (skeletonAnimation.state.GetCurrent(0).AnimationTime ==
					skeletonAnimation.state.GetCurrent(0).AnimationEnd)
				{
					if (animState == AnimState.IN || animState == AnimState.JUMP)
					{
						outCamera = true;
						nowAction = false;
					}
					else
					{
						animState = AnimState.IDLE;
						nowAction = false;
					}
				}
				// ���� �������̶�� �ִϸ��̼� ��ȯ X
				return;
			}
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
	void _setCurrentAnimation(AnimState state)
	{
		// ���� ����
		switch (state)
		{
			case AnimState.INTRO:
				_AsyncAnimation(animCilp[0], false, 1.5f);
				nowAction = true;
				return;
			case AnimState.IDLE:
				_AsyncAnimation(animCilp[1], true, 1.5f);
				break;
			case AnimState.IN:
				_AsyncAnimation(animCilp[2], false, 1.5f);
				if (skeletonAnimation.state.GetCurrent(0).AnimationTime !=
				skeletonAnimation.state.GetCurrent(0).AnimationEnd)
					nowAction = true;
				break;
			case AnimState.OUT:
				outCamera = false;
				_AsyncAnimation(animCilp[3], false, 0.8f);
				nowAction = true;
				break;
			case AnimState.JUMP:
				_AsyncAnimation(animCilp[4], false, 1.5f);
				if (skeletonAnimation.state.GetCurrent(0).AnimationTime !=
				skeletonAnimation.state.GetCurrent(0).AnimationEnd)
					nowAction = true;
				break;
			case AnimState.GROUND:
				outCamera = false;
				_AsyncAnimation(animCilp[5], false, 1.5f);
				if (skeletonAnimation.state.GetCurrent(0).AnimationTime < 0.7f)
					nowAction = true;
				break;
			case AnimState.BITE:
				_AsyncAnimation(animCilp[6], false, 1f);
				nowAction = true;
				break;
			case AnimState.SUMMON:
				_AsyncAnimation(animCilp[7], false, 1.5f);
				nowAction = true;
				break;
			case AnimState.SHOOT:
				_AsyncAnimation(animCilp[8], false, 1.5f);
				nowAction = true;
				break;
			case AnimState.HIT:
				_AsyncAnimation(animCilp[9], false, 1f);
				nowAction = true;
				break;
			case AnimState.DIE:
				_AsyncAnimation(animCilp[10], false, 1.2f);
				if (skeletonAnimation.state.GetCurrent(0).AnimationTime ==
					skeletonAnimation.state.GetCurrent(0).AnimationEnd)
					Dead = true;
				nowAction = true;
				break;
		}
	}
}
