using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class MapCreator : MonoBehaviour
{
    public int room_size = 22;
    public GameObject TileMapPrefab;
    public int MapSize = 20;
    public int rooms_count = 20;
    private Vector2Int start;
    private int[,] map_matrix;

    [SerializeField] private GameObject LootPrefab;
    [SerializeField] private DataBase db;
    [SerializeField] private Inventory inventory;
    [SerializeField] private Transform player;
    [SerializeField] List<GameObject> PropsPrefabs = new List<GameObject>();
    [SerializeField] private int MaxPropCount=0;

    public float Radius = 100f;
    public float Fade = 0.5f;
    private Vector2 TermalCenter;

    public static int LootCount = 0;

    private UnityAnalytics UnityAnalytics;
    void Start()
    {
        UnityAnalytics= FindObjectOfType<UnityAnalytics>();
        if (UnityAnalytics != null)
            UnityAnalytics.PlayerEnteredForest();
        TermalCenter = Vector2.zero;

        LootCount = 0;
        GenerateRooms();
    }

    private void GenerateRooms()
    {
    map_matrix = new int[MapSize, MapSize];
    start = new Vector2Int(MapSize / 2, MapSize / 2);
    map_matrix[start.x, start.y] = 1;
    if (start.x < MapSize)
        map_matrix[start.x + 1, start.y] = -1;

    Vector2Int pointer = start;
    int rooms_created = 1;

    while (rooms_created < rooms_count)
    {
        
        List<Vector2Int> directions = new List<Vector2Int>
        {
            Vector2Int.up,
            Vector2Int.down,
            Vector2Int.left,
            Vector2Int.right
        };

        Vector2Int selectedDirection = Vector2Int.zero;
        float bestChance = 0;

        foreach (var direction in directions)
        {
            Vector2Int potentialPosition = pointer + direction;

            if (IsPositionValid(potentialPosition))
            {
                float distance = Vector2.Distance(potentialPosition, start);
                float chance = 1f / (1f + distance * 1.5f);

                if (Random.value < chance && chance > bestChance)
                {
                    bestChance = chance;
                    selectedDirection = direction;
                }
            }
        }
        
        if (selectedDirection == Vector2Int.zero)
        {
            List<Vector2Int> validDirections = new List<Vector2Int>();
            foreach (var direction in directions)
            {
                if (IsPositionValid(pointer + direction))
                {
                    validDirections.Add(direction);
                }
            }

            if (validDirections.Count > 0)
            {
                selectedDirection = validDirections[Random.Range(0, validDirections.Count)];
            }
            else
            {
                break;
            }
        }
        
        Vector2Int new_pos = pointer + selectedDirection;
        map_matrix[new_pos.x, new_pos.y] = 1;
        pointer = new_pos;
        rooms_created++;
    }

    
    int[,] doors = GenerateMST(StretchMatrix(map_matrix));
    
    
    for (int i = 0; i < map_matrix.GetLength(0); i++)
    {
        for (int j = 0; j < map_matrix.GetLength(1); j++)
        {
            Boolean N=false, S=false, W=false, E=false;
            if(i!=0 && doors[i*2-1,j*2]==2) N = true;
            if(doors[i*2+1,j*2]==2 || new Vector2(i,j) == start) S = true;
            if(j!=0 && doors[i*2,j*2-1]==2) W = true;
            if(doors[i*2,j*2+1]==2) E = true;
            
            if (map_matrix[i, j] == 1)
            {
                CreateRoom(i, j, N, S, W, E);
            }
        }
    }

}


    private int[,] GenerateMST(int[,] matrix)
{
    List<List<Vehicle>> vehicles = new List<List<Vehicle>>();
    List<Vehicle> vehicles_list = new List<Vehicle>();
    for (int i = 0; i < matrix.GetLength(0); i++)
    {
        for (int j = 0; j < matrix.GetLength(1); j++)
        {
            if (matrix[i, j] == 1)
            {
                Vehicle vehicle = new Vehicle();
                vehicle.position = new Vector2Int(i, j);
                vehicle.un = vehicles.Count;
                List<Vehicle> vehiclist = new List<Vehicle>();
                vehiclist.Add(vehicle);
                vehicles_list.Add(vehicle);
                vehicles.Add(vehiclist);
            }
        }
    }

    foreach (var v in vehicles_list)
    {
        for (int i1 = -2; i1 <= 2; i1+=2)
        {
            for (int i2 = -2; i2 <= 2; i2+=2)
            {
                if (!(i1 == 0 && i2 == 0) && (i1 == 0 || i2 == 0))
                {
                    int newX = v.position.x + i1;
                    int newY = v.position.y + i2;
                    
                    if (newX >= 0 && newX < matrix.GetLength(0) &&
                        newY >= 0 && newY < matrix.GetLength(1))
                    {
                        if (matrix[newX, newY] == 1)
                        {
                            foreach (var vehicle in vehicles_list)
                            {
                                if (vehicle.position.x == newX && vehicle.position.y == newY)
                                {
                                    if (vehicle.un != v.un)
                                    {
                                        int midX = (v.position.x + vehicle.position.x) / 2;
                                        int midY = (v.position.y + vehicle.position.y) / 2;
                                        if (midX >= 0 && midX < matrix.GetLength(0) &&
                                            midY >= 0 && midY < matrix.GetLength(1))
                                        {
                                            
                                            matrix[midX, midY] = 2;
                                        }

                                        foreach (var new_v in vehicles[vehicle.un])
                                        {
                                            new_v.un = v.un;
                                            vehicles[v.un].Add(new_v);
                                        }
                                        vehicles[vehicle.un].Clear();
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    // for (int i = 0; i < matrix.GetLength(0); i++)
    // {
    //     string st = "";
    //     for (int j = 0; j < matrix.GetLength(1); j++)
    //     {
    //         st += matrix[i, j].ToString() + " ";
    //     }
    //     print(st);
    // }
    return matrix;
}


    private class Vehicle
    {
        public Vector2Int position;
        public int un;
    };



    private int[,] StretchMatrix(int[,] inputMap)
    {
        int mapSize = inputMap.GetLength(0);
        int newMapSize = mapSize * 2;  // Увеличиваем размер карты
        int[,] stretchedMap = new int[newMapSize, newMapSize];

        // Переносим единицы в новую карту с шагом 2
        for (int i = 0; i < mapSize; i++)
        {
            for (int j = 0; j < mapSize; j++)
            {
                if (inputMap[i, j] == 1)
                {
                    stretchedMap[i * 2, j * 2] = 1; // Расставляем единицы через одну клетку
                }
            }
        }
        return stretchedMap;
    }



    
    private bool IsAdjacentToRoom(int x, int y)
    {
        if (x >= 0 && x < MapSize && y >= 0 && y < MapSize)
        {
            return map_matrix[x, y] == 1;
        }
        return false;
    }
    
    private bool IsPositionValid(Vector2Int position)
    {
        if (position.x < 0 || position.x >= MapSize || position.y < 0 || position.y >= MapSize || position.x > start.x)
            return false;
        if (map_matrix[position.x, position.y] != 0)
            return false;
        return true;
    }
    private void GenerateRandomLoot(Vector2 pos, GameObject parent)
    {
        if (Random.Range(0, 2) == 0 && LootPrefab!=null)
        {
            GameObject Loot = Instantiate(LootPrefab, parent.transform);
            Loot.transform.position = new Vector2(Random.Range(pos.x - room_size / 3, pos.x + room_size / 3), Random.Range(pos.y - room_size / 3, pos.y + room_size / 3));
            Loot.GetComponent<Loot>().Init(Random.Range(0, db.itemCount), Random.Range(1, 10), db, inventory, player);
            LootCount++;
        }
    }
   
    private void GenerateRandomProps(Vector2 pos, GameObject parent)
    {
        int PropsCount = Random.Range(0, MaxPropCount+1);

        for(int i=0;i< PropsCount; i++)
        {
            GameObject Prop = Instantiate(PropsPrefabs[Random.Range(0, PropsPrefabs.Count)], parent.transform);
            Prop.transform.position = new Vector2(Random.Range(pos.x - room_size / 3, pos.x + room_size / 3), Random.Range(pos.y - room_size / 3, pos.y + room_size / 3));
        }
    }
    private void CreateRoom(int x, int y, Boolean N, Boolean S, Boolean W, Boolean E)
    {
        GameObject room = Instantiate(TileMapPrefab);
        
        room.transform.SetParent(gameObject.transform, false);
        room.transform.position = new Vector3((y - start.y) * room_size, (-x + start.x) * room_size, 0);
        
        
        GenerateRandomLoot(room.transform.position, room);
        if(x!=start.x && y!=start.y)
            GenerateRandomProps(room.transform.position, room);

        GenerateTileMap room_gen = room.GetComponentInChildren<GenerateTileMap>();
        room_gen.SizeField = new Vector2Int(room_size, room_size);
        
        // if (Random.value <= 0.1f && IsAdjacentToRoom(x, y - 1)) W = true;
        // if (Random.value <= 0.1f && IsAdjacentToRoom(x, y + 1)) E = true;
        // if (Random.value <= 0.1f && IsAdjacentToRoom(x - 1, y)) N = true;
        // if (Random.value <= 0.1f && (IsAdjacentToRoom(x + 1, y))) S = true;
        
        if (room_gen != null)
        {
            room_gen.PaintTileMap(TermalCenter, Fade, Radius,N, S, W, E); 
        }
    }

}