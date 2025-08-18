using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class poolManager : MonoBehaviour
{
    // �� ������
    public GameObject[] Prefab; 

    // ������Ʈ Ǯ ����Ʈ
    List<GameObject>[] pools;

    private void Awake()
    {     
        // ������Ʈ Ǯ ����Ʈ �迭 �ʱ�ȭ
        pools = new List<GameObject>[Prefab.Length];

        // ����Ʈ ���θ� for������ �ʱ�ȭ
        for(int i = 0; i< pools.Length; i++)
        {
            pools[i] = new List<GameObject>();
        }

    }

    public GameObject Get(int index)
    {
        // ���ӿ�����Ʈ select�� �ʱⰪ�� null�� �ʱ�ȭ
        GameObject select = null;

        // ������Ʈ Ǯ ����Ʈ ���θ� �ݺ� ��ȸ
        foreach (GameObject Enemy in pools[index])
        { 
            // ������ ������Ʈ�� ��Ȱ��ȭ �Ǿ����� ��
            if(!Enemy.activeSelf)
            {
                // ������ ������Ʈ�� select�� �Ҵ�
                select = Enemy;
                // �Ҵ��� select�� Ȱ��ȭ
                select.SetActive(true);
                // ���� �� foreach���� �������� 
                break;
            }
        }
        // ������ ������Ʈ�� ã���� ���ٸ�
        if(select == null)
        {
            // ������ ������Ʈ�� �����ϰ� select�� �Ҵ�
            select = Instantiate(Prefab[index]);
            // ������ ������Ʈ�� ������ƮǮ�� �߰�
            pools[index].Add(select);
        }
        return select;
    }
}
