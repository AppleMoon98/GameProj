using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Slot
{
    public int id = 0;
    public int count = 0;
}

[System.Serializable]
public class Item
{
    public int id;
    public string name;
    public string description;
    public Sprite icon;
    public int maxCount = 99;
    public bool isUseable = false;
}

public class ItemManager : MonoBehaviour
{
    public UserInterfaceManager userInterfaceManager;

    public List<Slot> slots = new();
    public List<Image> slotImages = new();
    public List<Item> items = new();

    public Inventory inventory;
    public Inventory storage;
    public int slotIndex = -1;

    private void Start()
    {
        // 다른 오브젝트 Awake()에서 inventory, storage 오브젝트를 찾아서 참조해야함
        // 해당 메서드를 Awake()에서 실행하게 되면 오류가 남
        SlotIndexUpdate();
    }

    private void SlotIndexUpdate()
    {
        for (int i = 0; i < slots.Count; i++)
        {
            Inven_Slot inven_Slot = slotImages[i].GetComponentInParent<Inven_Slot>();
            inven_Slot.slotIndex = i;
            inven_Slot.slotImage.gameObject.SetActive(false);
        }
    }
    // 아이템 슬롯들 값 지정

    public void InventorySwitch()
    {
        // 툴팁이 열려있으면 종료
        if (inventory.GetActive())
            userInterfaceManager.CloseTooltip();

        // 창고와 인벤이 동시에 켜져있을 때 I 누르면
        if (storage.GetActive() && inventory.GetActive())
            storage.SetActive(false);

        inventory.SetActive(!inventory.GetActive());
        if (inventory.subInventory.GetComponent<SubInventory>().gameObject.activeSelf)
            inventory.subInventory.GetComponent<SubInventory>().OnExit();
    }
    // 인벤토리 열고 닫고

    public void SlotChange(int slotIndex)
    {
        // this.slotIndex → 기존 슬롯 Index / sloIndex → 이동할 슬롯
        // src → 기존 / dest → 이동
        // bool 값의 경우 T → 창고 / F → 인벤토리

        // 슬롯 인덱스가 같거나, -1이거나, 비어있으면 return
        if (slotIndex == this.slotIndex || slotIndex == -1 || this.slotIndex == -1 || slots[this.slotIndex].id == 0)
        {
            this.slotIndex = -1;
            return;
        }

        // src -> dest
        Slot src = slots[this.slotIndex];
        Slot dest = slots[slotIndex];

        // true -> 창고, false -> 인벤토리
        bool isSrc = this.slotIndex >= 20;
        bool isDest = slotIndex >= 20;

        Item srcItemData = items.Find(x => x.id == src.id);
        Item destItemData = items.Find(x => x.id == dest.id);

        // ====================
        // 인벤토리 및 창고 구현
        // ====================
        // 1. 창고 : 최대 개수 무제한
        // 2. 인벤토리 : 최대 개수 제한
        // 조건 순서는 정상 -> 초과 순으로 작성
        // ====================

        // ====================
        // 1. 창고 -> 인벤토리
        // ====================
        if (isSrc && !isDest)
            // ====================
            // 1-1. 빈 슬롯으로 이동 (이동 정상/초과)
            // ====================
            if (dest.id == 0)
                if (src.count <= srcItemData.maxCount)
                    MoveSlot(src, dest, src.count);
                else
                    MoveSlot(src, dest, srcItemData.maxCount);
            // ====================
            // 1-2. 같은 아이템으로 이동 (병합 정상/초과)
            // ====================
            else if (dest.id == src.id)
                MergeSlot(src, dest, srcItemData.maxCount);
            // ====================
            // 1-3. 다른 아이템으로 이동 (스왑 정상/초과)
            // ====================
            else if (src.count <= srcItemData.maxCount)
                SwapSlot(src, dest);
            else
                Debug.Log("스왑 불가 / 1-3 else문 참고");
        // ====================
        // 2. 인벤토리 -> 창고
        // ====================
        else if (!isSrc && isDest)
            // ====================
            // 2-1. 빈 슬롯으로 이동 (이동 정상)
            // ====================
            if (dest.id == 0)
                MoveSlot(src, dest, src.count);
            // ====================
            // 2-2. 같은 아이템으로 이동 (병합 정상)
            // ====================
            else if (dest.id == src.id)
                MergeSlot(src, dest, int.MaxValue);
            // ====================
            // 2-3. 다른 아이템으로 이동 (스왑 정상/초과)
            // ====================
            else if (dest.count <= destItemData.maxCount)
                SwapSlot(src, dest);
            else
                Debug.Log("스왑 불가 / 2-3 else문 참고");
        // ====================
        // 3. 내부 이동: 창고 -> 창고
        // ====================
        else if (isSrc && isDest)
            // ====================
            // 3-1. 빈 슬롯으로 이동 (이동 정상)
            // ====================
            if (dest.id == 0)
                MoveSlot(src, dest, src.count);
            // ====================
            // 3-2. 같은 아이템으로 이동 (병합 정상)
            // ====================
            else if (dest.id == src.id)
                MergeSlot(src, dest, int.MaxValue);
            // ====================
            // 3-3. 다른 아이템으로 이동 (스왑 정상)
            // ====================
            else
                SwapSlot(src, dest);
        // ====================
        // 4. 내부 이동: 인벤토리 -> 인벤토리
        // ====================
        else if (!isSrc && !isDest)
            // ====================
            // 4-1. 빈 슬롯으로 이동 (이동 정상)
            // ====================
            if (dest.id == 0)
                MoveSlot(src, dest, src.count);
            // ====================
            // 4-2. 같은 아이템으로 이동 (병합 정상, 초과 스왑)
            // ====================
            else if (dest.id == src.id)
            {
                int temp = srcItemData.maxCount - dest.count;
                if (temp > 0)
                    MergeSlot(src, dest, srcItemData.maxCount);
                else
                    SwapSlot(src, dest);
            }
            // ====================
            // 4-3. 다른 아이템으로 이동 (스왑 정상)
            // ====================
            else
                SwapSlot(src, dest);

        ReloadFilter(this.slotIndex);
        ReloadFilter(slotIndex);
        this.slotIndex = -1;
    }
    // 슬롯의 아이템을 교환하는 메서드

    public void ReloadSlot(int slotIndex)
    {
        Item item = items.Find(x => x.id == slots[slotIndex].id);

        slotImages[slotIndex].sprite = item.icon;
        slotImages[slotIndex].GetComponentInChildren<Text>().text = slots[slotIndex].count.ToString();
        slotImages[slotIndex].gameObject.SetActive(true);
    }
    // 인벤토리 슬롯을 새로고침하는 메서드. 슬롯에 아이템이 없으면 해당 슬롯 이미지를 비활성화합니다

    public void ReloadFilter(int slotIndex)
    {
        if (slots[slotIndex].id != 0)
            ReloadSlot(slotIndex);
        else
            slotImages[slotIndex].gameObject.SetActive(false);
    }
    // 인벤토리 슬롯의 아이콘을 새로고침하는 메서드. ReloadSlot으로 연결됨

    public bool AddItem(int id, int count)
    {
        Item item = items.Find(x => x.id == id);
        if (item == null) return false;

        // 같은 아이템이 있는 슬롯을 먼저 찾고, 그 슬롯에 추가할 수 있는지 확인
        for (int i = 0; i < 20; i++)
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

            if (count == 0)
                return true;
        }

        // 빈 슬롯을 찾아서 아이템을 추가
        for (int i = 0; i < 20; i++)
        {
            Slot slot = slots[i];

            if (slot.id != 0)
                continue;

            slot.id = item.id;

            int add = item.maxCount > count ? count : item.maxCount;
            slot.count = add;
            count -= add;
            ReloadSlot(i);

            if (count < 1)
                return true;
        }

        return false;
    }
    // 아이템을 인벤토리에 추가하는 메서드

    private void MoveSlot(Slot src, Slot dest, int amount)
    {
        dest.id = src.id;
        dest.count = amount;
        src.count -= amount;

        if (src.count <= 0)
            src.id = 0;
    }
    // 슬롯 체인지 - 이동

    private void MergeSlot(Slot src, Slot dest, int maxCount)
    {
        int space = maxCount - dest.count;
        int moveAmount = src.count > space ? space : src.count;

        dest.count += moveAmount;
        src.count -= moveAmount;

        if (src.count <= 0)
            src.id = 0;
    }
    // 슬롯 체인지 - 병합

    private void SwapSlot(Slot src, Slot dest)
    {
        int tempId = src.id;
        int tempCount = src.count;

        src.id = dest.id;
        src.count = dest.count;

        dest.id = tempId;
        dest.count = tempCount;
    }
    // 슬롯 체인지 - 스왑
}
