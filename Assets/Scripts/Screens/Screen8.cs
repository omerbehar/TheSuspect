using System;
using Screens.Bases;
using TMPro;
using UnityEngine;

namespace Screens
{
    public class Screen8 : ScreenBase
    {
        private TMP_InputField[] inputFields = new TMP_InputField[3];
        [SerializeField] private int keycode;
        private int[] keycodes = new int[3];
        protected override void Start()
        {
            Init();
        }

        private void Init()
        {
            base.Start();
            inputFields = GetComponentsInChildren<TMP_InputField>();
            keycodes[0] = keycode / 100;
            keycodes[1] = keycode / 10 % 10;
            keycodes[2] = keycode % 10;
            
            for (int i = 0; i < inputFields.Length; i++)
            {
                inputFields[i].characterLimit = 1;
                int i1 = i;
                inputFields[i].onValueChanged.AddListener(delegate { MoveToNextField(i1); });
            }
        }

        private void MoveToNextField(int i)
        {
            if (i < inputFields.Length - 1)
            {
                inputFields[i + 1].Select();
            }
            
        }

        private void Update()
        {
            GetKeycode();
        }

        private void GetKeycode()
        {
            //if return is pressed in an empty field move to previous field
            for (int i = inputFields.Length - 1; i >= 0; i--)
            {
                if (inputFields[i].isFocused && inputFields[i].text == "" && Input.GetKeyDown(KeyCode.Backspace))
                {
                    if (i > 0)
                    {
                        inputFields[i - 1].Select();
                        inputFields[i - 1].text = "";
                    }
                    break;
                }
            }
        }
    }
}