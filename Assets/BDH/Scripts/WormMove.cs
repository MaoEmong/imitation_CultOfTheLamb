using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WormMove : MonoBehaviour
{
	// 몸체 배열
	public Transform[] bodies;
	// 뒷머리 오브젝트
	public GameObject BackHead;

	// 움직일 때 Horizontal
	public IEnumerator MoveCorH(float h)
	{
		// 움직이는 방향에 따라 역방향으로 몸체 이동
		if (h < 0)
		{
			if (bodies[0].localPosition.x < -h * 0.5f)
				bodies[0].localPosition += new Vector3(-h * 0.5f, 0, 0) * 5 * Time.deltaTime;

			yield return null;

			if (bodies[1].localPosition.x < -h * 1.0f)
				bodies[1].localPosition += new Vector3(-h * 1.0f, 0, 0) * 5 * Time.deltaTime;

			yield return null;

			if (bodies[2].localPosition.x < -h * 1.5f)
				bodies[2].localPosition += new Vector3(-h * 1.5f, 0, 0) * 5 * Time.deltaTime;

			yield return null;

			if (bodies.Length > 3)
			{
				if (bodies[3].localPosition.x < -h * 2.0f)
					bodies[3].localPosition += new Vector3(-h * 2.0f, 0, 0) * 5 * Time.deltaTime;
			}
		}
		else if (h > 0)
		{
			if (bodies[0].localPosition.x > -h * 0.5f)
				bodies[0].localPosition += new Vector3(-h * 0.5f, 0, 0) * 5 * Time.deltaTime;

			yield return null;

			if (bodies[1].localPosition.x > -h * 1.0f)
				bodies[1].localPosition += new Vector3(-h * 1.0f, 0, 0) * 5 * Time.deltaTime;

			yield return null;

			if (bodies[2].localPosition.x > -h * 1.5f)
				bodies[2].localPosition += new Vector3(-h * 1.5f, 0, 0) * 5 * Time.deltaTime;

			yield return null;

			if (bodies.Length > 3)
			{
				if (bodies[3].localPosition.x > -h * 2.0f)
					bodies[3].localPosition += new Vector3(-h * 2.0f, 0, 0) * 5 * Time.deltaTime;
			}
		}
	}
	// 움직일 때 Vertical
	public IEnumerator MoveCorV(float v)
	{
		// 움직이는 방향에 따라 역방향으로 몸체 이동
		if (v < 0)
		{
			if (bodies[0].localPosition.z < -v * 0.5f)
				bodies[0].localPosition += new Vector3(0, 0, -v * 0.5f) * 5 * Time.deltaTime;

			yield return null;

			if (bodies[1].localPosition.z < -v * 1.0f)
				bodies[1].localPosition += new Vector3(0, 0, -v * 1.0f) * 5 * Time.deltaTime;

			yield return null;

			if (bodies[2].localPosition.z < -v * 1.5f)
				bodies[2].localPosition += new Vector3(0, 0, -v * 1.5f) * 5 * Time.deltaTime;

			yield return null;

			if (bodies.Length > 3)
			{
				if (bodies[3].localPosition.z < -v * 2.0f)
					bodies[3].localPosition += new Vector3(0, 0, -v * 2.0f) * 5 * Time.deltaTime;
			}
		}
		else if (v > 0)
		{
			upMove(true);

			if (bodies[0].localPosition.z > -v * 0.5f)
				bodies[0].localPosition += new Vector3(0, 0, -v * 0.5f) * 5 * Time.deltaTime;

			yield return null;

			if (bodies[1].localPosition.z > -v * 1.0f)
				bodies[1].localPosition += new Vector3(0, 0, -v * 1.0f) * 5 * Time.deltaTime;

			yield return null;

			if (bodies[2].localPosition.z > -v * 1.5f)
				bodies[2].localPosition += new Vector3(0, 0, -v * 1.5f) * 5 * Time.deltaTime;

			yield return null;

			if (bodies.Length > 3)
			{
				if (bodies[3].localPosition.z > -v * 2.0f)
					bodies[3].localPosition += new Vector3(0, 0, -v * 2.0f) * 5 * Time.deltaTime;
			}
		}
	}

	// 움직임이 멈출 때 Horizontal
	public IEnumerator EndCorH()
	{
		if (bodies[0].localPosition.x > 0)
		{
			if (bodies.Length > 3)
			{
				if (bodies[3].localPosition.x > 0)
					bodies[3].localPosition += new Vector3(-2.0f, 0, 0) * 5 * Time.deltaTime;

				yield return null;

				if (bodies[2].localPosition.x > 0)
					bodies[2].localPosition += new Vector3(-1.5f, 0, 0) * 5 * Time.deltaTime;

				yield return null;

				if (bodies[1].localPosition.x > 0)
					bodies[1].localPosition += new Vector3(-1.0f, 0, 0) * 5 * Time.deltaTime;

				yield return null;

				if (bodies[0].localPosition.x > 0)
					bodies[0].localPosition += new Vector3(-0.5f, 0, 0) * 5 * Time.deltaTime;
			}
			else
			{
				if (bodies[2].localPosition.x > 0)
					bodies[2].localPosition += new Vector3(-1.5f, 0, 0) * 5 * Time.deltaTime;

				yield return null;

				if (bodies[1].localPosition.x > 0)
					bodies[1].localPosition += new Vector3(-1.0f, 0, 0) * 5 * Time.deltaTime;

				yield return null;

				if (bodies[0].localPosition.x > 0)
					bodies[0].localPosition += new Vector3(-0.5f, 0, 0) * 5 * Time.deltaTime;
			}
		}
		else if (bodies[0].localPosition.x < 0)
		{
			if (bodies.Length > 3)
			{
				if (bodies[3].localPosition.x < 0)
					bodies[3].localPosition += new Vector3(2.0f, 0, 0) * 5 * Time.deltaTime;

				yield return null;

				if (bodies[2].localPosition.x < 0)
					bodies[2].localPosition += new Vector3(1.5f, 0, 0) * 5 * Time.deltaTime;

				yield return null;

				if (bodies[1].localPosition.x < 0)
					bodies[1].localPosition += new Vector3(1.0f, 0, 0) * 5 * Time.deltaTime;

				yield return null;

				if (bodies[0].localPosition.x < 0)
					bodies[0].localPosition += new Vector3(0.5f, 0, 0) * 5 * Time.deltaTime;
			}
			else
			{
				if (bodies[2].localPosition.x < 0)
					bodies[2].localPosition += new Vector3(1.5f, 0, 0) * 5 * Time.deltaTime;

				yield return null;

				if (bodies[1].localPosition.x < 0)
					bodies[1].localPosition += new Vector3(1.0f, 0, 0) * 5 * Time.deltaTime;

				yield return null;

				if (bodies[0].localPosition.x < 0)
					bodies[0].localPosition += new Vector3(0.5f, 0, 0) * 5 * Time.deltaTime;
			}
		}
	}
	// 움직임이 멈출 때 Vertical
	public IEnumerator EndCorV()
	{
		if (bodies[0].localPosition.z > 0)
		{
			if (bodies.Length > 3)
			{
				if (bodies[3].localPosition.z > 0)
					bodies[3].localPosition += new Vector3(0, 0, -2.0f) * 5 * Time.deltaTime;

				yield return null;

				if (bodies[2].localPosition.z > 0)
					bodies[2].localPosition += new Vector3(0, 0, -1.5f) * 5 * Time.deltaTime;

				yield return null;

				if (bodies[1].localPosition.z > 0)
					bodies[1].localPosition += new Vector3(0, 0, -1.0f) * 5 * Time.deltaTime;

				yield return null;

				if (bodies[0].localPosition.z > 0)
					bodies[0].localPosition += new Vector3(0, 0, -0.5f) * 5 * Time.deltaTime;
			}
			else
			{
				if (bodies[2].localPosition.z > 0)
					bodies[2].localPosition += new Vector3(0, 0, -1.5f) * 5 * Time.deltaTime;

				yield return null;

				if (bodies[1].localPosition.z > 0)
					bodies[1].localPosition += new Vector3(0, 0, -1.0f) * 5 * Time.deltaTime;

				yield return null;

				if (bodies[0].localPosition.z > 0)
					bodies[0].localPosition += new Vector3(0, 0, -0.5f) * 5 * Time.deltaTime;
			}
		}
		else if (bodies[0].localPosition.z < 0)
		{
			if (bodies.Length > 3)
			{
				if (bodies[3].localPosition.z < 0)
					bodies[3].localPosition += new Vector3(0, 0, 2.0f) * 5 * Time.deltaTime;

				yield return null;

				if (bodies[2].localPosition.z < 0)
					bodies[2].localPosition += new Vector3(0, 0, 1.5f) * 5 * Time.deltaTime;

				yield return null;

				if (bodies[1].localPosition.z < 0)
					bodies[1].localPosition += new Vector3(0, 0, 1.0f) * 5 * Time.deltaTime;

				yield return null;

				if (bodies[0].localPosition.z < 0)
					bodies[0].localPosition += new Vector3(0, 0, 0.5f) * 5 * Time.deltaTime;

				upMove(false);
			}
			else
			{
				if (bodies[2].localPosition.z < 0)
					bodies[2].localPosition += new Vector3(0, 0, 1.5f) * 5 * Time.deltaTime;

				yield return null;

				if (bodies[1].localPosition.z < 0)
					bodies[1].localPosition += new Vector3(0, 0, 1.0f) * 5 * Time.deltaTime;

				yield return null;

				if (bodies[0].localPosition.z < 0)
					bodies[0].localPosition += new Vector3(0, 0, 0.5f) * 5 * Time.deltaTime;

				upMove(false);
			}
		}
	}

	// 위로 움직일 때
	void upMove(bool isUp)
	{
		// 뒤통수 오브젝트 ON
		if (isUp)
			BackHead.gameObject.SetActive(true);
		// 뒤통수 오브젝트 OFF
		else
			BackHead.gameObject.SetActive(false);
	}
}
