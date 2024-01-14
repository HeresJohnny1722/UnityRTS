using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// Manages the input field where you input your gmail
/// </summary>
public class EmailManager : MonoBehaviour
{
    public InputField inputField;
    public TextMeshProUGUI placeholderText;

    /// <summary>
    /// Sets the input field text to the saved gmail, so you can tell what email was last saved
    /// </summary>
    public void Start()
    {
        inputField.text = SQLdatabase.Instance.LoadEmail();
    }

    /// <summary>
    /// Saves the email to the SQL lite database, also sets the receiving email on the Error Handler class to the inputted email
    /// </summary>
    public void SaveEmail()
    {
        SQLdatabase.Instance.SaveEmail(inputField.text);
        ErrorHandler.instance.receivingEmail = inputField.text;


    }
}
