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

        public void Set(GameData gameData)
        {

            if (tileLib == null)
                LoadResources();

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

            string[] directionNames = Enum.GetNames(typeof(Move.Direction));
            int selectedDirection = GUILayout.SelectionGrid((int)moveDirection, directionNames, directionNames.Length, GUILayout.Height(175), GUILayout.Width(175 * directionNames.Length));
            moveDirection = (Move.Direction)selectedDirection;

            string[] moveNames = Enum.GetNames(typeof(Move.Direction));
            int selectedMove = GUILayout.SelectionGrid((int)moveType, moveNames, moveNames.Length, GUILayout.Height(175), GUILayout.Width(175 * moveNames.Length));
            moveType = (Move.Type)selectedMove;

            if (GUILayout.Button("Move", GUILayout.Height(75)))
            {
                gameData.Board.MovePlayer(moveDirection, moveType);
            }


        }

    }

}