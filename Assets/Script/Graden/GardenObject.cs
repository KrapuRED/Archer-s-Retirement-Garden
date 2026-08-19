using UnityEngine;

public class GardenObject : MonoBehaviour
{
    [SerializeField] private GardenItemSO gardenItemSo;
    
    private Vector2Int _anchorCell;
    private bool _isInitialized;

    public void Initialize(GardenItemSO gardenItemData, Vector2Int anchorCell)
    {
        gardenItemSo    = gardenItemData;
        _anchorCell     = anchorCell;
        _isInitialized  = true;
    }
    
    public void SellGardenObject()
    {
        if (!_isInitialized || gardenItemSo == null)
        {
            Debug.LogError($"[{name} (SellGardenObject)] Not initialized with placement data - can't free grid cells.");
            return;
        }

        Debug.Log($"[{name} (SellGardenObject)] Sell this Object");
        
        GridManager.Instance.RemoveFootPrint(_anchorCell, gardenItemSo.objectSize);
        
        Destroy(gameObject);
    }
}
