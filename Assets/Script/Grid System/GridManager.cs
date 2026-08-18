using System;
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
    
    [Header("Cell Visuals")]
    [SerializeField] private GridCell cellPrefab;
    [SerializeField] private bool generateOnAwake = true;
    
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
                Vector3 offSetWorldPos = new Vector3(worldPos.x + offset, 0f, worldPos.z + offset);
                
                GridCell instance = Instantiate(cellPrefab, offSetWorldPos, Quaternion.identity, locationGrid);
                
                instance.name = $"Cell_{x}_{z}";
                instance.Init(cell);
                
                _cells[cell] = instance;
            }
        }
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
        Vector3 local = new Vector3(cell.x * gridSize, 0f, cell.y * gridSize) - HalfExtents;
 
        return locationGrid != null
            ? locationGrid.TransformPoint(local)
            : local;
    }

    public bool IsInsideBounds(Vector2Int cell)
    {
        return cell.x >= 0 && cell.x < gridWidth && cell.y >= 0 && cell.y < gridHeight;
    }

    public bool IsCellOccupied(Vector2Int cell)
    {
        return _occupiedCells.ContainsKey(cell);
    }

    public bool CanPlace(Vector2Int cell)
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
    
    #region TEST

    private void OnDrawGizmos()
    {
        if (locationGrid == null) return;

        Matrix4x4 previousMatrix = Gizmos.matrix;
        Gizmos.matrix = locationGrid.localToWorldMatrix;
 
        Gizmos.color = Color.gray;
        Vector3 offset = -HalfExtents;
 
        for (int x = 0; x <= gridWidth; x++)
        {
            Vector3 start = offset + new Vector3(x * gridSize, 0f, 0f);
            Vector3 end = offset + new Vector3(x * gridSize, 0f, gridHeight * gridSize);
            Gizmos.DrawLine(start, end);
        }
 
        for (int z = 0; z <= gridHeight; z++)
        {
            Vector3 start = offset + new Vector3(0f, 0f, z * gridSize);
            Vector3 end = offset + new Vector3(gridWidth * gridSize, 0f, z * gridSize);
            Gizmos.DrawLine(start, end);
        }
        
        Gizmos.color = Color.red;
        foreach (var cell in _occupiedCells.Keys)
        {
            Vector3 center = offset + new Vector3(cell.x * gridSize, 0f, cell.y * gridSize);
            Gizmos.DrawCube(center + Vector3.up * 0.05f, new Vector3(gridSize * 0.9f, 0.1f, gridSize * 0.9f));
        }
 
        Gizmos.matrix = previousMatrix;
    }
    #endregion

}
