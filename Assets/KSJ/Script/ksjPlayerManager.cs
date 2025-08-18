using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

// �÷��̾ ������ �������� ī�װ��� �����ϱ� ���� �̳�
// �κ��丮���� �гο��� ���
public enum ItemType
{
    Resource,
    Food,
    Object
}

// �������� �����ۼ��� ������� ���� �̳�
// ��������� �κ��丮 ��� ��뿹��
public enum ItemCode
{
    Gold,
    Grass
}

// �������� ���� �⺻���� ����
// ������Ƽ�� ���� �����ϸ� ��
public class Item
{
    // �������� ����. �̳��� Ȱ���� ��
    protected ItemType itemType;
    public ItemType ItemType
    {
        get { return itemType; }
    }

    protected ItemCode code;
    public ItemCode Code
    { get { return code; } }
    
    protected string name;

    // �����۰��� �ؽ�Ʈ���� ǥ�õ� �ش� �������� �̸�
    public string Name
    {
        get { return name; }
    }

    // �ش� �������� ����
    // FindItem �޼��带 ���� ���� �������� ã�� �ش��ε����� count�ʵ常 �ø� �� ����
    protected int count;
    public int Count
    {
        get { return count; }
        set { count = value; }
    }

    // �������� �߰� ������ ���ϰ� �ϱ����� ������
    public Item(ItemType itemType, ItemCode code, string name, int count,string description,string refer)
    {
        this.itemType = itemType;
        this.code = code;
        this.name = name;
        this.count = count;
        this.description = description;
        this.refer = refer; 
    }

    // �������� ���� ��ũ����
    // �������� �����̳� ������ ����
    protected string description;
    public string Description
    {
        get { return description; }
        set { description = value; }
    }

    // ������ �ش�������� �ǻ��ó�� ǥ���� ��������
    protected string refer;
    public string Refer
    {
        get { return refer; }
        set { refer = value; }
    }
}

public class ksjPlayerManager : MonoBehaviour
{
    #region ���̵��ξƿ� �����ʵ�
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


    // ���� ���� �������� ���� ����� �ʵ�
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

    // ������ �������� üũ
    private bool isVideoEnded = false;
    public bool IsVideoEnded
    {
        get { return isVideoEnded; }
        set { isVideoEnded = value; }
    }


    // �κ��丮
    public List<Item> inventory = new();

    //
    #region ��������� �����ʵ�

    // �÷��̾� ���� ���� ui
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
    /// �÷��̾��� ���⸦ �ٲܶ� ȣ���� �޼����Դϴ�.
    /// �Ű������� ���� ������ �⺻����� �ٲ�ϴ�.
    /// </summary>
    /// <param name="weapon">�̳Ѱ����� �޴� ������ Ÿ��</param>
    /// <param name="weaponLevel">������ ����</param>
    /// <param name="weaponATK">������ ���ݷ�</param>
    ///  <param name="weaponSpeed">������ ���ݼӵ�</param>
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
    /// ��ũ��Ʈ������ ���������� pm�� ������ �ٲٰ���� �� ����ϴ� �޼���.
    /// �Ű������� ȣ���ϴ� ��ũ��Ʈ���� ����� �������� �ʵ��Դϴ�.
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
    #region �÷��̾� �������ͽ�
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
    #region �̱��� �ν��Ͻ�
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
    #region UI����� �ʵ�


    // ������
    [SerializeField] 
    private GameObject MenuCanvasPrefab;
    [SerializeField]
    private GameObject MainCanvasPrefab;
    [SerializeField]
    private GameObject popUpCanvasPrefab;


    // �޴� ĵ����
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
    // Ű�Է¿� ���� �޴� ���� Ŭ���� ���п�
    bool isOpenedMenu = false;
    public bool IsOpenedMenu
    {
        get { return isOpenedMenu; }
    }

    // ���� ĵ���� 
    [SerializeField]
    private GameObject mainCanvas;
    public GameObject MainCanvas
    {
        get { return mainCanvas; }
        set { mainCanvas = value; }
    }

    // �˾�����
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

    // �׽�Ʈ�� ��ǲ�Դϴ�.
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
    /// �κ��丮�� ���ο� �ش�������� �����ϴ��� ã���� ȣ��Ǵ� �޼���.
    /// ��ȯ�� �ش� �κ��丮�� �ε���
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
    /// �κ��丮�� ���ϴ� �������� �߰��Ҷ� ȣ���ϴ� �޼���.
    /// �˾�â�� ���� ȣ��Ǵ� ����
    /// </summary>
    /// <param name="obj"></param>
    public void AddItemToInven(ItemCode itemcode,int count)
    {
        // �κ��丮���� �߰��� �������� �̹� �����ϴ��� �˻�
        var index = FindItem(itemcode);
        var pup = popupPanel.GetComponent<ksjPopUpPanel>();
        // ���ٸ� ���� �������� ����Ʈ�� �߰�
        if (index == null)
        {   
            switch (itemcode)
            {
                case ItemCode.Gold:
                    inventory.Add(new Item(ItemType.Resource, ItemCode.Gold, "���", count, "Ȳ���� ���� ������ ���� ��� ���� ���Ѿư��� �����̴�.", "�°� ������ �Ͽ� ���̴� ���Դϴ�."));
                    pup.InitPopUp(itemcode, count);
                    break;
                case ItemCode.Grass:
                    inventory.Add(new Item(ItemType.Food, ItemCode.Grass, "Ǯ", count, "�β��� �ŵ��� ���ִ�, �߻��� ���� ��Ĩ�ϴ�.", "����� ���Ǵ� �ܼ��� ����Դϴ�."));
                    pup.InitPopUp(itemcode, count);
                    break;
            }
        }
        // �ִٸ� �ش� �������� �ε����� ���ڸ� �÷���
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
    /// �÷��̾��� �ǰݽý���.
    /// �Ű����� �������� ������ ���� ��ȣ���� �־ ȣ���ؾ��մϴ�.
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
    /// �÷��̾� ���꿡�� ����� FŰ�� ���� ��ȣ�ۿ��� �� ȣ��Ǵ� �޼����Դϴ�.
    /// �÷��̾��� ���� ������ ������ ��ȣ�ۿ��� ����� ��ȯ�Ͽ� ������ ��ü�մϴ�.
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
                weaponNameInUI = "�ĸ��� ��" + weaponLevelInUI;
                weaponInfo.itemDesc.GetComponent<TextMeshProUGUI>().text =
                    "���� �� ���� ������ ������ �����̴�.";
                weaponInfo.itemRefer.GetComponent<TextMeshProUGUI>().text =
                  "�ӵ��� ������ ���� �⺻ ������ ���Դϴ�.";

                break;
            case WeaponType.AXE:
                weaponNameInUI = "�г��� ����" + weaponLevelInUI;
                weaponInfo.itemDesc.GetComponent<TextMeshProUGUI>().text =
                    "������ ���� �߼��� �г�� �ڵ��� �ִ�.";
                weaponInfo.itemRefer.GetComponent<TextMeshProUGUI>().text =
                   "�ӵ��� ������ ������ ������ �� �� �ִ� �����Դϴ�.";
                break;
            case WeaponType.DAGGER:
                weaponNameInUI = "�豳���� Į" + weaponLevelInUI;
                weaponInfo.itemDesc.GetComponent<TextMeshProUGUI>().text =
                    "���ش� ���� �ʰ�, ������ �� ���� �ִ�.";
                weaponInfo.itemRefer.GetComponent<TextMeshProUGUI>().text =
                   "���ش� ������ ��ó�� ������ ���� �� ������ �� �ִ� Į�Դϴ�.";
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
        // �޴�ui ����
        GameObject menuObj = Instantiate(MenuCanvasPrefab);
        belongingsPanel = menuObj.GetComponentInChildren<BelongingsPanel>();
        menuObj.SetActive(false);
        MenuCanvas = menuObj;

        // ����ui ����
        GameObject mainObj = Instantiate(MainCanvasPrefab);
        MainCanvas = mainObj;
        var main = MainCanvas.GetComponentInChildren<ksjMainUI>();
        main.SetHeart();
        MainCanvas.GetComponentInChildren<MoneyPanel>().RefreshMoney();

        // �˾�����
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


    // ���̵� �ξƿ��� �ڷ�ƾ
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
