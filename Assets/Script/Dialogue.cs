using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class Dialogue : MonoBehaviour
{
    [SerializeReference] TextMeshProUGUI dialogueBox;
    public string[] lines;
    public float textSpeed = 0.5f;

    private int index;

    public static event Action dialogueDone;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (dialogueBox.text == lines[index])
            {
                NextLine();
            }
            else
            {
                StopAllCoroutines();
                dialogueBox.text = lines[index];
            }
        }
    }

    public void StartDialogue()
    {
        index = 0;
        StartCoroutine(TypeLine());
    }

    private IEnumerator TypeLine()
    {
        foreach (char c in lines[index].ToCharArray())
        {
            dialogueBox.text += c;
            yield return new WaitForSeconds(textSpeed);
        }
    }

    private void NextLine()
    {
        if (index < lines.Length - 1)
        {
            index++;
            dialogueBox.text = string.Empty;
            StartCoroutine(TypeLine());
        }
        else
        {
            dialogueDone?.Invoke();
            gameObject.SetActive(false);
        }
    }

    public void SetLines(string[] newLines)
    {
        lines = newLines;
    }
}
