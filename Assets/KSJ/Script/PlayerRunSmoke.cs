using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRunSmoke : MonoBehaviour
{
    [SerializeField]
    private GameObject[] smokePrefabs;

    private WaitForSeconds smokeDelay = new WaitForSeconds(0.5f);

    public void StartRunSmokeProcess()
    {
        StartCoroutine(InitRunSmoke());
    }


    IEnumerator InitRunSmoke()
    {
        int smokeIndex = 0;
        while(true)
        {
            smokeIndex++;
            Instantiate(smokePrefabs[smokeIndex]);

            yield return smokeDelay;
        }

    }


}
