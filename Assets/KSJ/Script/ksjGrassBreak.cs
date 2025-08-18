using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ksjGrassBreak : ksjBreakableObject
{
    [SerializeField]
    private BrkObjType BrkObjType;
    [SerializeField]
    private GameObject[] grassParticle;

    // grass 아이템 드랍
    [SerializeField]
    private GameObject item;

    private void Update()
    {
        if (!isBroken)
            return;

        for (int i = 0; i < grassParticle.Length; i++)
        {
            if (grassParticle[i].transform.position.y <0 && grassParticle[i].GetComponent<Rigidbody>() != null)
            {
                Destroy(grassParticle[i].GetComponent<Rigidbody>());
            }
        }

        for (int i = 0; i < grassParticle.Length; i++)
        {
            grassParticle[i].transform.forward = Camera.main.transform.forward;
        }
    }

    public override void StartParticle()
    {
        SoundManager.Sound.Play("Object/Grass/GrassCut0");

        for (int i = 0; i < mainObject.Length; i++)
        {
            mainObject[i].SetActive(false);
        }
        
        for(int i=0;i<subObject.Length;i++)
        {
            subObject[i].SetActive(true);
        }

        for (int i = 0; i < grassParticle.Length; i++)
        {
            grassParticle[i].SetActive(true);

            var rigid = grassParticle[i].transform.GetComponent<Rigidbody>();

            float randX = Random.Range(-1, 1);
            float randY = Random.Range(6, 10);
            float randZ = Random.Range(-1, 1);

            rigid.AddForce(new Vector3(randX, randY,randZ), ForceMode.Impulse);
        }


        DropGrass();

        isBroken = true;

    }

    public void DropGrass()
    {
        GameObject obj;


        obj = Instantiate(item);
        obj.GetComponent<ItemObject>().Count = 1;
        obj.GetComponent<ItemObject>().ObjectCode = ItemCode.Grass;


        obj.transform.position = transform.position + new Vector3(0, 0.5f, -0.5f);


    }

    //================김해준 추가=================
    public override void CallBreakableSound()
    {
        SoundManager.Sound.Play("KHJ/Effect/Object/Grass_Breakable");
    }

    public override void CallHitSound()
    {
        SoundManager.Sound.Play("KHJ/Effect/Object/Grass_Hit");
    }

}
