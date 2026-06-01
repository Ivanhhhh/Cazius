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

        _typingCoroutine = StartCoroutine(TypeRoutine(text));
    }

    private IEnumerator TypeRoutine(string text)
    {
        IsComplete = false;

        dialogText.text = text; // Full rich text immediately
        dialogText.ForceMeshUpdate();

        int totalCharacters = dialogText.textInfo.characterCount;

        dialogText.maxVisibleCharacters = 0;

        for (int i = 0; i <= totalCharacters; i++)
        {
            dialogText.maxVisibleCharacters = i;
            yield return new WaitForSeconds(charDelay);
        }

        IsComplete = true;
        OnComplete?.Invoke();
    }

    public void Skip()
    {
        if (IsComplete) return;

        if (_typingCoroutine != null)
            StopCoroutine(_typingCoroutine);

        dialogText.maxVisibleCharacters = int.MaxValue;

        IsComplete = true;
        OnComplete?.Invoke();
    }
}
