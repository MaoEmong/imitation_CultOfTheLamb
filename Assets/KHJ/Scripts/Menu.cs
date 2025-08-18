using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// ���� �޴�
public class Menu : MonoBehaviour
{
    // �θ�ü
    public Title myParant;
    // ������ �ѹ�
    public int SelectNum;
    // ���� �޴���
    public GetMenuNumber[] Menus;

    // �Է�Ȯ��
    public bool isInput = false;
    // �޴� ���ȴ��� Ȯ��
    public bool isOpenMenu = false;

	// UI ����ĳ��Ʈ
	[SerializeField]
	private GameObject canv;
	GraphicRaycaster m_RayCaster;
	PointerEventData m_PointerEventData;
	EventSystem _event;

    // ���� �޴�
    public SettingMenu settings;

    // ������ Panel
    public CreditMenu credits;

	void Start()
    {
        // ���� �ѹ��� 0 = Start
        SelectNum = 0;

        // ���� �޴�����Ʈ ��ü �̴ϼȶ���¡
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

		// ������ �޴��� �̴ϼȶ���¡, ��Ȱ��ȭ
		settings.Init(this);
        settings.gameObject.SetActive(false);

        credits.Init(this);
        credits.gameObject.SetActive(false);

		// UI ����ĳ��Ʈ
		m_RayCaster = canv.GetComponent<GraphicRaycaster>();
		_event = GameObject.Find("EventSystem").GetComponent<EventSystem>();


	}

	void Update()
    {
        // Ű�Է� Ȯ��
        InputKey();
        // UI ����ĳ��Ʈ
        CheckRay();
        // �޴� �̹��� ü����
        MenuImageChanged();
        // ���� �޴� ����
        SelectCheck();
	}     
    // Ű�Է�
    void InputKey()
    {
        // Ű�Է��� Ȯ�εǸ� ���� �Է��� return
        if (isInput)
            return;
        // w �Է� ��(�޴� ���� �ø���)
        if(Input.GetKeyDown(KeyCode.W))
        {
            // ���óѹ����� 0���� �۰ų� ������(0�ϋ�, ó�� �޴�)
            if (SelectNum <= 0)
                // ����
                return;
            // �ƴ϶�� -1
            SelectNum--;
        }
        // s�Է� ��(�޴� �Ʒ��� ������)
        if(Input.GetKeyDown(KeyCode.S))
        {
            // ���� �ѹ����� 3���� ũ�ų� �۴ٸ�(3�ϋ�, ������ �޴�)
            if (SelectNum >= 3)
                // ����
                return;
            // �ƴ϶�� +1
            SelectNum++;
        }
        // �����̽��� �Է� ��
        if(Input.GetKeyDown(KeyCode.Space))
        {            
            Debug.Log("Input Space");
            // �߰� �Է� ����
            isInput = true;
            // �޴� ����
            isOpenMenu = true;
        }

    }

    void CheckRay()
    {
		// Ű�Է��� Ȯ�εǸ� ���� �Է��� return
		if (isInput)
			return;

		m_PointerEventData = new PointerEventData(_event);
		m_PointerEventData.position = Input.mousePosition;
		List<RaycastResult> results = new List<RaycastResult>();

        // ���콺���� ����ĳ��Ʈ �߻�
		m_RayCaster.Raycast(m_PointerEventData, results);

        // ���콺�� ui���� ������
		if (_event.IsPointerOverGameObject())
		{
            // �浹�� ��ü�� ������ ��������
			GameObject hit = results[0].gameObject;

            // �����Ͱ� ������� �ʴٸ�
			if (hit != null)
			{
                // �ش� ��ü�� �̸� ����ϰ�
				Debug.Log(hit.gameObject.name);

                // �޴� ������ ��������
                GetMenuNumber menuNum = hit.gameObject.GetComponent<GetMenuNumber>();

                // �޴� �����͵� ������� �ʴٸ�
                if(menuNum != null)
                {
                    if(SelectNum != menuNum.MenuNumber)
						SoundManager.Sound.Play("KHJ/Effect/ChangeMenu");

					// ������ ����
					SelectNum = menuNum.MenuNumber;
                }

			}

		}
	}

    // ���콺 �浹 �� �ѹ��� �޾ƿ���
    public void SetInputMouse(int num)
    {
        if (isInput || isOpenMenu)
            return;
        SelectNum = num;
    }
    // ���콺 Ŭ�� �˻�
    public void MouseInput(int num)
    {
		if (isInput || isOpenMenu)
			return; 
        SelectNum = num;
        isInput = true;
        isOpenMenu = true;
	}

    // �޴� �̹��� ����
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

    // �޴� �̹��� �׼�
    IEnumerator SpriteAction(Image image)
    {
        // ������ ����
        // 0.5 -> 1.2 -> 1.0
        Vector3 OriginScale = new Vector3(1, 1, 1);

        // ������ Ŀ���� �׼�
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

        // ������ �۾����� �׼�
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


    // �޴� ����
    void SelectCheck()
    {
        // �߰� �Է� ����
        if (!isOpenMenu)
            return;

        // �޴� Ŭ�� �� ����
        SoundManager.Sound.Play("KHJ/Effect/SelectMenu");

        // ���� �ѹ����� ���� �޴� ����
        switch(SelectNum)
        {
            // �÷���
            case 0:
                SelectStart();
                break;
            // ����
            case 1:
                SelectSetting();
                break;
            // ũ����
            case 2:
                SelectCredit();
                break;
            // ������
            case 3:
                SelectQuit();
                break;
        }
        // �߰� �Է� ����
        isOpenMenu = false;

    }

    // �÷��� ���� �� �Լ�
    void SelectStart()
    {
        Debug.Log("Select Start");
        SoundManager.Sound.Play("KHJ/Effect/StartGame");
        // ȭ�� ���� �� ���̵�
        StartCoroutine(StartMethodCor());
	}
	// �÷��� ���� �� ȭ�� ������ ���� �ڷ�ƾ
	IEnumerator WaitAndActive()
	{
		yield return new WaitForSeconds(1.5f);
		myParant.SceneHideImage.gameObject.SetActive(true);

        // ���⼭ �÷��̾����� �̵�
        SceneManager.LoadScene("LoadWaitRoom");

	}
    // ��ŸƮ �Լ� �� �ڷ�ƾ
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
    // ���� �޴� �ݱ�
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

    // ũ����(������) ���� �� �Լ�
	void SelectCredit()
    {
		Debug.Log("Select Credit");

        if (!credits.gameObject.activeSelf)
        {
            credits.gameObject.SetActive(true);
            StartCoroutine(MenuPanelAction(credits.gameObject));
        }

	}
    // ũ���� �޴� �ݱ�
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

    // �޴� ���� �׼�
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

    // ������ ���� �� �Լ�
	void SelectQuit()
    {
		Debug.Log("Select Quit");
        StartCoroutine(CloseGame());
	}
    // ���� �����ڷ�ƾ
    IEnumerator CloseGame()
    {
        // �Է� Ȯ�� �� ��� ���
        yield return new WaitForSeconds(0.5f);
        // ȭ�� ������
		StartCoroutine(myParant.FadeInImage(myParant.SceneHideImage));
        // ȭ���� ������ �������� ���� ���
        yield return new WaitForSeconds(1.0f);

        // ����Ƽ �����Ϳ��� ���� �� �÷��̸�� ����
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        
        // �ƴ϶�� ���� ����
#else
        Application.Quit();
#endif


    }
}
