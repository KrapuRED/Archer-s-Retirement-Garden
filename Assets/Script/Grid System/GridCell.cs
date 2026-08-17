using UnityEngine;

public class GridCell : MonoBehaviour
{
    public Vector2Int Coordinates { get; private set; }
 
    [SerializeField] private Renderer cellRenderer;
    [SerializeField] private Color defaultColor = Color.white;
    [SerializeField] private Color highlightColor = Color.yellow;

    public void Init(Vector2Int coordinates)
    {
        Coordinates = coordinates;
    }
    
    public void SetHighlighted(bool highlighted)
    {
        if (cellRenderer == null) return;
        cellRenderer.material.color = highlighted ? highlightColor : defaultColor;
    }
}
