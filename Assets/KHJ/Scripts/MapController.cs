using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// �� ��Ʈ�ѷ�
// 4���� �ʵ幭�� ����
public class MapController : MonoBehaviour
{
	// ���� �������� �ѹ���
	public int StageNumber = 0;

	// �ʿ� �����ϴ� �ʵ�[�������� ��][�� ���������� ���� �ʵ�]
	//public Field[4][] Fields;
	public Field[] Fields1;
	public Field[] Fields2;
	public Field[] Fields3;
	public Field[] Fields4;

	public List<Field[]> TotalField;

	// �÷��̾� ���� ���� ������[�������� �ѹ�]
	public Transform[] PlayerSpawnPos;
	// �÷��̾�
	public GameObject Player;

	// ȭ�� FadeInOut �̹���
	public Image ChangeImage;

	private void Start()
	{
		var pm = ksjPlayerManager.Instance;
		pm.getCanvas();

		ChangeImage = pm.FadeImage;

        // ȭ�� ��ȯ�� �̹��� Ȱ��ȭ
        ChangeImage.gameObject.SetActive(true);
		ChangeImage.color = new Color(ChangeImage.color.r, ChangeImage.color.g, ChangeImage.color.b, 1);

		// �ʵ� ������Ʈ ����
		TotalField = new() 
		{
			Fields1,Fields2,Fields3,Fields4,
		};

		StageNumber = ksjPlayerManager.Instance.CurStage;

		// ksjPlayerManager.Instance.getCanvas();

		// �������� �ʱ�ȭ
		for (int i = 0; i < TotalField.Count; i++)
		{
			// ���� �������� �̴ϼȶ���¡
			if( i == StageNumber)
				foreach(var n in TotalField[StageNumber])
				{
					n.gameObject.SetActive(true);
					n.Init(this);
				}	
			// �׿� �������� ��Ȱ��ȭ
			else
				foreach(var n in TotalField[i])
				{
					n.gameObject.SetActive(false);
				}
		}

		// ���� ���������� �ʵ尡 ������ ��� ��Ż�� �ʱ�ȭ
		for(int i  = 0; i < TotalField[StageNumber].Length; i++)
		{
			for(int j = 0; j < TotalField[StageNumber][i].Portals.Length; j++) 
			{
				TotalField[StageNumber][i].Portals[j].Init(this);
			}
		}

		// �÷��̾� ��ġ��Ű��
		// Player = GameManager.Instance.Player;
		Player.transform.position = PlayerSpawnPos[StageNumber].position;

		// ȭ�� ��ȯ �̹��� �׽�Ʈ��
		//FadeInOutImage();

		// ���� �� ���� �� ȭ�� ȿ��
		StartCoroutine(FadeIn());


		//=============================== ���� �߰� 
		SoundManager.Sound.Play("KHJ/Bgm/Field/Stage_Standard", SoundManager.SoundType.Bgm);
		StartCoroutine(BirdSound());

	}

	// ȭ�� ��ȯ ȿ��
	public void FadeInOutImage()
	{
		StartCoroutine(FadeInOut());
	}

	IEnumerator FadeInOut()
	{
        var pm = ksjPlayerManager.Instance;
        ChangeImage = pm.FadeImage;

        ChangeImage.gameObject.SetActive(true);
		ChangeImage.color = new Color(ChangeImage.color.r, ChangeImage.color.g, ChangeImage.color.b, 0);
		float value = 0.3f;
		float curColor = 0;
		while(true)
		{
			yield return new WaitForSeconds(0.07f);
			curColor += value;
			ChangeImage.color = new Color(ChangeImage.color.r, ChangeImage.color.g, ChangeImage.color.b, curColor);

			if (curColor >= 0.99f)
				break;

		}
		yield return new WaitForSeconds(0.3f);
		curColor = 1;
		while (true)
		{
			yield return new WaitForSeconds(0.07f);
			curColor -= value;
			ChangeImage.color = new Color(ChangeImage.color.r, ChangeImage.color.g, ChangeImage.color.b, curColor);

			if (curColor <= 0.0f)
			{
				ChangeImage.gameObject.SetActive(false);
				break;
			}

		}
	}
	IEnumerator FadeIn()
	{
        var pm = ksjPlayerManager.Instance;
        ChangeImage = pm.FadeImage;

        ChangeImage.gameObject.SetActive(true);
		float value = 0.3f;
		float curColor = 0;
		curColor = 1;
		ChangeImage.color = new Color(ChangeImage.color.r, ChangeImage.color.g, ChangeImage.color.b, curColor);
		while (true)
		{
			yield return new WaitForSeconds(0.07f);
			curColor -= value;
			ChangeImage.color = new Color(ChangeImage.color.r, ChangeImage.color.g, ChangeImage.color.b, curColor);

			if (curColor <= 0.0f)
			{
				ChangeImage.gameObject.SetActive(false);
				break;
			}

		}

	}

	// ���Ҹ� �ڷ�ƾ
	IEnumerator BirdSound()
	{
		float timer = UnityEngine.Random.Range(20.0f, 30.0f);
		int value = UnityEngine.Random.Range(0, 3);

		while(true)
		{
			yield return new WaitForSeconds(timer);
			
			switch(value)
			{
				case 0:
					SoundManager.Sound.Play("KHJ/Effect/Bird/Bird1");
					break;
				case 1:
					SoundManager.Sound.Play("KHJ/Effect/Bird/Bird2");
					break;
				case 2:
					SoundManager.Sound.Play("KHJ/Effect/Bird/Bird3");
					break;
				case 3:
					SoundManager.Sound.Play("KHJ/Effect/Bird/Bird4");
					break;
			}

			timer = UnityEngine.Random.Range(20.0f, 30.0f);
			value = UnityEngine.Random.Range(0, 3);
		}

	}

}
