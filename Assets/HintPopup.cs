using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HintPopup : MonoBehaviour
{
   [SerializeField] private Button closeButton;
   
   private void Awake()
   {
      closeButton.onClick.AddListener(OnCloseButtonClicked);
   }
   
   private void OnCloseButtonClicked()
   {
      gameObject.SetActive(false);
   }
}
