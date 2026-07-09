using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Inven_Slot : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IDropHandler, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public int slotIndex;
    public Image slotImage;
    public InvType invType;
    public Inventory inventory;

    void Awake()
    {
        slotImage = transform.Find("ItemImage").GetComponent<Image>();
        inventory = GetComponentInParent<Inventory>();
        invType = inventory.invType;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (inventory.slots[slotIndex].id == 0)
            return;

        inventory.dragging = true;
        inventory.slotIndex = slotIndex;

        if(slotImage != null)
            slotImage.raycastTarget = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (inventory.slots[slotIndex].id == 0)
            return;

        slotImage.rectTransform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if(slotImage != null)
            slotImage.raycastTarget = true;
        slotImage.rectTransform.localPosition = Vector3.zero;
        inventory.dragging = false;
    }

    public void OnDrop(PointerEventData eventData)
    {
        inventory.SlotChange(slotIndex, invType);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(inventory.slots[slotIndex].id == 0)
            return;

        Item item = inventory.manager.items.Find(x => x.id == inventory.slots[slotIndex].id);
        inventory.userInterfaceManager.OpenTooltip(item.name, item.description, eventData.position);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (inventory.slots[slotIndex].id == 0)
            return;

        inventory.userInterfaceManager.CloseTooltip();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (inventory.slots[slotIndex].id == 0 || inventory.invType == InvType.Storage)
            return;

        Item item = inventory.manager.items.Find(x => x.id == inventory.slots[slotIndex].id);
        inventory.subInventory.OnActive(item);
    }
}
