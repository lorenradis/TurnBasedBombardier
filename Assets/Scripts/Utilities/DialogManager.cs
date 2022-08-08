using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using TMPro;

public class DialogManager : MonoBehaviour
{
    public GameObject questionPanel;

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

    public GameObject dialogPanel;

    [SerializeField]
    private TextMeshProUGUI dialogText;

    private bool isActive = false;
    private bool anyKey = false;

    public static DialogManager instance;

    private List<string> messages = new List<string>();

    private float waitTime = .025f;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.Log("There were two dialog managers");
            Destroy(gameObject);
        }
        questionPanel.SetActive(false);
        dialogPanel.SetActive(false);
    }

    public void ShowQuestion(string questionMessage, Action yesAction, Action noAction)
    {
        GameManager.instance.EnterMenuState();
        EventSystem.current.SetSelectedGameObject(yesButton.gameObject);
        AddCallbacks(yesAction, noAction);
        yesText.text = "Yes";
        noText.text = "No";
        questionPanel.SetActive(true);
        isActive = true;
        StartCoroutine(DisplayQuestion(questionMessage));
    }

    public void ShowQuestion(string questionMessage, string yesMessage, string noMessage, Action yesAction, Action noAction)
    {

        GameManager.instance.EnterMenuState();
        EventSystem.current.SetSelectedGameObject(yesButton.gameObject);
        AddCallbacks(yesAction, noAction);
        yesText.text = yesMessage;
        noText.text = noMessage;
        questionPanel.SetActive(true);
        isActive = true;
        StartCoroutine(DisplayQuestion(questionMessage));
    }

    private void AddCallbacks(Action yesAction, Action noAction)
    {
        yesButton.onClick.RemoveAllListeners();
        noButton.onClick.RemoveAllListeners();

        yesButton.onClick.AddListener(delegate
        {
            CloseQuestionWindow();
            yesAction();
        });
        noButton.onClick.AddListener(delegate
        {
            CloseQuestionWindow();
            noAction();
        });
    }

    public void ShowDialog(string dialogMessage)
    {
        messages.Add(dialogMessage);
        if(!isActive)
        {
            GameManager.instance.EnterMenuState();
            dialogPanel.SetActive(true);
            isActive = true;
            StartCoroutine(DisplayDialog());
        }
    }

    private IEnumerator DisplayQuestion(string questionMessage)
    {
        float speedUp = 1f;

        questionText.text = questionMessage;

        for (int i = 0; i < questionMessage.Length; i++)
        {
            questionText.maxVisibleCharacters = i+1;
            speedUp = Input.anyKey ? .5f : 1f;
            yield return new WaitForSeconds(waitTime * speedUp);
        }

        yesButton.gameObject.SetActive(true);
        noButton.gameObject.SetActive(true);
    }

    private IEnumerator DisplayDialog()
    {
        string message = messages[0];

        messages.RemoveAt(0);

        float speedUp = 1f;

        dialogText.text = message;

        for (int i = 0; i < message.Length; i++)
        {
            dialogText.maxVisibleCharacters = i+1;

            speedUp = Input.anyKey ? .5f : 1f;
            yield return new WaitForSeconds(waitTime * speedUp);
        }

        bool waitingOnInput = true;

        while(waitingOnInput)
        {
            waitingOnInput = !Input.anyKeyDown;
            yield return null;
        }

        if(messages.Count > 0)
        {
            StartCoroutine(DisplayDialog());
        }
        else
        {
            CloseDialogWindow();
        }
    }

    private void CloseDialogWindow()
    {
        isActive = false;
        dialogPanel.SetActive(false);
        GameManager.instance.ExitMenuState();
    }

    private void CloseQuestionWindow()
    {
        isActive = false;
        questionPanel.SetActive(false);
        yesButton.gameObject.SetActive(false);
        noButton.gameObject.SetActive(false);
        GameManager.instance.ExitMenuState();
    }


}
