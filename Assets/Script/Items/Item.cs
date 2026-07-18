using UnityEngine;
using UnityEngine.Localization;

[CreateAssetMenu(fileName = "Item", menuName = "Script/Item")]
public class Item : ScriptableObject
{
    [Header("기본 정보")]
    public int id;
    public string productName;
    public string description;
    public Sprite icon;
    public int count;
    public int maxCount = 99;
    public bool isUseable = false;

    [Header("생산 정보")]
    public CraftType type;
    public Slot[] craftMaterials;
}

[System.Serializable]
public class Slot
{
    public int id = 0;
    public int count = 0;
}
