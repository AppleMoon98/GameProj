using System;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;
using static UnityEditor.Progress;

public enum CraftType
{
    Wood,
    CraftingTable
}

public class Crafting : MonoBehaviour
{
    [Header("참조")]
    public GameManager gameManager;
    public ItemManager itemManager;
    public UserInterfaceManager userInterfaceManager;
    public Player pleyer;

    [SerializeField] private Transform content;
    [SerializeField] private CraftSlot slotPrefab;

    [Header("기본")]
    public CraftType type;      // 제작 아이템 띄울 타입 (Item에서 Type가 같은 것을 가져옴)
    public Text countText;     // 생산 개수
    int resultCount = 1;        // 실효값

    [Header("제작 결과")]
    public Item resultItem;     // none
    public Image resultIcon;
    public Text quantityText;
    public Text itemText;

    [Header("재료 표시")]
    public Slot[] slots;        // Item.craftMaterials 가져오면 됨
    public Image[] icons;    // 반복문으로 검색시켜서 가져옴

    [Header("조합식")]
    public List<Item> itemList = new();
    public List<CraftSlot> slotList = new();

    private void OnEnable()
    {
        // ====================
        // CraftType가 같은 제작 아이템 검색
        // item.craftMaterials이 존재하는 아이템은 전부 출력
        // ====================
        slotList.Clear();   // 서브 제작창 초기화

        foreach (Item item in itemManager.items)
        {
            if (item.type != type) continue;
            if (item.craftMaterials == null || item.craftMaterials.Length == 0) continue;

            itemList.Add(item);
        }

        if (itemList.Count == 0) return;

        // ====================
        // 조합식 출력하기
        // ====================
        RefreshItemList();
        ResultItemReload(itemList[0]);
    }

    public void ResultItemReload(Item resultItem)
    {
        // ====================
        // 아이템 조합창에 띄워줌
        // ResultItem = 제작 목표 아이템
        // ====================
        this.resultItem = resultItem;
        resultIcon.sprite = resultItem.icon;
        quantityText.text = resultItem.count.ToString();
        itemText.text = LocalizationSettings.StringDatabase.GetLocalizedString("Item Table", resultItem.productName);

        for (int i = 0; i < slots.Length; i++)
        {
            // 비어있는 재료 슬롯 구별
            if (resultItem.craftMaterials.Length <= i)
            {
                slots[i].id = 0;
                icons[i].gameObject.SetActive(false);
                continue;
            }

            slots[i].id = resultItem.craftMaterials[i].id;
            slots[i].count = resultItem.craftMaterials[i].count;

            Item item = Array.Find(itemManager.items, x => x.id == resultItem.craftMaterials[i].id);

            if(item == null)
            {
                Debug.Log("아이템을 찾을 수 없습니다.");
                return;
            }

            icons[i].sprite = item.icon;

            Text iconText = icons[i].transform.GetChild(0).GetComponent<Text>();
            iconText.text = $"{item.count}/{resultItem.craftMaterials[i].count}";
        }
    }

    private CraftSlot GetSlot()
    {
        // ====================
        // 슬롯을 가져올 때, 탐색용
        // 서브 제작창 전용
        // ====================
        foreach (CraftSlot slot in slotList)
            if (!slot.gameObject.activeSelf)
            {
                slot.gameObject.SetActive(true);
                return slot;
            }

        CraftSlot newSlot = Instantiate(slotPrefab, content);
        slotList.Add(newSlot);
        return newSlot;
    }

    public void RefreshItemList()
    {
        // ====================
        // 서브 크래프팅 슬롯 초기화 후 재부팅
        // ====================
        foreach (CraftSlot slot in slotList)
            slot.gameObject.SetActive(false);

        foreach (Item item in itemList)
        {
            CraftSlot slot = GetSlot();
            slot.item = item;
            slot.iconImage.sprite = item.icon;
        }
    }

    public void OnCraftButton()
    {
        // ====================
        // ResultItem = 목표 아이템 (item)
        // slots = 아이템 재료 배열 (item.craftMaterials)
        // resultCount = 생성 개수 (private int)
        // ====================

        // 제작하기 위한 재료가 충분한지 체크, 없으면 리턴
        foreach (Slot slot in slots)
            if (slot != null || slot.id != 0)
                if (!itemManager.CheckItem(slot.id, slot.count * resultCount))
                    continue;
        //Debug.Log("생산품에 필요한 재료 체크 완료");

        // 아이템을 삭제하면서, 다른 조건에 걸린 경우 로그 출력
        // 개수가 모자른 경우 false를 반환하기 때문에 확인 가능
        foreach (Slot slot in slots)
            itemManager.DropItem(slot.id, slot.count * resultCount);
        //Debug.Log("아이템 제거 완료, 인벤토리에서 사라진 아이템 직접 확인");

        // 해당 위치 애니메이션이 나온 뒤
        // 해당 코루틴에 아이템 생성 메서드를 넣어야 할 거 같은데

        // 아이템 인벤토리에 생성
        itemManager.LootItem(resultItem.id, resultCount);
        //Debug.Log("아이템 생산 완료");
    }

    public void OnNextButton()
    {
        // ====================
        // 제조 개수 증가
        // # 99개를 넘을 수 없음
        // ====================
        if (resultCount >= 99) return;

        resultCount++;
        ReloadResultCount();
    }

    public void OnBackButton()
    {
        // ====================
        // 제조 개수 감소
        // # 1개 미만으로 감소할 수 없음
        // ====================
        if (resultCount <= 1) return;

        resultCount--;
        ReloadResultCount();
    }

    public void ReloadResultCount()
    {
        // ====================
        // 재료 슬롯 개수만 초기화
        // ====================
        for (int i = 0; i < resultItem.craftMaterials.Length; i++)
        {
            Text iconText = icons[i].transform.GetChild(0).GetComponent<Text>();
            Item item = Array.Find(itemManager.items, x => x.id == resultItem.craftMaterials[i].id);
            iconText.text = $"{item.count}/{resultItem.craftMaterials[i].count * resultCount}";
        }

        countText.text = resultCount.ToString();
    }

    public void OnCraftingUI(CraftType type)
    {
        // 애니메이션 동작 + 나무 캘 때처럼 캐릭터가 굳어 있다가 제작을 완성할 것
    }
}
