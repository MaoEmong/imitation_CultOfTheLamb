using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public enum BrkObjType
{
    Grass,
    Tent,
    Stone,
    WoodenRedStick,
    Stump
}

// 오브젝트의 타입을 확인
// 확인된 타입에 따라 저장된 파티클을 재생

public class ksjBreakableObject : MonoBehaviour
{
    public BrkObjType objType;
    public int maxObjHP;
    public int curObjHP;

    public GameObject[] mainObject;
    public GameObject[] subObject;

    protected bool isBroken = false;
    protected bool isQuake = false;

    // 오브젝트 퀘이크에서 사용할 원본위치
    protected Vector3 originPos;
    // 흔들릴 시간
    protected float maxQuakeTime=0.5f;
    protected float curQuakeTime;

    // 스타트에서 인스펙터로 설정한 오브젝트 타입을 기반으로 체력을 초기화한다.    
    private void Start()
    {
        switch(objType)
        {
            case BrkObjType.Grass:
                maxObjHP = 1;
                break;
            case BrkObjType.Stone:
                maxObjHP = 2;
                break;
            case BrkObjType.WoodenRedStick:
                maxObjHP = 2;
                break;
            case BrkObjType.Tent:
                maxObjHP = 2;
                break;
            case BrkObjType.Stump:
                maxObjHP = 2;
                break;
        }
        curObjHP = maxObjHP;
        originPos = transform.position;
    }


    /// <summary>
    /// 오브젝트의 타입에 따라 파티클의 날아가는 형태, 힘이 다를 예정이므로 가상함수로 작성. 
    /// 상속받은 브레이커블오브젝트에서 재정의한다.
    /// 부숴진 후, 파티클을 만든다. 
    /// </summary>
    public virtual void StartParticle()
    {
    }

    /// <summary>
    /// 상속받은 브레이커블오브젝트에서 재정의
    /// 타격받고 부숴지지않은 오브젝트의 흔들림 효과
    /// </summary>
    public virtual void QuakeObject()
    {
        curQuakeTime += Time.deltaTime;

        for (int i = 0; i < mainObject.Length; i++)
        {
            mainObject[i].transform.position += new Vector3(Random.RandomRange(-0.05f, 0.05f), 0, 0);
        }

        if (curQuakeTime >= maxQuakeTime)
        {
            isQuake = false;
            for (int i = 0; i < mainObject.Length; i++)
            {
                mainObject[i].transform.position = originPos;
            }
        }
    }

    /// <summary>
    /// 브레이커블 오브젝트가 피격판정을 받을 때 호출될 메서드
    /// </summary>

    //===================== 231001 김해준 수정==============
    public void HitObject()
    {
        if (curObjHP <= 0)
            return;

        print(objType + "hit");

        curObjHP--;

        if (curObjHP <= 0)
        {
            isQuake = false;
            StartParticle();
            CallBreakableSound();
        }
        else if (curObjHP > 0)
        {
            isQuake = true;
            CallHitSound();
        }
    }

    //===================== 231001 김해준 추가==============
    public virtual void CallHitSound() { }
    public virtual void CallBreakableSound() { }

}
