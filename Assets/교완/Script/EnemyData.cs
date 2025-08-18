using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData",menuName = "Scriptable Object/EnemyData",order =int.MaxValue)]
public class EnemyData : ScriptableObject
{
   // 적 이름
    [SerializeField]
    private string EnemyName;
    public string m_Name { get { return EnemyName; } }

    // 적 공격력
    [SerializeField]
    private float EnemyPower;
    public float m_Power { get { return EnemyPower; } }

    // 적 현재 HP
    [SerializeField]
    private float EnemyHP;
    public float m_HP { get {  return EnemyHP; } }

    // 적 최대 HP
    [SerializeField]
    private float EnemyMaxHP;
    public float m_Max_HP { get { return EnemyMaxHP; } }

    // 적 감지 거리
    [SerializeField]
    private float FindDistance;
    public float m_Find { get { return FindDistance; } }

    // 적 공격 거리
    [SerializeField]
    private float AttackDistance;
    public float m_Attack { get { return AttackDistance; } }

    // 터렛 공격 거리
    [SerializeField]
    private float ShotDistance;
    public float m_Shot { get { return ShotDistance; } }    
}
