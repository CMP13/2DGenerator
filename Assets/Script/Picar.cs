using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Picar : MonoBehaviour
{
    private Vector3 objetivo;

    private Camera camara;

    [SerializeField] LayerMask mascaraSuelo;
    private Tilemap tilemap;
    private Tilemap tilemapFondo;
    [SerializeField] private TileBase tileFondo;
    [SerializeField] GameObject puntaPico;
    [SerializeField] GameObject pico;

    // Start is called before the first frame update
    void Start()
    {
        camara = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        tilemap = GameObject.FindGameObjectWithTag("Mapa").GetComponent<Tilemap>();
        tilemapFondo = GameObject.FindGameObjectWithTag("Fondo").GetComponent<Tilemap>();
    }

    // Update is called once per frame
    void Update()
    {
       objetivo = camara.ScreenToWorldPoint(Input.mousePosition);
        float anguloRad = Mathf.Atan2(objetivo.y - transform.position.y, objetivo.x - transform.position.x);
        float anguloGrad = (180 / Mathf.PI) * anguloRad - 90;
        transform.rotation = Quaternion.Euler(0, 0, anguloGrad);
        DestroyTileDetection();
    }

    void DestroyTileDetection()
    {


        if (Input.GetKeyDown(KeyCode.Space))
        {
            Vector3Int positionTile = tilemap.WorldToCell(puntaPico.transform.position);
            TileBase til = tilemap.GetTile(positionTile);
            if (til != null)
            {
                // Tenemos que mover la detección un poco a la derecha porque queremos borrar el bloque
                // de la derecha a partir de la detección, la detección si la dejamos en el punto exacto
                // en el que se cruza el raycast y el tile dará resultados inconsistentes ya que
                // está justo en mitad de dos posiciones del tile. Por lo tanto se le suma 0.1 a la derecha
                
                tilemap.SetTile(positionTile, null);
                tilemapFondo.SetTile(positionTile, tileFondo);
                Debug.Log($"Borrando tile: {positionTile}");
            }
        }
        /*
        if (Input.GetKeyDown(KeyCode.Space))
        {
            RaycastHit2D rayHit = Physics2D.Raycast(transform.position, camara.ScreenToWorldPoint(Input.mousePosition)-transform.position, 1f);
            if (rayHit.transform != null)
            {
                
                Vector3Int positionTile = tilemap.WorldToCell(rayHit.point);
                tilemap.SetTile(positionTile, null);
                Debug.Log(positionTile);
            }
                          
            //Debug.DrawRay(transform.position, Input.mousePosition - transform.position, Color.blue, 4, false);
            Debug.DrawRay(transform.position, camara.ScreenToWorldPoint(Input.mousePosition)-transform.position, Color.blue, 4, false);
            Debug.Log(transform.position + " " + Input.mousePosition + "");
        }*/
    }
}
