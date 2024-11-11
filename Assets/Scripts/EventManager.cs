using System.Collections;
using System.Collections.Generic;
using TMPro;
using TMPro.EditorUtilities;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EventManager : MonoBehaviour // Klasa utworzona, ¿eby prze³¹czaæ scenê z startowej, na g³ówn¹ i do zarz¹dzania przyciskami
{                                         // Scena startowa prosi o pozwolenie i potem mapa ju¿ ³adnie dzia³a
    [Header("Login Panel Switching")]
    [SerializeField]
    GameObject LoginPanel;
    [SerializeField]
    Button SwitchLoginButton;

    [Header("Register Panel Switching")]
    [SerializeField]
    GameObject RegisterPanel;
    [SerializeField]
    Button SwitchRegisterButton;

    [Header("Login Form")]
    [SerializeField]
    TMP_InputField LoginInput;
    [SerializeField]
    TMP_InputField PasswordInput;
    [SerializeField]
    Button LoginButton;

    [Header("Login By Google")]
    [SerializeField]
    Button GoogleLoginButton;

    [Header("Register Form")]
    [SerializeField]
    TMP_InputField RLoginInput;
    [SerializeField]
    TMP_InputField RPasswordInput;
    [SerializeField]
    TMP_InputField RRepeatPasswordInput;
    [SerializeField]
    Button RegisterButton;

    [Header("Register By Google")]
    [SerializeField]
    Button GoogleRegisterButton;

    [Header("Error Message")]
    [SerializeField]
    TMP_Text ErrorMessage;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OpenMainGameScene() // Przelaczanie na glowna gre - mape
    {
        SceneManager.LoadScene("MainGame");
    }

    public void Login()
    {
        if (LoginInput.text == "" || PasswordInput.text == "")
        {
            ErrorMessage.text = "Empty fields!";
        }
        else
        {
            ErrorMessage.text = "";
        }
    }

    public void Register()
    {
        if (RLoginInput.text == "" || RPasswordInput.text == "")
        {
            ErrorMessage.text = "Empty fields!";
        }
        else
        {
            ErrorMessage.text = "";
        }
    }

    public void SwitchToLogin()
    {
        LoginPanel.SetActive(true);
        RegisterPanel.SetActive(false);

        SwitchLoginButton.image.color = new Color(72f / 255f, 72f / 255f, 72f / 255f);
        SwitchRegisterButton.image.color = new Color(38f / 255f, 38f / 255f, 38f / 255f);

        ErrorMessage.text = "";
    }

    public void SwitchToRegister()
    {
        LoginPanel.SetActive(false);
        RegisterPanel.SetActive(true);

        SwitchLoginButton.image.color = new Color(38f / 255f, 38f / 255f, 38f / 255f);
        SwitchRegisterButton.image.color = new Color(72f / 255f, 72f / 255f, 72f / 255f);

        ErrorMessage.text = "";
    }
}
