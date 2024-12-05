using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Camera cam;
    private Vector2Int camera_pos = new Vector2Int(0, 0);
    private int _roomWidth;
    private int _roomHeight;
    private Rigidbody2D _rb;
    private Vector3 _originalScale;
    public float speed = 10;
    public static Action PressEButton;
    public GameObject droppedItemPrefab;

    [HideInInspector] public static int Money;
    [SerializeField] private TextMeshProUGUI CoinTMP;

    [HideInInspector] public bool MovingRight;
    [HideInInspector] public bool MovingDown;
    [HideInInspector] public bool MovingLeft;
    [HideInInspector] public bool MovingUp;

    public static int SkinSelected = 1;
    public static bool FreezeMove = false;
    public bool PlayerAnimationFreeze = false;

    public GameObject faceR;
    public GameObject faceL;
    public GameObject faceU;
    public GameObject faceD;

    public Animator playerAnimations;
    
    public DataBase dataBase;
    public Inventory inventory;

    public GameObject SettingMenu;
    private void Awake()
    {
        Application.targetFrameRate = 120;
        _rb = GetComponent<Rigidbody2D>();
        _originalScale = transform.localScale;
    }
    private void Start()
    {
        MoneyUpdate();
    }
    public void MoneyUpdate()
    {
        if(CoinTMP!=null)
            CoinTMP.text = Money.ToString();
    }
    public void dropItem(int id, int count)
    {
        GameObject droppedItem = Instantiate(droppedItemPrefab, transform.position, Quaternion.identity);
        MapCreator.LootCount++;
        Loot dp = droppedItem.GetComponent<Loot>();
        dp.item_id = id;
        dp.count = count;
        dp.db = dataBase;
        dp.inventory = inventory;
        dp.player = gameObject.GetComponent<Transform>();
        
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0;
        dp.direction = (mousePosition - transform.position).normalized*3f;
    }
    
    void FixedUpdate()
    {
        
        Vector2 movement = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        if (FreezeMove)
            movement = Vector2.zero;

        movement.Normalize();

        if (Math.Abs(movement.x) > Math.Abs(movement.y))
        {
            _rb.velocity = speed * movement;
        }
        else
        {
            _rb.velocity = speed * movement;
        }

        //Debug.Log(movement);
        //_rb.MovePosition(_rb.position + speed * Time.fixedDeltaTime * movement.normalized);

        if (!PlayerAnimationFreeze)
        {


            if (movement.x < 0 && movement.y == 0)
            {
                MovingLeft = true;
                faceR.SetActive(false);
                faceL.SetActive(true);
                faceD.SetActive(false);
                faceU.SetActive(false);
                playerAnimations.Play("WalkL_Skin_" + SkinSelected);

                //transform.localScale = new Vector3(-Mathf.Abs(_originalScale.x), _originalScale.y, _originalScale.z);
            }
            else
            {
                MovingRight = false;
            }
            if (movement.x > 0 && movement.y == 0)
            {
                MovingRight = true;
                faceR.SetActive(true);
                faceL.SetActive(false);
                faceD.SetActive(false);
                faceU.SetActive(false);
                playerAnimations.Play("WalkR_Skin_" + SkinSelected);

                //transform.localScale = new Vector3(Mathf.Abs(_originalScale.x), _originalScale.y, _originalScale.z);
            }
            else
            {
                MovingLeft = false;
            }

            if (movement.y > 0)
            {
                MovingUp = true;
                faceR.SetActive(false);
                faceL.SetActive(false);
                faceD.SetActive(false);
                faceU.SetActive(true);
                playerAnimations.Play("WalkU_Skin_" + SkinSelected);
            }
            else
            {
                MovingDown = false;
            }
            if (movement.y < 0)
            {
                MovingDown = true;
                faceR.SetActive(false);
                faceL.SetActive(false);
                faceD.SetActive(true);
                faceU.SetActive(false);
                playerAnimations.Play("WalkD_Skin_" + SkinSelected);
            }
            else
            {
                MovingUp = false;
            }

            if (movement.x == 0 && movement.y == 0)
            {
                if (faceD.activeInHierarchy == true)
                    playerAnimations.Play("IdleD_Skin_" + SkinSelected);
                if (faceU.activeInHierarchy == true)
                    playerAnimations.Play("IdleU_Skin_" + SkinSelected);
                if (faceR.activeInHierarchy == true)
                    playerAnimations.Play("IdleR_Skin_" + SkinSelected);
                if (faceL.activeInHierarchy == true)
                    playerAnimations.Play("IdleL_Skin_" + SkinSelected);
            }
        }
    }
    
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.E))
        {
            PressEButton?.Invoke();
            if (inventory.isOpen == true)
            {
                inventory.ToggleInventory();
            }
        }
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if (SettingMenu != null && SettingMenu.activeInHierarchy == false)
                SettingMenu.SetActive(true);
            else if (SettingMenu != null)
                SettingMenu.SetActive(false);
        }
        if(cam!=null)
            cam.transform.position = Vector3.Lerp(cam.transform.position, new Vector3(transform.position.x, transform.position.y, -10), Time.deltaTime * 10);
    }
}