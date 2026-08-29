using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GardenItemSO", menuName = "GardenSO/GardenItemSO")]
public class GardenItemSO : ScriptableObject
{
    public string gardenItemName;
    public string gardenItemDescription;
    public Sprite gardenItemImage;
    public int gardenItemBasePrice;
    public int priceIncrease;
    
    public List<BoostStatusData> gardenBoostItemDatas;
    
    [Header("Object Garden ItemSO Configuration")]
    public Vector2Int objectSize;
    public GameObject objectPlacement;
}
