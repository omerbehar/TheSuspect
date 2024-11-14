using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class KeyboardButtonController : MonoBehaviour
{
    [SerializeField] Image containerFillImage;
    [SerializeField] TextMeshProUGUI containerText;
    [SerializeField] TextMeshProUGUI containerActionText;
    [SerializeField] Image containerIcon;

    public Color pressedColor = Color.gray;
    private Color originalFillColor;

    private void Start()
    {
        originalFillColor = containerFillImage.color;

        // Add event trigger
        EventTrigger trigger = gameObject.AddComponent<EventTrigger>();
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerDown;
        entry.callback.AddListener((data) => { OnPress(); });
        trigger.triggers.Add(entry);

        entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerUp;
        entry.callback.AddListener((data) => { OnRelease(); });
        trigger.triggers.Add(entry);
    }

    public void OnPress()
    {
        containerFillImage.color = pressedColor;
    }

    public void OnRelease()
    {
        containerFillImage.color = originalFillColor;
    }

    public void SetContainerFillColor(Color color) => containerFillImage.color = color;
    public void SetContainerTextColor(Color color) => containerText.color = color;
    public void SetContainerActionTextColor(Color color)
    {
        containerActionText.color = color;
        containerIcon.color = color;
    }

    public void AddLetter()
    {
        if (GameManagerKB.Instance != null)
        {
            GameManagerKB.Instance.AddLetter(containerText.text);
        }
        else
        {
            Debug.Log(containerText.text + " is pressed");
        }
    }

    public void DeleteLetter()
    {
        if (GameManagerKB.Instance != null)
        {
            GameManagerKB.Instance.DeleteLetter();
        }
        else
        {
            Debug.Log("Last char deleted");
        }
    }

    public void SubmitWord()
    {
        if (GameManagerKB.Instance != null)
        {
            GameManagerKB.Instance.SubmitWord();
        }
        else
        {
            Debug.Log("Submitted successfully!");
        }
    }
}