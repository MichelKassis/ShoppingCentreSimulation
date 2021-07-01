using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnObstacle : MonoBehaviour
{
    public Vector3 center;
    public Vector3 size;


    public GameObject tablePrefab;
    public GameObject chairPrefab;

    public GameObject planterPrefab;

    public int horizDistance = 6;
    public int vertDistance = 3;
    public int vertDistancePlant = 6;
    public int numberOfTablePacks;

    // Start is called before the first frame update
    void Start()
    {
       numberOfTablePacks = 1;
       createTablePack(numberOfTablePacks);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public GameObject spawnTable()
    {
        Vector3 pos = center + new Vector3(Random.Range(-size.x / 2, size.x / 2), 0.0f, Random.Range(-size.z / 2, size.z / 2));
        GameObject table1 = Instantiate(tablePrefab, pos, Quaternion.identity);
        table1.gameObject.tag = "Table";
        return table1;
    }

    public GameObject spawnChairsAroundTable(Transform Objtransform, int chairGroupNumber)
    {
        GameObject chairGroup = new GameObject("ChairGroup" + chairGroupNumber);

        GameObject chair1 = Instantiate( chairPrefab, Objtransform.position + ( transform.right * horizDistance), transform.rotation );
        GameObject chair2 = Instantiate( chairPrefab, Objtransform.position - ( transform.right * horizDistance), transform.rotation );
        
        GameObject plant = Instantiate( planterPrefab, Objtransform.position + ( transform.forward * vertDistancePlant), transform.rotation );

        chair1.transform.SetParent(chairGroup.transform);
        chair2.transform.SetParent(chairGroup.transform);

        chair1.gameObject.tag = "Chair";
        chair2.gameObject.tag = "Chair";

        if (Random.value >= 0.5)
        {
        GameObject chair3 = Instantiate( chairPrefab, Objtransform.position + ( transform.forward * vertDistance), transform.rotation );
        GameObject chair4 = Instantiate( chairPrefab, Objtransform.position - ( transform.forward * vertDistance), transform.rotation );

        chair3.transform.SetParent(chairGroup.transform);
        chair4.transform.SetParent(chairGroup.transform);

        chair3.gameObject.tag = "Chair";
        chair4.gameObject.tag = "Chair";

        }
        else {
        GameObject chair3 = Instantiate( chairPrefab, Objtransform.position + ( transform.right * horizDistance/2 ) + ( transform.forward * vertDistance), transform.rotation );
        GameObject chair4 = Instantiate( chairPrefab, Objtransform.position + ( transform.right * horizDistance/2 ) - ( transform.forward * vertDistance), transform.rotation );
        GameObject chair5 = Instantiate( chairPrefab, Objtransform.position - ( transform.right * horizDistance/2 ) - ( transform.forward * vertDistance), transform.rotation );
        GameObject chair6 = Instantiate( chairPrefab, Objtransform.position - ( transform.right * horizDistance/2 ) + ( transform.forward * vertDistance), transform.rotation );

        chair3.transform.SetParent(chairGroup.transform);
        plant.transform.SetParent(chairGroup.transform);
        chair4.transform.SetParent(chairGroup.transform);
        chair5.transform.SetParent(chairGroup.transform);
        chair6.transform.SetParent(chairGroup.transform);

        chair3.gameObject.tag = "Chair";
        plant.gameObject.tag = "Plant";
        chair4.gameObject.tag = "Chair";
        chair5.gameObject.tag = "Chair";
        chair6.gameObject.tag = "Chair";
        
        }
        return chairGroup;
    }

    public void createTablePack(int numberOfTables){
        for (int i = 0; i <numberOfTables; i++)
        {
            
            GameObject table = spawnTable();
            GameObject chairGroup = spawnChairsAroundTable(table.transform, i+1);

            GameObject tableChairCombo = new GameObject ("Combo"+i);
            table.transform.SetParent(tableChairCombo.transform);
            chairGroup.transform.SetParent(tableChairCombo.transform);

            BoxCollider boxCollider = FitColliderToChildren(tableChairCombo);
            boxCollider.isTrigger = true;
        }
    }

    private BoxCollider FitColliderToChildren (GameObject parentObject)
   {
        BoxCollider bc = parentObject.GetComponent<BoxCollider>();
        if(bc==null){bc = parentObject.AddComponent<BoxCollider>();}
        Bounds bounds = new Bounds (Vector3.zero, Vector3.zero);
        bool hasBounds = false;
        Renderer[] renderers =  parentObject.GetComponentsInChildren<Renderer>();
        foreach (Renderer render in renderers) {
            if (hasBounds) {
                bounds.Encapsulate(render.bounds);
            } else {
                bounds = render.bounds;
                hasBounds = true;
           }
       }
        if (hasBounds) {
            bc.center = bounds.center - parentObject.transform.position;
            bc.size = bounds.size;
      } else {
            bc.size = bc.center = Vector3.zero;
            bc.size = Vector3.zero;
      }
      return bc;
   }
 

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1,0,0,0.5f);
        Gizmos.DrawCube(transform.localPosition + center, size);


    }
}
