using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// 메인 메뉴
public class Menu : MonoBehaviour
{
    // 부모객체
    public Title myParant;
    // 선택지 넘버
    public int SelectNum;
    // 하위 메뉴들
    public GetMenuNumber[] Menus;

    // 입력확인
    public bool isInput = false;
    // 메뉴 열렸는지 확인
    public bool isOpenMenu = false;

	// UI 레이캐스트
	[SerializeField]
	private GameObject canv;
	GraphicRaycaster m_RayCaster;
	PointerEventData m_PointerEventData;
	EventSystem _event;

    // 세팅 메뉴
    public SettingMenu settings;

    // 제작진 Panel
    public CreditMenu credits;

	void Start()
    {
        // 최초 넘버링 0 = Start
        SelectNum = 0;

        // 하위 메뉴리스트 전체 이니셜라이징
        for (int i =  0; i < Menus.Length; i++)
        {
            Menus[i].Init(this);
            if (i == 0)
            {
                Menus[i].spriteImage.enabled = true;
                Menus[i].menuText.enabled = false;
            }
            else
            {
                Menus[i].spriteImage.enabled = false;
                Menus[i].menuText.enabled = true;
			}
        }

		// 각각의 메뉴들 이니셜라이징, 비활성화
		settings.Init(this);
        settings.gameObject.SetActive(false);

        credits.Init(this);
        credits.gameObject.SetActive(false);

		// UI 레이캐스트
		m_RayCaster = canv.GetComponent<GraphicRaycaster>();
		_event = GameObject.Find("EventSystem").GetComponent<EventSystem>();


	}

	void Update()
    {
        // 키입력 확인
        InputKey();
        // UI 레이캐스트
        CheckRay();
        // 메뉴 이미지 체인지
        MenuImageChanged();
        // 하위 메뉴 오픈
        SelectCheck();
	}     
    // 키입력
    void InputKey()
    {
        // 키입력이 확인되면 이후 입력은 return
        if (isInput)
            return;
        // w 입력 시(메뉴 위로 올리기)
        if(Input.GetKeyDown(KeyCode.W))
        {
            // 선택넘버링이 0보다 작거나 같을때(0일떄, 처음 메뉴)
            if (SelectNum <= 0)
                // 리턴
                return;
            // 아니라면 -1
            SelectNum--;
        }
        // s입력 시(메뉴 아래로 내리기)
        if(Input.GetKeyDown(KeyCode.S))
        {
            // 선택 넘버링이 3보다 크거나 작다면(3일떄, 마지막 메뉴)
            if (SelectNum >= 3)
                // 리턴
                return;
            // 아니라면 +1
            SelectNum++;
        }
        // 스페이스바 입력 시
        if(Input.GetKeyDown(KeyCode.Space))
        {            
            Debug.Log("Input Space");
            // 추가 입력 제한
            isInput = true;
            // 메뉴 오픈
            isOpenMenu = true;
        }

    }

    void CheckRay()
    {
		// 키입력이 확인되면 이후 입력은 return
		if (isInput)
			return;

		m_PointerEventData = new PointerEventData(_event);
		m_PointerEventData.position = Input.mousePosition;
		List<RaycastResult> results = new List<RaycastResult>();

        // 마우스에서 레이캐스트 발사
		m_RayCaster.Raycast(m_PointerEventData, results);

        // 마우스가 ui위에 있을때
		if (_event.IsPointerOverGameObject())
		{
            // 충돌한 객체의 데이터 가져오기
			GameObject hit = results[0].gameObject;

            // 데이터가 비어있지 않다면
			if (hit != null)
			{
                // 해당 객체의 이름 출력하고
				Debug.Log(hit.gameObject.name);

                // 메뉴 데이터 가져오기
                GetMenuNumber menuNum = hit.gameObject.GetComponent<GetMenuNumber>();

                // 메뉴 데이터도 비어있지 않다면
                if(menuNum != null)
                {
                    if(SelectNum != menuNum.MenuNumber)
						SoundManager.Sound.Play("KHJ/Effect/ChangeMenu");

					// 선택지 설정
					SelectNum = menuNum.MenuNumber;
                }

			}

		}
	}

    // 마우스 충돌 시 넘버링 받아오기
    public void SetInputMouse(int num)
    {
        if (isInput || isOpenMenu)
            return;
        SelectNum = num;
    }
    // 마우스 클릭 검사
    public void MouseInput(int num)
    {
		if (isInput || isOpenMenu)
			return; 
        SelectNum = num;
        isInput = true;
        isOpenMenu = true;
	}

    // 메뉴 이미지 변경
    void MenuImageChanged()
    {
		for (int i = 0; i < Menus.Length; i++)
		{
			if (i == SelectNum)
			{
                if (!Menus[i].spriteImage.enabled)
                {
                    Menus[i].spriteImage.enabled = true;
                    StartCoroutine(SpriteAction(Menus[i].spriteImage));
                    Menus[i].menuText.enabled = false;
                }
			}
			else
			{
                if (Menus[i].spriteImage.enabled)
                {
                    Menus[i].spriteImage.enabled = false;
                    Menus[i].menuText.enabled = true;
                }
			}
		}
	}

    // 메뉴 이미지 액션
    IEnumerator SpriteAction(Image image)
    {
        // 스케일 조정
        // 0.5 -> 1.2 -> 1.0
        Vector3 OriginScale = new Vector3(1, 1, 1);

        // 스케일 커지는 액션
        image.transform.localScale = new Vector3(1, 0.5f, 1);
        Vector3 TargetScale = new Vector3(0, 1.2f, 0) - new Vector3(0, 0.5f, 0);
        TargetScale = TargetScale.normalized;
        float speed = 5.0f;

        while(true)
        {
            yield return null;

            if (!image.enabled)
            {
                image.transform.localScale = new Vector3(1, 1, 1);
                break;
            }
            else if (image.transform.localScale.y >= 1.2f)
                break;

            image.transform.localScale += TargetScale * speed * Time.deltaTime;


        }

        // 스케일 작아지는 액션
        TargetScale = new Vector3(0, 1.0f, 0) - new Vector3(0, 1.2f, 0);
        TargetScale = TargetScale.normalized;
        while(true)
        {

			if (!image.enabled)
			{
				image.transform.localScale = new Vector3(1, 1, 1);
				break;
			}
			else if (image.transform.localScale.y <= 1.0f)
				break;

			image.transform.localScale += TargetScale * speed * Time.deltaTime;

		}


	}


    // 메뉴 오픈
    void SelectCheck()
    {
        // 추가 입력 제한
        if (!isOpenMenu)
            return;

        // 메뉴 클릭 시 사운드
        SoundManager.Sound.Play("KHJ/Effect/SelectMenu");

        // 선택 넘버링에 따른 메뉴 구분
        switch(SelectNum)
        {
            // 플레이
            case 0:
                SelectStart();
                break;
            // 세팅
            case 1:
                SelectSetting();
                break;
            // 크레딧
            case 2:
                SelectCredit();
                break;
            // 나가기
            case 3:
                SelectQuit();
                break;
        }
        // 추가 입력 제한
        isOpenMenu = false;

    }

    // 플레이 선택 시 함수
    void SelectStart()
    {
        Debug.Log("Select Start");
        SoundManager.Sound.Play("KHJ/Effect/StartGame");
        // 화면 가린 후 씬이동
        StartCoroutine(StartMethodCor());
	}
	// 플레이 선택 시 화면 가리기 위한 코루틴
	IEnumerator WaitAndActive()
	{
		yield return new WaitForSeconds(1.5f);
		myParant.SceneHideImage.gameObject.SetActive(true);

        // 여기서 플레이씬으로 이동
        SceneManager.LoadScene("LoadWaitRoom");

	}
    // 스타트 함수 용 코루틴
    IEnumerator StartMethodCor()
    {
        yield return new WaitForSeconds(0.5f);
		StartCoroutine(myParant.FadeInImage(myParant.SceneHideImage));
		StartCoroutine(WaitAndActive());
	}

	void SelectSetting()
    {
		Debug.Log("Select Setting");

        if (!settings.gameObject.activeSelf)
        {
            settings.gameObject.SetActive(true);
            settings.Init(this);
            StartCoroutine(MenuPanelAction(settings.gameObject));
        }

	}
    // 세팅 메뉴 닫기
    public void CloseSettingMenu()
    {
        if (settings.gameObject.activeSelf)
        {
            SoundManager.Sound.Play("KHJ/Effect/CloseMenu");
            settings.gameObject.SetActive(false);
        }

		isInput = false;
		isOpenMenu = false;
	}

    // 크레딧(제작진) 선택 시 함수
	void SelectCredit()
    {
		Debug.Log("Select Credit");

        if (!credits.gameObject.activeSelf)
        {
            credits.gameObject.SetActive(true);
            StartCoroutine(MenuPanelAction(credits.gameObject));
        }

	}
    // 크레딧 메뉴 닫기
    public void CloseCreditMenu()
    {
        if (credits.gameObject.activeSelf)
        {
            SoundManager.Sound.Play("KHJ/Effect/CloseMenu");
			credits.gameObject.SetActive(false);
        }

		isInput = false;
		isOpenMenu = false;
	}

    // 메뉴 열때 액션
    IEnumerator MenuPanelAction(GameObject obj)
    {
        obj.transform.position -= new Vector3(150, 0, 0);

        Vector3 Dir = (transform.position - obj.transform.position).normalized;
        float endTime = 0.1f;
        float curTime = 0;
        
        float speed = (150.0f)/endTime;

        while(curTime < endTime)
        {
            yield return null;
            if (!obj.activeSelf)
                break;

            curTime += Time.deltaTime;
            obj.transform.position += Dir * speed * Time.deltaTime;
        }
        obj.transform.position = transform.position;

    }

    // 나가기 선택 시 함수
	void SelectQuit()
    {
		Debug.Log("Select Quit");
        StartCoroutine(CloseGame());
	}
    // 게임 종료코루틴
    IEnumerator CloseGame()
    {
        // 입력 확인 후 잠깐 대기
        yield return new WaitForSeconds(0.5f);
        // 화면 가려짐
		StartCoroutine(myParant.FadeInImage(myParant.SceneHideImage));
        // 화면이 완전히 가려질때 까지 대기
        yield return new WaitForSeconds(1.0f);

        // 유니티 에디터에서 실행 시 플레이모드 종료
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        
        // 아니라면 게임 종료
#else
        Application.Quit();
#endif


    }
}
