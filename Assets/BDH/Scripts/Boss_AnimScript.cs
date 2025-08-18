using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Boss_AnimScript : MonoBehaviour
{
	// 스파인 애니메이션
	SkeletonAnimation skeletonAnimation;

	// 애니매이션 재생을 위한 클립
	public AnimationReferenceAsset[] animCilp;

	// 애니메이션 타입
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

	// 액션 중
	public bool nowAction;
	// 구멍&점프
	public bool outCamera;
	// 사망 상태
	bool Dead;

	// 현재 애니메이션 처리 상태
	public AnimState animState;

	// 현재 진행 중인 애니메이션
	public string currentAnimation;

	private void Start()
	{
		skeletonAnimation = GetComponent<SkeletonAnimation>();
	}

	private void Update()
	{
		_setCurrentAnimation(animState);
	}

	// 애니메이션 재생 메소드
	void _AsyncAnimation(AnimationReferenceAsset Clip, bool loop, float timeScale)
	{
		if (Dead)
			return;

		if (outCamera)
			return;

		// 현재 공격 애니메이션 출력 중일 때
		if (nowAction)
		{
			if (animState == AnimState.GROUND)
			{
				// 애니메이션 진행시간 == 애니메이션 종료시간 <= 애니메이션 상태 검사 후 종료 시 공격 상태 끝
				if (skeletonAnimation.state.GetCurrent(0).AnimationTime > 0.7f)
				{
					animState = AnimState.OUT;
					nowAction = false;
				}	
			}
			else
			{
				// 애니메이션 진행시간 == 애니메이션 종료시간 <= 애니메이션 상태 검사 후 종료 시 공격 상태 끝
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
				// 아직 진행중이라면 애니메이션 전환 X
				return;
			}
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
