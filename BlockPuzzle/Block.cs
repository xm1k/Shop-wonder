using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Block : MonoBehaviour
{
    public Vector2 CurrentPos => new Vector2(currentPos.x, currentPos.y);

    [SerializeField] private SpriteRenderer _blockPrefab;
    [SerializeField] private List<Sprite> _blockSprites;
    [SerializeField] private float _blockSpawnSize;

    private RectTransform rectTransform;
    private Vector2 startPos;
    private Vector2 previousPos;
    private Vector2 currentPos;
    private List<SpriteRenderer> blockSprites;
    private List<Vector2Int> blockPositions;
    private bool wasCorrect = false;

    private const int TOP = 2;
    private const int BOTTOM = 1;
    public int item_id;
    public float rotation = 0.0f;
    public Vector2 bias = new Vector2(0.0f, 0.0f);


    private void Update()
    {
        if (rotation < 0.0f)
        {
            rectTransform.Rotate(0.0f, 0.0f, -10.0f);
            rotation += 10.0f;
        }
    }

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void Init(List<Vector2Int> blocks, Vector2 start, Color color, int it_id)
    {
        startPos = start;
        previousPos = start;
        currentPos = start;
        blockPositions = new List<Vector2Int>(blocks);
        blockSprites = new List<SpriteRenderer>();
        item_id = it_id;

        for (int i = 0; i < blockPositions.Count; i++)
        {
            SpriteRenderer spawnedBlock = Instantiate(_blockPrefab, transform);
            spawnedBlock.sprite = _blockSprites[0];
            spawnedBlock.transform.localPosition = new Vector3(blockPositions[i].x, blockPositions[i].y, 0);
            spawnedBlock.GetComponent<SpriteRenderer>().color = color;
            blockSprites.Add(spawnedBlock);
        }
    
        rectTransform.localScale = Vector3.one * _blockSpawnSize;
        rectTransform.anchoredPosition = startPos;
        ElevateSprites(true);
    }

    


    public void Layer(int x)
    {
        foreach (var blockSprite in blockSprites)
        {
            blockSprite.sortingOrder = x;
        }
    }
    
    public void UpdatePos(Vector2 offset)
    {
        currentPos += offset;
        rectTransform.anchoredPosition = currentPos;
    }

    public void ElevateSprites(bool reverse = false)
    {
        foreach (var blockSprite in blockSprites)
        {
            blockSprite.sortingOrder = reverse ? BOTTOM : TOP;
        }
    }

    public List<Vector2Int> BlockPositions()
    {
        List<Vector2Int> result = new List<Vector2Int>();
        foreach (var pos in blockPositions)
        {
            Vector2Int blockPos = new Vector2Int(pos.y, pos.x);
            result.Add(blockPos + new Vector2Int(
                Mathf.FloorToInt(currentPos.y),
                Mathf.FloorToInt(currentPos.x)
                ));
        }
        return result;
    }

    public void UpdateIncorrectMove()
    {
        currentPos = previousPos;
        rectTransform.anchoredPosition = currentPos;
        if (!wasCorrect)
        {
            currentPos = new Vector2(-100,-100);
            previousPos = new Vector2(-100,-100);
            transform.parent.GetComponent<GameManager>().gridBlocks.Remove(this);
            transform.parent.GetComponent<GameManager>().return_item(item_id);
            Destroy(gameObject);
        }
        else
        {
            currentPos = new Vector2(-100,-100);
            previousPos = new Vector2(-100,-100);
            rectTransform.anchoredPosition = currentPos;
            transform.parent.GetComponent<GameManager>().return_item(item_id);
            Destroy(gameObject);
        }
    }

    public void UpdateStartMove()
    {
        currentPos = startPos;
        previousPos = startPos;
        rectTransform.anchoredPosition = currentPos;
        transform.parent.GetComponent<GameManager>().return_item(item_id);
        Destroy(gameObject);
    }

    public void UpdateCorrectMove()
    {
        currentPos.x = Mathf.FloorToInt(currentPos.x) + 0.5f;
        currentPos.y = Mathf.FloorToInt(currentPos.y) + 0.5f;
        previousPos = currentPos;
        rectTransform.anchoredPosition = currentPos;
        wasCorrect = true;
    }
    
    public void RotateBlock()
    {
        rotation = -90f;
        List<Vector2Int> new_blockPositions = new List<Vector2Int>();
        foreach (var blockPos in blockPositions)
        {
            Vector2Int new_pos = new Vector2Int(blockPos.y, -blockPos.x);
            new_blockPositions.Add(new_pos);
        }
        blockPositions = new_blockPositions;
    }


}
