using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class ButtonTooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Button button;

    [SerializeField] private GameObject tooltip;
    [SerializeField] private TextMeshProUGUI costText;

    private void Start()
    {
        // Get the Button component attached to the GameObject
        button = GetComponent<Button>();

        // Check if a Button component is attached
        if (button != null)
        {
            // Add an EventTrigger component if not already attached
            EventTrigger eventTrigger = button.gameObject.GetComponent<EventTrigger>();
            if (eventTrigger == null)
            {
                eventTrigger = button.gameObject.AddComponent<EventTrigger>();
            }

            // Create a new entry for the OnPointerEnter event
            EventTrigger.Entry entryEnter = new EventTrigger.Entry();
            entryEnter.eventID = EventTriggerType.PointerEnter;
            entryEnter.callback.AddListener((data) => { OnPointerEnterDelegate((PointerEventData)data); });

            // Create a new entry for the OnPointerExit event
            EventTrigger.Entry entryExit = new EventTrigger.Entry();
            entryExit.eventID = EventTriggerType.PointerExit;
            entryExit.callback.AddListener((data) => { OnPointerExitDelegate((PointerEventData)data); });

            // Add the entries to the EventTrigger events
            eventTrigger.triggers.Add(entryEnter);
            eventTrigger.triggers.Add(entryExit);
        }
        else
        {
            Debug.LogError("Button component not found on GameObject: " + gameObject.name);
        }
    }

    // Implement the OnPointerEnter method
    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("Button Highlighted: " + gameObject.name);
        //costText.text = "Cost in gold of " + gameObject.name;
        tooltip.SetActive(true);
    }

    // Implement the OnPointerExit method
    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("Button Unhighlighted: " + gameObject.name);
        tooltip.SetActive(false);
    }

    // The OnPointerEnterDelegate method to handle the enter event
    private void OnPointerEnterDelegate(PointerEventData data)
    {
        // This is an empty method, as it will be replaced by the listener callback
    }

    // The OnPointerExitDelegate method to handle the exit event
    private void OnPointerExitDelegate(PointerEventData data)
    {
        // This is an empty method, as it will be replaced by the listener callback
    }
}
