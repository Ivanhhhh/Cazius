using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class DialogTypewriter : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float charDelay = 0.03f;

    [Header("References")]
    [SerializeField] private TMP_Text dialogText;

    public bool IsComplete { get; private set; }
    public event Action OnComplete;

    private Coroutine _typingCoroutine;

    public void Play(string text)
    {
        if (_typingCoroutine != null)
            StopCoroutine(_typingCoroutine);

        IsComplete = false;
        dialogText.text = string.Empty;
        _typingCoroutine = StartCoroutine(TypeRoutine(text));
    }

    public void Skip()
    {
        if (IsComplete) return;

        if (_typingCoroutine != null)
            StopCoroutine(_typingCoroutine);

        // Show full text immediately
        if (dialogText != null)
            dialogText.text = dialogText.text; // already partially set; finish via the event path

        IsComplete = true;
        OnComplete?.Invoke();
    }

    private IEnumerator TypeRoutine(string text)
    {
        dialogText.text = string.Empty;

        foreach (char c in text)
        {
            dialogText.text += c;
            yield return new WaitForSeconds(charDelay);
        }

        IsComplete = true;
        OnComplete?.Invoke();
    }
}
