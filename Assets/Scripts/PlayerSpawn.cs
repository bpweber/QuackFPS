using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerSpawn : NetworkBehaviour
{
    public Transform spawnList;

    public override void OnNetworkSpawn()
    {
        int randSpawnNum = Random.Range(0, spawnList.childCount);
        transform.GetComponent<CharacterController>().enabled = false;
        transform.position = spawnList.GetChild(randSpawnNum).position;
        transform.LookAt(new Vector3(spawnList.position.x, transform.position.y, spawnList.position.z));
        transform.GetComponent<CharacterController>().enabled = true;

        base.OnNetworkSpawn();
    }
}
