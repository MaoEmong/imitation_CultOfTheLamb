using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

// 타이틀
public class Title : MonoBehaviour
{
    // 화면 가리는 용도의 이미지
    public Image SceneHideImage;

    // 타이틀 이미지/택스트
    public Image TitleImage;
    public Text TitleText;

	// True일떄 입력 가능
	bool isInput = false;

	// 카메라
	public Camera MainCamera;

	// 카메라 도착 위치
	public Transform CameraGoalPos1;
	public Transform CameraGoalPos2;
	public Transform CameraGoalPos3;
	// 메뉴 UI
	public Menu myMenu;

	// 캐릭터 오브젝트
	public GameObject CharactorObject;

	// 카메라 도착과 동시에 비활성화 시킬 오브젝트들
	public GameObject[] ActiveFalseObj;
	// 흔들림 효과를 받을 오브젝트들
	public GameObject[] ShakeActionObj;

	// 비디오 플레이어
	public VideoPlayer videos;
    void Start()
    {
		// 카메라 최초 위치 설정
		MainCamera.transform.position = new Vector3(0, 3, -50);

		// 캐릭터 오브젝트 비활성화
		CharactorObject.SetActive(false);


		// 사운드 조절
		SoundManager.Sound.SetBGMVol(0.15f);
		SoundManager.Sound.SetEffectVol(0.15f);

		// 타이틀 UI들 초기화
		TitleInit();

		// 최초 화면 밝아짐
		StartCoroutine(titleInitImage());


		// 메뉴에 부모객체 등록
		myMenu.myParant = this;

		foreach (var n in ShakeActionObj)
		{
			Transform[] Childs = n.transform.GetComponentsInChildren<Transform>();
			foreach(var v in Childs)
			{
				StartCoroutine(ShakeAction(v.gameObject));
			}

		}

    }

    void Update()
    {
		CheckFirstInput();

    }

	// UI이미지들 초기화(비활성화)
	void TitleInit()
	{
		SceneHideImage.gameObject.SetActive(true);
		TitleImage.gameObject.SetActive(false);
		TitleText.gameObject.SetActive(false);
		myMenu.gameObject.SetActive(false);
	}

	// 이미지 점점 옅어짐
	public IEnumerator FadeOutImage(Image image)
    {
		image.gameObject.SetActive(true);
		image.color = new Color(image.color.r, image.color.g, image.color.b, 1);

		float endTime = 1.0f;
		float curTime = 0.0f;
		float speed = (1f * Time.deltaTime) / endTime;
		float curColor = 1;
		image.color = new Color(image.color.r, image.color.g, image.color.b, curColor);

		while (curTime < endTime)
		{
			yield return null;
			curColor -= speed;
			image.color = new Color(image.color.r, image.color.g, image.color.b, curColor);
			curTime += Time.deltaTime;
			if (curColor <= 0.1f)
				break;
		}
		image.gameObject.SetActive(false);

	}
	// 이미지 점점 짙어짐
	public IEnumerator FadeInImage(Image image)
	{
		image.gameObject.SetActive(true);
		image.color = new Color(image.color.r, image.color.g, image.color.b, 0);

		float endTime = 1.0f;
		float curTime = 0.0f;
		float speed = (1f * Time.deltaTime) / endTime;
		float curColor = 0;
		image.color = new Color(image.color.r, image.color.g, image.color.b, curColor);

		while (curTime < endTime)
		{
			yield return null;
			curColor += speed;
			image.color = new Color(image.color.r, image.color.g, image.color.b, curColor);
			curTime += Time.deltaTime;
			if (curColor >= 1.2f)
				break;
		}
		image.color = new Color(image.color.r, image.color.g, image.color.b, 1);

	}

	// 텍스트 브레쓰(깜빡임) 효과
	IEnumerator TextBreath(Text _text)
	{
		float breathDelay = 0.13f;

		Color textColor = _text.color;

		while (true)
		{
			yield return new WaitForSeconds(0.1f);

			_text.color -= new Color(textColor.r, textColor.g, textColor.b, breathDelay);

			// 텍스트의 투명도가 각각의 값 사이를 순환함
			if (_text.color.a >= 0.9f || _text.color.a <= 0.2f)
				breathDelay *= -1;
		}
	}
	// 최초 이미지들 세팅
	IEnumerator titleInitImage()
	{
		yield return new WaitForSeconds(0.15f);
		SceneHideImage.gameObject.SetActive(false);

		// 비디오 재생 확인
		while(true)
		{
			yield return null;

			if (videos.time>= (videos.length* 0.99f))
				break;
			
		}

		yield return new WaitForSeconds(0.5f);

		// 비디오화면 비활성화
		videos.enabled = false;

		// UI작동
		StartCoroutine(FadeOutImage(SceneHideImage));
		// 타이틀 사운드 출력
		SoundManager.Sound.Play("KHJ/BGM/TitleBgm", SoundManager.SoundType.Bgm);


		yield return new WaitForSeconds(1.5f);
		StartCoroutine(FadeInImage(TitleImage));
		StartCoroutine(FadeInImage(TitleImage.transform.GetChild(0).GetComponent<Image>()));
		yield return new WaitForSeconds(1.5f);
		// 타이틀 텍스트 깜빡임
		TitleText.gameObject.SetActive(true);
		StartCoroutine(TextBreath(TitleText));
		isInput = true;

	}
	// 키입력 체크
	void CheckFirstInput()
	{
		// 최초 키입력 1번만 받을것
		if (!isInput || !TitleText.gameObject.activeSelf)
			return;

		// 어떠한 키든 입력 시 더이상의 키입력을 받지 않음
		if (Input.anyKey)
		{
			isInput = false;

			SoundManager.Sound.Play("KHJ/Effect/TouchScreen",SoundManager.SoundType.Effect);
			StartCoroutine(TouchScreen());
		}

	}
	// 카메라 움직임
	IEnumerator CameraMove()
	{
		// 1번 위치로 이동
		Vector3 Dir = CameraGoalPos1.position - MainCamera.transform.position;
		Dir = Dir.normalized;
		float dist = Vector3.Distance(MainCamera.transform.position, CameraGoalPos1.position);
		float time = 2.0f;
		float speed = dist / time;
		float curtime = 0;
		while(curtime < time)
		{
			yield return null;
			curtime += Time.deltaTime;

			MainCamera.transform.position += Dir * speed * Time.deltaTime;
		}

		// 2번 위치로 이동
		Dir = CameraGoalPos2.position - MainCamera.transform.position;
		Dir = Dir.normalized;
		dist = Vector3.Distance(MainCamera.transform.position, CameraGoalPos2.position);
		time = 1.0f;
		speed = (dist / time);
		curtime = 0;

		while(curtime<time)
		{
			yield return null;
			curtime += Time.deltaTime;
			MainCamera.transform.position += Dir * speed * Time.deltaTime;

		}

		// 3번 위치로 이동함과 동시에 메뉴 활성화, 불필요한 오브젝트 비활성화
		myMenu.gameObject.SetActive(true);

		foreach (var n in ActiveFalseObj)
			n.gameObject.SetActive(false);

		yield return new WaitForSeconds(0.1f);
		Dir = CameraGoalPos3.position - MainCamera.transform.position;
		Dir = Dir.normalized;
		dist = Vector3.Distance(MainCamera.transform.position, CameraGoalPos3.position);
		time = 1.0f;
		speed = (dist / time);
		curtime = 0;

		while (curtime < time)
		{
			yield return null;
			curtime += Time.deltaTime;
			MainCamera.transform.position += Dir * speed * Time.deltaTime;

		}


	}	
	// 흔들림 액션
	IEnumerator ShakeAction(GameObject obj)
	{
		// 최초 포지션
		Vector3 OriginPos = obj.transform.position;

		// 수정할 포지션값
		float randomX;
		float randomY;

		while(true)
		{
			yield return new WaitForSeconds(0.3f);

			randomX = Random.Range(-0.01f, 0.01f);
			randomY = Random.Range(-0.01f, 0.01f);

			obj.transform.position = OriginPos + new Vector3(randomX, randomY, 0);

			// 오브젝트가 비활성화 상태라면 break
			if (!obj.activeSelf)
				break;

		}


	}
	// 화면 터치
	IEnumerator TouchScreen()
	{
		yield return new WaitForSeconds(1.0f);

		// 캐릭터 오브젝트 활성화
		CharactorObject.SetActive(true);

		SoundManager.Sound.Play("KHJ/Effect/CameraMove", SoundManager.SoundType.Effect);

		// 타이틀 이미지 사라짐과 동시에 카메라 이동
		StartCoroutine(FadeOutImage(TitleImage));
		StartCoroutine(FadeOutImage(TitleImage.transform.GetChild(0).GetComponent<Image>()));
		TitleText.gameObject.SetActive(false);
		StartCoroutine(CameraMove());

	}

}
