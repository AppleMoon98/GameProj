using UnityEngine;

public class Inventory : MonoBehaviour
{
    public ItemManager itemManager;
    public UserInterfaceManager userInterfaceManager;
    public SubInventory subInventory;

    private void OnEnable()
    {
        Debug.Log(gameObject.name.Contains("Storage"));
    }

    private void OnDisable()
    {
        Debug.Log(gameObject.name + " : OnDisable");
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
}
