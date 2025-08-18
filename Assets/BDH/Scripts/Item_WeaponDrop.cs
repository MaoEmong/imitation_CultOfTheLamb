using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class Item_WeaponDrop : MonoBehaviour
{
    Weapon_AnimScript script;

	public WeaponType type;

	public int level;
	public float atk;
	public float weaponSpeed;

	private void Start()
	{
		script= GetComponentInChildren<Weapon_AnimScript>();
	}

	public void setWeapon(WeaponType wtype)
	{	

        switch (wtype)
		{
			case WeaponType.SWORD:
				script._type = WeaponType.SWORD;
				type = WeaponType.SWORD;
				weaponSpeed = 1.0f;
                atk = 5;				
				break;
			case WeaponType.AXE:
				script._type = WeaponType.AXE;
				type = WeaponType.AXE;
				weaponSpeed = 2.0f;
                atk = 8;
				break;
			case WeaponType.DAGGER:
				script._type = WeaponType.DAGGER;
				type = WeaponType.DAGGER;
				weaponSpeed = 0.7f;
                atk = 3;
				break;
		}
    }

	public void setLevel(int rand)
	{
		level = rand;
	}

	public int changeLevel()
	{
		return level;
	}

	public float changeAtk()
	{
		return atk;
	}
}
