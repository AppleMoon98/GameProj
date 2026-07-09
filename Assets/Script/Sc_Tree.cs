using UnityEngine;

public class Sc_Tree : MonoBehaviour, Interactable
{
    [SerializeField] GameObject highlight;
    
    public void SetHighlight(bool active)
    {
        highlight.SetActive(active);
    }
}
