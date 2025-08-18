using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_HumanAnimScript : MonoBehaviour
{
	// 스파인 애니메이션
	SkeletonAnimation skeletonAnimation;
	// 애니매이션 재생을 위한 클립
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

	// 현재 Charge 상태 여부
	public bool nowCharge;
	// 현재 Impact 상태 여부
	bool nowImpact;
	// 현재 Action 상태 여부
	public bool nowAction;

	// 몬스터 플레이어 인지 상태
	public bool Find;
	// 몬스터 사망 상태
	public bool Dead;

	// 현재 애니메이션 처리 상태
	public AnimState animState;

	// 현재 진행 중인 애니메이션
	public string currentAnimation;

	// 애니메이션 타입
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

	// 애니메이션 재생 메소드
	void _AsyncAnimation(AnimationReferenceAsset Clip, bool loop, float timeScale)
	{
		// 현재 구르기 애니메이션 출력 중일 때
		if (animState != AnimState.HIT && (nowCharge || nowImpact))
		{
			// 애니메이션 진행시간 == 애니메이션 종료시간 <= 애니메이션 상태 검사 후 종료 시 특정 상태 끝
			if (skeletonAnimation.state.GetCurrent(0).AnimationTime ==
				skeletonAnimation.state.GetCurrent(0).AnimationEnd)
			{
				nowCharge = false;
				nowImpact = false;
			}
			// 아직 진행중이라면 애니메이션 전환 X
			return;
		}
		if (nowAction)
		{
			nowCharge = false;
			nowImpact = false;

			// 애니메이션 진행시간 == 애니메이션 종료시간 <= 애니메이션 상태 검사 후 종료 시 특정 상태 끝
			if (skeletonAnimation.state.GetCurrent(0).AnimationTime ==
				skeletonAnimation.state.GetCurrent(0).AnimationEnd)
			{
				nowAction = false;
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

	// 현재 상태에 따라 애니메이션 재생 전환
	void _setCurrentAnimation(AnimState state)
	{
		// 현재 상태
		switch (state)
		{
			// IDLE
			case AnimState.IDLE:
				// IDLE 애니메이션, 루프 O, 애니메이션 재생 시간 1배속
				_AsyncAnimation(moveCilp[0], true, 1);
				break;
			case AnimState.MOVE:
				// 이동 방향에 따라 다른 애니메이션 출력
				moveDir();
				break;
			case AnimState.CHARGE:
				// CHARGE 애니메이션, 루프 X, 애니메이션 재생 시간 1배속
				if (type == HumanType.Archer)
					_AsyncAnimation(attackCilp[2], false, 1);
				else
					_AsyncAnimation(attackCilp[0], false, 1);

				if (skeletonAnimation.state.GetCurrent(0).AnimationTime !=
				skeletonAnimation.state.GetCurrent(0).AnimationEnd)
					nowCharge = true;
				break;
			case AnimState.IMPACT:
				// IMPACT 애니메이션, 루프 X, 애니메이션 재생 시간 1배속
				if (type == HumanType.Archer)
					_AsyncAnimation(attackCilp[3], false, 1);
				else
					_AsyncAnimation(attackCilp[1], false, 1);
				break;
			case AnimState.HIT:
				// HIT 애니메이션, 루프 X, 애니메이션 재생 시간 1배속
				_AsyncAnimation(moveCilp[3], false, 1);
				break;
			case AnimState.DIE:
				// DIE 애니메이션, 루프 X, 애니메이션 재생 시간 1배속
				_AsyncAnimation(moveCilp[4], false, 1);
				Dead = true;
				break;
		}
	}

	// MOVE 애니메이션 출력 메소드
	void moveDir()
	{
		float v = 0;
		// 위 이동
		if (v > 0)
			// Move 애니메이션, 루프 O, 애니메이션 재생 시간 1배속
			_AsyncAnimation(moveCilp[2], true, 1);
		// 기본 이동
		else
			// Move 애니메이션, 루프 O, 애니메이션 재생 시간 1배속
			_AsyncAnimation(moveCilp[1], true, 1);
	}
}
