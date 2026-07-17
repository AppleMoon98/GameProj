using NUnit.Framework.Internal;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemManager : MonoBehaviour
{
    private const int INVEN_MAX_COUNT = 20;

    public UserInterfaceManager userInterfaceManager;

    public List<Slot> slots = new();
    public List<Inven_Slot> invenSlots = new();
    public List<Item> items = new();    // 아이템은 나중에 txt나 xls로 빼거나 그냥 cs 새로 만들거나 할거임

    public Inventory inventory;
    public Inventory storage;
    public int slotIndex = -1;

    // 정리되면 이걸로 처리할 것
    //public Dictionary<int, Item> itemDic = new();
    //private void Awake()
    //{
    //    foreach(var item in items)
    //    {
    //        itemDic[item.id] = item;
    //    }
    //}

    // 기존: Item item = items.Find(x => x.id == src.id);
    // 변경:
    //if (itemDictionary.TryGetValue(src.id, out Item item))
    //{
    // 아이템을 찾았을 때의 처리
    //}

    private void Awake()
    {
        for (int i = 0; i < invenSlots.Count; i++)
            invenSlots[i].VariableSetting();
        SlotIndexUpdate();
    }

    private void SlotIndexUpdate()
    {
        // ====================
        // 1. Inven_Slot의 SlotIndex, SetActive 설정
        // ====================
        for (int i = 0; i < slots.Count; i++)
        {
            invenSlots[i].slotIndex = i;
            ReloadSlot(i, items.Find(x => x.id == slots[i].id));
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
            inventory.subInventory.GetComponent<SubInventory>().OnExit();
    }

    public void ReloadSlot(int slotIndex, Item item)
    {
        invenSlots[slotIndex].UpdateUI(item, slots[slotIndex].count);
    }

    public void ItemDuplicateInput()
    {
        for(int i = 0; i < INVEN_MAX_COUNT; i++)
        {
            if (slots[i].id == 0)
                continue;

            for(int j = INVEN_MAX_COUNT; j < slots.Count; j++)
            {
                if (slots[i].id == slots[j].id)
                {
                    MergeSlot(slots[i], slots[j], int.MaxValue);
                    ReloadSlot(i, null);
                    ReloadSlot(j, items.Find(x => x.id == slots[j].id));
                    break;
                }
            }
        }
    }

    public bool DropItem(int id, int count)
    {
        // ====================
        // 0. 기본 설정
        // ====================

        // 말도 안되는 것들 리턴
        if (count <= 0 || id <= 0) return false;

        // 람다식으로 item값 서칭, 없으면 리턴
        Item item = items.Find(x => x.id == id);
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
        {
            count = temp;
        }

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

        Item srcItemData = items.Find(x => x.id == src.id);
        Item destItemData = items.Find(x => x.id == dest.id);

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
        srcItemData = items.Find(x => x.id == src.id);
        destItemData = items.Find(x => x.id == dest.id);
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

    
}
