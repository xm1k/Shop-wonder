using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class HideColliderTileMapsOnPlay : MonoBehaviour
{
    private TilemapRenderer _tilemapRenderer;
    void Start()
    {
        _tilemapRenderer = GetComponent<TilemapRenderer>();
        _tilemapRenderer.enabled = false;
    }
}
