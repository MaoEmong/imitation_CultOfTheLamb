using Spine.Unity;
using UnityEngine;

public class Enemy_TurretAnimScript : MonoBehaviour
{
	// 스파인 애니메이션
	SkeletonAnimation skeletonAnimation;
	// 애니매이션 재생을 위한 클립
	public AnimationReferenceAsset[] animCilp;

	public bool find;

	// 현재 Action 상태 여부
	public bool nowAction;

	// 현재 애니메이션 처리 상태
	public AnimState animState;

	// 현재 진행 중인 애니메이션
	public string currentAnimation;

	// 애니메이션 타입
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

	// 애니메이션 재생 메소드
	void _AsyncAnimation(AnimationReferenceAsset Clip, bool loop, float timeScale)
	{
		// 히트 중일때
		if (nowAction)
		{
			// 애니메이션 진행시간 == 애니메이션 종료시간 <= 애니메이션 상태 검사 후 종료 시 특정 상태 끝
			if (skeletonAnimation.state.GetCurrent(0).AnimationTime ==
				skeletonAnimation.state.GetCurrent(0).AnimationEnd)
			{
				nowAction = false;
				curTime = 0;
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
	}

	// 현재 상태에 따라 애니메이션 재생 전환
	void _setCurrentAnimation(AnimState state)
	{
		// 현재 상태
		switch (state)
		{
			// IDLE
			case AnimState.IDLE:
				// IDLE 애니메이션, 루프 O, 애니메이션 재생 시간 1배속
				_AsyncAnimation(animCilp[0], true, 1);
				break;
			case AnimState.SHOOT:
				// SHOOT 애니메이션, 루프 O, 애니메이션 재생 시간 1배속
				_AsyncAnimation(animCilp[1], false, 1);
				nowAction = true;
				break;
			case AnimState.DIE:
				// SHOOT 애니메이션, 루프 O, 애니메이션 재생 시간 1배속
				_AsyncAnimation(animCilp[1], false, 1);
				Dead = true;
				break;
		}
	}
}
