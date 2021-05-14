using pixelfat.CatsTale;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Game Resources Object", menuName = "pixelfat/A Cats Tale/Game Resources Object", order = 1)]
public class GameResources : ScriptableObject
{

    [SerializeField]
    public List<TileResourceItem> tileResourceItems = new List<TileResourceItem>(); 

    public TileViewBase NewTile(Tile tile)
    {

        foreach (TileResourceItem tileitem in tileResourceItems)
            if (tile.type == tileitem.type)
                return Instantiate<TileViewBase>(tileitem.tileViewResource);

        return null;

    }

}

[Serializable]
public class TileResourceItem
{

    public Tile.TileType type;
    public TileViewBase tileViewResource;

}