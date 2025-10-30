﻿using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Collections;
using System.Threading;

public class Stage : MonoBehaviour
{
    public GameObject playerPrefab;
    public GameObject tilePrefab;
    private GameObject[] tileObjs;
    private GameObject player;

    public int mapWidth = 20;
    public int mapHeight = 20;

    [Range(0f, 0.9f)]
    public float erodePercent = 0.5f;
    public int erodeIteration = 2;
    [Range(0f, 0.9f)]
    public float lakePercent = 0.1f;

    [Range(0f, 0.9f)]
    public float treePercent = 0.1f;
    [Range(0f, 0.9f)]
    public float hillPercent = 0.1f;
    [Range(0f, 0.9f)]
    public float moutainPercent = 0.1f;
    [Range(0f, 0.9f)]
    public float townPercent = 0.1f;
    [Range(0f, 0.9f)]
    public float monsterPercent = 0.1f;

    public Vector2 tileSize = new Vector2(16, 16);

    //public Texture2D islandTexture;
    public Sprite[] islandSprites;
    public Sprite[] fowSprites;

    private Map map;

    public Map Map
    {
        get { return map; }
    }

    private Vector3 firstTilePos;

    GraphSearch search = new GraphSearch();

    private float moveSpeed = 20f;

    private bool isMoving = false;
    private CancellationTokenSource movementCts;

    public int ScreenPosToTileId(Vector3 screenPos)
    {
        screenPos.z = Mathf.Abs(transform.position.z - cam.transform.position.z);
        var worldPos = Camera.main.ScreenToWorldPoint(screenPos);
        return WorldPosToTileId(worldPos);
    }

    public int WorldPosToTileId(Vector3 worldPos)
    {
        var pivot = firstTilePos;
        pivot.x -= tileSize.x * 0.5f;
        pivot.y += tileSize.y * 0.5f;

        var diff = worldPos - pivot;
        int x = Mathf.FloorToInt(diff.x / tileSize.x);
        int y = -Mathf.CeilToInt(diff.y / tileSize.y);

        x = Mathf.Clamp(x, 0, mapWidth - 1);
        y = Mathf.Clamp(y, 0, mapHeight - 1);

        return y * mapWidth + x;
    }

    public Vector3 GetTilePos(int y, int x)
    {
        var pos = firstTilePos;
        pos.x += tileSize.x * x;
        pos.y -= tileSize.y * y;
        return pos;
    }

    public Vector3 GetTilePos(int tileId)
    {
        return GetTilePos(tileId / mapWidth, tileId % mapWidth);
    }

    private void ResetStage()
    {
        bool succeed = false;
        while (!succeed)
        {
            map = new Map();
            map.Init(mapHeight, mapWidth);
            succeed = map.CreateIsland(erodePercent, erodeIteration, lakePercent,
                treePercent, hillPercent, moutainPercent, townPercent, monsterPercent);

            Debug.Log(succeed);
        }
        CreateGrid();
        CreatePlayer();
    }

    private void CreatePlayer()
    {
        if (player != null)
        {
            Destroy(player);
        }
        player = Instantiate(playerPrefab, GetTilePos(map.startTile.id), Quaternion.identity);
    }

    private void CreateGrid()
    {
        if (tileObjs != null)
        {
            foreach (var tile in tileObjs)
            {
                Destroy(tile.gameObject);
            }
        }
        tileObjs = new GameObject[mapHeight * mapWidth];

        firstTilePos = Vector3.zero;
        firstTilePos.x -= mapWidth * tileSize.x * 0.5f;
        firstTilePos.y += mapHeight * tileSize.y * 0.5f;
        var pos = firstTilePos;
        for (int i = 0; i < mapHeight; ++i)
        {
            for (int j = 0; j < mapWidth; ++j)
            {
                var tileId = i * mapWidth + j;
                var tile = map.tiles[tileId];

                var newGo = Instantiate(tilePrefab, transform);
                newGo.transform.localPosition = pos;
                pos.x += tileSize.x;
                newGo.name = $"Tile ({i} , {j})";
                tileObjs[tileId] = newGo;
                DecorateTile(tileId);
            }
            pos.x = firstTilePos.x;
            pos.y -= tileSize.y;
        }
    }

    public void DecorateTile(int tileId)
    {
        var tile = map.tiles[tileId];
        var tileGo = tileObjs[tileId];
        var ren = tileGo.GetComponent<SpriteRenderer>();
        if (tile.autoTileId != (int)TileTypes.Empty)
        {
            ren.sprite = islandSprites[tile.autoTileId];
        }
        else
        {
            ren.sprite = null;
        }

        // if (tile.isVisited)
        // {
        //     if (tile.autoTileId != (int)TileTypes.Empty)
        //     {
        //         ren.sprite = islandSprites[tile.autoTileId];
        //     }
        //     else
        //     {
        //         ren.sprite = null;
        //     }
        // }
        // else
        // {
        //     ren.sprite = fowSprites[tile.autoFowId];
        // }
    }

    public int visiteRadius = 1;

    public void OnTileVisited(Tile tile)
    {
        int centerX = tile.id % mapWidth;
        int centerY = tile.id / mapWidth;

        int radius = visiteRadius;
        for (int i = -radius; i <= radius; ++i)
        {
            for (int j = -radius; j <= radius; ++j)
            {
                int x = centerX + j;
                int y = centerY + i;
                if (x < 0 || x >= mapWidth || y < 0 || y >= mapHeight)
                    continue;

                int id = y * mapWidth + x;
                map.tiles[id].isVisited = true;
                DecorateTile(id);
            }
        }
        radius += 1;
        for (int i = -radius; i <= radius; ++i)
        {
            for (int j = -radius; j <= radius; ++j)
            {
 
                if (i == radius || i == -radius || j == radius || j == -radius)
                {
                    int x = centerX + j;
                    int y = centerY + i;
                    if (x < 0 || x >= mapWidth || y < 0 || y >= mapHeight)
                        continue;

                    int id = y * mapWidth + x;
                    map.tiles[id].UpdateAutoFowId();
                    DecorateTile(id);
                }
            }
        }
    }

    private Camera cam;

    private void Awake()
    {
        cam = Camera.main;
    }
    // Update is called once per frame

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Debug.Log(ScreenPosToTileId(Input.mousePosition));
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            CancelMovement();
            ResetStage();
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (map == null || isMoving)
                return;

            var targetTileId = ScreenPosToTileId(Input.mousePosition);
            var targetTile = map.tiles[targetTileId];

            search.Init(map);

            if (search.AStar(map.startTile, targetTile))
            {
                CancelMovement();

                movementCts = new CancellationTokenSource();

                MoveAlongPathAsync(movementCts.Token).Forget();
            }
        }
    }

    private async UniTaskVoid MoveAlongPathAsync(CancellationToken cancellationToken)
    {
        isMoving = true;

        try
        {
            for (int i = 1; i < search.tiles.Count; i++)
            {
                var targetTile = search.tiles[i];
                var targetPosition = GetTilePos(targetTile.id);

                while (Vector3.Distance(player.transform.position, targetPosition) > 0.01f)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    player.transform.position = Vector3.MoveTowards(player.transform.position, targetPosition, moveSpeed * Time.deltaTime);

                    await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken);
                }

                player.transform.position = targetPosition;

                OnTileVisited(targetTile);

                map.startTile = targetTile;
            }
        }
        catch (System.OperationCanceledException)
        {
            Debug.Log("실패");
        }

        isMoving = false;
    }

    private void CancelMovement()
    {
        if (movementCts != null)
        {
            movementCts.Cancel();
            movementCts.Dispose();
            movementCts = null;
        }
    }

    private void OnDestroy()
    {
        CancelMovement();
    }
}
