using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroySelfWhenTrigger : MonoBehaviour
{
    [SerializeField] private GameObject _destroyObj;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_destroyObj != null)
        {
            Instantiate(_destroyObj, transform.position, Quaternion.identity);
        }
        Destroy(gameObject);
    }
}
