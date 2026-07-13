using UnityEngine;

public class InteractObject : MonoBehaviour, Interactable
{
    Animator animator;
    [SerializeField] GameObject highlight;
    public InteractionType interactionType;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void SetHighlight(bool active)
    {
        highlight.SetActive(active);
    }

    public void SetTrigger(string trigger)
    {
        animator.SetTrigger(trigger);
    }
}
