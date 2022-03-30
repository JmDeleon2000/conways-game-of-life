using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Conway : MonoBehaviour
{
    Tilemap tm;
    TileBase[] tileArray;
    bool[][,] data;
    int dt_swap = 0;
    int x_size = 0;
    int bound_offsetx = 0;
    int inbound_offsetx = 0;
    int y_size = 0;
    int bound_offsety = 0;
    int inbound_offsety = 0;
    [SerializeField] float birth_rate = 25f;
    const int bound_padding = 2;


    private void Start()
    {
        tm = GetComponent<Tilemap>();
        BoundsInt bound = tm.cellBounds;

        x_size = bound.max.x - bound.min.x;
        y_size = bound.max.y - bound.min.y;
        bound_offsetx = x_size / 2 + 2;
        bound_offsety = y_size / 2 + 2;
        inbound_offsetx = x_size / 2 + bound_padding/2;
        inbound_offsety = y_size / 2 + bound_padding/2;
        data = new bool[2][,];
        data[0] = new bool[x_size + bound_padding, y_size + bound_padding];
        data[1] = new bool[x_size + bound_padding, y_size + bound_padding];

        tileArray = tm.GetTilesBlock(tm.cellBounds);
        for (int i = tm.cellBounds.xMin+1; i < tm.cellBounds.xMax-1; i++)
            for (int j = tm.cellBounds.yMin+1; j < tm.cellBounds.yMax-1; j++)
            {
                tm.SetTileFlags(new Vector3Int(i, j, 0), TileFlags.None);
                data[dt_swap][i + bound_offsetx, j + bound_offsety] = Random.Range(1, 100) < birth_rate;
            }

    }

    
    private void FixedUpdate()
    {
        for (int i = tm.cellBounds.xMin; i < tm.cellBounds.xMax; i++)
            for (int j = tm.cellBounds.yMin; j < tm.cellBounds.yMax; j++)
            {
                int count = 0;
                if (data[dt_swap][i + inbound_offsetx - 1, j + inbound_offsety - 1]) count++;    //top left
                if (data[dt_swap][i + inbound_offsetx, j + inbound_offsety - 1]) count++;        //top
                if (data[dt_swap][i + inbound_offsetx + 1, j + inbound_offsety - 1]) count++;    //top right
                if (data[dt_swap][i + inbound_offsetx - 1, j + inbound_offsety]) count++;        //left
                if (data[dt_swap][i + inbound_offsetx + 1, j + inbound_offsety]) count++;        //right
                if (data[dt_swap][i + inbound_offsetx - 1, j + inbound_offsety + 1]) count++;    //bottom left
                if (data[dt_swap][i + inbound_offsetx, j + inbound_offsety + 1]) count++;        //bottom
                if (data[dt_swap][i + inbound_offsetx + 1, j + inbound_offsety + 1]) count++;    //bottom right

                data[(dt_swap + 1) % 2][i + inbound_offsetx
                    , j + inbound_offsety] = data[dt_swap][i + inbound_offsetx, j + inbound_offsety];
                
                //Any live cell with two or three live neighbours lives on to the next generation
                data[(dt_swap + 1) % 2][i + inbound_offsetx, j + inbound_offsety] =
                    data[dt_swap][i + inbound_offsetx, j +inbound_offsety] && (count == 2 || count == 3);

                //Any live cell with fewer than two live neighbours dies, as if by underpopulation.
                data[(dt_swap + 1) % 2][i +inbound_offsetx, j +inbound_offsety] =
                    !(data[dt_swap][i +inbound_offsetx, j +inbound_offsety] && count < 2);

                //Any live cell with more than three live neighbours dies, as if by overpopulation.
                data[(dt_swap + 1) % 2][i +inbound_offsetx, j +inbound_offsety] =
                    !(data[dt_swap][i +inbound_offsetx, j +inbound_offsety] && count > 3);

                //Any dead cell with exactly three live neighbours becomes a live cell, as if by reproduction.
                data[(dt_swap + 1) % 2][i +inbound_offsetx, j +inbound_offsety] =
                    (!data[dt_swap][i +inbound_offsetx, j +inbound_offsety]) && count == 3;
            }
        dt_swap = (dt_swap + 1) % 2;
        update_tile_info();
        repopulate();
    }

    void update_tile_info()
    {
        for (int i = tm.cellBounds.xMin; i < tm.cellBounds.xMax; i++)
            for (int j = tm.cellBounds.yMin; j < tm.cellBounds.yMax; j++)
                if (data[dt_swap][i + bound_offsetx , j + bound_offsety]) 
                    tm.SetColor(new Vector3Int(i, j, 0), Color.white);
                else
                    tm.SetColor(new Vector3Int(i, j, 0), Color.black);
    }


    void repopulate()
    {
        for (int i = tm.cellBounds.xMin + 1; i < tm.cellBounds.xMax - 1; i++)
            for (int j = tm.cellBounds.yMin + 1; j < tm.cellBounds.yMax - 1; j++)
                if (data[dt_swap][i + inbound_offsetx, j + inbound_offsety])
                    return;

        for (int i = tm.cellBounds.xMin + 1; i < tm.cellBounds.xMax - 1; i++)
            for (int j = tm.cellBounds.yMin + 1; j < tm.cellBounds.yMax - 1; j++)
                data[dt_swap][i + inbound_offsetx, j + inbound_offsety] = 
                    Random.Range(1, 100) < birth_rate;
            
    }

}
