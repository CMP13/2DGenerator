using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movimiento : MonoBehaviour
{
    private Rigidbody2D rigid;
    private Animator anim;
    public float velocidadh;
    public float salto;

    [Header("Caja para suelo")]
    public float posicionboxX = 0f;
    public float posicionboxY = 0f;
    public float tamanoboxX = 10f;
    public float tamanoboxY = 10f;
    public LayerMask mascarasuelo;
    // Start is called before the first frame update
    void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        rigid.velocity = new Vector2(Input.GetAxis("Horizontal") * velocidadh, rigid.velocity.y);
        BoxCast(new Vector2(transform.position.x - posicionboxX, transform.position.y - posicionboxY), new Vector2(tamanoboxX, tamanoboxY), 0f, new Vector2(0, 0), 0f, mascarasuelo);
        anim.SetFloat("Velocity", Mathf.Abs(rigid.velocity.x)+Mathf.Abs(rigid.velocity.y));


        //anim.SetFloat("VelY", Mathf.Abs(rigid.velocity.y));
        if (Input.GetKeyDown(KeyCode.W) && compruebaSuelo())
        {
            //anim.SetTrigger("Salto");
            rigid.AddForce(new Vector2(0, salto));
        }
        Giro();

    }
    
    public void Giro()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            transform.rotation = Quaternion.Euler(0,180,0);
        } else if (Input.GetKeyDown(KeyCode.D))
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }

    private bool compruebaSuelo()
    {
        bool suelo = Physics2D.BoxCast(new Vector2(transform.position.x, transform.position.y - posicionboxY), new Vector2(tamanoboxX, tamanoboxY), 0f, new Vector2(0, 0), 0f, mascarasuelo);
        return suelo;
    }

    static public RaycastHit2D BoxCast(Vector2 origen, Vector2 size, float angle, Vector2 direction, float distance, int mask)
    {
        RaycastHit2D hit = Physics2D.BoxCast(origen, size, angle, direction, distance, mask);

        //Setting up the points to draw the cast
        Vector2 p1, p2, p3, p4, p5, p6, p7, p8;
        float w = size.x * 0.5f;
        float h = size.y * 0.5f;
        p1 = new Vector2(-w, h);
        p2 = new Vector2(w, h);
        p3 = new Vector2(w, -h);
        p4 = new Vector2(-w, -h);

        Quaternion q = Quaternion.AngleAxis(angle, new Vector3(0, 0, 1));
        p1 = q * p1;
        p2 = q * p2;
        p3 = q * p3;
        p4 = q * p4;

        p1 += origen;
        p2 += origen;
        p3 += origen;
        p4 += origen;

        Vector2 realDistance = direction.normalized * distance;
        p5 = p1 + realDistance;
        p6 = p2 + realDistance;
        p7 = p3 + realDistance;
        p8 = p4 + realDistance;


        //Drawing the cast
        Color castColor = hit ? Color.red : Color.blue;
        Debug.DrawLine(p1, p2, castColor);
        Debug.DrawLine(p2, p3, castColor);
        Debug.DrawLine(p3, p4, castColor);
        Debug.DrawLine(p4, p1, castColor);

        Debug.DrawLine(p5, p6, castColor);
        Debug.DrawLine(p6, p7, castColor);
        Debug.DrawLine(p7, p8, castColor);
        Debug.DrawLine(p8, p5, castColor);

        Debug.DrawLine(p1, p5, Color.grey);
        Debug.DrawLine(p2, p6, Color.grey);
        Debug.DrawLine(p3, p7, Color.grey);
        Debug.DrawLine(p4, p8, Color.grey);
        if (hit)
        {
            Debug.DrawLine(hit.point, hit.point + hit.normal.normalized * 0.2f, Color.yellow);
        }

        return hit;
    }
}
