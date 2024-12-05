using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TriggerChest : MonoBehaviour
{
    private bool IsTriggered;
    [SerializeField] private TextMeshPro PressText;
    [SerializeField] private EndScript EndManager;

    private void Start()
    {
        PressText?.gameObject.SetActive(false);
    }
    private void OnEnable()
    {
        Player.PressEButton += PressEAction;
    }
    private void OnDisable()
    {
        Player.PressEButton -= PressEAction;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            IsTriggered = true;
            PressText?.gameObject.SetActive(true);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            IsTriggered = false;
            PressText?.gameObject.SetActive(false);
        }
    }

    private void PressEAction()
    {
        if (IsTriggered)
        {
            StartCoroutine(EndManager.StartEndCutScene());
            IsTriggered = false;
        }
    }
}
