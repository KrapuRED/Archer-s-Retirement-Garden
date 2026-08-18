using UnityEngine;

public class GardenItemButton : MonoBehaviour
{
   [SerializeField] private GardenItemSO gardenItemSO;

   public void OnClickButton()
   {
      Debug.Log($"[{name} Click Button] Clicked {gardenItemSO.gradeItemName}");
   }
   
   public void OnHoldButton()
   {
      Debug.Log($"[{name} Click Button] Hold {gardenItemSO.gradeItemName}");
   }

}
