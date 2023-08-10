using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IntroScene3 : MonoBehaviour
{
    [SerializeField] private Button addNameInputFieldButton;
    [SerializeField] private Button nextButton;
    [SerializeField] private List<TMP_InputField> nameInputFields = new();
    [SerializeField] private Transform inputFieldsLayout;
    [SerializeField] private GameObject inputFieldPrefab;
    // Start is called before the first frame update
    void Start()
    {
        addNameInputFieldButton.onClick.AddListener(AddNameInputField);
        LayoutRebuilder.ForceRebuildLayoutImmediate(inputFieldsLayout.GetComponent<RectTransform>());
    }

    private void AddNameInputField()
    {
        GameObject inputFieldGameObject = Instantiate(inputFieldPrefab, inputFieldsLayout);
        nameInputFields.Add(inputFieldGameObject.GetComponent<TMP_InputField>());
        LayoutRebuilder.ForceRebuildLayoutImmediate(inputFieldsLayout.GetComponent<RectTransform>());
        if (nameInputFields.Count == 8)
        {
            addNameInputFieldButton.gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
