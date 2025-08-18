using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Enemy_AnimScript;

public class Chest_AnimScript : MonoBehaviour
{
	// ������ �ִϸ��̼�
	SkeletonAnimation skeletonAnimation;
	// �ִϸ��̼� ����� ���� Ŭ��
	public AnimationReferenceAsset[] animCilp;

	// ���� ����� ����
	public bool isHide;

	// ���� �ִϸ��̼� ����
	bool nowAnim;

	// ���� �ִϸ��̼� ó�� ����
	public AnimState animState;

	// ���� ���� ���� �ִϸ��̼�
	string currentAnimation;

	// �ִϸ��̼� Ÿ��
	public enum AnimState
	{
		HIDE,
		REVEAL,
		OPEN,
		ROCK
	}

	void Start()
    {
		skeletonAnimation = GetComponent<SkeletonAnimation>();
	}

	public void startSet(AnimState state)
	{
		animState = state;
	}

    void Update()
    {
		_setCurrentAnimation(animState);
	}

	// �ִϸ��̼� ��� �޼ҵ�
	void _AsyncAnimation(AnimationReferenceAsset Clip, bool loop, float timeScale)
	{
		// ���� ���� �ִϸ��̼� ��� ���� ��(��Ʈ ���̸� �ִϸ��̼� ����)
		if (nowAnim)
		{
			if (skeletonAnimation.state.GetCurrent(0).AnimationTime ==
				skeletonAnimation.state.GetCurrent(0).AnimationEnd)
			{
				if (animState == AnimState.REVEAL)
					animState = AnimState.OPEN;
				nowAnim = false;
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
	}

	// ���� ���¿� ���� �ִϸ��̼� ��� ��ȯ
	void _setCurrentAnimation(AnimState state)
	{
		// ���� ����
		switch (state)
		{
			// IDLE
			case AnimState.HIDE:
				// IDLE �ִϸ��̼�, ���� O, �ִϸ��̼� ��� �ð� 1���
				_AsyncAnimation(animCilp[0], true, 1);
				break;
			case AnimState.REVEAL:
				// REVEAL �ִϸ��̼�, ���� X, �ִϸ��̼� ��� �ð� 1���
				_AsyncAnimation(animCilp[1], false, 1);
				if (skeletonAnimation.state.GetCurrent(0).AnimationTime !=
					skeletonAnimation.state.GetCurrent(0).AnimationEnd)
					nowAnim = true;
				break;
			case AnimState.OPEN:
				// OPEN �ִϸ��̼�, ���� X, �ִϸ��̼� ��� �ð� 1���
				_AsyncAnimation(animCilp[2], false, 1);
				nowAnim = true;
				break;
			case AnimState.ROCK:
				// ROCK �ִϸ��̼�, ���� O, �ִϸ��̼� ��� �ð� 1���
				_AsyncAnimation(animCilp[3], true, 1);
				break;
		}
	}
}
