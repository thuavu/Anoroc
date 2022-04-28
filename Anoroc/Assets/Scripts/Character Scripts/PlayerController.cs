using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(PlayerMotor))]
public class PlayerController : MonoBehaviour
{
    public float speed = 6f;
    public float gravity = -5f;
    
    public float velocityY = 0f; 

    public LayerMask movementMask;

    public Inventory inventory;

    public float turnSmoothTime = 0.1f;
    float turnSmoothVelocity;

    public Transform newCam;

    [SerializeField] private Camera mainCam;
    [SerializeField] private GameObject thirdPersonCam;

    [SerializeField] private GameObject Hand;
    [SerializeField] private GameObject Flames;

    public HUD Hud;

    private IInventoryItem mItemToPickup = null;
    private IInventoryItem mCurrentItem = null;
    private bool mLockPickup = false;
    private bool Player1Send = false;

    private Animator animator;
    PlayerMotor motor;

    PhotonView view;

    // Start is called before the first frame update
    void Start()
    {
        motor = GetComponent<PlayerMotor>();
        animator = GetComponentInChildren<Animator>();
        view = GetComponent<PhotonView>();
        if(!view.IsMine)
        {
            mainCam.enabled = true;
            thirdPersonCam.SetActive(false);
        }
        
    }

    /*
    void FixedUpdate()
    {
        if(mCurrentItem != null && Input.GetKeyDown(KeyCode.R))
        {
            DropCurrentItem();
        }
    }
    */
    

    // Update is called once per frame
    void Update()
    {
        if(view.IsMine){
            // Player movement for left mouse button
            /*if(Input.GetMouseButtonDown(0)) {
                Ray ray = cam.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if(Physics.Raycast(ray, out hit, 10000, movementMask)){
                    //Debug.Log("We hit " + hit.collider.name + " " + hit.point);
                    motor.MoveToPoint(hit.point);
                }
            }

            if(Input.GetMouseButtonDown(1)) {
                Ray ray = cam.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if(Physics.Raycast(ray, out hit, 10000)){
                    // Check if we hit an interactable
                }
            }*/

            
            float xDir = Input.GetAxisRaw("Horizontal");
            float zDir = Input.GetAxisRaw("Vertical");
            Vector3 dir = new Vector3(xDir, 0f, zDir).normalized;
            Vector3 point = new Vector3(xDir, 0f, zDir).normalized;

            if(dir.magnitude >= 0.1f){
                float targetAngle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg + newCam.eulerAngles.y;
                float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
                transform.rotation = Quaternion.Euler(0f, angle, 0f);

                Vector3 moveDir;
                if (Input.GetKey(KeyCode.S)){
                    moveDir = Quaternion.Euler(0f, targetAngle, 0f) * -Vector3.forward;
                }
                else{
                    moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
                }

                // Animator that controls walking animation manually
                animator.SetFloat("speedPercent", 0.75f);

                // Move charactor
                motor.MoveToPoint(moveDir.normalized * speed * Time.deltaTime); 
            }

            // Pick up items
            if(mItemToPickup != null && Input.GetKeyDown(KeyCode.F))
            {
                animator.SetTrigger("tr_pickup");
                inventory.AddItem(mItemToPickup);
                mItemToPickup.OnPickup();
                if (mItemToPickup.Name == "key")
                {
                    view.RPC("SetThisInactive", RpcTarget.All, 1);
                }
                else if (mItemToPickup.Name == "screwdriver")
                {
                    view.RPC("SetThisInactive", RpcTarget.All, 2);
                }
                Hud.CloseMessagePanel();
            }

            if (mCurrentItem != null && Input.GetKeyDown(KeyCode.R))
            {
                DropCurrentItem();
            }

            inventory.ItemUsed += Inventory_ItemUsed;
            inventory.ItemRemoved += Inventory_ItemRemoved;
        }
    }

    
    private void Inventory_ItemRemoved(object sender, InventoryEventArgs e)
    {
        IInventoryItem item = e.Item;

        GameObject goItem = (item as MonoBehaviour).gameObject;
        goItem.SetActive(true);
        goItem.transform.parent = null;

        if (item == mCurrentItem)
            mCurrentItem = null;

    }

    private void OnTriggerEnter(Collider other)
    {
        if (mLockPickup)
            return;

        IInventoryItem item = other.GetComponent<IInventoryItem>();
        if(item != null)
        {
            mItemToPickup = item;
            Hud.OpenMessagePanel("");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        IInventoryItem item = other.GetComponent<IInventoryItem>();
        if (item != null)
        {
            Hud.CloseMessagePanel();
            mItemToPickup = null;
        }
    }

    private void Inventory_ItemUsed(object sender, InventoryEventArgs e)
    {
        IInventoryItem item = e.Item;

        GameObject goItem = (item as MonoBehaviour).gameObject;
        goItem.SetActive(true);

        goItem.transform.parent = Hand.transform;

        mCurrentItem = e.Item;
    }

    [PunRPC]
    void SetThisInactive(int ID)
    {
        PhotonView.Find(ID).gameObject.SetActive(false);
    }

    [PunRPC]
    void SetThisInactiveParticle(int ID)
    {
        PhotonView.Find(ID).GetComponent<ParticleSystem>().enableEmission = false;
    }

    [PunRPC]
    void SetThisActive(int ID)
    {
        PhotonView.Find(ID).gameObject.SetActive(true);
    }

    [PunRPC]
    void TeleportKey(int ID)
    {
        PhotonView.Find(ID).gameObject.transform.position = new Vector3(10.224f, 1.46f, -13.1f);
    }

    [PunRPC]
    void TeleportScrewDriver(int ID)
    {
        PhotonView.Find(ID).gameObject.transform.position = new Vector3(-8.076f, 1.461f, -13.028f);
    }

    public void DropCurrentItem()
    {
        GameObject goItem = (mCurrentItem as MonoBehaviour).gameObject;

        inventory.RemoveItem(mCurrentItem);

        if (mItemToPickup.Name == "screwdriver" && Player1Send == true)
        {
            view.RPC("SetThisActive", RpcTarget.All, 2);
            view.RPC("SetThisInactiveParticle", RpcTarget.All, 3);
        }
        else if (mItemToPickup.Name == "screwdriver")
        {
            view.RPC("SetThisActive", RpcTarget.All, 2);
            view.RPC("TeleportScrewDriver", RpcTarget.All, 2);
            Player1Send = true;
        }
        else if (mItemToPickup.Name == "key" && Player1Send == true)
        {
            view.RPC("SetThisActive", RpcTarget.All, 1);
            view.RPC("TeleportKey", RpcTarget.All, 1);
        }
        else if (mItemToPickup.Name == "key")
        {
            view.RPC("SetThisActive", RpcTarget.All, 1);
        }

        goItem.transform.parent = null;

        //Invoke("DoDropItem", 0.25f);
    }

    public void DoDropItem()
    {
        mLockPickup = false;
        Destroy((mCurrentItem as MonoBehaviour).GetComponent<Rigidbody>());
        mCurrentItem = null;
    }
}
