using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopSpawn : MonoBehaviour
{

    public Vector3 center;
    public Vector3 size;

    private GameObject ShopsUp;
    private GameObject ShopsDown;

    public Vector3 centerOfStore;
    public Vector3 outOfStore;

    // Start is called before the first frame update
    void Start()
    {
        
        ShopsUp = GameObject.Find("/ShopsUp");
        ShopsDown = GameObject.Find("/ShopsDown");

        if (transform.root.gameObject == ShopsDown ) {

            center = new Vector3(0, 1f, -4.3f);
            size =  new Vector3 (4,0,4);
            centerOfStore = transform.position + center;
            outOfStore.x = centerOfStore.x;
            outOfStore.y = centerOfStore.y;
            outOfStore.z = centerOfStore.z + 5f;
        }
        else if (transform.root.gameObject == ShopsUp){
            center = new Vector3(0, 1f, 4.3f);
            size =  new Vector3 (4,0,4);
            centerOfStore = transform.position + center;
            outOfStore.x = centerOfStore.x;
            outOfStore.y = centerOfStore.y;
            outOfStore.z = centerOfStore.z - 5f;
        }
        else {

            Debug.Log("Error with entrances AHHHH");
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1,0,0,0.5f);
        Gizmos.DrawCube(centerOfStore, size);
    }
}
