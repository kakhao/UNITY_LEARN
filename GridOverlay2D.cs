using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridOverlay2D : MonoBehaviour
{
    public float cellSize = 20;
    public Color cellColor = Color.white;
    public Color gridColor = Color.white;

    private Vector2Int Cell1;
    private Material lineMaterial;

    public float UpdateInterval = 0.1f;
    private float UpdateTimer = 0;
    private bool tick;

    private const int MaxX = 1000;
    private const int MaxY = 1000;

    public int OffsetX = 0;
    public int OffsetY = 0;

    private int PrevOffsetX = 0;
    private int PrevOffsetY = 0;

    private bool[,] Cells = new bool[MaxX, MaxY];
    private bool[,] NextCells = new bool[MaxX, MaxY];
    private bool Started = false;

    private bool Dragging = false;
    private Vector3 DragPosition;
 
    void Update()
    {
        float MouseScroll = Input.mouseScrollDelta.y;
        cellSize += MouseScroll;
        if (Input.GetMouseButtonDown(0))
        {
            Cell1 = MouseToCellPos(Input.mousePosition);
            Cells[Cell1.x + OffsetX, Cell1.y + OffsetY] = !Cells[Cell1.x + OffsetX, Cell1.y + OffsetY];
        }
        if (Input.GetMouseButtonDown(1))
        {
            Dragging = true;
            DragPosition = Input.mousePosition;          
        }
        if (Input.GetMouseButtonUp(1))
        {
            PrevOffsetX = OffsetX;
            PrevOffsetY = OffsetY;
            Dragging = false;
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            SaveGame();
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            LoadGame();
        }
        if (Dragging)
        {
            OffsetX = PrevOffsetX - (int)((Input.mousePosition - DragPosition).x / cellSize);
            OffsetY = PrevOffsetY - (int)((Input.mousePosition - DragPosition).y / cellSize);
        }
        if (Input.GetButtonDown("Jump"))
            Started = !Started;
        if (Started)
        {
            UpdateTimer -= Time.fixedDeltaTime;
            if(UpdateTimer <= 0)
            {
                UpdateTimer = UpdateInterval;
                tick = !tick;
            }
            if (tick)
                CheckCells();
            else
                UpdateCells();
        }
    }

    void CreateLineMaterial()
    {
        if (!lineMaterial)
        {
            var shader = Shader.Find("Hidden/Internal-Colored");
            lineMaterial = new Material(shader);
            lineMaterial.hideFlags = HideFlags.HideAndDontSave;
            lineMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            lineMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            lineMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
            lineMaterial.SetInt("_ZWrite", 0);
        }
    }

    void OnPostRender()
    {
        float W = Screen.width;
        float H = Screen.height;
        float DX = cellSize / W;
        float DY = cellSize / H;
        int ScreenMaxX = (int)(W / cellSize);
        int ScreenMaxY = (int)(H / cellSize);

        GL.PushMatrix();
        CreateLineMaterial();
        lineMaterial.SetPass(0);
        GL.LoadOrtho();
        GL.Begin(GL.LINES);
        GL.Color(gridColor);
        for (float i = 0; i < 1; i += DY)
        {
            GL.Vertex3(0, i, 1);
            GL.Vertex3(1, i, 1);
        }
        for (float i = 0; i < 1; i += DX)
        {
            GL.Vertex3(i, 0, 1);
            GL.Vertex3(i, 1, 1);
        }
        GL.End();
        GL.Begin(GL.QUADS);
        GL.Color(cellColor);
        for (int x = Mathf.Clamp(-OffsetX, 0, ScreenMaxX); x < ScreenMaxX; x++)
            for(int y = Mathf.Clamp(-OffsetY, 0, ScreenMaxY); y < ScreenMaxY; y++)
                if (Cells[x + OffsetX, y + OffsetY])
                    DrawCell(x, y, DX, DY); 
        GL.End();
        GL.PopMatrix();
    }
    
    void DrawCell(float x, float y, float DX, float DY)
    {
        float PadX = DX / cellSize;
        float PadY = DY / cellSize;
        float x0 = x * DX ;
        float y0 = y * DY ;
        GL.Vertex3(x0 + PadX, y0 + PadY, 0);
        GL.Vertex3(x0 + DX - PadX, y0 + PadY, 0);
        GL.Vertex3(x0 + DX - PadX, y0 + DY - PadY, 0);
        GL.Vertex3(x0 + PadX, y0 + DY - PadY, 0);
    }

    Vector2Int MouseToCellPos(Vector2 MousePos)
    {
        return new Vector2Int((int)(MousePos.x / cellSize), (int)(MousePos.y / cellSize));
    }

    int GetNeighboursCount(int x0, int y0)
    {
        int NeighboursCount = 0;
        for (int y = y0 > 0 ? y0 - 1 : y0; y0 < MaxY - 1 ? y < y0 + 2 : y < y0 + 1; y++)
            for(int x = x0 > 0 ? x0 - 1 : x0; x0 < MaxX - 1 ? x < x0 + 2 : x < x0 + 1; x++)
                if (Cells[x, y])
                    NeighboursCount++;
        return NeighboursCount;
    }

    void CheckCells()
    {
        for (int yi = 0; yi < MaxY; yi++)
            for (int xi = 0; xi < MaxX; xi++)
            {
                int NeighboursCount = GetNeighboursCount(xi, yi);
                if (Cells[xi, yi])
                    NeighboursCount -= 1;
                NextCells[xi, yi] = NeighboursCount == 3 || NeighboursCount == 2 && Cells[xi, yi];       
            }
    }

    void UpdateCells()
    {
        for (int yi = 0; yi < MaxY; yi++)
            for (int xi = 0; xi < MaxX; xi++)
                Cells[xi, yi] = NextCells[xi, yi];
    }

    void SaveGame()
    {
        string str = "";
        for (int x = 0; x < MaxX; x++)
        {
            for (int y = 0; y < MaxY; y++)
            {
                if(Cells[x, y])
                    str += (str != "" ? "," : "")  + x + "," + y;             
            }
        }
        PlayerPrefs.SetString("SavedGame", str);
    }

    void LoadGame()
    {
        int x;
        int y;
        string str = PlayerPrefs.GetString("SavedGame");
        string[] words = str.Split(',');
        for (int i = 0; i < words.Length; i += 2)
        {
            x = int.Parse (words[i]);
            y = int.Parse(words[i + 1]);
            Cells[x, y] = true;
        }
    }
}
