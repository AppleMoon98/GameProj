using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Slot
{
    public int id = 0;
    public int count = 0;
}


public class UserInterfaceManager : MonoBehaviour
{
    public Inventory inventory;
    public Inventory storage;
    public GameObject tooltip;
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
        if (inventory.GetActive())
            CloseTooltip();

        inventory.SetActive(!inventory.GetActive());
        if(inventory.subInventory.GetComponent<SubInventory>().gameObject.activeSelf)
            inventory.subInventory.GetComponent<SubInventory>().OnExit();
    }

    public void OnStorage()
    {
        if(storage.GetActive())
            CloseTooltip();

        storage.SetActive(!storage.GetActive());
    }
}
