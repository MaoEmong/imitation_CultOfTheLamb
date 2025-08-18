using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_AttackScript : MonoBehaviour
{
    public int level;
    public float Damage;

	private void Start()
	{
        SetAtkDamae(0, 5);
	}

	public void SetAtkDamae(int lv, float atk)
    {
        level = lv;
        Damage= atk;
    }

    public float getDamage()
    {
        float damage = Damage + (float)level;
        return damage;
    }
}
