using Spine.Unity;
using UnityEngine;

public class Enemy_TurretAnimScript : MonoBehaviour
{
	// ������ �ִϸ��̼�
	SkeletonAnimation skeletonAnimation;
	// �ִϸ��̼� ����� ���� Ŭ��
	public AnimationReferenceAsset[] animCilp;

	public bool find;

	// ���� Action ���� ����
	public bool nowAction;

	// ���� �ִϸ��̼� ó�� ����
	public AnimState animState;

	// ���� ���� ���� �ִϸ��̼�
	public string currentAnimation;

	// �ִϸ��̼� Ÿ��
	public enum AnimState
	{
		IDLE,
		SHOOT,
		DIE,
	}

	float shootTime = 3f;
	float curTime;

	bool Dead;

	private void Start()
	{
		skeletonAnimation = GetComponent<SkeletonAnimation>();
	}

	private void Update()
	{
		if (Dead)
		{
			nowAction = false;
		}

		_setCurrentAnimation(animState);

		animState = AnimState.IDLE;

		if (find)
			curTime += Time.deltaTime;

		if (curTime > shootTime)
		{
			animState = AnimState.SHOOT;
		}
	}

	// �ִϸ��̼� ��� �޼ҵ�
	void _AsyncAnimation(AnimationReferenceAsset Clip, bool loop, float timeScale)
	{
		// ��Ʈ ���϶�
		if (nowAction)
		{
			// �ִϸ��̼� ����ð� == �ִϸ��̼� ����ð� <= �ִϸ��̼� ���� �˻� �� ���� �� Ư�� ���� ��
			if (skeletonAnimation.state.GetCurrent(0).AnimationTime ==
				skeletonAnimation.state.GetCurrent(0).AnimationEnd)
			{
				nowAction = false;
				curTime = 0;
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
			case AnimState.IDLE:
				// IDLE �ִϸ��̼�, ���� O, �ִϸ��̼� ��� �ð� 1���
				_AsyncAnimation(animCilp[0], true, 1);
				break;
			case AnimState.SHOOT:
				// SHOOT �ִϸ��̼�, ���� O, �ִϸ��̼� ��� �ð� 1���
				_AsyncAnimation(animCilp[1], false, 1);
				nowAction = true;
				break;
			case AnimState.DIE:
				// SHOOT �ִϸ��̼�, ���� O, �ִϸ��̼� ��� �ð� 1���
				_AsyncAnimation(animCilp[1], false, 1);
				Dead = true;
				break;
		}
	}
}
