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




    [SerializeField] private GameObject Hand;

    public HUD Hud;

    private IInventoryItem mItemToPickup = null;
    private IInventoryItem mCurrentItem = null;
    private bool mLockPickup = false;

    private Animator animator;

    //[SerializeField] private Camera cam;
    PlayerMotor motor;

    PhotonView view;

    // Start is called before the first frame update
    void Start()
    {
        motor = GetComponent<PlayerMotor>();
        animator = GetComponentInChildren<Animator>();
        view = GetComponent<PhotonView>();
        /*if(!view.IsMine)
        {
            cam.enabled = false;
        }*/
        
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
            /*Vector3 Movement = new Vector3 (Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            Ray ray;
            //RaycastHit hit;
            Vector3 point;

            if (Input.GetKey(KeyCode.A)){
                //point = Vector3.left + (motor.transform.position += Movement * speed * Time.deltaTime);
                //point = (motor.transform.position += Movement * speed * Time.deltaTime);
                //motor.transform.position += Movement * speed * Time.deltaTime;
                point = Vector3.left + (motor.transform.position += Movement * speed * Time.deltaTime);
                ray = cam.ScreenPointToRay(point);
                motor.MoveToPoint(point);

                Debug.Log("Left: " + Vector3.left);
                Debug.Log("Point: " + point);
                
            }   
            else if (Input.GetKey(KeyCode.D)){
                //point = Vector3.right + (motor.transform.position += Movement * speed * Time.deltaTime);
                //motor.transform.position += Movement * speed * Time.deltaTime;
                point = Vector3.right + (motor.transform.position += Movement * speed * Time.deltaTime);
                ray = cam.ScreenPointToRay(point);
                motor.MoveToPoint(point);

                //Debug.Log("Point: " + point);
            }
            else if (Input.GetKey(KeyCode.W)){
                //point = Vector3.up + (motor.transform.position += Movement * speed * Time.deltaTime);
                //motor.transform.position += Movement * speed * Time.deltaTime;
                point = (motor.transform.position += Movement * speed * Time.deltaTime);
                ray = cam.ScreenPointToRay(point);
                motor.MoveToPoint(point);

                //Debug.Log("Point: " + point);
            }
            else if (Input.GetKey(KeyCode.S)){
                //point = Vector3.down + (motor.transform.position += Movement * speed * Time.deltaTime);
                //motor.transform.position += Movement * speed * Time.deltaTime;
                point = (motor.transform.position += Movement * speed * Time.deltaTime);
                ray = cam.ScreenPointToRay(point);
                motor.MoveToPoint(point);

                //Debug.Log("Point: " + point);
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

                motor.MoveToPoint(moveDir.normalized * speed * Time.deltaTime);
                
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
