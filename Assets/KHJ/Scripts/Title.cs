using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

// Ÿ��Ʋ
public class Title : MonoBehaviour
{
    // ȭ�� ������ �뵵�� �̹���
    public Image SceneHideImage;

    // Ÿ��Ʋ �̹���/�ý�Ʈ
    public Image TitleImage;
    public Text TitleText;

	// True�ϋ� �Է� ����
	bool isInput = false;

	// ī�޶�
	public Camera MainCamera;

	// ī�޶� ���� ��ġ
	public Transform CameraGoalPos1;
	public Transform CameraGoalPos2;
	public Transform CameraGoalPos3;
	// �޴� UI
	public Menu myMenu;

	// ĳ���� ������Ʈ
	public GameObject CharactorObject;

	// ī�޶� ������ ���ÿ� ��Ȱ��ȭ ��ų ������Ʈ��
	public GameObject[] ActiveFalseObj;
	// ��鸲 ȿ���� ���� ������Ʈ��
	public GameObject[] ShakeActionObj;

	// ���� �÷��̾�
	public VideoPlayer videos;
    void Start()
    {
		// ī�޶� ���� ��ġ ����
		MainCamera.transform.position = new Vector3(0, 3, -50);

		// ĳ���� ������Ʈ ��Ȱ��ȭ
		CharactorObject.SetActive(false);


		// ���� ����
		SoundManager.Sound.SetBGMVol(0.15f);
		SoundManager.Sound.SetEffectVol(0.15f);

		// Ÿ��Ʋ UI�� �ʱ�ȭ
		TitleInit();

		// ���� ȭ�� �����
		StartCoroutine(titleInitImage());


		// �޴��� �θ�ü ���
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

	// UI�̹����� �ʱ�ȭ(��Ȱ��ȭ)
	void TitleInit()
	{
		SceneHideImage.gameObject.SetActive(true);
		TitleImage.gameObject.SetActive(false);
		TitleText.gameObject.SetActive(false);
		myMenu.gameObject.SetActive(false);
	}

	// �̹��� ���� ������
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
	// �̹��� ���� £����
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

	// �ؽ�Ʈ �극��(������) ȿ��
	IEnumerator TextBreath(Text _text)
	{
		float breathDelay = 0.13f;

		Color textColor = _text.color;

		while (true)
		{
			yield return new WaitForSeconds(0.1f);

			_text.color -= new Color(textColor.r, textColor.g, textColor.b, breathDelay);

			// �ؽ�Ʈ�� ������ ������ �� ���̸� ��ȯ��
			if (_text.color.a >= 0.9f || _text.color.a <= 0.2f)
				breathDelay *= -1;
		}
	}
	// ���� �̹����� ����
	IEnumerator titleInitImage()
	{
		yield return new WaitForSeconds(0.15f);
		SceneHideImage.gameObject.SetActive(false);

		// ���� ��� Ȯ��
		while(true)
		{
			yield return null;

			if (videos.time>= (videos.length* 0.99f))
				break;
			
		}

		yield return new WaitForSeconds(0.5f);

		// ����ȭ�� ��Ȱ��ȭ
		videos.enabled = false;

		// UI�۵�
		StartCoroutine(FadeOutImage(SceneHideImage));
		// Ÿ��Ʋ ���� ���
		SoundManager.Sound.Play("KHJ/BGM/TitleBgm", SoundManager.SoundType.Bgm);


		yield return new WaitForSeconds(1.5f);
		StartCoroutine(FadeInImage(TitleImage));
		StartCoroutine(FadeInImage(TitleImage.transform.GetChild(0).GetComponent<Image>()));
		yield return new WaitForSeconds(1.5f);
		// Ÿ��Ʋ �ؽ�Ʈ ������
		TitleText.gameObject.SetActive(true);
		StartCoroutine(TextBreath(TitleText));
		isInput = true;

	}
	// Ű�Է� üũ
	void CheckFirstInput()
	{
		// ���� Ű�Է� 1���� ������
		if (!isInput || !TitleText.gameObject.activeSelf)
			return;

		// ��� Ű�� �Է� �� ���̻��� Ű�Է��� ���� ����
		if (Input.anyKey)
		{
			isInput = false;

			SoundManager.Sound.Play("KHJ/Effect/TouchScreen",SoundManager.SoundType.Effect);
			StartCoroutine(TouchScreen());
		}

	}
	// ī�޶� ������
	IEnumerator CameraMove()
	{
		// 1�� ��ġ�� �̵�
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

		// 2�� ��ġ�� �̵�
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

		// 3�� ��ġ�� �̵��԰� ���ÿ� �޴� Ȱ��ȭ, ���ʿ��� ������Ʈ ��Ȱ��ȭ
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
	// ��鸲 �׼�
	IEnumerator ShakeAction(GameObject obj)
	{
		// ���� ������
		Vector3 OriginPos = obj.transform.position;

		// ������ �����ǰ�
		float randomX;
		float randomY;

		while(true)
		{
			yield return new WaitForSeconds(0.3f);

			randomX = Random.Range(-0.01f, 0.01f);
			randomY = Random.Range(-0.01f, 0.01f);

			obj.transform.position = OriginPos + new Vector3(randomX, randomY, 0);

			// ������Ʈ�� ��Ȱ��ȭ ���¶�� break
			if (!obj.activeSelf)
				break;

		}


	}
	// ȭ�� ��ġ
	IEnumerator TouchScreen()
	{
		yield return new WaitForSeconds(1.0f);

		// ĳ���� ������Ʈ Ȱ��ȭ
		CharactorObject.SetActive(true);

		SoundManager.Sound.Play("KHJ/Effect/CameraMove", SoundManager.SoundType.Effect);

		// Ÿ��Ʋ �̹��� ������� ���ÿ� ī�޶� �̵�
		StartCoroutine(FadeOutImage(TitleImage));
		StartCoroutine(FadeOutImage(TitleImage.transform.GetChild(0).GetComponent<Image>()));
		TitleText.gameObject.SetActive(false);
		StartCoroutine(CameraMove());

	}

}
