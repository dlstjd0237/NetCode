using UnityEngine;

public class Lifetime : MonoBehaviour
{
    [SerializeField] private float _lifetime;
    private float _currentLifetime;

    private void Update()
    {
        _currentLifetime += Time.deltaTime;
        if (_currentLifetime >= _lifetime)
            Destroy(gameObject);
    }
}
