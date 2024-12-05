using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Teleport : MonoBehaviour
{
    [HideInInspector] public int TeleportTo;
    private SoundsManager soundManager;

    private void Awake()
    {
        soundManager = GetComponent<SoundsManager>();
    }
    public void Load()
    {
        SceneManager.LoadScene(TeleportTo);
    }

    public void PlayTransition()
    {
        soundManager.PlaySound(0);
    }
}
