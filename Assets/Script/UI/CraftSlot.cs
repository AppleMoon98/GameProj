using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class CraftSlot : MonoBehaviour, IPointerEnterHandler, IPointerMoveHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("기본")]
    public Item item;

    [Header("참조")]
    public Crafting crafting;
    public Image iconImage;

    void Awake()
    {
        // ====================
        // 참조 연결
        // ====================
        Transform image = transform.GetChild(0);
        iconImage = image.GetComponent<Image>();
        crafting = transform.GetComponentInParent<Crafting>(true);
    }

    public void UpdateUI(Item item)
    {
        this.item = item;
        if(item == null || item.id == 0)
        {
            iconImage.gameObject.SetActive(false);
            return;
        }

        iconImage.sprite = item.icon;
        iconImage.gameObject.SetActive(true);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (crafting == null) return;

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

        crafting.userInterfaceManager.CloseTooltip();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        crafting.ResultItemReload(item);
    }
    // 아이템 이미지 밖으로 나가면 툴팁 제거
}
