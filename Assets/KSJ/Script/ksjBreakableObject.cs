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

// ������Ʈ�� Ÿ���� Ȯ��
// Ȯ�ε� Ÿ�Կ� ���� ����� ��ƼŬ�� ���

public class ksjBreakableObject : MonoBehaviour
{
    public BrkObjType objType;
    public int maxObjHP;
    public int curObjHP;

    public GameObject[] mainObject;
    public GameObject[] subObject;

    protected bool isBroken = false;
    protected bool isQuake = false;

    // ������Ʈ ����ũ���� ����� ������ġ
    protected Vector3 originPos;
    // ��鸱 �ð�
    protected float maxQuakeTime=0.5f;
    protected float curQuakeTime;

    // ��ŸƮ���� �ν����ͷ� ������ ������Ʈ Ÿ���� ������� ü���� �ʱ�ȭ�Ѵ�.    
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
    /// ������Ʈ�� Ÿ�Կ� ���� ��ƼŬ�� ���ư��� ����, ���� �ٸ� �����̹Ƿ� �����Լ��� �ۼ�. 
    /// ��ӹ��� �극��Ŀ�������Ʈ���� �������Ѵ�.
    /// �ν��� ��, ��ƼŬ�� �����. 
    /// </summary>
    public virtual void StartParticle()
    {
    }

    /// <summary>
    /// ��ӹ��� �극��Ŀ�������Ʈ���� ������
    /// Ÿ�ݹް� �ν��������� ������Ʈ�� ��鸲 ȿ��
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
    /// �극��Ŀ�� ������Ʈ�� �ǰ������� ���� �� ȣ��� �޼���
    /// </summary>

    //===================== 231001 ������ ����==============
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

    //===================== 231001 ������ �߰�==============
    public virtual void CallHitSound() { }
    public virtual void CallBreakableSound() { }

}
