using UnityEngine;
using UnityEngine.InputSystem;

public class Meme67Weapon : MonoBehaviour, IWeapon
{
    public bool isEquipped = false;
    
    [Header("子彈設定")]
    public GameObject bullet6Prefab;
    public GameObject bullet7Prefab;
    public Transform firePoint;

    [Header("武器參數")]
    public float fireCooldown = 0.2f;
    private float nextFireTime = 0f;
    private bool isSixNext = true;    
    private bool useLeftHand = true;  

    [Header("手部物件 (請從左邊列表拖入)")]
    public Transform leftHand;       
    public Transform rightHand;      
    public float handScale = 0.3f;    
    public float jumpHeight = 0.3f;   
    public float moveSpeed = 15f;     
    public float handSpacing = 0.4f;  

    [Header("方向修正")]
    public bool reverseLeftRight = true; 

    private float currentLeftJump = 0f;
    private float currentRightJump = 0f;
    
    private SpriteRenderer leftSR;
    private SpriteRenderer rightSR;

    void Start()
    {
        if (leftHand) leftSR = leftHand.GetComponent<SpriteRenderer>();
        if (rightHand) rightSR = rightHand.GetComponent<SpriteRenderer>();
    }

    public void Equip() { isEquipped = true; }
    public void Unequip() { isEquipped = false; }

    void Update()
    {
        if (!isEquipped) return; 

        if (Mouse.current.leftButton.wasPressedThisFrame && Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireCooldown;
        }

        currentLeftJump = Mathf.Lerp(currentLeftJump, 0, Time.deltaTime * moveSpeed);
        currentRightJump = Mathf.Lerp(currentRightJump, 0, Time.deltaTime * moveSpeed);
    }

    void LateUpdate()
    {
        if (!isEquipped) return;

        // 🌟 核心修改：不要看滑鼠了，直接讀取狐狸身體的狀態！
        // 1. 檢查武器的絕對縮放，如果是負數，代表狐狸被翻轉到左邊了
        bool isFacingLeft = (transform.lossyScale.x < 0);
        
        // 2. 加一層保險：如果狐狸是用 SpriteRenderer 翻轉的，我們也抓出來判斷
        SpriteRenderer foxSR = transform.root.GetComponentInChildren<SpriteRenderer>();
        if (foxSR != null && foxSR.flipX) 
        {
            isFacingLeft = true;
        }

        Vector3 basePos = transform.position;

        // 抵銷母物件「負數縮放」的比例，避免手跟著倒立
        Vector3 parentScale = transform.lossyScale;
        float scaleX = (Mathf.Abs(parentScale.x) > 0.01f) ? (handScale / parentScale.x) : handScale;
        float scaleY = (Mathf.Abs(parentScale.y) > 0.01f) ? (handScale / parentScale.y) : handScale;
        Vector3 counterScale = new Vector3(scaleX, scaleY, 1);

        // 根據面板的打勾狀態，決定最後要不要把手左右相反
        bool finalFlipX = reverseLeftRight ? !isFacingLeft : isFacingLeft;

        if (leftHand)
        {
            float xOffset = isFacingLeft ? handSpacing : -handSpacing;
            leftHand.position = basePos + new Vector3(xOffset, currentLeftJump, 0); 
            leftHand.rotation = Quaternion.identity; 
            leftHand.localScale = counterScale; 
            
            if (leftSR) { leftSR.flipX = finalFlipX; leftSR.flipY = false; }
        }

        if (rightHand)
        {
            float xOffset = isFacingLeft ? -handSpacing : handSpacing;
            rightHand.position = basePos + new Vector3(xOffset, currentRightJump, 0);
            rightHand.rotation = Quaternion.identity;
            rightHand.localScale = counterScale; 
            
            if (rightSR) { rightSR.flipX = finalFlipX; rightSR.flipY = false; }
        }
    }

    void Shoot()
    {
        GameObject bulletToSpawn = isSixNext ? bullet6Prefab : bullet7Prefab;
        if (bulletToSpawn && firePoint)
        {
            Instantiate(bulletToSpawn, firePoint.position, firePoint.rotation);
        }

        if (useLeftHand) currentLeftJump = jumpHeight;
        else currentRightJump = jumpHeight;

        isSixNext = !isSixNext;
        useLeftHand = !useLeftHand;
    }
}