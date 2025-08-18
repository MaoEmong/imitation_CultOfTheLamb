using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ksjStoneBreak : ksjBreakableObject
{
    [SerializeField]
    private GameObject[] stoneParticle;

    [SerializeField]
    private BrkObjType BrkObjType;

    private void Update()
    {

        if(isQuake)
        {
            QuakeObject();
        }

        if (!isBroken)
            return;

        transform.GetComponent<BoxCollider>().enabled = false;

        for (int i=0;i<stoneParticle.Length;i++)
        {
            if(stoneParticle[i].transform.position.y< 0 && stoneParticle[i].GetComponent<Rigidbody>() != null)
            {
                Destroy(stoneParticle[i].GetComponent<Rigidbody>());
            }
        }

        for (int i = 0; i < stoneParticle.Length; i++)
        {
            stoneParticle[i].transform.forward = Camera.main.transform.forward;
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


        for (int i = 0; i < stoneParticle.Length; i++)
        {
            stoneParticle[i].SetActive(true);

            var rigid = stoneParticle[i].transform.GetComponent<Rigidbody>();

            float randX = Random.Range(-1, 1);
            float randY = Random.Range(6, 10);
            float randZ = Random.Range(-0.5f, 1.5f);

            rigid.AddForce(new Vector3(randX, randY, randZ), ForceMode.Impulse);
        }

        isBroken = true;
    }

    //================±Ë«ÿ¡ÿ √ﬂ∞°=================

    public override void CallBreakableSound()
    {
        SoundManager.Sound.Play("KHJ/Effect/Object/Stone_Breakable");
    }

    public override void CallHitSound()
    {
        SoundManager.Sound.Play("KHJ/Effect/Object/Stone_Hit");
    }


}
