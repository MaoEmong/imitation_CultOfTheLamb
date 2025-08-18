using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    [SerializeField] float m_force = 0f;
    [SerializeField] Vector3 m_offset = Vector3.zero;

    float duration = 1f;

    Quaternion m_originRot;

    public static CameraShake instance;

	private void Awake()
	{
		if (instance == null)
        {
            instance = this;
        }
	}

	// Start is called before the first frame update
	void Start()
    {
        m_originRot = transform.rotation;
    }
  
    public IEnumerator ShakeCoroutine()
    {
        Vector3 t_originEuler = transform.eulerAngles;

        float startTime = 0;

        while (startTime < duration)
        {
			startTime += Time.deltaTime;

			yield return null;
		}

        startTime = 0;

		while (startTime < duration)
        {
            float t_rotX = Random.Range(-m_offset.x, m_offset.x);
            float t_rotY = Random.Range(-m_offset.y, m_offset.y);
            float t_rotZ = Random.Range(-m_offset.z, m_offset.z);

            Vector3 t_randomRot = t_originEuler + new Vector3(t_rotX, t_rotY, t_rotZ);
            Quaternion t_rot = Quaternion.Euler(t_randomRot);

            while (Quaternion.Angle(transform.rotation, t_rot) > 0.1f)
              {
                transform.rotation = Quaternion.RotateTowards(transform.rotation
                    , t_rot, m_force);

                yield return null;
              }

			startTime += Time.deltaTime;

			yield return null;
        }

        StartCoroutine(Reset());
	}

    IEnumerator Reset()
    {
        while(Quaternion.Angle(transform.rotation, m_originRot) > 0f)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, m_originRot, m_force * Time.deltaTime);
            yield return null;
        }
    }
}
