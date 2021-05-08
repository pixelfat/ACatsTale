using pixelfat.CatsTale;
using UnityEngine;

public class TileViewBase : MonoBehaviour
{

    public static float lateralSpacing = 1f;
    public static float verticalSpacing = .25f;

    public BoardData board;

    public Tile tile;

    public void RemoveFromPlay()
    {
        Destroy(gameObject);
    }

    public void Set(BoardData board, Tile tile)
    {

        this.board = board;
        this.tile = tile;

        board.OnTileUpdated += HandleTileUpdated;

        transform.position = GetPosition();

    }

    private Vector3 GetPosition()
    {

        Vector3 pos = board.GetTilePosition(tile);

        pos.x *= lateralSpacing;
        pos.y *= verticalSpacing;
        pos.z *= lateralSpacing;

        return pos;
    }

    private void HandleTileUpdated(Tile tileUpdated)
    {

        if (tileUpdated != tile)
            return;

        if(GetPosition() != transform.position)
        {

            Debug.Log("Tile moved.");
            transform.position = GetPosition();

        }

    }


}
