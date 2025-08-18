using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData",menuName = "Scriptable Object/EnemyData",order =int.MaxValue)]
public class EnemyData : ScriptableObject
{
   // �� �̸�
    [SerializeField]
    private string EnemyName;
    public string m_Name { get { return EnemyName; } }

    // �� ���ݷ�
    [SerializeField]
    private float EnemyPower;
    public float m_Power { get { return EnemyPower; } }

    // �� ���� HP
    [SerializeField]
    private float EnemyHP;
    public float m_HP { get {  return EnemyHP; } }

    // �� �ִ� HP
    [SerializeField]
    private float EnemyMaxHP;
    public float m_Max_HP { get { return EnemyMaxHP; } }

    // �� ���� �Ÿ�
    [SerializeField]
    private float FindDistance;
    public float m_Find { get { return FindDistance; } }

    // �� ���� �Ÿ�
    [SerializeField]
    private float AttackDistance;
    public float m_Attack { get { return AttackDistance; } }

    // �ͷ� ���� �Ÿ�
    [SerializeField]
    private float ShotDistance;
    public float m_Shot { get { return ShotDistance; } }    
}
