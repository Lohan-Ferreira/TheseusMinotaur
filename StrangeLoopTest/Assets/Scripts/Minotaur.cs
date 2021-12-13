using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Minotaur : MonoBehaviour
{

    private Player player;
    private Grid grid;
    private UnityEvent gotPlayer;
    [SerializeField]private LayerMask layer;
    private Rigidbody rb;
    private Vector3 step1, step2;
    private bool moving1,moving2;
    private float velocity = 10f;
    private Vector3 lastpos;


    private void Awake()
    {
        gotPlayer = new UnityEvent();
    }
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        grid = FindObjectOfType<Grid>();

        player.AddPlayerMovedListener(MoveToPlayer);
        rb = GetComponent<Rigidbody>();
        moving1 = false;
        moving2 = false;
        lastpos = transform.position;

    }

    private void MoveToPlayer()
    {
        //Save last position for UNDO
        lastpos = transform.position;

        //Get grid position of player
        Vector3Int playerpos = player.GetGridPos();

        //Calculate first step
        step1 = Step(transform.position,playerpos);

        //Calculate second step
        step2 = Step(step1, playerpos);

        //Set flag for moving
        moving1 = true;

    }

    private void FixedUpdate()
    {
        //Movement of first step
        if (moving1) rb.MovePosition(transform.position + (step1 - transform.position) * Time.deltaTime * velocity);

        //Check for end of movement
        if( (step1-transform.position).magnitude < 0.1)
        {
            moving1 = false;
            moving2 = true;
        }

        //Movement of second step
        if (moving2) rb.MovePosition(transform.position + (step2 - transform.position) * Time.deltaTime * velocity);

        //Check for end of movement
        if ( (step2 - transform.position).magnitude < 0.1)
        {
            moving2 = false;

            //Release player lock for next movement
            player.ReleaseLock();

            //Verify if got player after finishing movement
            Vector3Int pos = grid.WorldToCell(transform.position);
            Vector3Int playerpos = player.GetGridPos();

            if (pos == playerpos)
            {
                gotPlayer.Invoke();
            }

        }
    }


    private Vector3 Step(Vector3 realpos,Vector3Int playerpos)
    {
        Vector3Int pos = grid.WorldToCell(realpos);

        //Check player position for movement
        bool playerisRight = playerpos.x > pos.x;
        bool playerisUp = playerpos.y > pos.y;


        Vector3Int nextcell;
        Vector3 nextpos;

        //Check where to walk, horizontal first
        if (playerisRight)
        {
            
            nextcell = pos;
            nextcell.x += 1;

            //Get world position of next cell
            nextpos = grid.GetCellCenterWorld(nextcell);

            //Check for walls in this direction
            if(!Physics.Linecast(realpos, nextpos,layer))
            {

                
                return realpos + new Vector3(10, 0, 0);
            }
        }
        else if (!(playerpos.x == pos.x))
        {
            
            nextcell = pos;
            nextcell.x -= 1;

            //Get world position of next cell
            nextpos = grid.GetCellCenterWorld(nextcell);

            //Check for walls in this direction
            if (!Physics.Linecast(realpos, nextpos,layer))
            {
                return realpos + new Vector3(-10, 0, 0);
            }
        }

        if (playerisUp)
        {
            nextcell = pos;
            nextcell.y += 1;

            //Get world position of next cell
            nextpos = grid.GetCellCenterWorld(nextcell);

            //Check for walls in this direction
            if (!Physics.Linecast(realpos, nextpos,layer))
            {
                return realpos + new Vector3(0, 10, 0);
            }
        }
        else if(!(playerpos.y == pos.y))
        {
            nextcell = pos;
            nextcell.y -= 1;

            //Get world position of next cell
            nextpos = grid.GetCellCenterWorld(nextcell);

            //Check for walls in this direction
            if (!Physics.Linecast(realpos, nextpos,layer))
            {
                return realpos + new Vector3(0, -10, 0);
            }
        }


        return realpos;

    }

    public void AddGotPlayerListener(UnityAction action)
    {
        gotPlayer.AddListener(action);
    }

    //Undo Last Action
    public void Undo()
    {
        transform.position = lastpos;
    }
}
