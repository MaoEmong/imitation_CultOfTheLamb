using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WormMove : MonoBehaviour
{
	// ��ü �迭
	public Transform[] bodies;
	// �޸Ӹ� ������Ʈ
	public GameObject BackHead;

	// ������ �� Horizontal
	public IEnumerator MoveCorH(float h)
	{
		// �����̴� ���⿡ ���� ���������� ��ü �̵�
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
	// ������ �� Vertical
	public IEnumerator MoveCorV(float v)
	{
		// �����̴� ���⿡ ���� ���������� ��ü �̵�
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

	// �������� ���� �� Horizontal
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
	// �������� ���� �� Vertical
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

	// ���� ������ ��
	void upMove(bool isUp)
	{
		// ����� ������Ʈ ON
		if (isUp)
			BackHead.gameObject.SetActive(true);
		// ����� ������Ʈ OFF
		else
			BackHead.gameObject.SetActive(false);
	}
}
