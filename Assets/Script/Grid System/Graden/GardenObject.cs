using UnityEngine;

public class GardenObject : MonoBehaviour
{
    [SerializeField] private GardenItemSO gardenItemSo;
    
    private Vector2Int _anchorCell;
    private bool _isInitialized;
    private bool _isSellable;

    public GardenItemSO GardenItemSo => gardenItemSo;
    
    public void Initialize(GardenItemSO gardenItemData, Vector2Int anchorCell)
    {
        gardenItemSo    = gardenItemData;
        _anchorCell     = anchorCell;
        _isInitialized  = true;
        _isSellable     = true;
        
        GardenManager.Instance.RegisterGardenObject(this);
    }
    
    public void SellGardenObject()
    {
        if (!_isSellable) return;
        
        if (!_isInitialized || gardenItemSo == null)
        {
            Debug.LogError($"[{name} (SellGardenObject)] Not initialized with placement data - can't free grid cells.");
            return;
        }
        
        CurrencyManager.Instance.AddCurrency(gardenItemSo.gardenItemBasePrice);
        GridManager.Instance.RemoveFootPrint(_anchorCell, gardenItemSo.objectSize);
        GardenManager.Instance.UnregisterGardenObject(this);
        
        Destroy(gameObject);
    }
}
