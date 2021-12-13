using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;


public class Player : MonoBehaviour
{

    private Grid grid;
    private Vector3 movement;
    private int step = 10;
    private UnityEvent playerMoved,playerEscaped;
    [SerializeField] LayerMask layerMask;
    private float velocity = 5f;
    private Rigidbody rb;
    private bool moving = false;
    private Vector3 nextpos;
    private Vector3 lastpos;
    private bool movementLock;


    private void Awake()
    {
        playerMoved = new UnityEvent();
        playerEscaped = new UnityEvent();

    }
    void Start()
    {
        movement = Vector3.zero;
        grid = FindObjectOfType<Grid>();
        rb = GetComponent<Rigidbody>();
        lastpos = transform.position;
        movementLock = true;
    }




    void Update( )
    {

        

    }

    private void FixedUpdate()
    {
        if(moving)
        {
            rb.MovePosition(transform.position + (nextpos - transform.position) * Time.deltaTime * velocity);
            if ((nextpos - transform.position).magnitude < 0.1)
            {
                moving = false;
                playerMoved.Invoke();
            }
        }
       

 
    }

    public void Move(InputAction.CallbackContext context)
    {
        if(context.performed&&!movementLock&&!moving)
        {
            //Check for obstacles
            movement = (Vector3)context.ReadValue<Vector2>() * step;
            if (!Physics.Linecast(transform.position, transform.position + movement,layerMask))
            {
                lastpos = transform.position;
                nextpos = transform.position + movement;
                moving = true;
                movementLock = true;
               
            }

        }

    }

    //Wait functions for keyboard input and screen input
    public void Wait(InputAction.CallbackContext context)
    {
        if(context.performed && !movementLock && !moving)
        {
            playerMoved.Invoke();
        }

    }

    public void ButtonWait()
    {
        playerMoved.Invoke();
    }


    //Return player position on grid
    public Vector3Int GetGridPos()
    {
        return grid.WorldToCell(transform.position);
    }

    public void AddPlayerMovedListener(UnityAction action)
    {
        playerMoved.AddListener(action);
    }
    public void AddPlayerEscapedListener(UnityAction action)
    {
        playerEscaped.AddListener(action);
    }


    private void OnTriggerEnter(Collider other)
    {

        playerEscaped.Invoke();
        
    }

    //Undo Last Action
    public void Undo()
    {
        transform.position = lastpos;
    }

    //Release player for next movement, called after all other movements happened
    public void ReleaseLock()
    {
        movementLock = false;
    }

}
