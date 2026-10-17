using UnityEngine;
using UnityEngine.UI;
public class OpportunityInteractable : MonoBehaviour
{
    public Transform player;
    public RogueStateWorldState worldState;
    public Text prompt;
    public float interactionDistance = 3f;
    void Update()
    {
        if (!player || !worldState) return;
        bool near = Vector3.Distance(transform.position, player.position) <= interactionDistance;
        if (prompt) prompt.text = near ? "Press E to investigate" : "";
        if (near && Input.GetKeyDown(KeyCode.E)) worldState.Open();
    }
}