using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FPS : MonoBehaviour
{
    float fps;
    float DeltaTime = 0.0f;
    public TextMeshProUGUI FPS_Text;

    void Update()
    {
        DeltaTime += (Time.unscaledDeltaTime - DeltaTime) * 0.1f;
        fps = 1.0f / DeltaTime;
        FPS_Text.text = "FPS: " + (int)fps;
    }
}
