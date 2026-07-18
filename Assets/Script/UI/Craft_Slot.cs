using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class Craft_Slot : MonoBehaviour, IPointerEnterHandler, IPointerMoveHandler, IPointerExitHandler
{
    [Header("기본")]
    public int slotIndex;
    public Image iconImage;
    public Text countText;

    [Header("참조")]
    public Crafting crafting;

    public void VariableSetting()
    {
        Transform image = transform.GetChild(0);
        iconImage = image.GetComponent<Image>();
        countText = image.GetChild(0).GetComponent<Text>();

        crafting = transform.GetComponentInParent<Crafting>(true);
    }

    public void UpdateUI(Item item, int count)
    {
        if(item == null || item.id == 0)
        {
            iconImage.gameObject.SetActive(false);
            return;
        }

        iconImage.sprite = item.icon;
        countText.text = count.ToString();
        iconImage.gameObject.SetActive(true);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (crafting == null) return;

        if (crafting.itemManager.slots[slotIndex].id == 0)
            return;

        Item item = Array.Find(crafting.itemManager.items, x => x.id == crafting.itemManager.slots[slotIndex].id);
        string localizedName = LocalizationSettings.StringDatabase.GetLocalizedString("Item Table", item.productName);
        string localizedDesc = LocalizationSettings.StringDatabase.GetLocalizedString("Item Table", item.description);
        crafting.userInterfaceManager.OpenTooltip(localizedName, localizedDesc, eventData.position);
    }
    // 아이템 이미지 안으로 들어오면 툴팁 표시

    public void OnPointerMove(PointerEventData eventData)
    {
        crafting.userInterfaceManager.MoveTooltip(eventData.position);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (crafting == null) return;

        if (crafting.itemManager.slots[slotIndex].id == 0)
            return;

        crafting.userInterfaceManager.CloseTooltip();
    }
    // 아이템 이미지 밖으로 나가면 툴팁 제거
}
