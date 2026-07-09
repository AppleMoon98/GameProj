using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum InvType
{
    Inventory,
    Storage
}

public class Inventory : MonoBehaviour
{
    public List<Slot> slots = new();
    public List<Image> slotImages = new();
    public GameManager manager;
    public UserInterfaceManager userInterfaceManager;
    public SubInventory subInventory;
    public InvType invType;

    public int slotIndex = -1;
    public bool dragging = false;

    private void Awake()
    {
        SlotIndexUpdate();
    }

    public void SetActive(bool isActive)
    {
        gameObject.SetActive(isActive);
    }
    // 인벤토리를 활성화 또는 비활성화하는 메서드

    public bool GetActive()
    {
        return gameObject.activeSelf;
    }
    // 인벤토리 활성화 여부를 가져오는 메서드

    public void ReloadSlot(int slotIndex)
    {
        Item item = manager.items.Find(x => x.id == slots[slotIndex].id);

        slotImages[slotIndex].sprite = item.icon;
        slotImages[slotIndex].GetComponentInChildren<Text>().text = slots[slotIndex].count.ToString();
        slotImages[slotIndex].gameObject.SetActive(true);
    }
    // 인벤토리 슬롯을 새로고침하는 메서드. 슬롯에 아이템이 없으면 해당 슬롯 이미지를 비활성화합니다.

    public void ReloadFilter(int slotIndex)
    {
        if(slots[slotIndex].id != 0)
            ReloadSlot(slotIndex);
        else
            slotImages[slotIndex].gameObject.SetActive(false);
    }
    // 인벤토리 슬롯의 아이콘을 새로고침하는 메서드. ReloadSlot으로 연결됨.

    public bool AddItem(int id, int count)
    {
        Item item = manager.items.Find(x => x.id == id);
        if (item == null) return false;

        // 같은 아이템이 있는 슬롯을 먼저 찾고, 그 슬롯에 추가할 수 있는지 확인
        for (int i = 0; i < slots.Count; i++)
        {
            Slot slot = slots[i];

            if (slot.id != item.id)
                continue;

            if (slot.count >= item.maxCount)
                continue;

            int canAdd = item.maxCount - slot.count;
            int add = canAdd > count ? count : canAdd;

            slot.count += add;
            count -= add;
            ReloadSlot(i);

            if(count == 0)
            {
                return true;
            }
        }

        // 빈 슬롯을 찾아서 아이템을 추가
        for (int i = 0; i < slots.Count; i++)
        {
            Slot slot = slots[i];
            
            if(slot.id != 0)
                continue;

            slot.id = item.id;
            
            int add = item.maxCount > count ? count : item.maxCount;
            slot.count = add;
            count -= add;
            ReloadSlot(i);

            if (count < 1)
            {
                return true;
            }
        }

        return false;
    }
    // 아이템을 인벤토리에 추가하는 메서드.

    public void SlotChange(int slotIndex, InvType invType)
    {
        if (slotIndex == this.slotIndex)
            return;

        if (invType != this.invType)
            return;

        int oldSlotIndex = this.slotIndex;
        (slots[slotIndex], slots[oldSlotIndex]) = (slots[oldSlotIndex], slots[slotIndex]);

        ReloadFilter(slotIndex);
        ReloadFilter(oldSlotIndex);
    }
    // 슬롯의 아이템을 교환하는 메서드.

    private void SlotIndexUpdate()
    {
        for (int i = 0; i < slots.Count; i++)
        {
            Inven_Slot inven_Slot = slotImages[i].GetComponentInParent<Inven_Slot>();
            inven_Slot.slotIndex = i;
        }
    }
    // 인벤토리를 열었을 때, 슬롯들의 인덱스 값을 지정.
}
