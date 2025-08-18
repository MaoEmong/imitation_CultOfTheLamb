using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

// 플레이어가 소지한 아이템의 카테고리를 구분하기 위한 이넘
// 인벤토리관련 패널에서 사용
public enum ItemType
{
    Resource,
    Food,
    Object
}

// 직관적인 아이템세팅 입출력을 위한 이넘
// 드랍아이템 인벤토리 등에서 사용예정
public enum ItemCode
{
    Gold,
    Grass
}

// 아이템이 가질 기본적인 정보
// 프로퍼티를 통해 접근하면 됨
public class Item
{
    // 아이템의 종류. 이넘을 활용할 것
    protected ItemType itemType;
    public ItemType ItemType
    {
        get { return itemType; }
    }

    protected ItemCode code;
    public ItemCode Code
    { get { return code; } }
    
    protected string name;

    // 아이템관련 텍스트에서 표시될 해당 아이템의 이름
    public string Name
    {
        get { return name; }
    }

    // 해당 아이템의 수량
    // FindItem 메서드를 통해 같은 아이템을 찾아 해당인덱스의 count필드만 늘릴 수 있음
    protected int count;
    public int Count
    {
        get { return count; }
        set { count = value; }
    }

    // 아이템을 추가 생성을 편하게 하기위한 생성자
    public Item(ItemType itemType, ItemCode code, string name, int count,string description,string refer)
    {
        this.itemType = itemType;
        this.code = code;
        this.name = name;
        this.count = count;
        this.description = description;
        this.refer = refer; 
    }

    // 아이템이 가질 디스크립션
    // 아이템의 설정이나 컨셉을 설명
    protected string description;
    public string Description
    {
        get { return description; }
        set { description = value; }
    }

    // 유저가 해당아이템의 실사용처를 표시할 참조사항
    protected string refer;
    public string Refer
    {
        get { return refer; }
        set { refer = value; }
    }
}

public class ksjPlayerManager : MonoBehaviour
{
    #region 페이드인아웃 제어필드
    [SerializeField]
    private Image fadeImage;
    public Image FadeImage
    {
        get { return fadeImage; }
    }

    private float alpha;

    [SerializeField]
    private GameObject downMessage;

    private WaitForSeconds fadeDelay = new WaitForSeconds(0.005f);
    #endregion


    // 현재 게임 스테이지 정보 저장용 필드
    private int curStage = 3;
    public int CurStage
    {
        get { return curStage; }
        set { curStage = value; }
    }

    protected bool isGameStart = false;
    public bool IsGameStart
    {
        get { return isGameStart; }
        set { isGameStart = value; }
    }

    // 동영상 끝났는지 체크
    private bool isVideoEnded = false;
    public bool IsVideoEnded
    {
        get { return isVideoEnded; }
        set { isVideoEnded = value; }
    }


    // 인벤토리
    public List<Item> inventory = new();

    //
    #region 웨폰변경용 제어필드

    // 플레이어 웨폰 관련 ui
    [SerializeField]
    private WeaponInfo weaponInfo;


    private WeaponType curPlayerWeapon = WeaponType.SWORD;
    [SerializeField]
    private int curPlayerWeaponLV = 0;
    public int CurPlayerWeaponLV
    {
        get { return curPlayerWeaponLV; }
    }

    [SerializeField]
    private float curPlayerWeaponATK;
    private float curPlayerWeaponSpeed;
    [SerializeField]
    private float curPlayerWeaponDamage;
    public float CurPlayerWeaponDamage
    {
        get { return curPlayerWeaponDamage; }
    }

    [SerializeField]
    private Sprite[] upDownSprites = new Sprite[2];
    [SerializeField]
    private GameObject upDownDamage;
    [SerializeField]
    private GameObject upDownSpeed;

    public WeaponType CurPlayerWeapon
    {
        get { return curPlayerWeapon; }
        set { curPlayerWeapon = value; }
    }

    /// <summary>
    /// 플레이어의 무기를 바꿀때 호출할 메서드입니다.
    /// 매개변수를 넣지 않으면 기본무기로 바뀝니다.
    /// </summary>
    /// <param name="weapon">이넘값으로 받는 웨폰의 타입</param>
    /// <param name="weaponLevel">웨폰의 레벨</param>
    /// <param name="weaponATK">웨폰의 공격력</param>
    ///  <param name="weaponSpeed">웨폰의 공격속도</param>
    public void ChangePlayerWeapon(WeaponType weapon, int weaponLevel, float weaponATK, float weaponSpeed)
    {
        curPlayerWeapon = weapon;
        curPlayerWeaponLV = weaponLevel;
        curPlayerWeaponATK = weaponATK;
        curPlayerWeaponSpeed = weaponSpeed;
        curPlayerWeaponDamage = curPlayerWeaponATK + curPlayerWeaponLV;

        ChangePlayerWeaponInUI();
    }

    public void ChangePlayerWeaponInUI()
    {
        var weaponSprite = MainCanvas.GetComponentInChildren<WeaponSprite>();
        if (weaponSprite == null)
        {
            Debug.Log("weaponSpriteScript is null");
            return;
        }
        weaponSprite.gameObject.GetComponent<Image>().sprite =
            weaponSprite.WeaponSprites[(int)curPlayerWeapon];

        Debug.Log("Player Weapon's changed");
    }

    public void ChangePlayerWeapon()
    {
        curPlayerWeapon = WeaponType.SWORD;
        curPlayerWeaponLV = 0;
        curPlayerWeaponATK = 5;
        curPlayerWeaponSpeed = 1;
        curPlayerWeaponDamage = curPlayerWeaponATK + curPlayerWeaponLV;

        ChangePlayerWeaponInUI();
    }

    /// <summary>
    /// 스크립트내부의 무기정보를 pm의 정보로 바꾸고싶을 때 사용하는 메서드.
    /// 매개변수는 호출하는 스크립트내에 선언된 무기정보 필드입니다.
    /// </summary>
    /// <param name="targetType"></param>
    /// <param name="targetWeaponLevel"></param>
    /// <param name="targetWeaponATK"></param>
    /// <param name="targetWeaponSpeed"></param>
    /// <param name="targetWeaponDamage"></param>
    public void GetPlayerWeapon(out WeaponType targetType, out int targetWeaponLevel, out float targetWeaponATK, out float targetWeaponSpeed, out float targetWeaponDamage)
    {
        targetType = curPlayerWeapon;
        targetWeaponLevel = curPlayerWeaponLV;
        targetWeaponATK = curPlayerWeaponATK;
        targetWeaponSpeed = curPlayerWeaponSpeed;
        targetWeaponDamage = curPlayerWeaponDamage;
    }


    #endregion

    //
    #region 플레이어 스테이터스
    private float hp=4;
    public float HP
    {
        get { return hp; }
        set { hp = value; }
    }

    private float maxhp;
    public float MaxHP
    {
        get { return maxhp; }
    }
    #endregion

    //
    #region 싱글톤 인스턴스
    public static ksjPlayerManager _instance = null;

    public static ksjPlayerManager Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject obj;
                obj = GameObject.Find("PlayerManager");

                if (obj == null)
                {
                    obj = new GameObject("PlayerManager");
                    obj.AddComponent<ksjPlayerManager>();
                    _instance = obj.GetComponent<ksjPlayerManager>();
                }
                else
                {
                    _instance = obj.GetComponentInChildren<ksjPlayerManager>();
                }
            }

            return _instance;
        }
        set { _instance = value; }
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        _instance = null;
    }
    #endregion

    //
    #region UI제어용 필드


    // 프리팹
    [SerializeField] 
    private GameObject MenuCanvasPrefab;
    [SerializeField]
    private GameObject MainCanvasPrefab;
    [SerializeField]
    private GameObject popUpCanvasPrefab;


    // 메뉴 캔버스
    [SerializeField]
    private GameObject MenuCanvas;
    public GameObject Menu
    {
        get { return MenuCanvas; }
    }
    private BelongingsPanel belongingsPanel;
    
    public BelongingsPanel BelongingsPanel
    {
        get { return belongingsPanel;}
        set { belongingsPanel = value; }
    }
    // 키입력에 따른 메뉴 오픈 클로즈 구분용
    bool isOpenedMenu = false;
    public bool IsOpenedMenu
    {
        get { return isOpenedMenu; }
    }

    // 메인 캔버스 
    [SerializeField]
    private GameObject mainCanvas;
    public GameObject MainCanvas
    {
        get { return mainCanvas; }
        set { mainCanvas = value; }
    }

    // 팝업관련
    [SerializeField]
    private GameObject popUpCanvas;

    [SerializeField]
    private GameObject popupPanel;

    #endregion

    private void Start()
    {
        if (!isGameStart)
            ChangePlayerWeapon();
	}

    // 테스트용 인풋입니다.
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (!IsVideoEnded)
                return;

            if (!isOpenedMenu)
            {
                MenuCanvas.SetActive(true);
                belongingsPanel.ClearItemList();
                belongingsPanel.SetItemList();
                isOpenedMenu = true;
                SoundManager.Sound.Play("KSJ/UI/MenuOpen");
            }
            else
            {
                MenuCanvas.SetActive(false);
                isOpenedMenu = false;
                SoundManager.Sound.Play("KSJ/UI/MenuClose");
            }
        }

        if(Input.GetKeyDown(KeyCode.L))
        {
            PlayerDamagedProcess(-0.5f);
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            PlayerDamagedProcess(0.5f);
        }
    }

    /// <summary>
    /// 인벤토리의 내부에 해당아이템이 존재하는지 찾을때 호출되는 메서드.
    /// 반환은 해당 인벤토리의 인덱스
    /// </summary>
    /// <param name="itemCode"></param>
    /// <returns>asd</returns>
    public Item FindItem(ItemCode itemCode)
    {
        int index = inventory.FindIndex(x => x.Code == itemCode);
        if(index<0)
        {
            return null;
        }

        return inventory[index];
    }

    /// <summary>
    /// 인벤토리에 원하는 아이템을 추가할때 호출하는 메서드.
    /// 팝업창도 같이 호출되니 주의
    /// </summary>
    /// <param name="obj"></param>
    public void AddItemToInven(ItemCode itemcode,int count)
    {
        // 인벤토리에서 추가할 아이템이 이미 존재하는지 검사
        var index = FindItem(itemcode);
        var pup = popupPanel.GetComponent<ksjPopUpPanel>();
        // 없다면 새로 아이템을 리스트에 추가
        if (index == null)
        {   
            switch (itemcode)
            {
                case ItemCode.Gold:
                    inventory.Add(new Item(ItemType.Resource, ItemCode.Gold, "골드", count, "황금을 향한 열망은 종종 모든 것을 빼앗아가기 마련이다.", "온갖 유용한 일에 쓰이는 돈입니다."));
                    pup.InitPopUp(itemcode, count);
                    break;
                case ItemCode.Grass:
                    inventory.Add(new Item(ItemType.Food, ItemCode.Grass, "풀", count, "두껍고 매듭이 져있는, 야생과 같이 거칩니다.", "연료로 사용되는 단순한 재료입니다."));
                    pup.InitPopUp(itemcode, count);
                    break;
            }
        }
        // 있다면 해당 아이템의 인덱스에 숫자만 늘려줌
        else
        {
            index.Count += count;
            pup.InitPopUp(itemcode, count);            
        }

        if(itemcode == ItemCode.Gold)
        {
            MainCanvas.GetComponentInChildren<MoneyPanel>().RefreshMoney();
        }
    }

    /// <summary>
    /// 플레이어의 피격시스템.
    /// 매개변수 데미지는 증감에 따른 부호까지 넣어서 호출해야합니다.
    /// </summary>
    /// <param name="damage"></param>
    public void PlayerDamagedProcess(float damage)
    {

        var main = MainCanvas.GetComponentInChildren<ksjMainUI>();



        if (HP <=0)
            return;
        HP += damage;
        if (HP >= 4.0f)
            HP = 4.0f;


        if (damage > 0)
        {
            main.SetHeartPlus();

            return;
        }

        if (main==null)
        {
            Debug.Log("mainUI is null");
            return;
        }
        else
        {
            Debug.Log("mainUI is found");
            main.SetHeart();
        }

        if (HP<= 0)
        {
            Debug.Log("player's down,,");
            StartFadeIn();
            SoundManager.Sound.Play("Player/Death/Master.assets [692]");
        }
        else
        {
            SoundManager.Sound.Play("Player/Hit/Master.assets [458]");
        }
    }


    /// <summary>
    /// 플레이어 무브에서 무기와 F키를 통해 상호작용할 때 호출되는 메서드입니다.
    /// 플레이어의 현재 무기의 정보를 상호작용한 무기와 교환하여 정보를 교체합니다.
    /// </summary>
    /// <param name="weaponType"></param>
    /// <param name="weaponLV"></param>
    /// <param name="weapnATK"></param>
    /// <param name="weaponSpeed"></param>
    public void SetWeaponDataInUI(WeaponType weaponType,int weaponLV,float weapnATK,float weaponSpeed)
    {
        string weaponNameInUI="";
        string weaponLevelInUI="";        

        switch(weaponLV)
        {
            case 1:
                weaponLevelInUI = "I";
                break;
            case 2:
                weaponLevelInUI = "II";
                break;
            case 3:
                weaponLevelInUI = "III";
                break;
            case 4:
                weaponLevelInUI = "IV";
                break;
            default:
                break;
        }

        switch (weaponType)
        {
            case WeaponType.SWORD:
                weaponNameInUI = "파멸의 검" + weaponLevelInUI;
                weaponInfo.itemDesc.GetComponent<TextMeshProUGUI>().text =
                    "막을 수 없는 폭력의 강대한 도구이다.";
                weaponInfo.itemRefer.GetComponent<TextMeshProUGUI>().text =
                  "속도가 적당히 빠른 기본 형태의 검입니다.";

                break;
            case WeaponType.AXE:
                weaponNameInUI = "분노의 도끼" + weaponLevelInUI;
                weaponInfo.itemDesc.GetComponent<TextMeshProUGUI>().text =
                    "오래전 잊힌 야수의 분노로 뒤덮혀 있다.";
                weaponInfo.itemRefer.GetComponent<TextMeshProUGUI>().text =
                   "속도가 낮지만 강력한 공격을 할 수 있는 도끼입니다.";
                break;
            case WeaponType.DAGGER:
                weaponNameInUI = "배교자의 칼" + weaponLevelInUI;
                weaponInfo.itemDesc.GetComponent<TextMeshProUGUI>().text =
                    "피해는 입지 않고, 오로지 줄 수만 있다.";
                weaponInfo.itemRefer.GetComponent<TextMeshProUGUI>().text =
                   "피해는 낮지만 빛처럼 빠르게 여러 번 공격할 수 있는 칼입니다.";
                break;
        }

        weaponInfo.itemName.GetComponent<TextMeshProUGUI>().text = weaponNameInUI;
        

        var damage = curPlayerWeaponATK + curPlayerWeaponLV;
        var targetDamage = weapnATK + weaponLV;
        float resultDamage = damage - targetDamage;

        weaponInfo.damage.GetComponent<TextMeshProUGUI>().text = string.Format("{0:F1}", math.abs(resultDamage));

        var speed = curPlayerWeaponSpeed;
        float resultSpeed = speed - weaponSpeed;
        weaponInfo.speed.GetComponent<TextMeshProUGUI>().text = string.Format("{0:F1}", math.abs(resultSpeed));
       

        if (resultDamage<0)
        {            
            weaponInfo.damage.GetComponent<TextMeshProUGUI>().color = Color.green;
            upDownDamage.GetComponent<Image>().sprite = upDownSprites[0];
            upDownDamage.SetActive(true);

        }
        else if(resultDamage>0)
        {
            weaponInfo.damage.GetComponent<TextMeshProUGUI>().color = Color.red;
            upDownDamage.GetComponent<Image>().sprite = upDownSprites[1];
            upDownDamage.SetActive(true);
        }     
        else
        {
            weaponInfo.damage.GetComponent<TextMeshProUGUI>().color = Color.white;
            upDownDamage.SetActive(false);
        }

        if(resultSpeed>0)
        {
            weaponInfo.speed.GetComponent<TextMeshProUGUI>().color = Color.green;
            upDownSpeed.GetComponent<Image>().sprite = upDownSprites[0];
            upDownSpeed.SetActive(true);
        }
        else if(resultSpeed<0)
        {
            weaponInfo.speed.GetComponent<TextMeshProUGUI>().color = Color.red;
            upDownSpeed.GetComponent<Image>().sprite = upDownSprites[1];
            upDownSpeed.SetActive(true);
        }
        else
        {
            weaponInfo.speed.GetComponent<TextMeshProUGUI>().color = Color.white;
            upDownSpeed.SetActive(false);
        }


        weaponInfo.gameObject.SetActive(true);
        SoundManager.Sound.Play("KSJ/Weapon/WeaponInfoOpen");
    }

    public void SleepWeaponDataUI()
    {
        weaponInfo.gameObject.SetActive(false);
    }

    public void SetPlayerDefault()
    {
        ChangePlayerWeapon();
        hp = 4.0f;
        var main = MainCanvas.GetComponentInChildren<ksjMainUI>();
        main.SetHeart();
        inventory.Clear();
    }

    public void getCanvas()
    {
        // 메뉴ui 관련
        GameObject menuObj = Instantiate(MenuCanvasPrefab);
        belongingsPanel = menuObj.GetComponentInChildren<BelongingsPanel>();
        menuObj.SetActive(false);
        MenuCanvas = menuObj;

        // 메인ui 관련
        GameObject mainObj = Instantiate(MainCanvasPrefab);
        MainCanvas = mainObj;
        var main = MainCanvas.GetComponentInChildren<ksjMainUI>();
        main.SetHeart();
        MainCanvas.GetComponentInChildren<MoneyPanel>().RefreshMoney();

        // 팝업관련
        GameObject popUpObj = Instantiate(popUpCanvasPrefab);
        popUpCanvas = popUpObj;
        popupPanel = popUpCanvas.transform.GetChild(0).gameObject;
        weaponInfo = popUpCanvas.transform.GetChild(1).GetComponent<WeaponInfo>();
        upDownDamage = weaponInfo.transform.GetChild(5).gameObject.transform.GetChild(1).gameObject;
        upDownSpeed = weaponInfo.transform.GetChild(6).gameObject.transform.GetChild(1).gameObject;        
        weaponInfo.gameObject.SetActive(false);
        downMessage = popUpCanvas.transform.GetChild(2).gameObject;


        ChangePlayerWeaponInUI();
        //ChangePlayerWeapon();

        // MainCanvas = GameObject.Find("MainUICanvas");

        // MenuCanvas = GameObject.Find("MenuCanvas");
        // MenuCanvas.SetActive(false);

        // belongingsPanel = MenuCanvas.GetComponentInChildren<BelongingsPanel>();

        // MainCanvas = GameObject.Find("MainUICanvas");
        // MainCanvas.GetComponentInChildren<MoneyPanel>().RefreshMoney();

        // popupPanel = GameObject.Find("PopUpCanvas").transform.GetChild(0).gameObject;
        // weaponInfo = GameObject.Find("PopUpCanvas").transform.GetChild(1).GetComponent<WeaponInfo>();




        fadeImage = MainCanvas.gameObject.transform.GetChild(3).GetComponent<Image>();
        fadeImage.gameObject.SetActive(false);

    }


    // 페이드 인아웃용 코루틴
    private void StartFadeIn()
    {
        alpha = 0;
        fadeImage.color = new Color(0, 0, 0, alpha);
        fadeImage.gameObject.SetActive(true); 
        downMessage.SetActive(false);
        StartCoroutine(FadeIn());
    }

    private void FinishFadeIn()
    {
        StopCoroutine(FadeIn());
        // fadeImage.color = new Color(0, 0, 0, alpha);
        downMessage.SetActive(true);        
    }


    IEnumerator FadeIn()
    {
        while (true)
        {
            fadeImage.gameObject.SetActive(true);
            alpha += Time.deltaTime;
            fadeImage.color = new Color(0, 0, 0, alpha);

            if (alpha >= 1.0f)
            {
                FinishFadeIn();
                break;
            }

            yield return fadeDelay;
        }
    }



}
