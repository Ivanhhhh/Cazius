using UnityEngine;
using System.Collections;

using System.Collections.Generic;
public class OrbitManager : MonoBehaviour
{
    [Header("Configuración de Órbita")]
    [SerializeField] private Transform _centerPoint;
    [SerializeField] private float _sharedRadius = 3f;
    [SerializeField] private float _sharedSpeed = 4f;

    [Header("Inclinación Aleatoria (Efecto Átomo)")]
    [Tooltip("Si se activa, cada bala generada tendrá una inclinación orbital diferente.")]
    [SerializeField] private bool _randomizeTilt = true;
    [SerializeField] private Vector3 _minTilt = new Vector3(-45f, 0f, -45f);
    [SerializeField] private Vector3 _maxTilt = new Vector3(45f, 0f, 45f);

    [Header("Lista Dinámica (Solo lectura)")]
    [SerializeField] private List<OrbitMovement> _orbitingObjects = new List<OrbitMovement>(); 

    [Header("Boost de velocidad de las piedras")]
    [SerializeField] private float _originalSharedSpeed;
    [SerializeField] private Coroutine _speedBoostCoroutine;

    public bool HasProjectiles
    {
        get
        {
            if (RemoveDestroyed()) RearrangeOrbit();
            return _orbitingObjects.Count > 0;
        }
    }

    void Start()
    {
        if (_centerPoint == null) _centerPoint = transform;

        foreach (var obj in _orbitingObjects)
        {
            ApplyRandomTilt(obj);
        }
        
        RearrangeOrbit();
    }

    public OrbitMovement GetNextProjectile()
    {
        if (RemoveDestroyed()) RearrangeOrbit();
        return _orbitingObjects.Count > 0 ? _orbitingObjects[0] : null;
    }

    private bool RemoveDestroyed()
    {
        bool removed = false;
        for (int i = _orbitingObjects.Count - 1; i >= 0; i--)
        {
            if (_orbitingObjects[i] == null)
            {
                _orbitingObjects.RemoveAt(i);
                removed = true;
            }
        }
        return removed;
    }

    public void AddProjectileToOrbit(OrbitMovement newObj)
    {
        if (!_orbitingObjects.Contains(newObj))
        {
            ApplyRandomTilt(newObj);
            _orbitingObjects.Add(newObj);
            RearrangeOrbit(); // asigna _manager y _centerPoint al nuevo, y reacomoda a todos
        }
    }

    public void RemoveFromOrbit(OrbitMovement obj)
    {
        if (_orbitingObjects.Contains(obj))
        {
            _orbitingObjects.Remove(obj);
            RearrangeOrbit(); 
        }
    }

    public void RearrangeOrbit()
    {
        RemoveDestroyed();
        int totalObjects = _orbitingObjects.Count;

        for (int i = 0; i < totalObjects; i++)
        {
            if (_orbitingObjects[i] == null) continue;

            float angleOffset = i * ((Mathf.PI * 2f) / totalObjects);
            
            _orbitingObjects[i]._centerPoint = _centerPoint;
            _orbitingObjects[i]._radius = _sharedRadius;
            _orbitingObjects[i]._orbitSpeed = _sharedSpeed;
            
            _orbitingObjects[i].InitializeOrbit(angleOffset, this);
        }
    }


    private void ApplyRandomTilt(OrbitMovement obj)
    {
        if (_randomizeTilt && obj != null)
        {
            obj._orbitTilt = new Vector3(
                Random.Range(_minTilt.x, _maxTilt.x),
                Random.Range(_minTilt.y, _maxTilt.y),
                Random.Range(_minTilt.z, _maxTilt.z)
            );
        }
    }


    public void BoostSharedSpeed(float multiplier, float duration)
    {
        if (_speedBoostCoroutine != null)
            StopCoroutine(_speedBoostCoroutine);

        _speedBoostCoroutine = StartCoroutine(SpeedBoostRoutine(multiplier, duration));
    }

    private IEnumerator SpeedBoostRoutine(float multiplier, float duration)
    {
        _originalSharedSpeed = _sharedSpeed;
        SetSharedSpeed(_originalSharedSpeed * multiplier);

        yield return new WaitForSeconds(duration);

        SetSharedSpeed(_originalSharedSpeed);
        _speedBoostCoroutine = null;
    }

    // Actualiza la velocidad sin resetear ángulo ni estado de merge
    private void SetSharedSpeed(float newSpeed)
    {
        _sharedSpeed = newSpeed;

        foreach (var obj in _orbitingObjects)
        {
            if (obj != null)
                obj._orbitSpeed = _sharedSpeed;
        }
    }
    public void FireAllAsBullets(Vector3 targetPosition, float bulletSpeed)
    {
        RemoveDestroyed();
        if (_orbitingObjects.Count == 0) return;

        // Copia para no romper el foreach cuando cada uno se remueva de la lista.
        var snapshot = new List<OrbitMovement>(_orbitingObjects);

        foreach (var obj in snapshot)
        {
            if (obj == null) continue;
            obj.FireAsBullet(targetPosition, bulletSpeed);
        }
    }

}
