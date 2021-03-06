using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed;
    public GameObject[] weapons;
    public bool[] hasWeapons;
    public GameObject[] grenade;

    public int ammo;
    public int coin;
    public int health;
    public int hasGr;

    public int maxAmmo;
    public int maxCoin;
    public int maxHealth;
    public int maxHasGr;


    float hAxis;
    float vAxis;

    bool rDown;
    bool jDown;
    bool iDown;
    bool fDown;
    bool sDown1;
    bool sDown2;
    bool sDown3;
    bool sDown4;

    bool isSwap;
    bool isJump;
    bool isDodge;
    bool isFireReady;


    Vector3 moveVec;
    Vector3 dodgeVec;

    Animator anim;
    Rigidbody rigid;
    GameObject gObject;
    Weapon equipWeapon;
    int equipWeaponIndex=-1;
    float fireDelay;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody>();

    }
    void Start()
    {
        
    }

    void Update()
    {
        GetInput();
        PlayerMove();
        Turn();
        Jump();
        Dodge();
        Interation();
        Swap();
        Attack();
    }
    void GetInput()
    {
        hAxis = Input.GetAxisRaw("Horizontal");
        vAxis = Input.GetAxisRaw("Vertical");
        rDown = Input.GetButton("Run");
        jDown = Input.GetButtonDown("Jump");
        iDown = Input.GetButtonDown("Interation");
        fDown = Input.GetButtonDown("Fire1");
        sDown1 = Input.GetButtonDown("Swap1");
        sDown2 = Input.GetButtonDown("Swap2");
        sDown3 = Input.GetButtonDown("Swap3");
        sDown4 = Input.GetButtonDown("Swap4");

    }
    void PlayerMove()
    {
        moveVec = new Vector3(hAxis, 0, vAxis).normalized;
        if (isDodge)
            moveVec = dodgeVec;
        if (isSwap)
            moveVec = Vector3.zero;

        transform.position += moveVec * speed * (rDown ? 1.75f : 1f) * Time.deltaTime;

        anim.SetBool("isWalk", moveVec != Vector3.zero);
        anim.SetBool("isRun", rDown);
    }
    void Turn()
    {
        transform.LookAt(transform.position + moveVec);
    }
    void Jump()
    {
        if (jDown && !rDown &&!isJump && !isDodge &&!isSwap)
        {
            rigid.AddForce(Vector3.up * 5, ForceMode.Impulse);
            anim.SetBool("isJump", true);
            anim.SetTrigger("doJump");
            isJump = true;

        }
    }
    void Dodge()
    {
        if (jDown && rDown && !isJump && !isDodge&& !isSwap)
        {
            dodgeVec = moveVec;
            speed *= 2;
            anim.SetTrigger("doDodge");
            isDodge = true;

            Invoke("DodgeOut", 0.5f);
        }
    }
    void DodgeOut()
    {
        speed *= 0.5f;
        isDodge = false;
    }
    void Interation()
    {
        if(iDown && gObject != null)
        {
            if (gObject.tag == "Weapon")
            {
                Item item = gObject.GetComponent<Item>();
                int weaponIndex = item.value;
                hasWeapons[weaponIndex] = true;

                Destroy(gObject);
            }
        }
    }
    void Attack()
    {
        if (equipWeapon == null)
            return;

        fireDelay += Time.deltaTime;
        isFireReady = equipWeapon.rate < fireDelay;

        if(fDown&&isFireReady && !isDodge && !isSwap)
        {
            equipWeapon.Use();
            anim.SetTrigger("doSwing");
            fireDelay = 0;
        }
    }
    void Swap()
    {
        if (sDown1 && (!hasWeapons[0] || equipWeaponIndex == 0))
            return;
        if (sDown2 && (!hasWeapons[1] || equipWeaponIndex == 1))
            return;
        if (sDown3 && (!hasWeapons[2] || equipWeaponIndex == 2))
            return;
        if (sDown4 && (!hasWeapons[3] || equipWeaponIndex == 3))
            return;

        int wIndex = -1;
        if (sDown1) wIndex = 0;
        if (sDown2) wIndex = 1;
        if (sDown3) wIndex = 2;
        if (sDown4) wIndex = 3;

        if((sDown1 || sDown2 || sDown3 || sDown4)&& !isJump && !isDodge && !isSwap)
        {
            if (equipWeapon != null)
                equipWeapon.gameObject.SetActive(false);

            equipWeaponIndex = wIndex;
            equipWeapon = weapons[wIndex].GetComponent<Weapon>();
            equipWeapon.gameObject.SetActive(true);
            if (sDown2)
                weapons[4].SetActive(true);

            if (sDown1 || sDown3 || sDown4)
                weapons[4].SetActive(false);

            anim.SetTrigger("doSwap");
            isSwap = true;

            Invoke("SwapOut", 0.4f);

        }
    }
    void SwapOut()
    {
        isSwap = false;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Floor")
        {
            anim.SetBool("isJump", false);
            isJump = false;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Item")
        {
            Item item = other.GetComponent<Item>();
            switch (item.type)
            {
                case Item.Type.Ammo:
                    ammo += item.value;
                    if (ammo > maxAmmo)
                        ammo = maxAmmo;
                    break;
                case Item.Type.Coin:
                    coin += item.value;
                    if (coin > maxCoin)
                        coin = maxCoin;
                    break;
                case Item.Type.Heart:
                    health += item.value;
                    if (health > maxHealth)
                        health = maxHealth;
                    break;
                case Item.Type.Grenade:
                    grenade[hasGr].SetActive(true);
                    hasGr += item.value;
                    if (hasGr > maxHasGr)
                        hasGr = maxHasGr;
                    break;
                    
            }
            Destroy(other.gameObject);
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Weapon")
            gObject = other.gameObject;

    }
    private void OnTriggerExit(Collider other)
    {
        
    }
}
