using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using Unity.Mathematics;
using UnityEngine;

public class ksjMainUI : MonoBehaviour
{

    [SerializeField]
    private GameObject[] heartPrefabs; // ��Ȱ��ȭ�� �ڽ� ��Ʈ ������
    [SerializeField]
    private GameObject heartDisplay; // ��Ʈ�� ������ ���� ������Ʈ

    private void Start()
    {
        var StatusInit = ksjPlayerManager.Instance;
        
    }

    /// <summary>
    /// �ܺο��� ������ ��Ʈ�� ���Ž�Ű�� �뵵�� �޼���
    /// </summary>
    public void SetHeart()
    {
        var StatusInit = ksjPlayerManager.Instance;

        // hp�� ������ �������� ���� �� half����.
        bool isHalf = StatusInit.HP % 1 != 0 ? true : false;

        Debug.Log("PlayerHp : " + StatusInit.HP);

        for(int i =0;i<heartPrefabs.Length;i++)
        {
            heartPrefabs[i].SetActive(false);
        }

        
        for(int i=0;i< (int)Mathf.Ceil(StatusInit.HP); i++)
        {
            // �Ҽ����� ���� �� ������ ���
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

        // hp�� ������ �������� ���� �� half����.
        bool isHalf = StatusInit.HP % 1 != 0 ? true : false;

        Debug.Log("PlayerHp : " + StatusInit.HP);

        for (int i = 0; i < heartPrefabs.Length; i++)
        {
            heartPrefabs[i].SetActive(false);
        }


        for (int i = 0; i < (int)Mathf.Ceil(StatusInit.HP); i++)
        {
            // �Ҽ����� ���� �� ������ ���
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
