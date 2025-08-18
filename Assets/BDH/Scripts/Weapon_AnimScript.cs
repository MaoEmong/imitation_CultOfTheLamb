using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Enemy_AnimScript;

// ���� Ÿ��
public enum WeaponType
{
    SWORD,
    AXE,
    DAGGER,
    GAUNTLETS
}

public class Weapon_AnimScript : MonoBehaviour
{
	// ������ �ִϸ��̼�
	SkeletonAnimation skeletonAnimation;
	// �ִϸ��̼� ����� ���� Ŭ��
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

	// �ִϸ��̼� ��� �޼ҵ�
	void _AsyncAnimation(AnimationReferenceAsset Clip, bool loop, float timeScale)
	{
		// ���� ������� �ִϸ��̼��� �ٽ� ����� ��
		if (Clip.name.Equals(currentAnimation))
			// �ִϸ��̼� ��ȯ X
			return;

		// �ش� �ִϸ��̼� ���
		skeletonAnimation.state.SetAnimation(0, Clip, loop).TimeScale = timeScale;

		// ���� �ִϸ��̼��� �̸��� ����
		currentAnimation = Clip.name;
	}
}
