using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Enemy_AnimScript;

public class Chest_AnimScript : MonoBehaviour
{
	// 스파인 애니메이션
	SkeletonAnimation skeletonAnimation;
	// 애니매이션 재생을 위한 클립
	public AnimationReferenceAsset[] animCilp;

	// 현재 비등장 상태
	public bool isHide;

	// 현재 애니메이션 상태
	bool nowAnim;

	// 현재 애니메이션 처리 상태
	public AnimState animState;

	// 현재 진행 중인 애니메이션
	string currentAnimation;

	// 애니메이션 타입
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

	// 애니메이션 재생 메소드
	void _AsyncAnimation(AnimationReferenceAsset Clip, bool loop, float timeScale)
	{
		// 현재 공격 애니메이션 출력 중일 때(히트 중이면 애니메이션 변경)
		if (nowAnim)
		{
			if (skeletonAnimation.state.GetCurrent(0).AnimationTime ==
				skeletonAnimation.state.GetCurrent(0).AnimationEnd)
			{
				if (animState == AnimState.REVEAL)
					animState = AnimState.OPEN;
				nowAnim = false;
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
			case AnimState.HIDE:
				// IDLE 애니메이션, 루프 O, 애니메이션 재생 시간 1배속
				_AsyncAnimation(animCilp[0], true, 1);
				break;
			case AnimState.REVEAL:
				// REVEAL 애니메이션, 루프 X, 애니메이션 재생 시간 1배속
				_AsyncAnimation(animCilp[1], false, 1);
				if (skeletonAnimation.state.GetCurrent(0).AnimationTime !=
					skeletonAnimation.state.GetCurrent(0).AnimationEnd)
					nowAnim = true;
				break;
			case AnimState.OPEN:
				// OPEN 애니메이션, 루프 X, 애니메이션 재생 시간 1배속
				_AsyncAnimation(animCilp[2], false, 1);
				nowAnim = true;
				break;
			case AnimState.ROCK:
				// ROCK 애니메이션, 루프 O, 애니메이션 재생 시간 1배속
				_AsyncAnimation(animCilp[3], true, 1);
				break;
		}
	}
}
