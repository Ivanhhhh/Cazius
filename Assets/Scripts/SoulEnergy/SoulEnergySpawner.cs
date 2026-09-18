using UnityEngine;
using System.Collections.Generic;

public class SoulSpawner : MonoBehaviour
{
    [Header("Prefab")]
    [SerializeField] private SoulEnergyMovement prefabOrbe;

    [Header("Frecuencia de spawn")]
    [Tooltip("Segundos entre spawns. Se elige un valor aleatorio en este rango.")]
    [SerializeField] private float intervaloMin = 0.5f;
    [SerializeField] private float intervaloMax = 2f;

    [Tooltip("Máximo de orbes vivos a la vez. Se pausa el spawn si se llega.")]
    [SerializeField] private int maxOrbesVivos = 12;

    [Header("Límite de spawn")]
    [Tooltip("Cantidad total de orbes que puede spawnear este spawner. -1 = ilimitado.")]
    [SerializeField] private int maxSpawnsTotales = -1;

    [Tooltip("Si está activo, el spawner se desactiva cuando llega al límite.")]
    [SerializeField] private bool desactivarAlTerminar = true;

    [Tooltip("Si está activo, también se destruye el GameObject (si no, solo se desactiva el componente).")]
    [SerializeField] private bool destruirAlTerminar = false;

    [Tooltip("Evento opcional al terminar todos los spawns (para conectar cosas por Inspector).")]
    [SerializeField] private UnityEngine.Events.UnityEvent alTerminar;

    [Header("Impulso inicial")]
    [SerializeField] private float impulsoMin = 1f;
    [SerializeField] private float impulsoMax = 4f;
    [SerializeField] private bool impulsoHorizontal = true;

    [Header("Punto de spawn")]
    [SerializeField] private Transform puntoSpawn;
    [SerializeField] private float radioSpawn = 0.3f;

    [Header("Debug")]
    [SerializeField] private bool dibujarGizmos = true;

    // --- Estado interno ---
    private float timerProximoSpawn;
    private int spawnsRealizados;
    private bool terminado;
    private readonly List<SoulEnergyMovement> orbesVivos = new List<SoulEnergyMovement>();

    public bool Terminado => terminado;
    public int SpawnsRealizados => spawnsRealizados;

    private void OnEnable()
    {
        terminado = false;
        ProgramarProximoSpawn();
    }

    private void Start()
    {
        ProgramarProximoSpawn();
    }

    private void Update()
    {
        if (terminado) return;
        if (prefabOrbe == null) return;

        orbesVivos.RemoveAll(o => o == null || !o.gameObject.activeInHierarchy);

        if (maxSpawnsTotales >= 0 && spawnsRealizados >= maxSpawnsTotales)
        {
            Finalizar();
            return;
        }

        if (orbesVivos.Count >= maxOrbesVivos) return;

        timerProximoSpawn -= Time.deltaTime;
        if (timerProximoSpawn <= 0f)
        {
            SpawnearOrbe();
            ProgramarProximoSpawn();
        }
    }

    private void ProgramarProximoSpawn()
    {
        timerProximoSpawn = Random.Range(intervaloMin, intervaloMax);
    }

    private void SpawnearOrbe()
    {
        Vector3 origen = puntoSpawn != null ? puntoSpawn.position : transform.position;
        Vector3 offset = Random.insideUnitSphere * radioSpawn;
        offset.y = 0f;

        SoulEnergyMovement orbe = Instantiate(
            prefabOrbe,
            origen + offset,
            Quaternion.identity
        );

        orbesVivos.Add(orbe);
        orbe.AplicarImpulsoAleatorio(impulsoMin, impulsoMax, impulsoHorizontal);

        spawnsRealizados++;
    }

    private void Finalizar()
    {
        terminado = true;
        alTerminar?.Invoke();

        if (destruirAlTerminar)
        {
            Destroy(gameObject);
            return;
        }

        if (desactivarAlTerminar)
            enabled = false;
    }

    // --- API pública ---

    public void ForzarFin() => Finalizar();

    public void Reiniciar()
    {
        spawnsRealizados = 0;
        terminado = false;
        enabled = true;
        ProgramarProximoSpawn();
    }

    // --- Debug ---

    private void OnDrawGizmosSelected()
    {
        if (!dibujarGizmos) return;

        Vector3 origen = puntoSpawn != null ? puntoSpawn.position : transform.position;

        Gizmos.color = new Color(1f, 0.6f, 0f, 0.4f);
        Gizmos.DrawWireSphere(origen, radioSpawn);

        Gizmos.color = Color.yellow;
        const int pasos = 16;
        for (int i = 0; i < pasos; i++)
        {
            float ang = i * (360f / pasos) * Mathf.Deg2Rad;
            Vector3 dir = new Vector3(Mathf.Cos(ang), 0f, Mathf.Sin(ang));
            Gizmos.DrawRay(origen, dir * 1.2f);
        }
    }
}