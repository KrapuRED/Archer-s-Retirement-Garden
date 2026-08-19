using UnityEngine;

[CreateAssetMenu(fileName = "GardenItemSO", menuName = "GardenSO/GardenItemSO")]
public class GardenItemSO : ScriptableObject
{
    public string gardenItemName;
    public string gardenItemDescription;
    public int gardenItemCost;
    
    [Header("Object Garden ItemSO Configuration")]
    public Vector2Int objectSize;
    public GameObject objectPlacement;
}
