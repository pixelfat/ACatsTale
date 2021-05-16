using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace pixelfat.CatsTale
{

    /// <summary>
    /// saved in either playerprefs or as text file
    /// </summary>
    public class PersistentSaveGameData
    {

        // arcade status
        public int currentArcadeLvl = 0;
        public int arcadeRestartsRemaining = 3; // Lives
        public GameData currentArcadeBoard;

        // story status
        public int currentStoryLevel;

        public static PersistentSaveGameData Persistent { get { return _Persistent; } set { _Persistent = value; Save(); } }
        private static PersistentSaveGameData _Persistent;

        public static void Load()
        {

            // use file.io to load json text from file
            string dir = Application.persistentDataPath;
            string path = Path.Combine(dir, "SaveGameData.json");
            
            JsonSerializerSettings settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto };
            
            if (!File.Exists(path))
                File.WriteAllText(path, JsonConvert.SerializeObject(new PersistentSaveGameData(), settings));

            string saveGameDataJson = File.ReadAllText(path);

            PersistentSaveGameData loaded = JsonConvert.DeserializeObject<PersistentSaveGameData>(saveGameDataJson, settings);

            Debug.Log("Persistent data loaded." + path);

            _Persistent = loaded;

        }

        public static void Save()
        {

            // use file.io to load json text from file
            string dir = Application.persistentDataPath;
            string path = Path.Combine(dir, "SaveGameData.json");

            JsonSerializerSettings settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto };
            string json = JsonConvert.SerializeObject(_Persistent, settings);
            
            File.WriteAllText(path, json);

            Debug.Log($"Persistent data saved to {path}\n{json}");

        }

    }

    public class GameData
    {

        public delegate void GameDataEvent();
        public delegate void TileEvent(Tile t);

        [JsonIgnore]
        public GameDataEvent OnStateChanged, OnPlayerMove;
        [JsonIgnore]
        public TileEvent OnTileRemoved, OnTileAdded, OnTileUpdated;

        public enum State
        {

            START,
            IN_PLAY,
            FALL,
            STUCK,
            POSSIBLE_COMPLETION,
            COMPLETED

        }

        public readonly Dictionary<int, Dictionary<int, List<Tile>>> tiles;
        public readonly Move[] solution;

        public Position playerPos = new Position(0, 0);
        public State state = State.START;

        public static GameData FromJson(string json)
        {

            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            };

            return JsonConvert.DeserializeObject<GameData>(json, settings);


        }

        public static GameData GenerateGameData(int moveCount)
        {

            Move[] moves = GenerateMoveList(moveCount);
            return GenerateGameData(moves);

        }

        public static GameData GenerateGameData(Move[] moves)
        {

            GameData boardData = new GameData(moves);

            StartTile startTile = new StartTile(moves[0].from.x, moves[0].from.y);

            boardData.Add(startTile);

            for (int i = 1; i < moves.Length; i++)
            {

                Tile newTile = newTile = new Tile(moves[i].from.x, moves[i].from.y);

                if (i < moves.Length)
                    if (moves[i].type == Move.Type.TP)
                        newTile = new TeleportTile(moves[i].from.x, moves[i].from.y, moves[i].to.x, moves[i].to.y);

                // if the NEXT move is tp then THIS tile is a teleport tile
                boardData.Add(newTile);

            }

            EndTile endTile = new EndTile(moves[moves.Length - 1].to.x, moves[moves.Length - 1].to.y);
            boardData.Add(endTile);

            return boardData;

        }

        public static Move[] GenerateMoveList(int moveCount)
        {

            List<Move> moves = new List<Move>();

            int x = 0, y = 0;
            Move.Type type = Move.Type.JUMP;
            for (int moveIndex = 0; moveIndex < moveCount; moveIndex++)
            {

                float northChance = 2;
                float southChance = 1;
                float eastChance = 1;
                float westChance = 1;

                float[] weights = new float[] { northChance, southChance, eastChance, westChance };

                Move.Direction dir = (Move.Direction)WeightedRandom(weights);

                float stepChance = 1;
                float hopChance = .5f;
                float tpChance = moveIndex == 0 || moveIndex > moveCount - 2 || type == Move.Type.TP ? 0 : .1f; // no chance if it's the first (start) move or the prv move was a tp

                weights = new float[] { stepChance, hopChance, tpChance };

                type = (Move.Type)WeightedRandom(weights);

                Position to = null;

                if (type == Move.Type.TP)
                {

                    // make this deterministic
                    int distX = (int)(UnityEngine.Random.value * Mathf.Sqrt(moveCount)) + 3;
                    int distY = (int)(UnityEngine.Random.value * Mathf.Sqrt(moveCount)) + 3;
                    to = new Position(x + distX, y + distY);

                }
                else
                {

                    int dist = type == Move.Type.JUMP ? 2 : 1;

                    switch (dir)
                    {
                        case Move.Direction.NORTH: to = new Position(x, y + dist); break;
                        case Move.Direction.SOUTH: to = new Position(x, y - dist); break;
                        case Move.Direction.EAST: to = new Position(x + dist, y); break;
                        case Move.Direction.WEST: to = new Position(x - dist, y); break;
                    }

                }

                Move newMove = new Move(moveIndex, dir, type, new Position(x, y), to);
                x = newMove.to.x;
                y = newMove.to.y;

                moves.Add(newMove);

                Debug.Log($"Move #{moveIndex}: {newMove}");

            }

            return moves.ToArray();

        }

        public static int WeightedRandom(float[] weights)
        {

            float total = 0;
            float[] totals = new float[weights.Length];

            for (int i = 0; i < weights.Length; i++)
            {
                total += weights[i];
                totals[i] = total;
            }

            float rnd = UnityEngine.Random.value * total;

            for (int i = 0; i < totals.Length; i++)
                if (rnd < totals[i])
                    return i;

            return totals.Length;

        }

        [JsonConstructor]
        public GameData(Dictionary<int, Dictionary<int, List<Tile>>> tiles, Move[] solution, Position playerPosition, int state)
        {
            this.tiles = tiles;
            this.solution = solution;
            this.playerPos = playerPosition;
            this.state = (State)state;
        }

        public GameData(Move[] moves)
        {

            tiles = new Dictionary<int, Dictionary<int, List<Tile>>>();
            solution = moves;

        }

        public GameData(Dictionary<int, Dictionary<int, List<Tile>>> data, Move[] solution)
        {

            this.tiles = new Dictionary<int, Dictionary<int, List<Tile>>>(data);
            this.solution = solution;

        }

        public void MovePlayer(Move.Direction direction, Move.Type type)
        {

            Position to = new Position(0, 0);

            int dist = type == Move.Type.JUMP ? 2 : 1;

            switch (direction)
            {

                case Move.Direction.NORTH: to = new Position(playerPos.x, playerPos.y + dist); break;
                case Move.Direction.SOUTH: to = new Position(playerPos.x, playerPos.y - dist); break;
                case Move.Direction.EAST: to = new Position(playerPos.x + dist, playerPos.y); break;
                case Move.Direction.WEST: to = new Position(playerPos.x - dist, playerPos.y); break;

            }

            // update state
            if (state == State.START)
            {
                state = State.IN_PLAY;
                OnStateChanged?.Invoke();
            }

            if (state != State.IN_PLAY && state != State.STUCK)
            {
                Debug.LogWarning("Game no longer in play.");
                return;
            }

            Tile fromTile = GetTileAt(playerPos);
            Tile toTile = GetTileAt(to);

            if (fromTile != null)
                Remove(fromTile);

            // move player 
            if (toTile != null && toTile.GetType() == typeof(TeleportTile))
            {
                Remove(toTile);
                playerPos = ((TeleportTile)toTile).to;
                toTile = GetTileAt(playerPos);
                Debug.Log($"<color=magenta>Teleport tile.</color>");
            }
            else
                playerPos = to;

            // jump to empty space?
            if (toTile == null)
            {

                Debug.Log("Player jumped to an empty position.");
                state = State.FALL;
                OnStateChanged?.Invoke();

            }
            else if (GetTiles().Length == 1)
                if (toTile.type == Tile.TileType.END)
                {
                    state = State.COMPLETED;
                    OnStateChanged?.Invoke();
                    Debug.Log($"<color=green>Stage Completed.</color>");
                }
                else
                {
                    state = State.COMPLETED;
                    OnStateChanged?.Invoke();
                    Debug.Log($"<color=yellow>Stage Completed [NOT END TILE].</color>");
                }

            // no valid move?
            if (fromTile.type != Tile.TileType.END && GetValidMoves().Length == 0)
            {
                Debug.Log("Player stuck (no tiles to jump to).");
                state = State.STUCK;
                OnStateChanged?.Invoke();
            }

            if (toTile != null)
                Debug.Log($"New player position: {playerPos} {toTile.GetType()}");

            OnPlayerMove?.Invoke();

        }

        public Move[] GetValidMoves()
        {

            List<Move> validMoves = new List<Move>();
            Position to = null;

            foreach (Move.Type moveType in new Move.Type[] { Move.Type.HOP, Move.Type.JUMP })
            {

                int dist = moveType == Move.Type.JUMP ? 2 : 1;

                foreach (Move.Direction dir in Enum.GetValues(typeof(Move.Direction)))
                {

                    switch (dir)
                    {

                        case Move.Direction.NORTH: to = new Position(playerPos.x, playerPos.y + dist); break;
                        case Move.Direction.SOUTH: to = new Position(playerPos.x, playerPos.y - dist); break;
                        case Move.Direction.EAST: to = new Position(playerPos.x + dist, playerPos.y); break;
                        case Move.Direction.WEST: to = new Position(playerPos.x - dist, playerPos.y); break;

                    }

                    Tile toTile = GetTileAt(to);

                    if(toTile != null)
                        validMoves.Add(new Move(0, dir, moveType, playerPos, to));

                }

            }

            return validMoves.ToArray();

        }

        public string ToJson()
        {

            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            };

            return JsonConvert.SerializeObject(this, settings);

        }

        public Tile[] GetTiles()
        {

            List<Tile> allTiles = new List<Tile>();

            foreach (int x in tiles.Keys)
                foreach (int y in tiles[x].Keys)
                    allTiles.AddRange(tiles[x][y]);

            return allTiles.ToArray();

        }

        public Vector3 GetTilePosition(Tile tile)
        {
            
            int depth = GetTileDepth(tile);

            return new Vector3(
               tile.position.x,
               0 - depth,
               tile.position.y);
        }

        public int GetTileCountAt(Position pos)
        {

            if (!tiles.ContainsKey(pos.x))
                return 0;

            if (!tiles[pos.x].ContainsKey(pos.y))
                return 0;

            return tiles[pos.x][pos.y].Count;

        }

        public Tile GetTileAt(Position pos)
        {

            if (!tiles.ContainsKey(pos.x))
                return null;

            if (!tiles[pos.x].ContainsKey(pos.y))
                return null;

            if (tiles[pos.x][pos.y].Count == 0)
                return null;

            return tiles[pos.x][pos.y][0];

        }

        public int GetTileDepth(Tile tile)
        {

            if (!tiles.ContainsKey(tile.position.x))
                return -1;

            if (!tiles[tile.position.x].ContainsKey(tile.position.y))
                return -1;

            if (!tiles[tile.position.x][tile.position.y].Contains(tile))
                return -1;

            return tiles[tile.position.x][tile.position.y].IndexOf(tile);

        }

        private void Add(Tile tile)
        {

            if (!tiles.ContainsKey(tile.position.x))
                tiles.Add(tile.position.x, new Dictionary<int, List<Tile>>());

            if (!tiles[tile.position.x].ContainsKey(tile.position.y))
                tiles[tile.position.x].Add(tile.position.y, new List<Tile>());

            if (tiles[tile.position.x][tile.position.y].Contains(tile))
                Debug.LogWarning($"Tile already added to position {tile.position.x},{tile.position.y}");
            else
                tiles[tile.position.x][tile.position.y].Add(tile);

            OnTileAdded?.Invoke(tile);

        }

        private bool Remove(Tile tile)
        {

            if (!tiles.ContainsKey(tile.position.x))
            {
                Debug.LogWarning($"No tiles at position {tile.position.x},{tile.position.y}");
                return false;
            }

            if (!tiles[tile.position.x].ContainsKey(tile.position.y))
            {
                Debug.LogWarning($"No tiles at position {tile.position.x},{tile.position.y}");
                return false;
            }

            if (!tiles[tile.position.x][tile.position.y].Remove(tile))
            {
                Debug.LogWarning($"Position does not contain tile: {tile.position.x},{tile.position.y}");
                return false;
            }

            OnTileRemoved?.Invoke(tile);

            foreach (Tile t in tiles[tile.position.x][tile.position.y])
                OnTileUpdated?.Invoke(t);

            return true;

        }

    }

    public class Position
    {

        public readonly int x, y;

        public Position(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public override string ToString()
        {
            return $"Position: {x},{y}";
        }

    }

    public class Move
    {

        public enum Direction
        {
            NORTH, // +y
            SOUTH, // -Y
            EAST, // +X
            WEST // -X 
        }

        public enum Type
        {

            //START,

            HOP,
            JUMP, 
            TP

            //END
        }

        public Direction direction;
        public Type type;

        public int index;
        public Position from, to;

        public Move(int index, Direction direction, Type type, Position from, Position to)
        {

            this.index = index;
            this.direction = direction;
            this.type = type;
            this.from = from;
            this.to = to;

            
        }

        public override string ToString()
        {
            return $"Direction: {direction}, type: {type} from: {from}, to: {to}";
        }

    }

    public class Tile
    {

        public enum TileType
        {

            START,
            NORMAL,
            TELEPORT,
            END

        }

        public Position position;

        public TileType type = TileType.NORMAL;

        public Tile(int x, int y)
        {

            this.position = new Position(x, y);
            type = TileType.NORMAL;

        }

        [JsonConstructor]
        private Tile(Position position)
        {

            this.position = position;
            this.type = TileType.NORMAL;

        }

    }

    public class StartTile : Tile
    {
        public StartTile(int x, int y) : base(x, y)
        {

            type = TileType.START;

        }

    }

    public class EndTile : Tile
    {

        public EndTile(int x, int y) : base(x, y)
        {

            type = TileType.END;

        }

    }

    public class TeleportTile : Tile
    {

        public Position to;

        public TeleportTile(int x, int y, int toX, int toY) : base(x, y)
        {

            to = new Position(toX, toY);
            type = TileType.TELEPORT;

        }

        [JsonConstructor]
        private TeleportTile(Position position, Position to) : base(position.x, position.y)
        {

            this.to = to;
            this.type = TileType.TELEPORT;

        }

    }



}
