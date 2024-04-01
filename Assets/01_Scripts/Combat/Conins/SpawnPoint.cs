using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
public class SpawnPoint : NetworkBehaviour
{
    public string pointName;
    public Vector3 Position => transform.position;
    [field: SerializeField]
    public float Radius { get; private set; } = 10.0f;
    public List<Vector3> SpawnPoints { get; private set; }
    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            SpawnPoints = MapManager.Instance.GetAvailabePositionList(transform.position, Radius);
        }

    }

#if UNITY_EDITOR

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, Radius);
        Gizmos.color = Color.white;
    }

#endif
}
