using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public class GenerateTileMap : MonoBehaviour
{
    [SerializeField] List<CustomDictionary> TileDictionary = new List<CustomDictionary>();

    [SerializeField] private Tilemap MainTileMap;
    [SerializeField] private Tilemap CollisionMap;
    [SerializeField] private Tile CollisionTile;

    [SerializeField] public Vector2Int SizeField;
    private Vector2Int Offset;

    [SerializeField] public AnimationCurve StoneSpawnPercent; 
    [SerializeField] public AnimationCurve GravelSpawnPercent;
    [SerializeField] public AnimationCurve GrassSpawnPercent;

    [SerializeField] public AnimationCurve EdgeSpawnPercent;

    [SerializeField] bool DoorN = false;
    [SerializeField] bool DoorS = false;
    [SerializeField] bool DoorW = false;
    [SerializeField] bool DoorE = false;
    void Start()
    {
        // PaintTileMap();
    }
    void Update()
    {
        // if(Input.GetKeyDown(KeyCode.Y)) 
        //     PaintTileMap();
    }

    public void PaintTileMap(Vector2 TermalCenter, float Fade, float Raduis, bool doorN = false, bool doorS = false, bool doorW = false, bool doorE = false)
    {
        Offset = new Vector2Int(-SizeField.x / 2, -SizeField.y / 2);
        DoorN = doorN;
        DoorS = doorS;
        DoorW = doorW;
        DoorE = doorE;
        GenerateTileMapFunction();
        for (int i = 0; i < SizeField.x; i++)
        {
            for (int j = 0; j < SizeField.y; j++)
            {
                if (!doorS && j == 0)
                {
                    CollisionMap.SetTile(new Vector3Int(i + Offset.x, j + Offset.y, 0), CollisionTile);
                    EmptyTiles[i, j] = "Stone";
                }
                if (!doorN && j == SizeField.y - 1)
                {
                    CollisionMap.SetTile(new Vector3Int(i + Offset.x, j + Offset.y, 0), CollisionTile);
                    EmptyTiles[i, j] = "Stone";
                }
                if (!doorW && i == 0)
                {
                    CollisionMap.SetTile(new Vector3Int(i + Offset.x, j + Offset.y, 0), CollisionTile);
                    EmptyTiles[i, j] = "Stone";
                }
                if (!doorE && i == SizeField.x - 1)
                {
                    CollisionMap.SetTile(new Vector3Int(i + Offset.x, j + Offset.y, 0), CollisionTile);
                    EmptyTiles[i, j] = "Stone";
                }

                Tile RandomTile = GetRandomTileByName(EmptyTiles[i, j]);
                //RandomTile.color = new Color(1f,1f,1f);
                //if (EmptyTiles[i, j] == "Dirt" || EmptyTiles[i, j] == "Grass")
                //{
                    Vector2 ParentPos = gameObject.transform.parent.position;
                    float Koiff = 1f - (Vector2.Distance(new Vector2(i+ParentPos.x, j+ParentPos.y), TermalCenter) / Raduis) * Fade;
                    RandomTile.color = new Color(Koiff, Koiff, Koiff);
                //}

                MainTileMap.SetTile(new Vector3Int(i + Offset.x, j + Offset.y, 0), RandomTile);
                if (EmptyTiles[i, j] == "Stone" || EmptyTiles[i, j] == "Gravel")
                    CollisionMap.SetTile(new Vector3Int(i + Offset.x, j + Offset.y, 0), CollisionTile);
            }
        }
    }


    private Tile GetRandomTileByName(string Name) 
    {
        for(int i=0;i<TileDictionary.Count;i++)
        {
            if (Name == TileDictionary[i].Name)
            {
                return TileDictionary[i].Values[UnityEngine.Random.Range(0, TileDictionary[i].Values.Count)];
            }
        }
        return null;
    }
    private string[,] EmptyTiles;
    private void GenerateTileMapFunction()
    {
        Vector2 Center = new Vector2(Offset.x, Offset.y);
        float MaxDistance = Vector2.Distance(Center, new Vector2(Offset.x * 2, Offset.y * 2));
        EmptyTiles = new string[SizeField.x, SizeField.y];
        for(int i=0;i<SizeField.x;i++)
        {
            for(int j=0;j<SizeField.y;j++)
            {
                float DistanceCenter = Vector2.Distance(Center, new Vector2(i + Offset.x * 2, j + Offset.y * 2));
                float DistanceX = Vector2.Distance(new Vector2(i + Offset.x * 2, Offset.y), new Vector2(i + Offset.x * 2, j + Offset.y * 2));
                float DistanceY = Vector2.Distance(new Vector2(Offset.x, j + Offset.y * 2), new Vector2(i + Offset.x * 2, j + Offset.y * 2));


                float DistanceInPercent = DistanceCenter / MaxDistance;    
                float DistanceInPercent_X = DistanceX / (SizeField.y/2);
                float DistanceInPercent_Y = DistanceY / (SizeField.x/2);

                EmptyTiles[i, j] = GetNameByDistance(DistanceInPercent, DistanceInPercent_X, DistanceInPercent_Y, GetQuarter(new Vector2(i + Offset.x * 2, j + Offset.y * 2),Center));
            }
        }
    }
    private string GetNameByDistance(float DistanceInPercent, float DistanceInPercent_X,float DistanceInPercent_Y,int Quarter)
    {
        float Result = UnityEngine.Random.Range(0f, 1f);

        switch(Quarter)
        {
            case 1:
                if (!DoorN)
                    DistanceInPercent_Y = 1;
                if (!DoorW)
                    DistanceInPercent_X = 1;
                break;
            case 2:
                if (!DoorN)
                    DistanceInPercent_Y = 1;
                if (!DoorE)
                    DistanceInPercent_X = 1;
                break;
            case 3:
                if (!DoorS)
                    DistanceInPercent_Y = 1;
                if (!DoorE)
                    DistanceInPercent_X = 1;
                break;
            case 4:
                if (!DoorS)
                    DistanceInPercent_Y = 1;
                if (!DoorW)
                    DistanceInPercent_X = 1;
                break;
        }

        if (Result <= StoneSpawnPercent.Evaluate(DistanceInPercent)*EdgeSpawnPercent.Evaluate(DistanceInPercent_X)*EdgeSpawnPercent.Evaluate(DistanceInPercent_Y))
            return "Stone";
        else if (Result <= GravelSpawnPercent.Evaluate(DistanceInPercent) * EdgeSpawnPercent.Evaluate(DistanceInPercent_X) * EdgeSpawnPercent.Evaluate(DistanceInPercent_Y))
            return "Gravel";
        else if (Result <= GrassSpawnPercent.Evaluate(DistanceInPercent) * EdgeSpawnPercent.Evaluate(DistanceInPercent_X) * EdgeSpawnPercent.Evaluate(DistanceInPercent_Y))
            return "Grass";
        else
            return"Dirt";
    }
    private int GetQuarter(Vector2 Dot, Vector2 Center) 
    {
        if (Dot.x <= Center.x && Dot.y >= Center.y)
            return 1;
        else if (Dot.x >= Center.x && Dot.y >= Center.y)
            return 2;
        else if (Dot.x >= Center.x && Dot.y <= Center.y)
            return 3;
        else if (Dot.x <= Center.x && Dot.y <= Center.y)
            return 4;
        else
            return 1;
    }
}
[Serializable]
public class CustomDictionary
{
    public string Name;
    public List<Tile> Values;
}