using pixelfat.CatsTale;
using UnityEngine;

public class GameCamera : MonoBehaviour
{

    public Move.Direction facing;

    public GameData gameData;
    public new Camera camera;

    public Vector3 angle;
    public float distance = 3f;
    private Vector3 playerWorldPos;

    // Start is called before the first frame update
    void Start()
    {

        if(gameData == null)
        {

            Debug.LogError("no game data set");
            enabled = false;
            return;

        }

        playerWorldPos = GetPlayerPosition(gameData.Board);
        angle = new Vector3(0, 1.5f, -2.5f).normalized;

        camera = new GameObject("Camera").AddComponent<Camera>();
        camera.transform.SetParent(transform);
        camera.transform.localRotation = Quaternion.Euler(30, 0, 0);
        camera.transform.localPosition = angle * distance;

        gameData.Board.OnPlayerMove += HandlePlayerMove;

    }

    private void HandlePlayerMove()
    {
        playerWorldPos = GetPlayerPosition(gameData.Board);
    }

    // Update is called once per frame
    void Update()
    {

        transform.localPosition = Vector3.Lerp(transform.localPosition, playerWorldPos, 0.03f);
        camera.transform.localPosition = angle * distance;

        switch (facing)
        {
            case Move.Direction.NORTH:
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, 0, 0), 0.03f);
                break;
            case Move.Direction.SOUTH:
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, 180, 0), 0.03f);
                break;
            case Move.Direction.EAST:
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, 90, 0), 0.03f);
                break;
            case Move.Direction.WEST:
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, -90, 0), 0.03f);
                break;
        }

        distance -= Input.mouseScrollDelta.y;
        distance = Mathf.Clamp(distance, 3, 50);

    }

    public static Vector3 GetPlayerPosition(BoardData board)
    {

        Vector3 pos = new Vector3(board.playerPos.x,0,board.playerPos.y);

        pos.x *= TileViewBase.lateralSpacing;
        pos.y *= TileViewBase.verticalSpacing;
        pos.z *= TileViewBase.lateralSpacing;

        return pos;
    }

}
