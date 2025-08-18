using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TransparentObject ?
/*
    캐릭터와 카메라 사이에 캐릭터를 가리는 오브젝트 검사

    해당 오브젝트의 Material Rendering Mode를 "False" 로 변경, 
    Source를 "Albedo alpha", Albedo alpha 값을 천천히 내린다.
    알파 값을 다 내렸다면 타이머를 동작한다.

    오브젝트가 투명화가 진행이 다 되었는데도 
    캐릭터를 가지고 있다면, 타이머를 계속 초기화 한다.

    아니라면, 지정한 시간이 다 경과되었을 경우, 다시 Reset 한다.
*/

// 카메라에 들어갈 스크립트

public class TransparentObject : MonoBehaviour
{
    public bool IsTransparent { get; private set; } = false;

    private MeshRenderer[] renderers;
    private WaitForSeconds delay = new WaitForSeconds(0.01f);
    private WaitForSeconds resetDelay = new WaitForSeconds(0.005f);
    private const float THRESHOLD_ALPHA = 0.25f;
    private const float THRESHOLD_MAX_TIMER = 0.000001f;

    private bool isReseting = false;
    private float timer = 0f;
    private Coroutine timeCheckCoroutine;
    private Coroutine resetCoroutine;
    private Coroutine becomeTransparentCoroutine;

    [SerializeField]
    private GameObject player;


    void Awake()
    {
       renderers = GetComponentsInChildren<MeshRenderer>();
    }

	private void Start()
	{
        if (GameObject.Find("Player") != null)
            player = GameObject.Find("Player").gameObject;
	}

	public void BecomeTransparent()
    {
        if (IsTransparent)
        {
            timer = 0f;
            return;
        }

        if (resetCoroutine != null && isReseting)
        {
            isReseting = false;
            IsTransparent = false;
            StopCoroutine(resetCoroutine);
        }

        SetMaterialTransparent();
        IsTransparent = true;
        becomeTransparentCoroutine = StartCoroutine(BecomeTransparentCoroutine());
    }


    #region #Run-Time 중에 RenderingMode 바꾸는 메소드 목록
    private void SetMaterialalRenderingMode(Material material, float mode, int renderQueue)
    {
        material.SetFloat("_Mode", mode);
        //material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        //material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        // material.SetInt("ZWrite", 0);
        // material.DisableKeyword("_ALPHATEST_ON");
        // material.EnableKeyword("_ALPHABLEND_ON");
        // material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        material.renderQueue = renderQueue;
    }

    private void SetMaterialTransparent()
    {
        for (int i = 0; i < renderers.Length; i++)
        {
            foreach(Material material in renderers[i].materials)
            {
                SetMaterialalRenderingMode(material, 1f, 2000);
            }
        }
    }

    private void SetMaterialOpaque()
    {
        for (int i = 0; i < renderers.Length; i++)
        {
            foreach (Material material in renderers[i].materials)
            {
                SetMaterialalRenderingMode(material, 1f, 2000);
            }
        }
    }

    #endregion

    public void ResetOriginalTransparent()
    {
        SetMaterialOpaque();
        resetCoroutine = StartCoroutine(ResetOriginalTransparentCoroutine());
    }

    private IEnumerator BecomeTransparentCoroutine()
    {
        while (true)
        {
            bool isComplete = false;

            for (int i = 0; i < renderers.Length; i++)
            {
                // if (renderers[i].material.color.a > THRESHOLD_ALPHA)
                //     isComplete = false;
               

                var curCutoff = renderers[i].material.GetFloat("_Cutoff");
                curCutoff += Time.deltaTime*2;
                renderers[i].material.SetFloat("_Cutoff", curCutoff);

                //if (renderers[i].material.GetFloat("_Cutoff") >= 0.8f)
                //   isComplete = true;

                if (renderers[i].material.GetFloat("_Cutoff") >= 0.9f)
                {
                    isComplete = true;
                }
                    

                // Color color = renderers[i].material.color;
                // color.a -= Time.deltaTime;
                // renderers[i].material.color = color;
            }


            if (isComplete)
            {
                CheckTimer();
                break;
            }

            yield return delay;
        }
    }

    private IEnumerator ResetOriginalTransparentCoroutine()
    {
        IsTransparent = false;


        while (true)
        {
            bool isComplete = false;

            for (int i = 0; i <renderers.Length; i++)
            {
                // if (renderers[i].material.color.a < 1f)
                //     isComplete = false;               

                var curCutoff = renderers[i].material.GetFloat("_Cutoff");
                curCutoff -= Time.deltaTime*2;
                renderers[i].material.SetFloat("_Cutoff", curCutoff);

                if (renderers[i].material.GetFloat("_Cutoff") <= 0.1f)
                {
                    isComplete = true ;
                }
                
    
               // Color color = renderers[i].material.color;
               // color.a += (byte)Time.deltaTime;
               // renderers[i].material.color = color;
            }

             if (isComplete)
             {
                 isReseting = false;
                 break;
             }

            yield return resetDelay;
        }
    }

    public void CheckTimer()
    {
        if (timeCheckCoroutine != null)
            StopCoroutine(timeCheckCoroutine);
        timeCheckCoroutine = StartCoroutine(CheckTimerCoroutine());
    }

    private IEnumerator CheckTimerCoroutine()
    {
        timer = 0f;

        while (true)
        {
            timer += Time.deltaTime;

            if (timer > THRESHOLD_MAX_TIMER)
            {
                isReseting = true;
                ResetOriginalTransparent();
                break;
            }
            yield return null;
        }
    }
    
    void LateUpdate()
    {
        Vector3 dir = (player.transform.position - transform.position);
        dir = dir.normalized;
        RaycastHit[] hits = Physics.RaycastAll(transform.position, dir, Mathf.Infinity,
            1 << LayerMask.NameToLayer("EnvironmentObject"));

        for (int i = 0; i < hits.Length; i++)
        {
            TransparentObject[] obj = hits[i].transform.GetComponentsInChildren<TransparentObject>();

            for (int j = 0; j < obj.Length; j ++)
            {
                obj[j]?.BecomeTransparent();
            }
        }
    }
}
