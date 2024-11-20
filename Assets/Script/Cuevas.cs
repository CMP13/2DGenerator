using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Cuevas : MonoBehaviour
{
    [SerializeField] GameObject Player;
    private int alturaSpawn;
    [Header("Ancho y alto de la generación")]
    [SerializeField] int width;
    [SerializeField] int height;
    [Header("Smoothness")]
    [SerializeField] float smoothness;
    [Header("Cave Gen")]
    [Range(0,1)]
    [SerializeField] float modifier;
    [Header("Cantidad de cueva respecto a tierra")]
    [Range(0, 1)]
    [SerializeField] float cavePercentage;
    [Header("Cantidad Vegetacion")]
    [Range(0, 1)]
    [SerializeField] float vegPercentage;
    [Header("Grupo Vegetacion")]
    [SerializeField] float agruparVeg;
    [Header("Cantidad Nube")]
    [Range(0, 1)]
    [SerializeField] float nubePercentage;
    [Header("Grupo Nube")]
    [Range(0, 1)]
    [SerializeField] float agruparNube;
    [Header("Semilla")]
    [SerializeField] float seed;
    [Header("Referencia al tilemap")]
    [SerializeField] Tilemap tilemap;
    [SerializeField] Tilemap Fondos;
    [Header("Referencia al tilerule")]
    [SerializeField] TileBase Hierba;
    [SerializeField] TileBase Piedra;
    [SerializeField] TileBase FondoPiedra;
    [SerializeField] TileBase Nieve;
    [SerializeField] TileBase Desierto;
    [SerializeField] TileBase Nube;
    [Header("Referencia Tiles Plantas")]
    [SerializeField] Tile[] PlantaPrado;
    [SerializeField] Tile[] PlantaNieve;
    

    private int offSetY;
    private int ultimaAltura;
    private int cambioPrad;
    private int cambioDes;

    int[,] map;
    // Start is called before the first frame update
    void Start()
    {
        cambioDes = cambioPrad = 0;
        offSetY = 0;
        map = new int[width, height];
        map = GenerateArray(width, height, true);
        map = TerrenGeneration(map);
        RenderMap(map, tilemap, Hierba, Piedra);
        Spawn();
    }

    public void Spawn()
    {
        Vector3Int v = new Vector3Int(width / 2, alturaSpawn, 0);
        Instantiate(Player, tilemap.CellToWorld(v), Quaternion.identity);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            seed = Random.Range(-1000000, 1000000);
            Generation();
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            tilemap.ClearAllTiles();
            Fondos.ClearAllTiles();
        }
    }

    /* Generar un array de todos 0 y tamaño width,height */
    public int[,] GenerateArray(int width, int height, bool empty)
    {
        int[,] map = new int[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                // Si empty es true 0, si es false 1
                map[x, y] = (empty) ? 0 : 1;
            }
        }
        return map;
    }

    /* Pinta los tile según el array map con valor en [x,y] 1 , si el valor es 0 no hay tile */
    public void RenderMap(int[,] map, Tilemap tilemap, TileBase Hierba, TileBase Piedra)
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                switch (map[x, y])
                {
                    case 1:
                        tilemap.SetTile(new Vector3Int(x, y, 0), Hierba); //Superficie normal
                        break;
                    case 2:
                        tilemap.SetTile(new Vector3Int(x, y, 0), Piedra); //Piedra 
                        break;
                    case 3:
                        Fondos.SetTile(new Vector3Int(x, y, 0), FondoPiedra); //Fondo de las cuevas
                        break;
                    case 4:
                        tilemap.SetTile(new Vector3Int(x, y, 0), Nieve); //SuperficieNevada
                        break;
                    case 5:
                        tilemap.SetTile(new Vector3Int(x, y, 0), Desierto); //SuperficieDesierto
                        break;
                    case 6:
                        Fondos.SetTile(new Vector3Int(x, y, 0), (Mathf.PerlinNoise(x,seed)<0.5f) ? PlantaPrado[0] : PlantaPrado[1]); //Plantas Prado
                        break;
                    case 7:
                        Fondos.SetTile(new Vector3Int(x, y, 0), (Mathf.PerlinNoise(x,seed) < 0.5f) ? PlantaNieve[0] : PlantaNieve[1]); //Plantas Nieve
                        break;
                    case 8:
                        Fondos.SetTile(new Vector3Int(x, y, 0), Nube);
                        break;
                }
            }
        }
    }

    /* Generamos la altura con perlin noise del terreno */
    //Indice numeros: 1-Hierba 2-Piedra 3-FondoCuevas 4-Nieve 5-Desierto 6-PlantaPrado 7-PlantaNieve
    public int[,] TerrenGeneration(int[,] map)
    {
        
        for (int x = 0; x < width; x++)
        {
            
            int tilSuperficie;
            if (x <= width * 0.3) //Nieve
            {
                tilSuperficie = 4;
                cavePercentage = 0.4f;
                smoothness = 50;
            } else if (x > width * 0.6) //Desierto
            {
                tilSuperficie = 5;
                cavePercentage = 0.1f;
                smoothness = 250;
            } else  //Pradera
            {
                tilSuperficie = 1;
                cavePercentage = 0.3f;
                smoothness = 150;
            }
            int perlinHeight = Mathf.RoundToInt(Mathf.PerlinNoise(x / smoothness, seed) * height);

            GenerarOffSet(tilSuperficie, perlinHeight);

            GenerarY(perlinHeight, x, map, tilSuperficie);

            

            GenerarPlanta(tilSuperficie, x, perlinHeight);
            
        }
        return map;
    }

    void Generation()
    {
        cambioDes = cambioPrad = 0;
        offSetY = 0;
        map = GenerateArray(width, height, true);
        map = TerrenGeneration(map);
        RenderMap(map, tilemap, Hierba, Piedra);
    }

    void GenerarY(int perlinHeight, int x ,int[,] map ,int tilSuperficie)
    {
        int altura = perlinHeight+offSetY;
        if (perlinHeight + offSetY >= height)
            altura = height;
        for (int y = 0; y < altura; y++)
        {
            if ((altura) - y > Random.Range(4, 6))
            {
                if (Mathf.PerlinNoise(x * modifier + seed, y * modifier + seed) > cavePercentage)
                    map[x, y] = 2;
                else
                    map[x, y] = 3;
            }
            else
            {
                map[x, y] = tilSuperficie;
            } 
        }
        ultimaAltura = altura;
        if (x == width * 0.5)
        {
            alturaSpawn= altura + 2; 
        }

        GenerarNube(altura, map, x);
    }

    void GenerarNube(int perlin, int[,] map, int x)
    {
        for (int i=perlin+10; i<height; i++)
        {
            if (Mathf.PerlinNoise(x * agruparNube + seed, i * agruparNube + seed) > nubePercentage)
                map[x, i] = 0;
            else
                map[x, i] = 8;
        }
    }

    public void GenerarOffSet(int zona, int perlin)
    {
        if (zona == 1 && cambioPrad == 0)
        {
            offSetY = ultimaAltura - perlin;
            cambioPrad++;
        } else if (zona == 5 && cambioDes == 0)
        {
            offSetY = ultimaAltura - perlin;
            cambioDes++;
        }
        
    }

    private void GenerarPlanta(int zona, int x, int perlin)
    {
        switch (zona)
        {
            case 1:
                if (perlin+offSetY<height)
                {
                    if (Mathf.PerlinNoise(x/agruparVeg , seed)<vegPercentage)
                        map[x, perlin + offSetY] = 6;
                }
                break;
            case 4:
                if (perlin + offSetY < height)
                {
                    if (Mathf.PerlinNoise(x / agruparVeg, seed) < vegPercentage)
                        map[x, perlin + offSetY] = 7;
                }
                break;
        }
    }

    



    
}
