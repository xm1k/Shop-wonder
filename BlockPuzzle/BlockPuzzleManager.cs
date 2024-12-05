using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] private Level _level;
    [SerializeField] private BGCell _bgCellPrefab;
    [SerializeField] private GameObject _Spawner;
    [SerializeField] private Block _blockPrefab;
    [SerializeField] private float _blockSpawnSize;
    [SerializeField] private float _blockHighLightSize;
    [SerializeField] private float _blockPutSize;
    [SerializeField] private Canvas canvas;
    [SerializeField] private Inventory inventory;
    [SerializeField] private GraphController graph;
    [SerializeField] private ProgressBar progressBar;
    [SerializeField] private GameObject potion;
    [SerializeField] private DataBase db;

    private BGCell[,] bgCellGrid;
    private bool hasGameFinished;
    private Block currentBlock;
    private Vector2 currentPos, previousPos;
    public List<Block> gridBlocks;
    

    private void refresh_graph()
    {
        progressBar.refresh(gridBlocks.Count);
        graph.ResolveGraph(gridBlocks);
    }
    
    private void Awake()
    {
        Instance = this;
        hasGameFinished = false;
        gridBlocks = new List<Block>();

        refresh_graph();
        Vector3 pos = new Vector3(0, 0, 0);
        pos.x = -50.0f * _level.Columns;
        pos.y = -50.0f * _level.Rows;
        GetComponent<RectTransform>().anchoredPosition = pos;
        SpawnGrid();

    }

    private void SpawnGrid()
    {
        bgCellGrid = new BGCell[_level.Rows, _level.Columns];
        for (int i = 0; i < _level.Rows; i++)
        {
            for (int j = 0; j < _level.Columns; j++)
            {
                BGCell bgcell = Instantiate(_bgCellPrefab, transform);
                bgcell.transform.position = transform.TransformPoint(new Vector3(j + 0.5f, i + 0.5f, 0f));
                bgcell.Init(_level.Data[i * _level.Columns + j]);
                bgCellGrid[i, j] = bgcell;
            }
        }
    }

    public void return_item(int item_id)
    {
        inventory.AddItem(item_id, 1);
    }

    public void SpawnBlock(List<Vector2Int> block_pos, Color color, int item_id)
    {
        Block block = Instantiate(_blockPrefab, transform);
        Vector3 blockSpawnPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        block.Init(block_pos, transform.InverseTransformPoint(blockSpawnPos), color, item_id);

        currentBlock = block;
        
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);
        mousePos2D = transform.InverseTransformPoint(mousePos);
        currentPos = mousePos2D;
        previousPos = mousePos2D;
            
        offset = currentBlock.GetComponent<RectTransform>().anchoredPosition - mousePos2D;
            
        currentBlock.ElevateSprites();
        currentBlock.Layer(20);
        currentBlock.transform.localScale = Vector3.one * _blockHighLightSize;
        if (gridBlocks.Contains(currentBlock))
        {
            gridBlocks.Remove(currentBlock);
            refresh_graph();
        }
            
        UpdateFilled();
        ResetHighLight();
        UpdateHighLight();
    }

    private Vector2 offset;
    private void Update()
    {
        if (hasGameFinished) return;
        
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);
        mousePos2D = transform.InverseTransformPoint(mousePos);

        if (Input.GetKeyDown(KeyCode.R) && currentBlock != null && currentBlock.rotation == 0.0f)
        {
            // Vector2 new_offset = new Vector2(offset.y, -offset.x);
            // offset = new_offset;
            currentBlock.RotateBlock();
            ResetHighLight();
            UpdateHighLight();
        }
        
        if (Input.GetMouseButtonDown(0))
        {
            int blockLayer = LayerMask.GetMask("block");
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero, Mathf.Infinity, blockLayer);
            if (!hit) return;
            if (hit.collider.gameObject.layer != LayerMask.NameToLayer("block")) return;
            currentBlock = hit.collider.transform.parent.GetComponent<Block>();
            if (currentBlock == null) return;
            currentPos = mousePos2D;
            previousPos = mousePos2D;
            
            offset = currentBlock.GetComponent<RectTransform>().anchoredPosition - mousePos2D;
            
            currentBlock.ElevateSprites();
            currentBlock.Layer(20);
            currentBlock.transform.localScale = Vector3.one * _blockHighLightSize;
            if (gridBlocks.Contains(currentBlock))
            {
                gridBlocks.Remove(currentBlock);
                refresh_graph();
            }
            
            UpdateFilled();
            ResetHighLight();
            UpdateHighLight();
        }
        else if (Input.GetMouseButton(0) && currentBlock != null)
        {
            currentPos = mousePos2D-offset;
            currentBlock.UpdatePos(currentPos - previousPos);
            currentBlock.GetComponent<RectTransform>().anchoredPosition = mousePos2D;
            previousPos = currentPos;
            ResetHighLight();
            UpdateHighLight();
        }
        else if (Input.GetMouseButtonUp(0) && currentBlock != null)
        {
            currentBlock.ElevateSprites(true);
            currentBlock.Layer(10);

            if (IsCorrectMove())
            {
                currentBlock.UpdateCorrectMove();
                currentBlock.transform.localScale = Vector3.one * _blockPutSize;
                gridBlocks.Add(currentBlock);
                refresh_graph();
            }
            else if (mousePos2D.y < 0)
            {
                currentBlock.UpdateStartMove();
                currentBlock.transform.localScale = Vector3.one * _blockSpawnSize;
            }
            else
            {
                currentBlock.UpdateIncorrectMove();
                if (currentBlock.CurrentPos.y > 0)
                {
                    gridBlocks.Add(currentBlock);
                    refresh_graph();
                    currentBlock.transform.localScale = Vector3.one * _blockPutSize;
                }
                else
                {
                    currentBlock.transform.localScale = Vector3.one * _blockSpawnSize;
                }
            }

            currentBlock = null;
            ResetHighLight();
            UpdateFilled();
        }
    }

    private void ResetHighLight()
    {
        for (int i = 0; i < _level.Rows; i++)
        {
            for (int j = 0; j < _level.Columns; j++)
            {
                if (!bgCellGrid[i, j].IsBlocked)
                {
                    bgCellGrid[i, j].ResetHighLight();
                }
            }
        }
    }

    public void makePotion()
    {
        if (gridBlocks.Count > 0)
        {
            foreach (var block in gridBlocks)
            {
                Destroy(block.gameObject);
            }
            GameObject newPotion = Instantiate(potion, transform.parent.transform);
            newPotion.GetComponent<Potion>().item = graph.getPotion();
            newPotion.GetComponent<Potion>().inventory = inventory;

            gridBlocks.Clear();
            refresh_graph();
        }
    }

    public void Reset()
    {
        foreach (Block block in gridBlocks)
        {
            block.UpdateStartMove();
        }
        gridBlocks.Clear();
        refresh_graph();
    }

    private void UpdateFilled()
    {
        for (int i = 0; i < _level.Rows; i++)
        {
            for (int j = 0; j < _level.Columns; j++)
            {
                if (!bgCellGrid[i, j].IsBlocked)
                {
                    bgCellGrid[i, j].IsFilled = false;
                }
            }
        }

        if (gridBlocks.Count >= 5)
        {
            foreach (var bgsell in bgCellGrid)
            {
                bgsell.IsFilled = true;
            }
        }
        
        foreach (var block in gridBlocks)
        {
            foreach (var pos in block.BlockPositions())
            {
                if (IsValidPos(pos))
                {
                    bgCellGrid[pos.x, pos.y].IsFilled = true;
                }
            }
        }
    }

    private void UpdateHighLight()
    {
        bool isCorrect = IsCorrectMove();
        foreach (var pos in currentBlock.BlockPositions())
        {
            if (IsValidPos(pos))
            {
                bgCellGrid[pos.x, pos.y].UpdateHighlight(isCorrect);
            }
        }
    }

    private bool IsCorrectMove()
    {
        foreach (var pos in currentBlock.BlockPositions())
        {
            if (!IsValidPos(pos) || bgCellGrid[pos.x, pos.y].IsFilled)
            {
                return false;
            }
        }
        return true;
    }

    private bool IsValidPos(Vector2Int pos)
    {
        return pos.x >= 0 && pos.y >= 0 && pos.x < _level.Rows && pos.y < _level.Columns;
    }

    // private void CheckWin()
    // {
    //     for (int i = 0; i < _level.Rows; i++)
    //     {
    //         for (int j = 0; j < _level.Columns; j++)
    //         {
    //             if (!bgCellGrid[i, j].IsFilled)
    //             {
    //                 return;
    //             }
    //         }
    //     }
    //
    //     hasGameFinished = true;
    //     StartCoroutine(GameWin());
    // }

    private IEnumerator GameWin()
    {
        yield return new WaitForSeconds(2f);
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }
}