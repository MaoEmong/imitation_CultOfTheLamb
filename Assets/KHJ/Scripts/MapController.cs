using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 맵 컨트롤러
// 4개의 필드묶음 제작
public class MapController : MonoBehaviour
{
	// 현재 스테이지 넘버링
	public int StageNumber = 0;

	// 맵에 존재하는 필드[스테이지 수][각 스테이지에 들어가는 필드]
	//public Field[4][] Fields;
	public Field[] Fields1;
	public Field[] Fields2;
	public Field[] Fields3;
	public Field[] Fields4;

	public List<Field[]> TotalField;

	// 플레이어 최초 스폰 포지션[스테이지 넘버]
	public Transform[] PlayerSpawnPos;
	// 플레이어
	public GameObject Player;

	// 화면 FadeInOut 이미지
	public Image ChangeImage;

	private void Start()
	{
		var pm = ksjPlayerManager.Instance;
		pm.getCanvas();

		ChangeImage = pm.FadeImage;

        // 화면 전환용 이미지 활성화
        ChangeImage.gameObject.SetActive(true);
		ChangeImage.color = new Color(ChangeImage.color.r, ChangeImage.color.g, ChangeImage.color.b, 1);

		// 필드 오브젝트 정리
		TotalField = new() 
		{
			Fields1,Fields2,Fields3,Fields4,
		};

		StageNumber = ksjPlayerManager.Instance.CurStage;

		// ksjPlayerManager.Instance.getCanvas();

		// 스테이지 초기화
		for (int i = 0; i < TotalField.Count; i++)
		{
			// 현재 스테이지 이니셜라이징
			if( i == StageNumber)
				foreach(var n in TotalField[StageNumber])
				{
					n.gameObject.SetActive(true);
					n.Init(this);
				}	
			// 그외 스테이지 비활성화
			else
				foreach(var n in TotalField[i])
				{
					n.gameObject.SetActive(false);
				}
		}

		// 현재 스테이지의 필드가 가지는 모든 포탈들 초기화
		for(int i  = 0; i < TotalField[StageNumber].Length; i++)
		{
			for(int j = 0; j < TotalField[StageNumber][i].Portals.Length; j++) 
			{
				TotalField[StageNumber][i].Portals[j].Init(this);
			}
		}

		// 플레이어 위치시키기
		// Player = GameManager.Instance.Player;
		Player.transform.position = PlayerSpawnPos[StageNumber].position;

		// 화면 전환 이미지 테스트용
		//FadeInOutImage();

		// 최초 씬 시작 시 화면 효과
		StartCoroutine(FadeIn());


		//=============================== 사운드 추가 
		SoundManager.Sound.Play("KHJ/Bgm/Field/Stage_Standard", SoundManager.SoundType.Bgm);
		StartCoroutine(BirdSound());

	}

	// 화면 전환 효과
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

	// 새소리 코루틴
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
