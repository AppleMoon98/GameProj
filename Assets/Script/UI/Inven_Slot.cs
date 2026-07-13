using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Inven_Slot : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IDropHandler, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public int slotIndex;
    public Image iconImg;
    public Text countTxt;
    public Inventory inventory;

    public void VariableSetting()
    {
        // 아이콘과 텍스트 추적
        Transform image = transform.GetChild(0);
        iconImg = image.GetComponent<Image>();
        countTxt = image.GetChild(0).GetComponent<Text>();
        
        inventory = transform.GetComponentInParent<Inventory>(true);
    }

    public void UpdateUI(Item item, int count)
    {
        if (item == null || item.id == 0)
        {
            iconImg.gameObject.SetActive(false);
            return;
        }

        iconImg.sprite = item.icon;
        countTxt.text = count.ToString();
        iconImg.gameObject.SetActive(true);

    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (inventory.itemManager.slots[slotIndex].id == 0)
            return;

        inventory.userInterfaceManager.dragging = true;
        inventory.itemManager.slotIndex = slotIndex;

        if (iconImg != null)
            iconImg.raycastTarget = false;
    }
    // 드래그 시작 시 슬롯 인덱스를 인벤토리로 보내고, 드래그 상태를 true

    public void OnDrag(PointerEventData eventData)
    {
        if (inventory.itemManager.slots[slotIndex].id == 0)
            return;

        iconImg.rectTransform.position = eventData.position;
    }
    // 드래그 중이면 아이템 이미지를 마우스 위치로 이동

    public void OnEndDrag(PointerEventData eventData)
    {
        if (iconImg != null)
            iconImg.raycastTarget = true;
        iconImg.rectTransform.localPosition = Vector3.zero;
        inventory.userInterfaceManager.dragging = false;
    }
    // 드래그가 끝나면 이미지 위치를 원래대로 돌려놓고 드래그 상태를 false로 변경

    public void OnDrop(PointerEventData eventData)
    {
        inventory.itemManager.SlotChange(slotIndex);
    }
    // 아이템 이미지 안에 드래그를 놓았을 경우

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (inventory.itemManager.slots[slotIndex].id == 0)
            return;

        Item item = inventory.itemManager.items.Find(x => x.id == inventory.itemManager.slots[slotIndex].id);
        inventory.userInterfaceManager.OpenTooltip(item.name, item.description, eventData.position);
    }
    // 아이템 이미지 안으로 들어오면 툴팁 표시

    public void OnPointerExit(PointerEventData eventData)
    {
        if (inventory.itemManager.slots[slotIndex].id == 0)
            return;

        inventory.userInterfaceManager.CloseTooltip();
    }
    // 아이템 이미지 밖으로 나가면 툴팁 제거

    public void OnPointerClick(PointerEventData eventData)
    {
        if (inventory.itemManager.slots[slotIndex].id == 0)
            return;

        if (slotIndex >= 20)
            return;

        Item item = inventory.itemManager.items.Find(x => x.id == inventory.itemManager.slots[slotIndex].id);
        inventory.subInventory.OnActive(item);
    }
    // 아이템 이미지를 선택 -> 서브 인벤 표시
}
