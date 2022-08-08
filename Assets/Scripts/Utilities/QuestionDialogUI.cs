using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class QuestionDialogUI : MonoBehaviour
{
    public static QuestionDialogUI instance;
    [SerializeField]
    private Button yesButton;
    [SerializeField]
    private Button noButton;
    [SerializeField]
    private TextMeshProUGUI questionText;
    [SerializeField]
    private TextMeshProUGUI yesText;
    [SerializeField]
    private TextMeshProUGUI noText;

    private Vector2 direction = Vector2.zero;

    private bool isActive = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            Debug.Log("There was an extra questiondialogui");
        }
        gameObject.SetActive(false);

    }

    public void ShowQuestion(string questionMessage, Action yesAction, Action noAction)
    {
        GameManager.instance.EnterMenuState();
        EventSystem.current.SetSelectedGameObject(yesButton.gameObject);
        AddCallbacks(yesAction, noAction);
        yesText.text = "Yes";
        noText.text = "No";
        questionText.text = questionMessage;
        gameObject.SetActive(true);
        isActive = true;
    }

    public void ShowQuestion(string questionMessage, string yesMessage, string noMessage, Action yesAction, Action noAction)
    {
        GameManager.instance.EnterMenuState();
        EventSystem.current.SetSelectedGameObject(yesButton.gameObject);
        AddCallbacks(yesAction, noAction);
        questionText.text = questionMessage;
        yesText.text = yesMessage;
        noText.text = noMessage;
        gameObject.SetActive(true);
        isActive = true;

    }

    private void AddCallbacks(Action yesAction, Action noAction)
    {
        yesButton.onClick.RemoveAllListeners();
        noButton.onClick.RemoveAllListeners();

        yesButton.onClick.AddListener(delegate
        {
            isActive = false;
            gameObject.SetActive(false);
            GameManager.instance.ExitMenuState();
            yesAction();
        });
        noButton.onClick.AddListener(delegate
        {
            isActive = false;
            gameObject.SetActive(false);
            GameManager.instance.ExitMenuState();
            noAction();
        });
    }

    private void OnConfirm()
    {

    }

    private void OnCancel()
    {

    }
}

/* Sample Yes/No Dialog
 * 
 *         QuestionDialogUI.instance.ShowQuestion("Exit the cave?", () =>
        {
            GameManager.instance.LoadNewScene("TownScene");
        }, () => {
            Debug.Log("That's a no on the exit!");
        });
 * 
 */