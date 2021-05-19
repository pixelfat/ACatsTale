using pixelfat.CatsTale;
using UnityEngine;

public class GameCamera : MonoBehaviour
{

    public Move.Direction facing;

    public GameData gameData;
    public new Camera camera;
    public Camera bgCamera;
    public Canvas bgCanvas;
    public Vector3 angle;
    public float distance = 3f;
    private Vector3 playerWorldPos;

    private Vector3 lastMousePos = Vector3.zero;
    private float touchDist = 0;
    private float lastDist = 0;

    // Start is called before the first frame update
    void Start()
    {

        if(gameData == null)
        {

            Debug.LogError("no game data set");
            enabled = false;
            return;

        }

        playerWorldPos = GetPlayerPosition(gameData);
        angle = new Vector3(0, 1.5f, -2.5f).normalized;

        camera = new GameObject("Foreground Camera").AddComponent<Camera>();
        camera.transform.SetParent(transform);
        camera.transform.localRotation = Quaternion.Euler(30, 0, 0);
        camera.transform.localPosition = angle * distance;
        camera.depth = 1;
        camera.clearFlags = CameraClearFlags.Color;
        camera.backgroundColor = new Color(.75f, .75f, .6f);

        //bgCamera = new GameObject("BG Camera").AddComponent<Camera>();
        //bgCamera.transform.SetParent(transform);
        //bgCamera.transform.localRotation = Quaternion.identity;
        //bgCamera.transform.localPosition = Vector3.zero;
        //bgCamera.depth = 0;
        //bgCamera.clearFlags = CameraClearFlags.Color;

        //bgCanvas = new GameObject("BG Canvas").AddComponent<Canvas>();

        gameData.OnPlayerMove += HandlePlayerMove;

    }

    private void HandlePlayerMove()
    {
        playerWorldPos = GetPlayerPosition(gameData);
    }

    // Update is called once per frame
    void Update()
    {

        transform.localPosition = Vector3.Lerp(transform.localPosition, playerWorldPos, 0.03f);

        camera.transform.localPosition = angle * distance;



        distance -= Input.mouseScrollDelta.y;


        if (Input.touchCount == 2)
        {
            Touch touch1 = Input.GetTouch(0);
            Touch touch2 = Input.GetTouch(1);

            if (touch1.phase == TouchPhase.Began && touch2.phase == TouchPhase.Began)
            {
                lastDist = Vector2.Distance(touch1.position, touch2.position);
            }

            if (touch1.phase == TouchPhase.Moved && touch2.phase == TouchPhase.Moved)
            {
                float newDist = Vector2.Distance(touch1.position, touch2.position);
                touchDist = lastDist - newDist;
                lastDist = newDist;

                // Your Code Here
                distance += touchDist * 0.1f;
            }
        }

        distance = Mathf.Clamp(distance, 2, 20);

        if (Input.GetMouseButtonDown(0))
            lastMousePos = Input.mousePosition;

        if (Input.GetMouseButton(0))
        {

            Vector3 delta = lastMousePos - Input.mousePosition;
            transform.localRotation = Quaternion.Euler(0, transform.localRotation.eulerAngles.y + delta.x, 0);
            lastMousePos = Input.mousePosition;
        }
        else
        {
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
        }
    }
    

    public static Vector3 GetPlayerPosition(GameData board)
    {

        Vector3 pos = new Vector3(board.playerPos.x,0,board.playerPos.y);

        pos.x *= TileViewBase.lateralSpacing;
        pos.y *= TileViewBase.verticalSpacing;
        pos.z *= TileViewBase.lateralSpacing;

        return pos;
    }

}
