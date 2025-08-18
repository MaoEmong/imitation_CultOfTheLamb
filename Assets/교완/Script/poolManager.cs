using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class poolManager : MonoBehaviour
{
    // 적 프리팹
    public GameObject[] Prefab; 

    // 오브젝트 풀 리스트
    List<GameObject>[] pools;

    private void Awake()
    {     
        // 오브젝트 풀 리스트 배열 초기화
        pools = new List<GameObject>[Prefab.Length];

        // 리스트 내부를 for문으로 초기화
        for(int i = 0; i< pools.Length; i++)
        {
            pools[i] = new List<GameObject>();
        }

    }

    public GameObject Get(int index)
    {
        // 게임오브젝트 select의 초기값을 null로 초기화
        GameObject select = null;

        // 오브젝트 풀 리스트 내부를 반복 순회
        foreach (GameObject Enemy in pools[index])
        { 
            // 선택한 오브젝트가 비활성화 되어있을 때
            if(!Enemy.activeSelf)
            {
                // 선택한 오브젝트를 select에 할당
                select = Enemy;
                // 할당한 select를 활성화
                select.SetActive(true);
                // 끝난 뒤 foreach문을 빠져나감 
                break;
            }
        }
        // 선택한 오브젝트를 찾을수 없다면
        if(select == null)
        {
            // 선택한 오브젝트를 생성하고 select에 할당
            select = Instantiate(Prefab[index]);
            // 생성된 오브젝트를 오브젝트풀에 추가
            pools[index].Add(select);
        }
        return select;
    }
}
