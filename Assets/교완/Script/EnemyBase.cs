using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    // 적 상태 제어 enum
   

    // 몬스터 공격력
    public float m_Power;

    // 몬스터 체력
    public float m_HP;

    // 몬스터 이동속도
    public float m_Speed;

    // 적 감지 거리
    public float m_findDis;

    // 적 공격 거리
    public float m_AtkDis;

    // 적 공격 딜레이
    public float m_AtkDeley;

    // 적 
    public float m_curTime;   

}
