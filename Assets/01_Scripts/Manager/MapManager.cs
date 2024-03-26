using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapManager : MonoBehaviour
{
    private static MapManager _instance;
    public static MapManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<MapManager>();
                if (_instance == null)
                {
                    Debug.LogError("Ther are no MapManager");
                }
            }
            return _instance;
        }
    }
    [SerializeField] private Tilemap _tilemap;
    [SerializeField] private LayerMask _whatIsObstacle;

    public List<Vector3> GetAvailabePositionList(Vector3 center, float radius)
    {
        int radiusInt = Mathf.CeilToInt(radius);
        Vector3Int tileCenter = _tilemap.WorldToCell(center);

        List<Vector3> pointList = new List<Vector3>();
        for (int i = -radiusInt; i <= radiusInt; ++i)
        {
            for (int j = -radiusInt; j <= radiusInt; j++)
            {
                if (Mathf.Abs(i) + Mathf.Abs(j) > radius) continue;

                Vector3Int cellPoint = tileCenter + new Vector3Int(j, i);
                TileBase tile = _tilemap.GetTile(cellPoint);

                if (tile != null) continue;

                Vector3 worldPos = _tilemap.GetCellCenterWorld(cellPoint);
                Collider2D col = Physics2D.OverlapCircle(worldPos, 0.5f, _whatIsObstacle);

                if (col != null) continue;
                pointList.Add(worldPos);
            }
        }
        return pointList;
    }
}


