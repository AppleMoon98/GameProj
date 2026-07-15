using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Scriptable Objects/Item")]
public class Item : ScriptableObject
{
    public int id;
    public string productName;
    public string description;
    public Sprite icon;
    public int maxCount = 99;
    public bool isUseable = false;
    public CraftType type;
    public Slot[] craftMaterials = new Slot[10];
}

public class Slot : ScriptableObject
{
    public int id = 0;
    public int count = 0;
}
