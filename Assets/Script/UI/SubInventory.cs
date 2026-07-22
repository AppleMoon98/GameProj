using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class SubInventory : MonoBehaviour
{
    [SerializeField] GameObject useButton;
    [SerializeField] Image itemImage;
    [SerializeField] Text itemName;
    [SerializeField] Text itemDescription;
    [SerializeField] int slotIndex;

    public void OnActive(Item item, int slotIndex)
    {
        this.slotIndex = slotIndex;
        itemImage.sprite = item.icon;
        itemName.text = LocalizationSettings.StringDatabase.GetLocalizedString("Item Table", item.productName);
        itemDescription.text = LocalizationSettings.StringDatabase.GetLocalizedString("Item Table", item.description);
        useButton.SetActive(item.isUseable);
        gameObject.SetActive(true);
    }

    public void OnUse()
    {
        // 사용시 작동할 코드 작성
    }

    public void OnDelete()
    {
        // 삭제시 작동할 코드 작성
    }
}
