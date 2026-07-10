using UnityEngine;
using UnityEngine.UI;

public class UserInterfaceManager : MonoBehaviour
{
    public bool dragging = false;
    public GameObject tooltip;

    [SerializeField] ItemManager itemManager;
    [SerializeField] GameObject escMenu;

    public void OpenTooltip(string title, string content, Vector3 position)
    {
        tooltip.transform.GetChild(0).GetComponent<Text>().text = title;
        tooltip.transform.GetChild(1).GetComponent<Text>().text = content;
        tooltip.transform.position = position;
        tooltip.SetActive(true);
    }

    public void CloseTooltip()
    {
        tooltip.SetActive(false);
    }

    void OnPause()
    {
        Debug.Log("Pause");
        if (escMenu.activeSelf)
        {
            Time.timeScale = 1f;
            escMenu.SetActive(false);
        }
        else
        {
            Time.timeScale = 0f;
            escMenu.SetActive(true);
        }
    }

    public void OnInventory()
    {
        itemManager.InventorySwitch();
    }

    public void StorageClose()
    {
        if(itemManager.storage.GetActive())
            CloseTooltip();

        itemManager.storage.SetActive(!itemManager.storage.GetActive());
    }

    public void StorageOpen()
    {
        // 창고를 열 때, 인벤이 없으면 켜줌
        if(!itemManager.inventory.GetActive())
            itemManager.inventory.SetActive(true);

        itemManager.storage.SetActive(true);
    }
}
