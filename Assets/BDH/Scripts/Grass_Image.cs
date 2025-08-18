using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Grass_Image : MonoBehaviour
{
	SpriteRenderer image;

    public Sprite[] imageList;
    public Material[] matList;

    public int ImageNum;

	private void Start()
	{
		image = GetComponent<SpriteRenderer>();

		image.sprite = imageList[ImageNum];
		image.material = matList[ImageNum];
	}

	private void Update()
	{
		image.sprite = imageList[ImageNum];
		image.material = matList[ImageNum];
	}
}
