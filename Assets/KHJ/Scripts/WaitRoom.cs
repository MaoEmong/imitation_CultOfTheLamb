using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

// ���� ��ũ��Ʈ
public class WaitRoom : MonoBehaviour
{
    // �÷��̾� ���� ������
    public Transform PlayerSpawnPos;
    // �÷��̾� ������Ʈ
    public GameObject Player;
    // ȭ�� ������ ���� ������Ʈ
    public Image BlackImage;
	// ���� �÷��̾�
	public VideoPlayer videos;

    void Start()
    {
		ksjPlayerManager.Instance.getCanvas();

		// ���� ����(���� ���� ���ֱ�)
		SoundManager.Sound.Clear();

		videos.enabled = false;

		StartCoroutine(StartVideo());

        BlackImage.gameObject.SetActive(true);

        // �÷��̾� ������Ʈ��ġ ����
        Player.transform.position = PlayerSpawnPos.position;
    }


    // �浹 �˻�(��Ż�� �浹 �ݶ��̴� ����������)
    private void OnTriggerEnter(Collider other)
    {
        // �÷��̾� �浹 ��
        if(other.CompareTag("Player"))
        {

            // ȭ�� ��ο�����
			StartCoroutine(FadeInImage(BlackImage));

			// �� �Ѿ�� �� ���� Ŭ����
			SoundManager.Sound.Clear();
			// �������� ���� ������ �̵�
			StartCoroutine(GoToStageScene());

        }
    }
	// �̹��� ���� ������
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
	// �̹��� ���� £����
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
	// �������� ���� ������ �̵�
	IEnumerator GoToStageScene()
	{
		yield return new WaitForSeconds(1.0f);

		SceneManager.LoadScene("Select");
	}

	// ������� ~ ȭ�����
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

			// ���� ��� Ȯ��
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
		// ȭ�� �������
		StartCoroutine(FadeOutImage(BlackImage));

	}

}
