using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(PlayerMotor))]
public class PlayerController : MonoBehaviour
{
    public float speed = 6;
    public float gravity = -5;
    
    public float velocityY = 0; 

    public LayerMask movementMask;

    public Inventory inventory;

    [SerializeField] private GameObject Hand;

    public HUD Hud;

    private IInventoryItem mItemToPickup = null;
    private IInventoryItem mCurrentItem = null;
    private bool mLockPickup = false;

    private Animator animator;

    [SerializeField] private Camera cam;
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
            cam.enabled = false;
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

            // Player movement WASD
            Vector3 Movement = new Vector3 (Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            Ray ray;

            if (Input.GetKey(KeyCode.A)){
                ray = cam.ScreenPointToRay(Vector3.left);
                motor.MoveToPoint(Vector3.left + (motor.transform.position += Movement * speed * Time.deltaTime));
            }   
            if (Input.GetKey(KeyCode.D)){
                ray = cam.ScreenPointToRay(Vector3.right);
                motor.MoveToPoint(Vector3.right + (motor.transform.position += Movement * speed * Time.deltaTime));
            }
            if (Input.GetKey(KeyCode.W)){
                ray = cam.ScreenPointToRay(Vector3.up);
                motor.MoveToPoint(Vector3.up + (motor.transform.position += Movement * speed * Time.deltaTime));
            }
            if (Input.GetKey(KeyCode.S)){
                ray = cam.ScreenPointToRay(Vector3.down);
                motor.MoveToPoint(Vector3.down + (motor.transform.position += Movement * speed * Time.deltaTime));
            }


            if(mItemToPickup != null && Input.GetKeyDown(KeyCode.F))
            {
                animator.SetTrigger("tr_pickup");
                inventory.AddItem(mItemToPickup);
                mItemToPickup.OnPickup();
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

    public void DropCurrentItem()
    {
        GameObject goItem = (mCurrentItem as MonoBehaviour).gameObject;

        inventory.RemoveItem(mCurrentItem);

        goItem.transform.position = new Vector3(0, 1, 0);

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
