using System;
using UnityEngine;
using UnityEngine.UI;

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

    [Header("기본")]
    public CraftType type;      // 제작 아이템 띄울 타입 (Item에서 Type가 같은 것을 가져옴)
    public Item[] items;

    public Text QuantityText;   // 소지 개수
    public Text ItemText;       // 아이템 이름
    public Text CountText;     // 생산 개수

    [Header("제작 슬롯")]
    public Slot[] slots;        // Item.craftMaterials 가져오면 됨
    public Image[] icons;    // 반복문으로 검색시켜서 가져옴

    private void Awake()
    {
        items = Array.FindAll(gameManager.itemManager.items, x => x.type == type);
    }

    public void OnCraftButton()
    {

    }

    public void OnNextButton()
    {

    }

    public void OnBackButton()
    {

    }
}
