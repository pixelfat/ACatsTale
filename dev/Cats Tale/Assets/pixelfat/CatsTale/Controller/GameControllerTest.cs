//using Newtonsoft.Json;
//using pixelfat.CatsTale;
//using UnityEditor;
//using UnityEngine;

///// <summary>
///// First levels are only 2d
///// ...then 2d with teleports?
///// ...then 2d with traps?
///// .. then 3d
///// .. then 3d with teleports?
///// ...then 3d with traps?
///// </summary>
//public class GameControllerTest : MonoBehaviour
//{
//    public bool doNextMove = false;

//    public float tileSize = .8f;
//    public float lateralSpacing = 1f;
//    public float verticalSpacing = .25f;

//    public GameData gameData;
//    public int moveIndex = 0;

//    // Start is called before the first frame update
//    private void Start()
//    {
        
//        gameData = new GameData(10);

//        string json = gameData.CurrentGame.ToJson();
//        Debug.Log($"{json}");

//        JsonSerializerSettings settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };
//        BoardData fromjson = Newtonsoft.Json.JsonConvert.DeserializeObject<BoardData>(json, settings);

//        gameData = new GameData(fromjson);
//        string tojson = gameData.CurrentGame.ToJson();
//        Debug.Log($"{tojson}");

//        GameView view = gameObject.AddComponent<GameView>();
//        view.Set(gameData);
//    }

//    // Update is called once per frame
//    private void Update()
//    {

//        if (doNextMove)
//        {

//            Debug.Log($"Move #{moveIndex}: {gameData.CurrentGame.solution[moveIndex]}");

//            gameData.CurrentGame.MovePlayer(gameData.CurrentGame.solution[moveIndex].direction, gameData.CurrentGame.solution[moveIndex].type);

//            moveIndex++;

//            if (moveIndex < gameData.CurrentGame.solution.Length)
//                if (gameData.CurrentGame.solution[moveIndex].type == Move.Type.TP)
//                    moveIndex++;

//            doNextMove = false;

//        }

//    }

//    private void OnDrawGizmos()
//    {

//        if (gameData == null)
//            return;

//        Vector3 posScale = new Vector3(lateralSpacing, verticalSpacing, lateralSpacing);

//        foreach (Tile t in gameData.CurrentGame.GetTiles())
//        {

//            float depth = gameData.CurrentGame.GetTileDepth(t);

//            if (depth == -1)
//                Debug.LogError("??");

//            Vector3 pos = gameData.CurrentGame.GetTilePosition(t);

//            pos.x *= lateralSpacing;
//            pos.y *= verticalSpacing;
//            pos.z *= lateralSpacing;

//            Vector3 size = new Vector3(
//               1f * tileSize,
//               .1f * tileSize,
//               1f * tileSize);

//            Color32 tileColor = Color.blue;

//            if (t.type == Tile.TileType.START)
//                tileColor = Color.green;

//            if (t.type == Tile.TileType.END)
//                tileColor = Color.yellow;

//            if (t.type == Tile.TileType.TELEPORT)
//            {
//                tileColor = Color.magenta;

//                Position toPos = ((TeleportTile)t).to;

//                Vector3 toPosV3 = new Vector3(
//                   toPos.x,
//                   0 - depth,
//                   toPos.y);

//                toPosV3.x *= lateralSpacing;
//                toPosV3.y *= verticalSpacing;
//                toPosV3.z *= lateralSpacing;

//                Gizmos.DrawLine(pos, toPosV3);

//            }

//            Gizmos.color = tileColor;

//            Gizmos.DrawWireCube(pos, size);
//            Gizmos.color = Color.white;
//        }

//        Vector3 playerPos = new Vector3(
//            gameData.CurrentGame.playerPos.x * lateralSpacing,
//            0,
//            gameData.CurrentGame.playerPos.y * lateralSpacing);

//        Gizmos.DrawWireSphere(playerPos, tileSize/2);

//    }

//    [CustomEditor(typeof(GameControllerTest))]
//    public class GameControllerTestEditor : Editor
//    {

//        public override void OnInspectorGUI()
//        {

//            GameControllerTest test = (GameControllerTest)target;

//            if (test.gameData == null)
//                return;

//            GUILayout.Label($"Move Index: {test.moveIndex}");
//            if (GUILayout.Button("Next Move"))
//                test.doNextMove = true;

//            GUILayout.Label($"Player Position: {test.gameData.CurrentGame.playerPos.x}, {test.gameData.CurrentGame.playerPos.y}");
//            GUILayout.Label($"Tiles Remaining: {test.gameData.CurrentGame.GetTiles().Length}");

//        }
//    }
//}
