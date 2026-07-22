using NUnit.Framework.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ItemManager : MonoBehaviour
{
    private const int INVEN_MAX_COUNT = 20;

    public UserInterfaceManager userInterfaceManager;

    public Slot[] slots;
    public InvenSlot[] invenSlots;
    public Item[] items;

    public Inventory inventory;
    public Inventory storage;
    public int slotIndex = -1;

    private void Awake()
    {
        for (int i = 0; i < invenSlots.Length; i++)
            SlotIndexUpdate();
    }

    private void SlotIndexUpdate()
    {
        // ====================
        // 1. Inven_Slot의 SlotIndex, SetActive 설정
        // ====================
        for (int i = 0; i < slots.Length; i++)
        {
            invenSlots[i].VariableSetting(i);
            ReloadSlot(i, Array.Find(items, x => x.id == slots[i].id));
        }
    }

    public void InventorySwitch()
    {
        // 툴팁이 열려있으면 종료
        if (inventory.GetActive())
            userInterfaceManager.CloseTooltip();

        // 창고와 인벤이 동시에 켜져있을 때 I 누르면
        if (storage.GetActive() && inventory.GetActive())
            userInterfaceManager.StorageClose();

        inventory.SetActive(!inventory.GetActive());
        if (inventory.subInventory.GetComponent<SubInventory>().gameObject.activeSelf)
            inventory.subInventory.GetComponent<SubInventory>().gameObject.SetActive(false);
    }

    public void ReloadSlot(int slotIndex, Item item)
    {
        invenSlots[slotIndex].UpdateUI(slots[slotIndex].count, item);
    }

    public bool LootItem(int id, int count)
    {
        // ====================
        // 0. 기본 설정
        // ====================

        // 말도 안되는 것들 리턴
        if (count <= 0 || id <= 0) return false;

        // 람다식으로 item값 서칭, 없으면 리턴
        Item item = System.Array.Find(items, x => x.id == id);
        if (item == null) return false;

        int temp = 0;           // 1번에선 사용 가능한 공간 수, 나머지는 필요한 값

        // ====================
        // 1. 사용 가능한 공간 체크
        // ====================
        for (int i = 0; i < INVEN_MAX_COUNT; i++)
        {
            if (slots[i].id == item.id)
                temp += (item.maxCount - slots[i].count);
            else if (slots[i].id == 0)
                temp += item.maxCount;
        }

        // 획득 아이템 수보다 낮으면 강제로 획득량을 낮춤
        if (temp < count)
            count = temp;
        item.count += count;

        // ====================
        // 2. 같은 아이템 슬롯 채움
        // ====================
        for (int i = 0; i < INVEN_MAX_COUNT; i++)
        {
            if (count <= 0) break;

            Slot slot = slots[i];
            if (slot.id != item.id || slot.count >= item.maxCount)
                continue;

            temp = Mathf.Min(item.maxCount - slot.count, count);
            slot.count += temp;
            count -= temp;

            ReloadSlot(i, item);
        }

        // ====================
        // 3. 마무리로 빈 슬롯 채움
        // ====================
        for (int i = 0; i < INVEN_MAX_COUNT; i++)
        {
            if (count <= 0) break;

            Slot slot = slots[i];
            if (slot.id != 0)
                continue;

            slot.id = item.id;
            temp = Mathf.Min(item.maxCount, count);
            slot.count = temp;
            count -= temp;

            ReloadSlot(i, item);
        }

        return true;
    }

    public bool DropItem(int id, int count)
    {
        // ====================
        // 0. 기본 설정
        // int id = 아이템 코드
        // int count = 소모되는 개수
        // return bool = false 개수 부족 / true 개수 충분
        // ====================

        // 말도 안되는 것들 리턴
        if (count <= 0 || id <= 0) return false;

        // 람다식으로 item값 서칭, 없으면 리턴
        Item item = System.Array.Find(items, x => x.id == id);
        if (item == null) return false;

        int temp = 0;   // 다용도 변수

        // ====================
        // 1. 가지고 있는 아이템 검색
        // temp = 현재 가진 해당 아이템 수
        // ====================
        for (int i = 0; i < slots.Length; i++)
            if (slots[i].id == item.id)
                temp += slots[i].count;

        // 가진 아이템 수가 필요한 것보다 적을 경우 리턴
        if (temp < count)
            return false;

        // ====================
        // 2. 아이템 소모
        // ====================
        for (int i = 0; i < slots.Length; i++)
        {
            if (count <= 0) break;

            Slot slot = slots[i];
            if (slot.id != item.id)
                continue;

            temp = Mathf.Min(slot.count, count);
            slot.count -= temp;
            count -= temp;

            if (slot.count == 0)
                ReloadSlot(i, null);
            else
                ReloadSlot(i, item);
        }

        return true;
    }

    public void DropItem(int slotIndex)
    {
        if (slotIndex > slots.Length) return;

        slots[slotIndex].id = 0;
        slots[slotIndex].count = 0;
    }

    public void SlotChange(int slotIndex)
    {
        // ====================
        // 0. 기본 설정
        // ====================

        // 슬롯 인덱스가 같거나, -1이거나, 비어있으면 return
        if (slotIndex == this.slotIndex || slotIndex == -1 || this.slotIndex == -1 || slots[this.slotIndex].id == 0)
        {
            this.slotIndex = -1;
            return;
        }

        // src -> dest
        Slot src = slots[this.slotIndex];
        Slot dest = slots[slotIndex];

        Item srcItemData = Array.Find(items, x => x.id == src.id);
        Item destItemData = Array.Find(items, x => x.id == dest.id);

        // 창고 인벤 판단
        static int GetMaxCount(int index, Item item)
        {
            if (item == null) return 0;
            return index >= INVEN_MAX_COUNT ? int.MaxValue : item.maxCount;
        }

        // 이동 종류에 따른 최대값 정의
        int destCount = GetMaxCount(slotIndex, srcItemData);

        // ====================
        // 1. 이동
        // ====================
        if (dest.id == 0)
            MoveSlot(src, dest, Mathf.Min(src.count, destCount));

        // ====================
        // 2. 병합
        // ====================
        else if (dest.id == src.id)
            MergeSlot(src, dest, destCount);

        // ====================
        // 3. 스왑
        // ====================
        else
        {
            int srcCount = GetMaxCount(this.slotIndex, destItemData);
            if (src.count <= destCount && dest.count <= srcCount)
                SwapSlot(src, dest);
            else
                Debug.Log("[ERROR] 최대 수량을 초과함 : ItemManager.SlotChange");
        }

        // ====================
        // 4. UI 갱신 및 초기화
        // ====================
        srcItemData = System.Array.Find(items, x => x.id == src.id);
        destItemData = System.Array.Find(items, x => x.id == dest.id);
        ReloadSlot(this.slotIndex, srcItemData);
        ReloadSlot(slotIndex, destItemData);
        this.slotIndex = -1;
    }

    private void MoveSlot(Slot src, Slot dest, int amount)
    {
        dest.id = src.id;
        dest.count = amount;
        src.count -= amount;

        if (src.count <= 0)
            src.id = 0;
    }

    private void MergeSlot(Slot src, Slot dest, int maxCount)
    {
        int space = maxCount - dest.count;
        int moveAmount = src.count > space ? space : src.count;

        dest.count += moveAmount;
        src.count -= moveAmount;

        if (src.count <= 0)
            src.id = 0;
    }

    private void SwapSlot(Slot src, Slot dest)
    {
        int tempId = src.id;
        int tempCount = src.count;

        src.id = dest.id;
        src.count = dest.count;

        dest.id = tempId;
        dest.count = tempCount;
    }

    public void ItemDuplicateInput()
    {
        // ====================
        // 인벤토리 자동넣기 버튼 기능
        // ====================
        for (int i = 0; i < INVEN_MAX_COUNT; i++)
        {
            if (slots[i].id == 0)
                continue;

            for (int j = INVEN_MAX_COUNT; j < slots.Length; j++)
            {
                if (slots[i].id == slots[j].id)
                {
                    MergeSlot(slots[i], slots[j], int.MaxValue);
                    ReloadSlot(i, null);
                    ReloadSlot(j, Array.Find(items, x => x.id == slots[j].id));
                    break;
                }
            }
        }
    }
}
