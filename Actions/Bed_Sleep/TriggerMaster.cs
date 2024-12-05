using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TriggerMaster : MonoBehaviour
{
    private bool IsTriggered;
    [SerializeField] private TextMeshPro PressText;
    [SerializeField] private GameObject UICanvas;

    private void Start()
    {
        PressText?.gameObject.SetActive(false);
        UICanvas?.SetActive(false);
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
            if (UICanvas != null && UICanvas.activeInHierarchy==true)
            {
                UICanvas.SetActive(false);
                UICanvas.transform.parent.GetComponent<SoundsManager>()?.PlaySound(0);
            }
        }
    }

    private void PressEAction()
    {
        if (IsTriggered && UICanvas.activeInHierarchy==false && UICanvas!=null)
        {
            UICanvas?.SetActive(true);
            UICanvas.transform.parent.GetComponent<SoundsManager>()?.PlaySound(0);
        }
    }
}
