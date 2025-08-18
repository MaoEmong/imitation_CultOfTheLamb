using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_AnimScript : MonoBehaviour
{
	// 스파인 애니메이션
	SkeletonAnimation skeletonAnimation;
	// 애니매이션 재생을 위한 클립
	public AnimationReferenceAsset[] moveCilp;
	public AnimationReferenceAsset[] attackCilp;

	// 몬스터 타입
	public enum MonsterType
	{
		Bat,
		Worm,
	}

	// 프리팹 몬스터 타입
	public MonsterType type;

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

	Vector3 upPos = new Vector3(0, 0.5f, 0);
	Vector3 downPos = new Vector3(0, -0.5f, 0);

	bool up;

	// 애니메이션 타입
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

	// 박쥐 히트 타임
	float batHit;
	// 벌레 차징 타임
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

	// 애니메이션 재생 메소드
	void _AsyncAnimation(AnimationReferenceAsset Clip, bool loop, float timeScale)
	{
		// 현재 공격 애니메이션 출력 중일 때(히트 중이면 애니메이션 변경)
		if (animState != AnimState.HIT && (nowCharge || nowImpact))
		{
			if (type == MonsterType.Worm)
			{
				// 차징 타임 증가
				wormCharge += Time.deltaTime;

				// 애니메이션 진행시간 > 애니메이션 종료시간 <= 애니메이션 상태 검사 후 종료 시 특정 상태 끝
				if (wormCharge > 0.5f)
					nowCharge = false;

				if (skeletonAnimation.state.GetCurrent(0).AnimationTime ==
					skeletonAnimation.state.GetCurrent(0).AnimationEnd)
					nowImpact = false;
			}
			else
			{
				// 애니메이션 진행시간 == 애니메이션 종료시간 <= 애니메이션 상태 검사 후 종료 시 특정 상태 끝
				if (skeletonAnimation.state.GetCurrent(0).AnimationTime ==
					skeletonAnimation.state.GetCurrent(0).AnimationEnd)
				{
					nowCharge = false;
					nowImpact = false;
				}
			}
			// 아직 진행중이라면 애니메이션 전환 X
			return;
		}
		// 히트 중일때
		if (nowAction)
		{
			// 박쥐일 때
			if (type == MonsterType.Bat)
			{
				nowCharge = false;
				nowImpact = false;

				// 히트 타임 증가
				batHit += Time.deltaTime;

				// 일정시간 이후 히트 해제
				if (batHit > 0.5f)
				{
					nowAction = false;
				}
			}
			// 벌레일 때
			else
			{
				// 공격 애니메이션 종료
				nowCharge = false;
				nowImpact = false;

				// 애니메이션 진행시간 == 애니메이션 종료시간 <= 애니메이션 상태 검사 후 종료 시 특정 상태 끝
				if (skeletonAnimation.state.GetCurrent(0).AnimationTime ==
					skeletonAnimation.state.GetCurrent(0).AnimationEnd)
				{
					nowAction = false;
				}
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

		// 몬스터가 박쥐일 때
		if (type == MonsterType.Bat)
		{
			// 박쥐 공격 애니메이션일때 예외처리
			if (currentAnimation == "attack")
				nowImpact = true;

			// 박쥐 히트 애니메이션일때 예외처리
			if (currentAnimation == "hit")
			{
				nowAction = true;
				batHit = 0;
			}
		}
		// 몬스터가 벌레일 때
		else if (type == MonsterType.Worm)
		{
			// 벌레 공격 애니메이션일때 예외처리
			if (currentAnimation == "attack-impact")
			{
				nowImpact = true;
				wormCharge = -1;
			}

			// 벌레 히트 애니메이션일때 예외처리
			if (currentAnimation == "land")
				nowAction = true;
		}

	}

	// 현재 상태에 따라 애니메이션 재생 전환
	void _setCurrentAnimation(AnimState state)
	{
		// 현재 상태
		switch (state)
		{
			// Bone
			case AnimState.Bone:
				// IDLE 애니메이션, 루프 O, 애니메이션 재생 시간 1배속
				_AsyncAnimation(moveCilp[4], false, 1);
				nowAction = true;
				break;
			// IDLE
			case AnimState.IDLE:
				// IDLE 애니메이션, 루프 O, 애니메이션 재생 시간 1배속
				_AsyncAnimation(moveCilp[0], true, 1);
				break;
			case AnimState.MOVE:
				// 이동 방향에 따라 다른 애니메이션 출력
				Move();
				break;
			case AnimState.CHARGE:
				// CHARGE 애니메이션, 루프 X, 애니메이션 재생 시간 1배속
				if (type == MonsterType.Worm)
				{
					// 벌레일 때 차징 시간 특정하여 인위적으로 차징 타임 구현
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
				// IMPACT 애니메이션, 루프 X, 애니메이션 재생 시간 1배속
				_AsyncAnimation(attackCilp[1], false, 1);
				break;
			case AnimState.HIT:
				// HIT 애니메이션, 루프 X, 애니메이션 재생 시간 1배속
				if (type == MonsterType.Worm)
					_AsyncAnimation(moveCilp[2], false, 1.5f);
				else
					_AsyncAnimation(moveCilp[2], false, 1);

				nowAction = true;
				break;
			case AnimState.DIE:
				// DIE 애니메이션, 루프 X, 애니메이션 재생 시간 1배속
				_AsyncAnimation(moveCilp[3], false, 1);
				nowAction = true;
				Dead = true;
				break;
		}
	}

	// MOVE 애니메이션 출력 메소드
	void Move()
	{
		// 박쥐 이동
		if (type == MonsterType.Bat)
			// Move 애니메이션, 루프 O, 애니메이션 재생 시간 1배속
			_AsyncAnimation(moveCilp[0], true, 1);
		// 벌레 이동
		else
			// Move 애니메이션, 루프 O, 애니메이션 재생 시간 1배속
			_AsyncAnimation(moveCilp[1], true, 1);
	}
}
