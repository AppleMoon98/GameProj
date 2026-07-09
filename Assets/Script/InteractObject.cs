using UnityEngine;

public class InteractObject : MonoBehaviour, Interactable
{
    [SerializeField] GameObject highlight;
    public InteractionType interactionType;

    private void Awake()
    {
        TypeCheck();
    }
    
    public void SetHighlight(bool active)
    {
        highlight.SetActive(active);
    }

    private void TypeCheck()
    {
        string type = gameObject.tag;
        switch (type)
        {
            case "NPC":
                interactionType = InteractionType.NPC;
                break;
            case "Tree":
                interactionType = InteractionType.Tree;
                break;
            case "Chest":
                interactionType = InteractionType.Chest;
                break;
            default:
                Debug.LogWarning("Unknown interaction type: " + type);
                break;
        }
    }
}
