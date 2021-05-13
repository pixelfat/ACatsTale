using System;
using System.Collections.Generic;
using UnityEngine;

namespace pixelfat.CatsTale
{

    public class GameView : MonoBehaviour
    {

        public static TileLibrary tileLib;

        public GameData gameData;
        public Dictionary<Tile, TileViewBase> tileViews = new Dictionary<Tile, TileViewBase>();

        public GameCamera cam;
        public PlayerView player;

        public void Set(GameData gameData)
        {

            Clear();

            this.gameData = gameData;

            if (tileLib == null)
                LoadResources();

            if (cam == null)
            {
                cam = new GameObject("Camera").AddComponent<GameCamera>();
                cam.transform.SetParent(transform);
            }

            if (player == null)
            {
                player = new GameObject("Player").AddComponent<PlayerView>();
                player.transform.SetParent(transform);
                player.OnEndMove += HandlePlayerMoved;
            }

            this.gameData = gameData;
            player.gameData = gameData;
            cam.gameData = gameData;
            
            gameData.Board.OnTileRemoved += HandleTileRemoved;

            foreach (Tile t in gameData.Board.GetTiles())
            {

                TileViewBase tileView =  tileLib.NewTile(t);

                if(tileView != null)
                {
                    tileView.Set(gameData.Board, t);
                    tileView.transform.SetParent(transform);
                    tileViews.Add(t, tileView);
                }

            }

        }

        public void Clear()
        {

            foreach (Tile tile in tileViews.Keys)
            {
                tileViews[tile].RemoveFromPlay();
                tileViews.Remove(tile);

            }

            tileViews = new Dictionary<Tile, TileViewBase>();

        }

        private void HandlePlayerMoved(Move move)
        {
            gameData.Board.MovePlayer(move.direction, move.type);
        }

        private void HandleTileRemoved(Tile t)
        {
            if(tileViews.ContainsKey(t))
            {

                Debug.Log("Tile removed from play");

                tileViews[t].RemoveFromPlay();

                tileViews.Remove(t);

            }
        }

        private static void LoadResources()
        {

            tileLib = Resources.Load<TileLibrary>("Tiles");

        }

        private void OnGUI()
        {

            GUILayout.BeginHorizontal();

            GUILayout.Space(85);

            if (GUILayout.Button("Jump", GUILayout.Height(75), GUILayout.Width(75)))
                gameData.Board.MovePlayer(cam.facing, Move.Type.JUMP);

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Left", GUILayout.Height(75), GUILayout.Width(75)))
                switch (cam.facing)
                {
                    case Move.Direction.NORTH: cam.facing = Move.Direction.WEST;break;
                    case Move.Direction.SOUTH: cam.facing = Move.Direction.EAST; break;
                    case Move.Direction.EAST: cam.facing = Move.Direction.NORTH; break;
                    case Move.Direction.WEST: cam.facing = Move.Direction.SOUTH; break;
                }

            if (GUILayout.Button("Hop", GUILayout.Height(75), GUILayout.Width(75)))
                gameData.Board.MovePlayer(cam.facing, Move.Type.HOP);

            if (GUILayout.Button("Right", GUILayout.Height(75), GUILayout.Width(75)))
                switch (cam.facing)
                {
                    case Move.Direction.NORTH: cam.facing = Move.Direction.EAST; break;
                    case Move.Direction.SOUTH: cam.facing = Move.Direction.WEST; break;
                    case Move.Direction.EAST: cam.facing = Move.Direction.SOUTH; break;
                    case Move.Direction.WEST: cam.facing = Move.Direction.NORTH; break;
                }

            GUILayout.EndHorizontal();

            if (GUILayout.Button("JSON", GUILayout.Height(75), GUILayout.Width(75)))
                Debug.Log(gameData.Board.ToJson());

        }
        private void OnDrawGizmos()
        {

            if (gameData == null)
                return;

            Vector3 posScale = new Vector3(TileViewBase.lateralSpacing, TileViewBase.verticalSpacing, TileViewBase.lateralSpacing);

            foreach (Tile t in gameData.Board.GetTiles())
            {

                float depth = gameData.Board.GetTileDepth(t);

                if (depth == -1)
                    Debug.LogError("??");

                Vector3 pos = gameData.Board.GetTilePosition(t);

                pos.x *= TileViewBase.lateralSpacing;
                pos.y *= TileViewBase.verticalSpacing;
                pos.z *= TileViewBase.lateralSpacing;

                Vector3 size = new Vector3(
                   1f,
                   .1f,
                   1f);

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

                    toPosV3.x *= TileViewBase.lateralSpacing;
                    toPosV3.y *= TileViewBase.verticalSpacing;
                    toPosV3.z *= TileViewBase.lateralSpacing;

                    Gizmos.DrawLine(pos, toPosV3);

                }

                Gizmos.color = tileColor;

                Gizmos.DrawWireCube(pos, size);
                Gizmos.color = Color.white;
            }

            Vector3 playerPos = new Vector3(
                gameData.Board.playerPos.x * TileViewBase.lateralSpacing,
                0,
                gameData.Board.playerPos.y * TileViewBase.lateralSpacing);

            Gizmos.DrawWireSphere(playerPos, 1f / 2f);

        }
    }

}