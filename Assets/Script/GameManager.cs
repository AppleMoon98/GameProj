using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

public enum InteractionType
{
    NPC,
    Tree,
    Chest
}

public class GameManager : MonoBehaviour
{
    public Player player;
    UserInterfaceManager userInterfaceManager;
    public List<Item> items = new();

    // UI <Inventory> 관련
    public GameObject[] itemSlot;

    void Awake()
    {
        userInterfaceManager = GetComponent<UserInterfaceManager>();
        ResetItemSlot();
        ReloadItemSlot();
    }

    void Update()
    {
        
    }

    public void AddItem(int id, int count)
    {
        userInterfaceManager.inventory.AddItem(id, count);
    }
    // 아이템 획득하였을 때 불러오는 코드

    void ResetItemSlot()
    {
        for (int i = 0; i < itemSlot.Length; i++)
        {
            ActiveItemSlot(i, false);
        }
    }
    // 인벤토리 아이템 슬롯 상태를 초기화하여 전부 false로 전환

    void ReloadItemSlot()
    {
        //foreach (Item item in items)
        //{
        //    if (item.slotIndex != -1)
        //    {
        //        ActiveItemSlot(item.slotIndex, true);
        //        itemSlot[item.slotIndex].GetComponent<Image>().sprite = item.icon;
        //        itemSlot[item.slotIndex].GetComponent<Image>()
        //            .GetComponentInChildren<Text>().text = item.count.ToString();
        //    }
        //}
    }
    // 아이템 획득 혹은 사용 후 인벤토리 리로드

    void ActiveItemSlot(int slotIndex, bool active) =>
        itemSlot[slotIndex].SetActive(active);
    // 아이템 슬롯 액티브 여부
}
