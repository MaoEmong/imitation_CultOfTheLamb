using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// 스테이지 변경 UI 스크립트
/*
 최초 ClearStage값은 0 == 클리어 없음
 1 == 1스테이지 클리어
 ...
 해당 값에 맞춰 selecNum의 최초값이 정해짐

 스테이지 선택 후 메인필드로 넘어갈때 스테이지 정보에 selectNum값을 넘겨주어야
 씬 로드 시 해당 넘버를 가진 스테이지 초기화가 진행됨

 메인필드씬으로 넘어갈때 스테이지 넘버를 저장할 변수와
 메인필드에서 셀렉트씬으로 넘어갈때 스테이지 클리어 확인용 변수
 총 두가지 변수를 게임매니저에 저장해야됨
 */
public class StageSelect : MonoBehaviour
{
    // 스테이지 이미지
    public Image[] StageBackgroundImage;
    // 스테이지 아웃라인
    public Image[] StageOutlineImage;
    // 스테이지 심볼
    public Image[] StageSymbolImage;
    // UI 백그라운드 심볼
    public Image[] UIBackGroundSymbolImage;
    // 이미지를 정렬한 레이아웃 판넬
    public GameObject LayoutPanel;
    // UI 전체 BackGround
    public RectTransform UIBackground;

    // FadeInOut효과용 이미지
    public Image FadeInOutImage;

    // 스테이지 선택 넘버
    [SerializeField]
    int selectNum = 0;

    // 클리어 스테이지
    public int ClearStage;

    // 플레이어 위치 이미지
    public Image CrownImage;

    // 플레이어 매니저
    ksjPlayerManager PM;

    // 추가 입력 제한용 변수
    bool StopInput = false;

    void Start()
    {
        SoundManager.Sound.Clear();

        SoundManager.Sound.Play("KHJ/Effect/SelectIntro");

        // 최초 화면 좌표는 선택한 넘버에 따라 변화하고
        // 현재 클리어 한 스테이지 정보에 맞춰 최초 화면 좌표 설정

        // 플레이어 데이터를 가져오고 클리어스테이지 변경
        PM = GameObject.Find("PlayerManager").GetComponent<ksjPlayerManager>();
        if(PM != null )
        {
            ClearStage = PM.CurStage;
        }
        
		if (ClearStage == 0)
			selectNum = 0;
        else
			selectNum = ClearStage - 1;


		// 최초 왕관이미지 비활성화
		CrownImage.enabled = false;

		// 최초 Select씬 액션
		StartCoroutine(InitCoroutine());
    }

    void Update()
    {
        CheckStage();
        OnKeyboard();
    }

    // 백그라운드 심볼 이미지 브레쓰 효과 코루틴
    IEnumerator BackGroundSymbolBreathAction()
    {
        float randTime;
        foreach(var n in UIBackGroundSymbolImage) 
        {
			randTime = Random.Range(0.0f, 1.0f);
            yield return null;
            StartCoroutine(ImageBreathAction(n, randTime));

        }
        
    }
    // 이미지 브레쓰 효과 코루틴
    IEnumerator ImageBreathAction(Image img,float DelayTime)
    {
        yield return new WaitForSeconds(DelayTime);
		float breathDelay = 0.13f;

		while (true)
		{
			yield return new WaitForSeconds(0.1f);

			img.color -= new Color(0, 0, 0, breathDelay);

			// 텍스트의 투명도가 각각의 값 사이를 순환함
			if (img.color.a >= 0.9f || img.color.a <= 0.2f)
				breathDelay *= -1;
		}
	}

    // 최초 실행 시 매뉴가 위에서 아래로 살짝 내려오는 액션 구현
    IEnumerator BackgroundMove()
    {
        Vector3 dir = (transform.position - UIBackground.position).normalized;
        float endTime = 0.3f;
        float curTime = 0.0f;
        float speed = (500.0f)/endTime;

		for (int i = 0; i < selectNum; i++)
		{
            MoveUpPanel();
		}

		while (curTime<endTime)
        {
            yield return null;

            curTime += Time.deltaTime;

            UIBackground.position += dir * speed * Time.deltaTime;

        }
        
    }

    // 선택지 색상 변화
    void CheckStage()
    {
        for(int i = 0; i < StageBackgroundImage.Length; i++)
        {
            // i값이 클리어한 스테이지 값보다 작다면(이미 클리어한 스테이지라면)
			if (i < ClearStage)
			{
				// 아웃라인과 심볼 이미지를 클리어 색상(시안)으로 바꾸기
				StageOutlineImage[i].color = Color.cyan;
				StageSymbolImage[i].color = Color.cyan;
			}

			// i가 현재 선택한 넘버와 같다면
			else if (i == selectNum)
            {
                // 아웃라인 색상은 빨간색, 심볼은 흰색
				StageOutlineImage[i].color = Color.red;
				StageSymbolImage[i].color = Color.white;
			}
            // 둘 다 아니라면 색상은 흰색
            else
            {
				StageOutlineImage[i].color = Color.white;
				StageSymbolImage[i].color = Color.white;
			}
		}
    }

    // StageUI Panel 업 다운 함수(바로 이동)
    void MoveDownPanel()
    {
		LayoutPanel.transform.position -= new Vector3(0, -200);
	}
	void MoveUpPanel()
    {
		LayoutPanel.transform.position += new Vector3(0, -200);
	}
    // StageUI Panel 업 다운 코루틴함수
    IEnumerator  PanelDownCoroutine()
    {
		float endTime = 0.5f;
		float curTime = 0.0f;
		float speed = 200.0f / endTime;

		while (curTime < endTime)
		{
			yield return null;

			curTime += Time.deltaTime;

			UIBackground.position -= new Vector3(0, -200).normalized * speed * Time.deltaTime;

		}
	}
	IEnumerator PanelUpCoroutine()
	{
		float endTime = 0.5f;
		float curTime = 0.0f;
		float speed = 200.0f / endTime;
        SoundManager.Sound.Play("KHJ/Effect/SelectStage");
		while (curTime < endTime)
		{
			yield return null;

			curTime += Time.deltaTime;

			UIBackground.position += new Vector3(0, -200).normalized * speed * Time.deltaTime;

		}
	}


    //=============================================== 입력
	// 키보드 입력
	void OnKeyboard()
    {
        if (StopInput)
            return;

        // W키 입력
        if(Input.GetKeyDown(KeyCode.W))
        {
            // 선택중인 숫자가 맥스값일 경우 입력 취소
            if (selectNum >= StageBackgroundImage.Length - 1)
                return;
            // 선택중인 숫자가 클리어스테이지값과 같다면 return
            else if (selectNum >= ClearStage)
                return;
            // 선택값++
            selectNum++;
            // 레이아웃 판넬 같이 올림
            //MoveUpPanel();
            StartCoroutine(PanelUpCoroutine());
		}
        // SpaceBar 입력
		if (Input.GetKeyDown(KeyCode.Space))
        {
            // 해당 스테이지로 이동
            Debug.Log($"Go To Stage{selectNum+1}");

            StartCoroutine(GoToLoadingScene());
        }

    }

    // 마우스 입력(입력은 UI_StageImage.cs에서 관리)
    public void SetSelectNumber(int number)
    {
        if (StopInput)
            return;

        // 클리어 스테이지는 1부터 시작이라고 생각
        if (number > ClearStage)
            return;

        // 클릭한 스테이지 넘버가 현재 넘버보다 클 경우
        if (selectNum < number)
        {
			// 큰 수치 만큼 레이아웃 판넬 위로 올림
			for (int i = 0; i < number - selectNum; i++)
            {
                //MoveUpPanel();
				StartCoroutine(PanelUpCoroutine());
			}

            selectNum = number;
		}

		// 클릭한 스테이지 넘버가 현재 넘버와 같을 경우
		else if(selectNum == number)
        {
            // 해당 스테이지로 이동
            Debug.Log($"Go To Stage{selectNum + 1}");

			selectNum = number;

			StartCoroutine(GoToLoadingScene());

		}
    }

    IEnumerator FadeOutImage(float delay, float time)
    {
        FadeInOutImage.gameObject.SetActive(true);
		yield return new WaitForSeconds(delay);

		float endTime = time;
		float curTime = 0.0f;
		float speed = (0.1f / endTime)/endTime;
        float curColor = 0;
        FadeInOutImage.color = new Color(FadeInOutImage.color.r, FadeInOutImage.color.g, FadeInOutImage.color.b, curColor);

		while (curTime < endTime)
		{
			yield return null;
            curColor += speed;
			FadeInOutImage.color = new Color(FadeInOutImage.color.r, FadeInOutImage.color.g, FadeInOutImage.color.b, curColor);
            curTime += Time.deltaTime;
		}


	}
    IEnumerator FadeInImage(float delay, float time)
    {
		yield return new WaitForSeconds(delay);

		float endTime = time;
		float curTime = 0.0f;
		float speed = (0.1f / endTime) / endTime;
		float curColor = 1;
		FadeInOutImage.color = new Color(FadeInOutImage.color.r, FadeInOutImage.color.g, FadeInOutImage.color.b, curColor);

		while (curTime < endTime)
		{
			yield return null;
			curColor -= speed;
			FadeInOutImage.color = new Color(FadeInOutImage.color.r, FadeInOutImage.color.g, FadeInOutImage.color.b, curColor);
			curTime += Time.deltaTime;
            if (curColor >= 1.0f)
                break;
		}
        FadeInOutImage.gameObject.SetActive(false);

	}

    // 코루틴 초기화 설정
    IEnumerator InitCoroutine()
    {
        FadeInOutImage.gameObject.SetActive(true);
		StartCoroutine(FadeInImage(0, 1));
        yield return new WaitForSeconds(0.3f);
		StartCoroutine(BackgroundMove());
		StartCoroutine(BackGroundSymbolBreathAction());
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(CrownOpen());
	}

    // 로딩씬으로 이동
    IEnumerator GoToLoadingScene()
    {
        StopInput = true;

        yield return new WaitForSeconds(0.3f);
        StartCoroutine(CrownMove());
        yield return new WaitForSeconds(0.7f);
        StartCoroutine(FadeOutImage(0, 1));
        yield return new WaitForSeconds(1.0f);
        // 게임매니저에 스테이지 데이터 넘겨주기
        ksjPlayerManager.Instance.CurStage = selectNum;

		SceneManager.LoadScene("Loading");
	}

    // 왕관 등장 액션
    IEnumerator CrownOpen()
    {
        CrownImage.enabled = true;

        CrownImage.transform.position = StageBackgroundImage[selectNum].transform.position;

		// 스케일 커지는 액션
        CrownImage.transform.localScale = Vector3.zero;
		Vector3 TargetScale = new Vector3(0.7f, 0.7f, 0.7f) - CrownImage.transform.localScale;
		TargetScale = TargetScale.normalized;
		float speed = 5.0f;

		while (true)
		{
			yield return null;

			if (!CrownImage.enabled)
			{
				CrownImage.transform.localScale = new Vector3(1, 1, 1);
				break;
			}
			else if (CrownImage.transform.localScale.x >= 0.7f)
				break;

			CrownImage.transform.localScale += TargetScale * speed * Time.deltaTime;


		}

	}

    // 왕관 움직이는 액션
    IEnumerator CrownMove()
    {

		Vector3 Dir = (StageBackgroundImage[selectNum].transform.position - CrownImage.transform.position).normalized;
		float endTime = 0.4f;
		float curTime = 0;
        float dist = Vector3.Distance(StageBackgroundImage[selectNum].transform.position, CrownImage.transform.position);
		float speed = (dist) / endTime;

        SoundManager.Sound.Play("KHJ/Effect/SelectMove");

		while (curTime < endTime)
		{
			yield return null;
			if (!CrownImage.enabled)
				break;

			curTime += Time.deltaTime;
			CrownImage.transform.position += Dir * speed * Time.deltaTime;
		}
		CrownImage.transform.position = StageBackgroundImage[selectNum].transform.position;


	}
}
