using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadWaitRoom : MonoBehaviour
{
	public Image SpinImage;

	public Text LoadText;

	void Start()
	{
		StartCoroutine(LoadSceneField());
	}

	// �񵿱� �� ��ȯ
	IEnumerator LoadSceneField()
	{
		// �̹��� ȸ��
		StartCoroutine(SpinImageMethod());

		// ��� ���
		yield return new WaitForSeconds(0.1f);

		// ���������� �۵��ϴ��� üũ
		LoadText.text = $"��ȯ��...10";

		// ���� �̸����� ��ȯ
		AsyncOperation op = SceneManager.LoadSceneAsync("WaitRoom");
		op.allowSceneActivation = false;


		// ��� ���
		yield return new WaitForSeconds(0.1f);

		// ����ȯ ���� üũ
		while (!op.isDone)
		{
			LoadText.text = $"��ȯ��...{Mathf.Ceil(op.progress * 100)}";

			// ����ȯ�� ���� �������� ��� ���
			if (op.progress >= 0.9f)
			{
				// ��� ���
				yield return new WaitForSeconds(1.0f);
				op.allowSceneActivation = true;
				yield break;
			}
		}
	}

	// �̹��� ȸ�� �׼�
	IEnumerator SpinImageMethod()
	{
		Vector3 spinrotate = new Vector3(0, 0, 30).normalized;
		float speed = spinrotate.z;

		while (true)
		{
			yield return new WaitForSeconds(0.1f);
			SpinImage.transform.rotation = new Quaternion(0, 0,
			  SpinImage.transform.rotation.z + speed, SpinImage.transform.rotation.w);
			if (SpinImage.transform.rotation.z >= 360 || SpinImage.transform.rotation.z <= -360)
				SpinImage.transform.rotation = new Quaternion(0, 0,
				  0, SpinImage.transform.rotation.w);

		}

	}
}
