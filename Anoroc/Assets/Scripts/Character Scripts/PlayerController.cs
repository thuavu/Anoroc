using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(PlayerMotor))]
public class PlayerController : MonoBehaviour
{
    public LayerMask movementMask;

    public Inventory inventory;

    public GameObject Hand;

    public HUD Hud;

    private IInventoryItem mItemToPickup = null;
    private IInventoryItem mCurrentItem = null;
    private bool mLockPickup = false;

    private Animator animator;


    Camera cam;
    PlayerMotor motor;

    PhotonView view;

    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
        motor = GetComponent<PlayerMotor>();
        animator = GetComponentInChildren<Animator>();
        view = GetComponent<PhotonView>();
        inventory.ItemUsed += Inventory_ItemUsed;
        inventory.ItemRemoved += Inventory_ItemRemoved;
    }

    void FixedUpdate()
    {
        if(mCurrentItem != null && Input.GetKeyDown(KeyCode.R))
        {
            DropCurrentItem();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(view.IsMine){
            // Player movement for left mouse button
            if(Input.GetMouseButtonDown(0)) {
                Ray ray = cam.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if(Physics.Raycast(ray, out hit, 100, movementMask)){
                    //Debug.Log("We hit " + hit.collider.name + " " + hit.point);
                    motor.MoveToPoint(hit.point);
                }
            }

            if(Input.GetMouseButtonDown(1)) {
                Ray ray = cam.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if(Physics.Raycast(ray, out hit, 100)){
                    // Check if we hit an interactable
                }
            }

            if(mItemToPickup != null && Input.GetKeyDown(KeyCode.F))
            {
                animator.SetTrigger("tr_pickup");
                inventory.AddItem(mItemToPickup);
                mItemToPickup.OnPickup();
                Hud.CloseMessagePanel();
            }
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
