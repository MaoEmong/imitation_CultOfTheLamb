using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ksjTentBreak : ksjBreakableObject
{
    [SerializeField]
    private BrkObjType BrkObjType;
    [SerializeField]
    private GameObject[] tentParticle;

    private void Update()
    {
        if (!isBroken)
            return;

        for (int i = 0; i < tentParticle.Length; i++)
        {
            if (tentParticle[i].transform.position.y <= 0.1 && tentParticle[i].GetComponent<Rigidbody>() != null)
            {
                Destroy(tentParticle[i].GetComponent<Rigidbody>());
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
        for (int i = 0; i < tentParticle.Length; i++)
        {
            tentParticle[i].SetActive(true);

            var rigid = tentParticle[i].transform.GetComponent<Rigidbody>();

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
