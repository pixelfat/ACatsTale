using pixelfat.CatsTale;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Tiles", menuName = "pixelfat/A Cats Tale/Tile View Library", order = 1)]
public class TileLibrary : ScriptableObject
{

    [SerializeField]
    public List<TileLibraryItem> tileItems = new List<TileLibraryItem>(); 

    public TileViewBase NewTile(Tile tile)
    {

        foreach (TileLibraryItem tileitem in tileItems)
            if (tile.type == tileitem.type)
                return Instantiate<TileViewBase>(tileitem.tileView);

        return null;

    }

}

[Serializable]
public class TileLibraryItem
{

    public Tile.TileType type;
    public TileViewBase tileView;

}