using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum InteractionType
{
    NPC,
    Tree,
    Chest
}

public class GameManager : MonoBehaviour
{
    public Player player;
    public UserInterfaceManager userInterfaceManager;
    public ItemManager itemManager;

    void Awake()
    {
        userInterfaceManager = GetComponent<UserInterfaceManager>();
        itemManager = GetComponent<ItemManager>();
    }

    void Update()
    {
        
    }

    public void AddItem(int id, int count)
    {
        itemManager.AddItem(id, count);
    }
    // 아이템 획득하였을 때 불러오는 코드
}
