using System.Collections.Generic;
using UnityEngine;

public class Enemy_MeteoriteSpawnZone : MonoBehaviour
{
    [Header("Detección")]
    [SerializeField] private Transform player;
    [SerializeField] private Vector3 zoneSize = new Vector3(5f, 3f, 5f);
    [SerializeField] private Vector3 zoneOffset = Vector3.zero;

    [Header("Prefabs")]
    [SerializeField] private GameObject singlePrefab;

    [Header("Spawn")]
    [SerializeField] private float spawnRadius = 1.5f;

    [Header("Predicción")]
    [SerializeField] private float predictionTime = 0.5f;


    // Cache / fallback
    private Rigidbody playerRb;
    private bool rbWarningShown;
    private Vector3 lastPlayerPos;
    private Vector3 estimatedVelocity;

    private void Start()
    {
        if (player == null)
        {
            if (GameManager.Instance == null)
            {
                Debug.LogError("[SpawnZone] GameManager.Instance es null. Asigna el player a mano en el inspector.", this);
            }
            else if (GameManager.Instance.Player == null)
            {
                Debug.LogError("[SpawnZone] GameManager.Instance.Player es null.", this);
            }
            else
            {
                player = GameManager.Instance.Player.transform;
            }
        }

        CachePlayerRigidbody();

        if (player != null)
            lastPlayerPos = player.position;
    }

    private void CachePlayerRigidbody()
    {
        if (player == null) return;

        // Busca en el objeto, en los padres y en los hijos.
        playerRb = player.GetComponent<Rigidbody>();
        if (playerRb == null) playerRb = player.GetComponentInParent<Rigidbody>();
        if (playerRb == null) playerRb = player.GetComponentInChildren<Rigidbody>();

        if (playerRb == null)
        {
            Debug.LogWarning($"[SpawnZone] No se encontró Rigidbody en '{player.name}' (ni en padres/hijos). " +
                             "Se usará la velocidad estimada por cambio de posición.", this);
        }
        else if (playerRb.gameObject != player.gameObject)
        {
            Debug.Log($"[SpawnZone] Rigidbody encontrado en '{playerRb.name}', no en '{player.name}'.", this);
        }
    }

    // Velocidad estimada por si no hay Rigidbody (o es kinematic).
    private void FixedUpdate()
    {
        if (player == null) return;

        float dt = Time.fixedDeltaTime;
        if (dt > 0f)
            estimatedVelocity = (player.position - lastPlayerPos) / dt;

        lastPlayerPos = player.position;
    }

    // ---------------------------------------------------------
    //  API pública
    // ---------------------------------------------------------

    public bool IsPlayerInside()
    {
        if (player == null) return false;
        return IsInsideZone(player.position);
    }

    public GameObject SpawnOne()
    {
        return SpawnOneInternal(false);
    }

    public List<GameObject> SpawnMultiple(int count)
    {
        return SpawnMultipleInternal(count, false);
    }



    private GameObject SpawnOneInternal(bool ignoreZoneCheck)
    {
        if (!CanSpawn(ignoreZoneCheck)) return null;

        Vector3 spawnPos = GetPredictedPosition() + GetRandomOffset();
        return Instantiate(singlePrefab, spawnPos, Quaternion.identity);
    }

    private List<GameObject> SpawnMultipleInternal(int count, bool ignoreZoneCheck)
    {
        List<GameObject> result = new List<GameObject>();

        if (!CanSpawn(ignoreZoneCheck)) return result;

        Vector3 predicted = GetPredictedPosition();

        for (int i = 0; i < count; i++)
        {
            float t = count > 1 ? (float)i / (count - 1) : 0f;
            Vector3 basePos = Vector3.Lerp(player.position, predicted, t);
            Vector3 spawnPos = basePos + GetRandomOffset();

            result.Add(Instantiate(singlePrefab, spawnPos, Quaternion.identity));
        }

        return result;
    }

    private bool CanSpawn(bool ignoreZoneCheck)
    {
        if (player == null)
        {
            Debug.LogWarning("[SpawnZone] No hay referencia al player.", this);
            return false;
        }

        if (singlePrefab == null)
        {
            Debug.LogWarning("[SpawnZone] Falta el prefab único.", this);
            return false;
        }

        if (!ignoreZoneCheck && !IsPlayerInside())
        {
            Debug.LogWarning("[SpawnZone] El player no está dentro de la zona.", this);
            return false;
        }

        return true;
    }

    // ---------------------------------------------------------
    //  Helpers
    // ---------------------------------------------------------

    private Vector3 GetZoneCenter()
    {
        return transform.position + transform.rotation * zoneOffset;
    }

    private bool IsInsideZone(Vector3 worldPos)
    {
        Vector3 localPos = transform.InverseTransformPoint(worldPos) - zoneOffset;
        Vector3 half = zoneSize * 0.5f;

        return Mathf.Abs(localPos.x) <= half.x &&
               Mathf.Abs(localPos.y) <= half.y &&
               Mathf.Abs(localPos.z) <= half.z;
    }

    private Vector3 GetPredictedPosition()
    {
        if (player == null) return Vector3.zero;

        // Reintenta si se perdió la referencia (p.ej. el player se respawneó).
        if (playerRb == null && !rbWarningShown)
            CachePlayerRigidbody();

        Vector3 velocity;

        if (playerRb != null && !playerRb.isKinematic)
        {
            // Unity 6: linearVelocity. Versiones anteriores: velocity.
            velocity = playerRb.linearVelocity;
        }
        else
        {
            if (!rbWarningShown)
            {
                rbWarningShown = true;
                string motivo = playerRb == null ? "no hay Rigidbody" : "el Rigidbody es kinematic (su velocity es 0)";
                Debug.LogWarning($"[SpawnZone] Predicción con velocidad estimada: {motivo}.", this);
            }
            velocity = estimatedVelocity;
        }

        return player.position + velocity * predictionTime;
    }

    private Vector3 GetRandomOffset()
    {
        Vector2 circle = Random.insideUnitCircle * spawnRadius;
        return new Vector3(circle.x, 0f, circle.y);
    }

    private void OnDrawGizmosSelected()
    {
        Matrix4x4 oldMatrix = Gizmos.matrix;
        Gizmos.matrix = Matrix4x4.TRS(GetZoneCenter(), transform.rotation, Vector3.one);

        Gizmos.color = new Color(0.2f, 0.9f, 0.4f, 0.25f);
        Gizmos.DrawCube(Vector3.zero, zoneSize);

        Gizmos.color = new Color(0.2f, 0.9f, 0.4f, 1f);
        Gizmos.DrawWireCube(Vector3.zero, zoneSize);

        Gizmos.matrix = oldMatrix;
    }
}