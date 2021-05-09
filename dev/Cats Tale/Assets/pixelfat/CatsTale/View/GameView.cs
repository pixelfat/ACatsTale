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

            if (tileLib == null)
                LoadResources();

            if(cam == null)
                cam = new GameObject("Camera").AddComponent<GameCamera>();

            if (player == null)
            {
                player = new GameObject("Player").AddComponent<PlayerView>();
                player.gameData = gameData;
                player.OnEndMove += HandlePlayerMoved;
            }

            cam.gameData = gameData;

            this.gameData = gameData;
            gameData.Board.OnTileRemoved += HandleTileRemoved;

            foreach (Tile t in gameData.Board.GetTiles())
            {

                TileViewBase tileView =  tileLib.NewTile(t);

                if(tileView != null)
                {
                    tileView.Set(gameData.Board, t);
                    tileViews.Add(t, tileView);
                }

            }

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

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        private Move.Type moveType;
        private Move.Direction moveDirection;
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

    }

}