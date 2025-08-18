using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using Unity.Mathematics;
using UnityEngine;

public class ksjMainUI : MonoBehaviour
{

    [SerializeField]
    private GameObject[] heartPrefabs; // 비활성화된 자식 하트 프리팹
    [SerializeField]
    private GameObject heartDisplay; // 하트를 보여줄 상위 오브젝트

    private void Start()
    {
        var StatusInit = ksjPlayerManager.Instance;
        
    }

    /// <summary>
    /// 외부에서 접근해 하트를 갱신시키는 용도의 메서드
    /// </summary>
    public void SetHeart()
    {
        var StatusInit = ksjPlayerManager.Instance;

        // hp를 나누어 나머지가 있을 때 half상태.
        bool isHalf = StatusInit.HP % 1 != 0 ? true : false;

        Debug.Log("PlayerHp : " + StatusInit.HP);

        for(int i =0;i<heartPrefabs.Length;i++)
        {
            heartPrefabs[i].SetActive(false);
        }

        
        for(int i=0;i< (int)Mathf.Ceil(StatusInit.HP); i++)
        {
            // 소수점이 있을 때 마지막 요소
            if(isHalf && (int)Mathf.Ceil(StatusInit.HP) == i+1)
            heartPrefabs[i].GetComponent<SkeletonGraphic>().AnimationState.SetAnimation(0,"lose-half",false);
            //    heartPrefabs[i].GetComponent<SkeletonGraphic>().initialSkinName = "half";
            else
            heartPrefabs[i].GetComponent<SkeletonGraphic>().initialSkinName = "normal";

            heartPrefabs[i].SetActive(true);




            //SpineEditorUtilities.ReloadSkeletonDataAssetAndComponent(heartPrefabs[i].GetComponent<SkeletonGraphic>());
        }
    }


    public void SetHeartPlus()
    {
        var StatusInit = ksjPlayerManager.Instance;

        // hp를 나누어 나머지가 있을 때 half상태.
        bool isHalf = StatusInit.HP % 1 != 0 ? true : false;

        Debug.Log("PlayerHp : " + StatusInit.HP);

        for (int i = 0; i < heartPrefabs.Length; i++)
        {
            heartPrefabs[i].SetActive(false);
        }


        for (int i = 0; i < (int)Mathf.Ceil(StatusInit.HP); i++)
        {
            // 소수점이 있을 때 마지막 요소
            if (isHalf && (int)Mathf.Ceil(StatusInit.HP) == i + 1)
                heartPrefabs[i].GetComponent<SkeletonGraphic>().AnimationState.SetAnimation(0, "fill-half-left", false);
            //    heartPrefabs[i].GetComponent<SkeletonGraphic>().initialSkinName = "half";
            else
            {
                if(i == (int)Mathf.Ceil(StatusInit.HP+1))
                {
                heartPrefabs[i].GetComponent<SkeletonGraphic>().AnimationState.SetAnimation(0, "fill-half-right", false);

                }

            }
                //heartPrefabs[i].GetComponent<SkeletonGraphic>().initialSkinName = "normal";

            heartPrefabs[i].SetActive(true);

            //SpineEditorUtilities.ReloadSkeletonDataAssetAndComponent(heartPrefabs[i].GetComponent<SkeletonGraphic>());
        }
    }

}
