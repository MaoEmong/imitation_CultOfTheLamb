using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Enemy_AnimScript;

// 웨폰 타입
public enum WeaponType
{
    SWORD,
    AXE,
    DAGGER,
    GAUNTLETS
}

public class Weapon_AnimScript : MonoBehaviour
{
	// 스파인 애니메이션
	SkeletonAnimation skeletonAnimation;
	// 애니매이션 재생을 위한 클립
	public AnimationReferenceAsset[] weaponCilp;

	string currentAnimation;

	public WeaponType _type;


	/// <summary>
	/// 
	/// </summary>
	private float curWeaponSpeed;
	public float CurWeaponSpeed
	{
		get { return curWeaponSpeed; }
	}

	public void TypeSet(WeaponType type)
	{
		_type = type;
	}

	private void Start()
	{
		skeletonAnimation = GetComponent<SkeletonAnimation>();
	}

	private void Update()
	{
		switch (_type)
		{
			case WeaponType.SWORD:
				_AsyncAnimation(weaponCilp[0], true, 1);
				curWeaponSpeed = 1.0f;
                break;
			case WeaponType.AXE:
				_AsyncAnimation(weaponCilp[1], true, 1);
				curWeaponSpeed = 2.0f;
				break;
			case WeaponType.DAGGER:
				_AsyncAnimation(weaponCilp[2], true, 1);
				curWeaponSpeed = 0.7f;
				break;
		}
	}

	// 애니메이션 재생 메소드
	void _AsyncAnimation(AnimationReferenceAsset Clip, bool loop, float timeScale)
	{
		// 현재 재생중인 애니메이션이 다시 재생될 때
		if (Clip.name.Equals(currentAnimation))
			// 애니메이션 전환 X
			return;

		// 해당 애니메이션 재생
		skeletonAnimation.state.SetAnimation(0, Clip, loop).TimeScale = timeScale;

		// 현재 애니메이션의 이름을 저장
		currentAnimation = Clip.name;
	}
}
