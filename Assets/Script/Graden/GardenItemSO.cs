using UnityEngine;

[CreateAssetMenu(fileName = "GardenItemSO", menuName = "GardenSO/GardenItemSO")]
public class GardenItemSO : ScriptableObject
{
    public string gradeItemName;
    public string gradeItemDescription;
    public int gardenItemCost;
    
    [Header("Object Garden ItemSO Configuration")]
    public Vector2Int objectSize;
    public GameObject objectPlacement;
}
