using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPeople : MonoBehaviour {

	public GameObject Shopper;
	
	public float numShoppers = 200f;
	
	public int rateOfSpawnShoppers = 2;
	
	void Start () {
		StartCoroutine(SpawnAgents(rateOfSpawnShoppers));
	}
	
	void Update () {
		
	}
	IEnumerator SpawnAgents(int rateOfSpawnShoppers) {
		for (int i = 0; i < numShoppers; i++) {
			Instantiate(Shopper, transform.position, Quaternion.identity);
			yield return new WaitForSeconds(rateOfSpawnShoppers);
		}
	}
}
