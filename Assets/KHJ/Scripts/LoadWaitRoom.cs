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

	// 비동기 씬 전환
	IEnumerator LoadSceneField()
	{
		// 이미지 회전
		StartCoroutine(SpinImageMethod());

		// 잠시 대기
		yield return new WaitForSeconds(0.1f);

		// 정상적으로 작동하는지 체크
		LoadText.text = $"소환중...10";

		// 씬의 이름으로 전환
		AsyncOperation op = SceneManager.LoadSceneAsync("WaitRoom");
		op.allowSceneActivation = false;


		// 잠시 대기
		yield return new WaitForSeconds(0.1f);

		// 씬전환 비율 체크
		while (!op.isDone)
		{
			LoadText.text = $"소환중...{Mathf.Ceil(op.progress * 100)}";

			// 씬전환이 거의 끝나갈때 잠시 대기
			if (op.progress >= 0.9f)
			{
				// 잠시 대기
				yield return new WaitForSeconds(1.0f);
				op.allowSceneActivation = true;
				yield break;
			}
		}
	}

	// 이미지 회전 액션
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
