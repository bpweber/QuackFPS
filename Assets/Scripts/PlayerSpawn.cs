using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerSpawn : NetworkBehaviour
{

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        float randX = Random.Range(-10f, 10f);
        float randZ = Random.Range(-10f, 10f);

        transform.GetComponent<CharacterController>().enabled = false;
        transform.position = new Vector3(randX, 1, randZ);
        transform.GetComponent<CharacterController>().enabled = true;
    }
}
