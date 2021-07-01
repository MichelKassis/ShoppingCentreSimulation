using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnAds : MonoBehaviour {


	public GameObject AdMan;
	
	public float numAds = 30f;
	
	public int rateOfSpawnAd = 10;
	
	void Start () {
		StartCoroutine(SpawnAgents(rateOfSpawnAd));
	}
	
	void Update () {
		
	}
	IEnumerator SpawnAgents(int rateOfSpawnShoppers) {
		for (int i = 0; i < numAds; i++) {
			Instantiate(AdMan, transform.position, Quaternion.identity);
			yield return new WaitForSeconds(rateOfSpawnShoppers);
		}
	}
}
