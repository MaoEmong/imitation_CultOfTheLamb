using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ksjWoodenRedStickBreak : ksjBreakableObject
{
    [SerializeField]
    private BrkObjType BrkObjType;
    [SerializeField]
    private GameObject[] woodenRedStickParticle;

    private void Update()
    {
        if (!isBroken)
            return;

        for (int i = 0; i < woodenRedStickParticle.Length; i++)
        {
            if (woodenRedStickParticle[i].transform.position.y < 0 && woodenRedStickParticle[i].GetComponent<Rigidbody>() != null)
            {
                Destroy(woodenRedStickParticle[i].GetComponent<Rigidbody>());
            }
        }

        for(int i = 0; i < woodenRedStickParticle.Length; i++)
        {
            woodenRedStickParticle[i].transform.forward = Camera.main.transform.forward;

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

        for (int i = 0; i < woodenRedStickParticle.Length; i++)
        {
            woodenRedStickParticle[i].SetActive(true);

            var rigid = woodenRedStickParticle[i].transform.GetComponent<Rigidbody>();

            float randX = Random.Range(-1, 1);
            float randY = Random.Range(6, 10);
            float randZ = Random.Range(-1, 1);

            rigid.AddForce(new Vector3(randX, randY, randZ), ForceMode.Impulse);
        }
        isBroken = true;

        transform.GetComponent<BoxCollider>().enabled = false;
    }

    public override void CallBreakableSound()
    {
        SoundManager.Sound.Play("KHJ/Effect/Object/Wooden_Breakable");
    }

    public override void CallHitSound()
    {
        SoundManager.Sound.Play("KHJ/Effect/Object/Wooden_Hit");
    }



}
