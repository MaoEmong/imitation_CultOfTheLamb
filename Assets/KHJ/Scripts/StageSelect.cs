using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// �������� ���� UI ��ũ��Ʈ
/*
 ���� ClearStage���� 0 == Ŭ���� ����
 1 == 1�������� Ŭ����
 ...
 �ش� ���� ���� selecNum�� ���ʰ��� ������

 �������� ���� �� �����ʵ�� �Ѿ�� �������� ������ selectNum���� �Ѱ��־��
 �� �ε� �� �ش� �ѹ��� ���� �������� �ʱ�ȭ�� �����

 �����ʵ������ �Ѿ�� �������� �ѹ��� ������ ������
 �����ʵ忡�� ����Ʈ������ �Ѿ�� �������� Ŭ���� Ȯ�ο� ����
 �� �ΰ��� ������ ���ӸŴ����� �����ؾߵ�
 */
public class StageSelect : MonoBehaviour
{
    // �������� �̹���
    public Image[] StageBackgroundImage;
    // �������� �ƿ�����
    public Image[] StageOutlineImage;
    // �������� �ɺ�
    public Image[] StageSymbolImage;
    // UI ��׶��� �ɺ�
    public Image[] UIBackGroundSymbolImage;
    // �̹����� ������ ���̾ƿ� �ǳ�
    public GameObject LayoutPanel;
    // UI ��ü BackGround
    public RectTransform UIBackground;

    // FadeInOutȿ���� �̹���
    public Image FadeInOutImage;

    // �������� ���� �ѹ�
    [SerializeField]
    int selectNum = 0;

    // Ŭ���� ��������
    public int ClearStage;

    // �÷��̾� ��ġ �̹���
    public Image CrownImage;

    // �÷��̾� �Ŵ���
    ksjPlayerManager PM;

    // �߰� �Է� ���ѿ� ����
    bool StopInput = false;

    void Start()
    {
        SoundManager.Sound.Clear();

        SoundManager.Sound.Play("KHJ/Effect/SelectIntro");

        // ���� ȭ�� ��ǥ�� ������ �ѹ��� ���� ��ȭ�ϰ�
        // ���� Ŭ���� �� �������� ������ ���� ���� ȭ�� ��ǥ ����

        // �÷��̾� �����͸� �������� Ŭ��������� ����
        PM = GameObject.Find("PlayerManager").GetComponent<ksjPlayerManager>();
        if(PM != null )
        {
            ClearStage = PM.CurStage;
        }
        
		if (ClearStage == 0)
			selectNum = 0;
        else
			selectNum = ClearStage - 1;


		// ���� �հ��̹��� ��Ȱ��ȭ
		CrownImage.enabled = false;

		// ���� Select�� �׼�
		StartCoroutine(InitCoroutine());
    }

    void Update()
    {
        CheckStage();
        OnKeyboard();
    }

    // ��׶��� �ɺ� �̹��� �극�� ȿ�� �ڷ�ƾ
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
    // �̹��� �극�� ȿ�� �ڷ�ƾ
    IEnumerator ImageBreathAction(Image img,float DelayTime)
    {
        yield return new WaitForSeconds(DelayTime);
		float breathDelay = 0.13f;

		while (true)
		{
			yield return new WaitForSeconds(0.1f);

			img.color -= new Color(0, 0, 0, breathDelay);

			// �ؽ�Ʈ�� ������ ������ �� ���̸� ��ȯ��
			if (img.color.a >= 0.9f || img.color.a <= 0.2f)
				breathDelay *= -1;
		}
	}

    // ���� ���� �� �Ŵ��� ������ �Ʒ��� ��¦ �������� �׼� ����
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

    // ������ ���� ��ȭ
    void CheckStage()
    {
        for(int i = 0; i < StageBackgroundImage.Length; i++)
        {
            // i���� Ŭ������ �������� ������ �۴ٸ�(�̹� Ŭ������ �����������)
			if (i < ClearStage)
			{
				// �ƿ����ΰ� �ɺ� �̹����� Ŭ���� ����(�þ�)���� �ٲٱ�
				StageOutlineImage[i].color = Color.cyan;
				StageSymbolImage[i].color = Color.cyan;
			}

			// i�� ���� ������ �ѹ��� ���ٸ�
			else if (i == selectNum)
            {
                // �ƿ����� ������ ������, �ɺ��� ���
				StageOutlineImage[i].color = Color.red;
				StageSymbolImage[i].color = Color.white;
			}
            // �� �� �ƴ϶�� ������ ���
            else
            {
				StageOutlineImage[i].color = Color.white;
				StageSymbolImage[i].color = Color.white;
			}
		}
    }

    // StageUI Panel �� �ٿ� �Լ�(�ٷ� �̵�)
    void MoveDownPanel()
    {
		LayoutPanel.transform.position -= new Vector3(0, -200);
	}
	void MoveUpPanel()
    {
		LayoutPanel.transform.position += new Vector3(0, -200);
	}
    // StageUI Panel �� �ٿ� �ڷ�ƾ�Լ�
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


    //=============================================== �Է�
	// Ű���� �Է�
	void OnKeyboard()
    {
        if (StopInput)
            return;

        // WŰ �Է�
        if(Input.GetKeyDown(KeyCode.W))
        {
            // �������� ���ڰ� �ƽ����� ��� �Է� ���
            if (selectNum >= StageBackgroundImage.Length - 1)
                return;
            // �������� ���ڰ� Ŭ������������� ���ٸ� return
            else if (selectNum >= ClearStage)
                return;
            // ���ð�++
            selectNum++;
            // ���̾ƿ� �ǳ� ���� �ø�
            //MoveUpPanel();
            StartCoroutine(PanelUpCoroutine());
		}
        // SpaceBar �Է�
		if (Input.GetKeyDown(KeyCode.Space))
        {
            // �ش� ���������� �̵�
            Debug.Log($"Go To Stage{selectNum+1}");

            StartCoroutine(GoToLoadingScene());
        }

    }

    // ���콺 �Է�(�Է��� UI_StageImage.cs���� ����)
    public void SetSelectNumber(int number)
    {
        if (StopInput)
            return;

        // Ŭ���� ���������� 1���� �����̶�� ����
        if (number > ClearStage)
            return;

        // Ŭ���� �������� �ѹ��� ���� �ѹ����� Ŭ ���
        if (selectNum < number)
        {
			// ū ��ġ ��ŭ ���̾ƿ� �ǳ� ���� �ø�
			for (int i = 0; i < number - selectNum; i++)
            {
                //MoveUpPanel();
				StartCoroutine(PanelUpCoroutine());
			}

            selectNum = number;
		}

		// Ŭ���� �������� �ѹ��� ���� �ѹ��� ���� ���
		else if(selectNum == number)
        {
            // �ش� ���������� �̵�
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

    // �ڷ�ƾ �ʱ�ȭ ����
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

    // �ε������� �̵�
    IEnumerator GoToLoadingScene()
    {
        StopInput = true;

        yield return new WaitForSeconds(0.3f);
        StartCoroutine(CrownMove());
        yield return new WaitForSeconds(0.7f);
        StartCoroutine(FadeOutImage(0, 1));
        yield return new WaitForSeconds(1.0f);
        // ���ӸŴ����� �������� ������ �Ѱ��ֱ�
        ksjPlayerManager.Instance.CurStage = selectNum;

		SceneManager.LoadScene("Loading");
	}

    // �հ� ���� �׼�
    IEnumerator CrownOpen()
    {
        CrownImage.enabled = true;

        CrownImage.transform.position = StageBackgroundImage[selectNum].transform.position;

		// ������ Ŀ���� �׼�
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

    // �հ� �����̴� �׼�
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
