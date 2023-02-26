using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridBoard : MonoBehaviour
{
    private Grid grid;
    // Start is called before the first frame update
    void Awake()
    {
        grid = GetComponent<Grid>();
    }

    public List<Transform> GetGridTransforms()
    {
        List<Transform> result = new();
        int width = 7;
        int height = 7;
        for (int i = 0; i < height; i++)
        {
            if (i % 2 == 0)
            {
                for (int j = 0; j < width; j++)
                {
                    result.Add(CreateSpace(i, j).transform);
                }
            }
            else
            {
                for (int j = width - 1; j >= 0; j--)
                {
                    result.Add(CreateSpace(i, j).transform);
                }
            }


        }
        return result;
    }

    private GameObject CreateSpace(int x, int y)
    {
        Vector3 spacePosition = grid.CellToWorld(new Vector3Int(x, y, 0));
        GameObject spaceObject = new();
        spaceObject.transform.position = spacePosition;
        return spaceObject;
    }
}
