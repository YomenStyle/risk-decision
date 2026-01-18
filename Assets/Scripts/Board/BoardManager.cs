using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public static BoardManager Instance;

    [Header("Board Size")]
    public int width = 5;
    public int height = 4;
    public float tileSize = 1f;

    private GameObject[,] grid;

    private void Awake()
    {
        Instance = this;
        grid = new GameObject[width, height];
    }

    // 좌표 → 월드 위치
    public Vector3 GridToWorld(Vector2Int gridPos)
    {
        return new Vector3(
            gridPos.x * tileSize,
            gridPos.y * tileSize,
            0f
        );
    }

    // 월드 → 좌표
    public Vector2Int WorldToGrid(Vector3 worldPos)
    {
        return new Vector2Int(
            Mathf.RoundToInt(worldPos.x / tileSize),
            Mathf.RoundToInt(worldPos.y / tileSize)
        );
    }

    // 범위 체크
    public bool IsInsideBoard(Vector2Int pos)
    {
        return pos.x >= 0 && pos.x < width &&
               pos.y >= 0 && pos.y < height;
    }

    // 배치 가능?
    public bool CanPlace(Vector2Int pos)
    {
        if (!IsInsideBoard(pos)) return false;
        return grid[pos.x, pos.y] == null;
    }

    // 유닛 배치
    public void PlaceUnit(GameObject unit, Vector2Int pos)
    {
        if (!CanPlace(pos)) return;

        grid[pos.x, pos.y] = unit;
        unit.transform.position = GridToWorld(pos);
    }

    // 유닛 제거
    public void RemoveUnit(Vector2Int pos)
    {
        if (!IsInsideBoard(pos)) return;
        grid[pos.x, pos.y] = null;
    }
}