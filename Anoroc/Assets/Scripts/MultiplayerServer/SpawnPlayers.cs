using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SpawnPlayers : MonoBehaviour
{
    public GameObject playerPrefab_1, playerPrefab_2;

    private void Start(){
        if (PhotonNetwork.CurrentRoom.PlayerCount == 1){
            PhotonNetwork.Instantiate(playerPrefab_1.name, new Vector3(-3f, 1f, -8f), Quaternion.identity);
        }
        else if(PhotonNetwork.CurrentRoom.PlayerCount == 2){
            PhotonNetwork.Instantiate(playerPrefab_2.name, new Vector3(11f, 1f, -10f), Quaternion.identity);
        }
    }
}
