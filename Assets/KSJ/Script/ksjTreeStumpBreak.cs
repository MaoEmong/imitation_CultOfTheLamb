using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ksjTreeStumpBreak : ksjBreakableObject
{
    [SerializeField]
    private GameObject[] stumpParticle;

    [SerializeField]
    private BrkObjType BrkObjType;

    private void Update()
    {
        if (!isBroken)
            return;

        for(int i=0;i<stumpParticle.Length;i++)
        {
            if (stumpParticle[i].transform.position.y< 0 && stumpParticle[i].GetComponent<Rigidbody>() != null)
            {
                Destroy(stumpParticle[i].GetComponent<Rigidbody>());
            }
        }
    }

    public override void StartParticle()
    {
        for (int i = 0; i < mainObject.Length; i++)
        {
            mainObject[i].SetActive(false);
        }

        for (int i = 0; i < subObject.Length; i++)
        {
            subObject[i].SetActive(true);
        }

        for (int i = 0; i < stumpParticle.Length; i++)
        {
            stumpParticle[i].SetActive(true);

            var rigid = stumpParticle[i].transform.GetComponent<Rigidbody>();

            float randX = Random.Range(-1, 1);
            float randY = Random.Range(6, 10);
            float randZ = Random.Range(-1, 1);

            rigid.AddForce(new Vector3(randX, randY, randZ), ForceMode.Impulse);
        }
        isBroken = true;
    }
    //================±Ë«ÿ¡ÿ √ﬂ∞°=================
    public override void CallBreakableSound()
    {
        SoundManager.Sound.Play("KHJ/Effect/Object/Wooden_Breakable");
    }

    public override void CallHitSound()
    {
        SoundManager.Sound.Play("KHJ/Effect/Object/Wooden_Hit");
    }

}
