using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SpawnPlayers : MonoBehaviour
{
    public GameObject playerPrefab_1, playerPrefab_2;

    public float minX, maxX, minZ, maxZ;

    private void Start(){
        Vector3 randomPosition = new Vector3(Random.Range(minX, maxX), 0, Random.Range(minZ, maxZ));

        if(PhotonNetwork.CurrentRoom.PlayerCount == 1){
            PhotonNetwork.Instantiate(playerPrefab_1.name, randomPosition, Quaternion.identity);
        }
        else if(PhotonNetwork.CurrentRoom.PlayerCount == 2){
            PhotonNetwork.Instantiate(playerPrefab_2.name, randomPosition, Quaternion.identity);
        }
            
    }
}
