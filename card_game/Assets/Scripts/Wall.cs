using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Wall : MonoBehaviour
{
    
    // Start is called before the first frame update
    void Start()
    {
        gameObject.GetComponent<Health>().Death += Death;
    }

    private void Death()
    {
        var tileMap = transform.parent.gameObject.GetComponent<Tilemap>();
        tileMap.SetTile(tileMap.WorldToCell(transform.position), null);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
