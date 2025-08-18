using System.Collections;
using System.Collections.Generic;
using Spine;
using UnityEngine;
using static ksjChest;

public class ksjChestController : MonoBehaviour
{
    [SerializeField]
    private GameObject chestPrefab;

    [SerializeField]
    private bool isGradedRandom = false;

    private ksjChest chest;

    public ChestSkin chestGrade;

    private void Start()
    {
        chest= chestPrefab.GetComponent<ksjChest>();        
        chest.parentTransform = transform;
        chest.SetChest(chestGrade, isGradedRandom);
    }


    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Y))
        {
            chest.chestAnim = ChestAnim.Reveal;
        }
    }
}
