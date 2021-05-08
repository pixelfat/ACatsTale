using pixelfat.CatsTale;
using UnityEditor;
using UnityEngine;

public class GameControllerTest : MonoBehaviour
{
    public bool doNextMove = false;

    public float tileSize = .8f;
    public float lateralSpacing = 1f;
    public float verticalSpacing = .25f;

    public GameData gameData;
    public int moveIndex = 0;

    // Start is called before the first frame update
    private void Start()
    {
        gameData = new GameData(59);

        GameView view = gameObject.AddComponent<GameView>();
        view.Set(gameData);

    }

    // Update is called once per frame
    private void Update()
    {

        if (doNextMove)
        {

            Debug.Log($"Move #{moveIndex}: {gameData.Board.solution[moveIndex]}");

            gameData.Board.MovePlayer(gameData.Board.solution[moveIndex].direction, gameData.Board.solution[moveIndex].type);

            moveIndex++;

            if (moveIndex < gameData.Board.solution.Length)
                if (gameData.Board.solution[moveIndex].type == Move.Type.TP)
                    moveIndex++;

            doNextMove = false;

        }

    }

    void OnDrawGizmos()
    {

        if (gameData == null)
            return;

        Vector3 posScale = new Vector3(lateralSpacing, verticalSpacing, lateralSpacing);

        foreach (Tile t in gameData.Board.GetTiles())
        {

            float depth = gameData.Board.GetTileDepth(t);

            if (depth == -1)
                Debug.LogError("??");

            Vector3 pos = gameData.Board.GetTilePosition(t);

            pos.x *= lateralSpacing;
            pos.y *= verticalSpacing;
            pos.z *= lateralSpacing;

            Vector3 size = new Vector3(
               1f * tileSize,
               .1f * tileSize,
               1f * tileSize);

            Color32 tileColor = Color.blue;

            if (t.type == Tile.TileType.START)
                tileColor = Color.green;

            if (t.type == Tile.TileType.END)
                tileColor = Color.yellow;

            if (t.type == Tile.TileType.TELEPORT)
            {
                tileColor = Color.magenta;

                Position toPos = ((TeleportTile)t).to;

                Vector3 toPosV3 = new Vector3(
                   toPos.x,
                   0 - depth,
                   toPos.y);

                toPosV3.x *= lateralSpacing;
                toPosV3.y *= verticalSpacing;
                toPosV3.z *= lateralSpacing;

                Gizmos.DrawLine(pos, toPosV3);

            }

            Gizmos.color = tileColor;

            Gizmos.DrawWireCube(pos, size);
            Gizmos.color = Color.white;
        }

        Vector3 playerPos = new Vector3(
            gameData.Board.playerPos.x * lateralSpacing,
            0,
            gameData.Board.playerPos.y * lateralSpacing);

        Gizmos.DrawWireSphere(playerPos, tileSize/2);

    }

    [CustomEditor(typeof(GameControllerTest))]
    public class GameControllerTestEditor : Editor
    {

        public override void OnInspectorGUI()
        {

            GameControllerTest test = (GameControllerTest)target;

            if (test.gameData == null)
                return;

            GUILayout.Label($"Move Index: {test.moveIndex}");
            if (GUILayout.Button("Next Move"))
                test.doNextMove = true;

            GUILayout.Label($"Player Position: {test.gameData.Board.playerPos.x}, {test.gameData.Board.playerPos.y}");
            GUILayout.Label($"Tiles Remaining: {test.gameData.Board.GetTiles().Length}");

        }
    }
}
