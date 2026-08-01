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

    public bool HasProjectiles => _orbitingObjects.Count > 0;

    void Start()
    {
        // Si olvidaste asignarlo en el Inspector, usa su propio Transform
        if (_centerPoint == null) _centerPoint = transform;

        // Le damos inclinación a cualquier objeto que hayas puesto manualmente antes de darle a Play
        foreach (var obj in _orbitingObjects)
        {
            ApplyRandomTilt(obj);
        }
        
        RearrangeOrbit();
    }

    public OrbitMovement GetNextProjectile()
    {
        return HasProjectiles ? _orbitingObjects[0] : null;
    }

    public void AddProjectileToOrbit(OrbitMovement newObj)
    {
        if (!_orbitingObjects.Contains(newObj))
        {
            // ¡MAGIA!: Al entrar a la órbita, le sorteamos su ángulo de inclinación
            ApplyRandomTilt(newObj);
            
            _orbitingObjects.Add(newObj);
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

    /// <summary>
    /// Genera un Vector3 aleatorio entre los mínimos y máximos y se lo aplica a la bala.
    /// </summary>
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

}
