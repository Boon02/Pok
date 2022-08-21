using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogManager : MonoBehaviour
{
    [SerializeField] private GameObject dialogBox;
    [SerializeField] private Text dialogText;
    [SerializeField] private int lettersPerSecond;

    public event Action OnShowDialog;
    public event Action OnCloseDialog;
    public event Action OnDialogFinish; 

    private int currentLine = 0;
    private Dialog dialog;
    private bool isTyping = false;

    public bool IsShowing { get; private set; }

    public static DialogManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        dialogBox.SetActive(false);
    }

    public IEnumerator ShowDialogText(string dialog, bool waitForInput = true, bool autoClose = true)
    {
        IsShowing = true;
        dialogBox.SetActive(true);
        
        yield return TypeDialog(dialog);

        if (waitForInput)
        {
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));
        }

        if (autoClose)
        {
            CloseDialog();
        }
        
    }

    public void CloseDialog()
    {
        dialogBox.SetActive(false);
        IsShowing = false;
    }

    public IEnumerator ShowDialog(Dialog dialog, Action OnFinish = null)
    {
        yield return new WaitForEndOfFrame();
        
        OnShowDialog?.Invoke();

        IsShowing = true;
        this.dialog = dialog;
        OnDialogFinish = OnFinish;
        dialogBox.SetActive(true);
        StartCoroutine(TypeDialog(dialog.Lines[0]));
    }

    public void HanldeUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Z) && !isTyping) 
        {
            ++currentLine;    
            if (currentLine < dialog.Lines.Count)
            {
                StartCoroutine(TypeDialog(dialog.Lines[currentLine]));
            }
            else
            {
                currentLine = 0;
                IsShowing = false;
                OnCloseDialog?.Invoke();
                OnDialogFinish?.Invoke();
                dialogBox.SetActive(false);
            }
        }
    }
    
    public IEnumerator TypeDialog(string dialog)
    {
        dialogBox.SetActive(true);
        isTyping = true;
        dialogText.text = "";
        foreach (var letter in dialog.ToCharArray())
        {
            dialogText.text += letter;
            yield return new WaitForSeconds(1f / lettersPerSecond);
        }
        
        yield return new WaitForSeconds(1f);
        isTyping = false;
    }
    
}
