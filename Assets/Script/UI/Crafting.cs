using System;
using System.Collections.Generic;
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

    [SerializeField] private Transform content;
    [SerializeField] private CraftSlot slotPrefab;

    [Header("기본")]
    public CraftType type;      // 제작 아이템 띄울 타입 (Item에서 Type가 같은 것을 가져옴)
    public Text CountText;     // 생산 개수
    int resultCount = 1;

    [Header("제작 결과")]
    public Item ResultItem;     // none
    public Image ResultIcon;
    public Text QuantityText;
    public Text ItemText;

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
        // ====================
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
        RefreshItemList(itemList);
        ResultItemReload(itemList[0]);
    }

    private void ResultItemReload(Item ResultItem)
    {
        // ====================
        // 아이템 조합창에 띄워줌
        // ====================
        this.ResultItem = ResultItem;
        ResultIcon.sprite = ResultItem.icon;
        QuantityText.text = ResultItem.count.ToString();
        ItemText.text = LocalizationSettings.StringDatabase.GetLocalizedString("Item Table", ResultItem.productName);

        for (int i = 0; i < slots.Length; i++)
        {
            if (ResultItem.craftMaterials.Length <= i)
            {
                icons[i].gameObject.SetActive(false);
                continue;
            }

            slots[i].id = ResultItem.craftMaterials[i].id;
            slots[i].count = ResultItem.count;
            icons[i].sprite = Array.Find(itemManager.items, x => x.id == ResultItem.craftMaterials[i].id).icon;
            Text iconText = icons[i].transform.GetChild(0).GetComponent<Text>();
            iconText.text = $"{ResultItem.count}/{ResultItem.craftMaterials[i].count}";
        }
    }

    private CraftSlot GetSlot()
    {
        // ====================
        // 슬롯을 가져올 때, 탐색용
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

    public void RefreshItemList(List<Item> items)
    {
        // ====================
        // 조합식 슬롯 초기화 후 재부팅
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

    }

    public void OnNextButton()
    {
        // ====================
        // 제조 개수 증가
        // ====================
        if (resultCount >= 99) return;

        resultCount++;
        ReloadResultCount();
    }

    public void OnBackButton()
    {
        // ====================
        // 제조 개수 감소
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
        for (int i = 0; i < ResultItem.craftMaterials.Length; i++)
        {
            Text iconText = icons[i].transform.GetChild(0).GetComponent<Text>();
            iconText.text = $"{ResultItem.count}/{ResultItem.craftMaterials[i].count * resultCount}";
        }

        CountText.text = resultCount.ToString();
    }

    public void OnCraftingUI(CraftType type)
    {
        // 애니메이션 동작 + 나무 캘 때처럼 캐릭터가 굳어 있다가 제작을 완성할 것
    }
}
