using pixelfat.CatsTale;
using UnityEngine;

public class PlayerView : MonoBehaviour
{

    public delegate void PlayerViewEvent();
    public delegate void PlayerMoveEvent(Move move);
    public PlayerMoveEvent OnStartMove, OnEndMove;

    public Move.Direction facing;

    public GameData gameData;
    private Animator animator;
    private bool isTurning = false, isJumping = false;
    private Move currentMove;
    public void TurnLeft()
    {

        animator.Play("Left");

        switch (facing)
        {
            case Move.Direction.NORTH: facing = Move.Direction.WEST; break;
            case Move.Direction.SOUTH: facing = Move.Direction.EAST; break;
            case Move.Direction.EAST: facing = Move.Direction.NORTH; break;
            case Move.Direction.WEST: facing = Move.Direction.SOUTH; break;
        }

        isTurning = true;

    }

    public void TurnRight()
    {

        animator.Play("Right");

        switch (facing)
        {
            case Move.Direction.NORTH: facing = Move.Direction.EAST; break;
            case Move.Direction.SOUTH: facing = Move.Direction.WEST; break;
            case Move.Direction.EAST: facing = Move.Direction.SOUTH; break;
            case Move.Direction.WEST: facing = Move.Direction.NORTH; break;
        }

        isTurning = true;

    }

    public void DoMove(Move.Type moveType)
    {

        switch (moveType)
        {
            case Move.Type.HOP:
                animator.Play("Hop");
                break;
            case Move.Type.JUMP:
                animator.Play("Jump");
                break;
            case Move.Type.TP:
                return;
        }

        isJumping = true;

        Position nextPos = GetNextPosition(facing, moveType, gameData.Board.playerPos);
        Tile nextTile = gameData.Board.GetTileAt(nextPos);

        // TODO: - Teleport FX
        if (nextTile != null)
            if (nextTile.type == Tile.TileType.TELEPORT)
            {
                // to-pos is stored in tile view
                Debug.Log("DO TELEPORT FX !!!");
            }

        currentMove = new Move(0, facing, moveType, gameData.Board.playerPos, nextPos);

        OnStartMove?.Invoke(currentMove);

    }

    // Start is called before the first frame update
    private void Start()
    {
        animator = gameObject.GetComponent<Animator>();
        GameObject catResource =  Resources.Load<GameObject>("Cat");
        GameObject catInstance = Instantiate<GameObject>(catResource);
        catInstance.transform.SetParent(transform);
        animator = catInstance.GetComponent<Animator>();
    }

    // Update is called once per frame
    private void Update()
    {

        UpdateState();
        
    }

    private void UpdateState() 
    {

        bool isIdle = animator.GetCurrentAnimatorStateInfo(0).IsName("Idle 1") || animator.GetCurrentAnimatorStateInfo(0).IsName("Idle 2");

        if (isIdle)
        {

            if (isJumping)
            {

                isJumping = false;
                Debug.Log("Move anim ended:" + currentMove);

                if (currentMove != null)
                    OnEndMove?.Invoke(currentMove);

                currentMove = null;

                Vector3 pos = GetPlayerPosition(gameData.Board);
                transform.position = pos;
                animator.transform.localPosition = Vector3.zero;

            }

            if (isTurning)
            {

                isTurning = false;
                Debug.Log("Turn anim ended:" + currentMove);

                Vector3 pos = GetPlayerPosition(gameData.Board);
                transform.position = pos;
                //animator.transform.localPosition = Vector3.zero;

            }

                if (Input.GetKeyDown(KeyCode.A))
                TurnLeft();

            if (Input.GetKeyDown(KeyCode.D))
                TurnRight();

            if (Input.GetKeyDown(KeyCode.W))
                DoMove(Move.Type.JUMP);

            if (Input.GetKeyDown(KeyCode.S))
                DoMove(Move.Type.HOP);

            if (Input.GetKeyDown(KeyCode.Space))
                animator.Play("Tail Flick");


        }
        else
        {



        }

    }

    public static Vector3 GetPlayerPosition(BoardData board)
    {

        Vector3 pos = new Vector3(board.playerPos.x, 0, board.playerPos.y);

        pos.x *= TileViewBase.lateralSpacing;
        pos.y *= TileViewBase.verticalSpacing;
        pos.z *= TileViewBase.lateralSpacing;

        return pos;

    }

    public static Position GetNextPosition(Move.Direction direction, Move.Type type, Position pos)
    {

        int dist = type == Move.Type.JUMP ? 2 : 1;

        switch (direction)
        {

            case Move.Direction.NORTH: return new Position(pos.x, pos.y + dist); 
            case Move.Direction.SOUTH: return new Position(pos.x, pos.y - dist); 
            case Move.Direction.EAST: return new Position(pos.x + dist, pos.y); 
            case Move.Direction.WEST: return new Position(pos.x - dist, pos.y);

               
        }

        return null;

    }

}
