using Screens;
using Screens.Bases;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameManagerKB : MonoBehaviour
{
    public static GameManagerKB Instance;
    private static readonly int KeyboardIn = Animator.StringToHash("keyboardIn");

    public TMP_InputField textBox;
    //[SerializeField] Text printBox;
    [SerializeField] private Animator keyboardAnimator;
    [SerializeField] private Animator keyboardAnimator2;
    [SerializeField] private Screen8StringCheck stringCheck;

    private void Start()
    {
        Instance = this;
        //printBox.text = "";
        //textBox.text = "";
    }

    public void DeleteLetter()
    {
        if (textBox.text.Length != 0) 
        {
            textBox.text = textBox.text.Remove(textBox.text.Length - 1, 1);
        }
        else
        {
            stringCheck?.DeletePreviousLetter();
        }
    }

    public void AddLetter(string letter)
    {
        //printBox.text += letter;
        
        textBox.text += letter;
        stringCheck?.OnAddedLetter();
    }

    public void SubmitWord()
    {
        keyboardAnimator.SetBool(KeyboardIn, false);
        keyboardAnimator2.SetBool(KeyboardIn, false);
        //printBox.text = textBox.text;
        //textBox.text = "";
        // Debug.Log("Text submitted successfully!");
    }
}
