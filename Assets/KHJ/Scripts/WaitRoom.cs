using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

// 대기방 스크립트
public class WaitRoom : MonoBehaviour
{
    // 플레이어 스폰 포지션
    public Transform PlayerSpawnPos;
    // 플레이어 오브젝트
    public GameObject Player;
    // 화면 가리기 위한 오브젝트
    public Image BlackImage;
	// 비디오 플레이어
	public VideoPlayer videos;

    void Start()
    {
		ksjPlayerManager.Instance.getCanvas();

		// 사운드 비우기(이전 사운드 없애기)
		SoundManager.Sound.Clear();

		videos.enabled = false;

		StartCoroutine(StartVideo());

        BlackImage.gameObject.SetActive(true);

        // 플레이어 오브젝트위치 설정
        Player.transform.position = PlayerSpawnPos.position;
    }


    // 충돌 검사(포탈만 충돌 콜라이더 가지고있음)
    private void OnTriggerEnter(Collider other)
    {
        // 플레이어 충돌 시
        if(other.CompareTag("Player"))
        {

            // 화면 어두워지기
			StartCoroutine(FadeInImage(BlackImage));

			// 씬 넘어가기 전 사운드 클리어
			SoundManager.Sound.Clear();
			// 스테이지 선택 씬으로 이동
			StartCoroutine(GoToStageScene());

        }
    }
	// 이미지 점점 옅어짐
	IEnumerator FadeOutImage(Image image)
	{
		image.gameObject.SetActive(true);
		image.color = new Color(image.color.r, image.color.g, image.color.b, 1);

		float endTime = 1.0f;
		float curTime = 0.0f;
		float speed = (1 * Time.deltaTime) / (endTime);
		float curColor = 1;
		image.color = new Color(0, 0, 0, curColor);

		while (curTime < endTime)
		{
			yield return null;
			curColor -= speed;
			image.color = new Color(0, 0, 0, curColor);
			curTime += Time.deltaTime;

			Debug.Log($"{curColor}");
			//if (curColor <= 0.1f)
			//	break;
		}
		image.gameObject.SetActive(false);

	}
	// 이미지 점점 짙어짐
	IEnumerator FadeInImage(Image image)
	{
		image.gameObject.SetActive(true);
		image.color = new Color(image.color.r, image.color.g, image.color.b, 0);

		float endTime = 1.0f;
		float curTime = 0.0f;
		float speed = (1 * Time.deltaTime) / (endTime);
		float curColor = 0;
		image.color = new Color(image.color.r, image.color.g, image.color.b, curColor);

		while (curTime < endTime)
		{
			yield return null;
			curColor += speed;
			image.color = new Color(image.color.r, image.color.g, image.color.b, curColor);
			curTime += Time.deltaTime;
			if (curColor >= 1.1f)
				break;
		}
		image.color = new Color(image.color.r, image.color.g, image.color.b, 1);

	}
	// 스테이지 선택 씬으로 이동
	IEnumerator GoToStageScene()
	{
		yield return new WaitForSeconds(1.0f);

		SceneManager.LoadScene("Select");
	}

	// 비디오출력 ~ 화면출력
	IEnumerator StartVideo()
	{
		if (!GameObject.Find("PlayerManager").GetComponent<ksjPlayerManager>().IsGameStart)
		{
			var pm = ksjPlayerManager.Instance;
			GameObject.Find("PlayerManager").GetComponent<ksjPlayerManager>().IsGameStart = true;
			
			pm.IsVideoEnded = false;
			pm.MainCanvas.SetActive(false);
			

			videos.enabled = true;
			yield return new WaitForSeconds(0.1f);
			BlackImage.gameObject.SetActive(false);
			SoundManager.Sound.Play("KHJ/Effect/StartVideoSound");

			// 비디오 재생 확인
			while (true)
			{
				yield return null;

				if (videos.time >= (videos.length * 0.99f))
					break;

			}
			yield return new WaitForSeconds(0.3f);
			videos.enabled = false;
			pm.IsVideoEnded = true;
			pm.MainCanvas.SetActive(true);

            SoundManager.Sound.Clear();

			SoundManager.Sound.Play("KHJ/Bgm/WaitRoomBgm", SoundManager.SoundType.Bgm);

			BlackImage.gameObject.SetActive(true);
			yield return new WaitForSeconds(1f);
		}
		// 화면 밝아지기
		StartCoroutine(FadeOutImage(BlackImage));

	}

}
