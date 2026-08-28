using UnityEngine;
using System.Collections.Generic;


public class GridManager : MonoBehaviour
{
    public static GridManager Instance { get; private set;}

    [Header("Grid Configuration")] 
    [SerializeField] private Transform locationGrid;

    [SerializeField] private float offset;
    [SerializeField] private float gridSize = 2f;
    [SerializeField] private int gridWidth = 20;
    [SerializeField] private int gridHeight = 20;
    [SerializeField] private bool centerGrid = true;
    
    [Header("Auto Fit (optional)")]
    [Tooltip("If assigned, locationGrid's position and gridWidth/gridHeight are automatically calculated from this renderer's bounds before generating the grid.")]
    [SerializeField] private Renderer targetSurface;
    [SerializeField] private GardenObject prefabMiddleBuilding;
    
    [Header("Cell Visuals")]
    [SerializeField] private GridCell cellPrefab;
    [SerializeField] private bool generateOnAwake = true;
    [SerializeField] private Material cellBeenOccupied;
    
    private readonly Dictionary<Vector2Int, GridCell> _cells = new();
    private readonly Dictionary<Vector2Int, GameObject> _occupiedCells = new();
 
    private Vector3 HalfExtents => centerGrid
        ? new Vector3(gridWidth * gridSize * 0.5f, 0f, gridHeight * gridSize * 0.5f)
        : Vector3.zero;

    private Vector3 _worldPos;
    
    public float GridSize => gridSize;
    public Vector3 Origin => locationGrid != null ? locationGrid.position : Vector3.zero;
    
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        AutoFillGrid();
        
        if (generateOnAwake)
            GenerateGrid();
        
        SpawnBuilding();
    }
    
    #region Grid Generation and Clearing

    private void AutoFillGrid()
    {
        if (targetSurface == null || locationGrid == null)
        {
            Debug.LogError($"[{name} AutoFillGrid] targetSurface is NULL!");
            return;
        }

        Bounds bounds = targetSurface.bounds;
        locationGrid.position = bounds.center;
        
        gridWidth = Mathf.Max(1, Mathf.RoundToInt(bounds.size.x / gridSize));
        gridHeight = Mathf.Max(1, Mathf.RoundToInt(bounds.size.z / gridSize));
    }
    
    private void GenerateGrid()
    {
        if (cellPrefab == null || locationGrid == null)
        {
            Debug.LogError($"[GridManager] Cannot find cell prefab or locationGrid for {name}");
            return;
        }

        ClearGrid();

        for (int x = 0; x < gridWidth; x++)
        {
            for (int z = 0; z < gridHeight; z++)
            {
                var cell = new Vector2Int(x, z);
                
                Vector3 worldPos = GridToWorld(cell);
                GridCell instance = Instantiate(cellPrefab, worldPos, Quaternion.identity, locationGrid);
                
                instance.name = $"Cell_{x}_{z}";
                instance.Init(cell);
                
                _cells[cell] = instance;
            }
        }
    }

    private void SpawnBuilding()
    {
        if (prefabMiddleBuilding == null) return;

        Vector2Int anchorCell = new Vector2Int(
            (gridWidth - prefabMiddleBuilding.GardenItemSo.objectSize.x) / 2,
            (gridHeight - prefabMiddleBuilding.GardenItemSo.objectSize.y) / 2
        );
        
        Vector3 worldPos = GetFootprintCenter(anchorCell, prefabMiddleBuilding.GardenItemSo.objectSize);
        
        GardenObject instance = Instantiate(prefabMiddleBuilding, worldPos, Quaternion.identity, locationGrid);
        
        //instance.Initialize(prefabMiddleBuilding.GardenItemSo, anchorCell);
        instance.name = $"Cell_MiddleBuilding";
        
        PlaceFootPrint(anchorCell, prefabMiddleBuilding.GardenItemSo.objectSize, instance.gameObject);
    }
    
    private void ClearGrid()
    {
        foreach (var cell in _cells.Values)
        {
            if (cell != null)
            {
                Destroy(cell.gameObject);
            }
        }
        _cells.Clear();
    }

    #endregion
    
    #region Controller Configuration
    
    public GridCell GetCell(Vector2Int cell)
    {
        _cells.TryGetValue(cell, out GridCell cellInstance);
        return cellInstance;
    }
    
    public Vector2Int WorldToGrid(Vector3 worldPosition)
    {
        Vector3 local = locationGrid != null ?
            locationGrid.InverseTransformPoint(worldPosition)
            : worldPosition;

        local += HalfExtents;
        int x = (Mathf.RoundToInt(local.x / gridSize));
        int z = (Mathf.RoundToInt(local.z / gridSize));
        
        return new Vector2Int(x, z);
    }
    #endregion

    #region Main Grid System

    public Vector3 GridToWorld(Vector2Int cell)
    {
        Vector3 local = new Vector3((cell.x * gridSize) + offset, 0f, (cell.y * gridSize)+ offset) - HalfExtents;
 
        return locationGrid != null
            ? locationGrid.TransformPoint(local)
            : local;
    }

    public Vector3 ClampToGridBounds(Vector3 worldPosition)
    {
        if (locationGrid == null) return worldPosition;
        
        Vector3 local = locationGrid.InverseTransformPoint(worldPosition);
        Vector3 half = HalfExtents;
        
        local.x = Mathf.Clamp(local.x, -half.x, half.x);
        local.z =  Mathf.Clamp(local.z, -half.z, half.z);
        
        return locationGrid.TransformPoint(local);
    }

    private bool IsInsideBounds(Vector2Int cell)
    {
        return cell.x >= 0 && cell.x < gridWidth && cell.y >= 0 && cell.y < gridHeight;
    }

    private bool IsCellOccupied(Vector2Int cell)
    {
        return _occupiedCells.ContainsKey(cell);
    }

    private bool CanPlace(Vector2Int cell)
    {
        return IsInsideBounds(cell) && !IsCellOccupied(cell);
    }

    public void PlaceObject(Vector2Int cell, GameObject obj)
    {
        if (!CanPlace(cell)) return;
        _occupiedCells[cell] = obj;
    }
    
    public void RemoveObject(Vector2Int cell)
    {
        _occupiedCells.Remove(cell);
    }
    #endregion

    public Vector3 GetFootprintCenter(Vector2Int anchorCell, Vector2Int sizeCell)
    {
        float centerX = (anchorCell.x + (sizeCell.x - 1) * 0.5f) * gridSize;
        float centerZ = (anchorCell.y + (sizeCell.y - 1) * 0.5f) * gridSize;
        Vector3 local = new Vector3(centerX + offset, offset, centerZ + offset) - HalfExtents;

        return locationGrid != null
            ? locationGrid.TransformPoint(local)
            : local;
    }

    public bool CanPlaceFootPrint(Vector2Int anchorCell, Vector2Int sizeCell)
    {
        for (int x = 0; x < sizeCell.x; x++)
        {
            for (int z = 0; z < sizeCell.y; z++)
            {
                if (!CanPlace(anchorCell + new Vector2Int(x, z)))
                    return false;
            }
        }
        return true;
    }

    public void PlaceFootPrint(Vector2Int anchorCell, Vector2Int sizeCell, GameObject obj)
    {
        bool canPlace = CanPlace(anchorCell + new Vector2Int(sizeCell.x, sizeCell.y));

        for (int x = 0; x < sizeCell.x; x++)
        {
            for (int z = 0; z < sizeCell.y; z++)
            {
                _occupiedCells[anchorCell + new Vector2Int(x, z)] = obj;
                if (_cells.TryGetValue(anchorCell + new Vector2Int(x, z), out GridCell cell))
                {
                    cell.SetHighlighted(canPlace);
                }
            }
        }
    }

    public void RemoveFootPrint(Vector2Int anchorCell, Vector2Int sizeCell)
    {
        for (int x = 0; x < sizeCell.x; x++)
        {
            for (int z = 0; z < sizeCell.y; z++)
            {
                var cellPos = anchorCell + new Vector2Int(x, z);
                _occupiedCells.Remove(cellPos);

                if (_cells.TryGetValue(cellPos, out GridCell cell))
                {
                    cell.SetHighlighted(false);
                }
            }
        }
    }
    
    private void OnDrawGizmos()
        {
            if (locationGrid == null) return;
    
            Matrix4x4 previousMatrix = Gizmos.matrix;
            Gizmos.matrix = locationGrid.localToWorldMatrix;
     
            Gizmos.color = Color.gray;
            Vector3 offsetCenter = -HalfExtents;
            Vector3 addOffest = new Vector3(offsetCenter.x + offset, offsetCenter.y, offsetCenter.z + + offset);
     
            for (int x = 0; x <= gridWidth; x++)
            {
                Vector3 start = offsetCenter + new Vector3(x * gridSize, 0f, 0f);
                Vector3 end = offsetCenter + new Vector3(x * gridSize, 0f, gridHeight * gridSize);
                Gizmos.DrawLine(start, end);
            }
     
            for (int z = 0; z <= gridHeight; z++)
            {
                Vector3 start = offsetCenter + new Vector3(0f, 0f, z * gridSize);
                Vector3 end = offsetCenter + new Vector3(gridWidth * gridSize, 0f, z * gridSize);
                Gizmos.DrawLine(start, end);
            }
            
            Gizmos.color = Color.red;
            foreach (var cell in _occupiedCells.Keys)
            {
                Vector3 center = addOffest + new Vector3(cell.x * gridSize, 0f, cell.y * gridSize);
                Gizmos.DrawCube(center + Vector3.up * 0.05f, new Vector3(gridSize * 0.9f, 0.1f, gridSize * 0.9f));
            }
     
            Gizmos.matrix = previousMatrix;
        }
}
