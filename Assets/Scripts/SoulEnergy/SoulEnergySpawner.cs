using UnityEngine;
using System.Collections.Generic;

public class SoulSpawner : MonoBehaviour
{
    // --- NUEVO: Estructura para los orbes con probabilidad ---
    [System.Serializable]
    public class SoulDrop
    {
        public SoulEnergyMovement prefab;
        [Tooltip("Peso de probabilidad. Ej: Si el Orbe Común tiene 80 y el Raro tiene 20, el común saldrá el 80% de las veces.")]
        public float peso = 1f;
    }

    [Header("Prefabs y Probabilidades")]
    [Tooltip("Agrega aquí los diferentes tipos de orbes y su peso de aparición.")]
    [SerializeField] private List<SoulDrop> posiblesOrbes = new List<SoulDrop>();

    [Header("Garantía de Spawn")]
    [Tooltip("Cantidad de orbes que se spawnearán SI O SÍ usando el prefab con el peso más alto antes de empezar a usar las probabilidades. 0 = desactivado.")]
    [SerializeField] private int spawnsGarantizadosDelMasPeso = 4;

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
        if (posiblesOrbes.Count == 0) return;

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
        // 1. Elegir qué prefab spawnear
        SoulEnergyMovement prefabElegido = ObtenerPrefabAleatorio();

        if (prefabElegido == null) return;

        // 2. Calcular posición
        Vector3 origen = puntoSpawn != null ? puntoSpawn.position : transform.position;
        Vector3 offset = Random.insideUnitSphere * radioSpawn;
        offset.y = 0f;

        // 3. Instanciar
        SoulEnergyMovement orbe = Instantiate(
            prefabElegido,
            origen + offset,
            Quaternion.identity
        );

        orbesVivos.Add(orbe);
        orbe.AplicarImpulsoAleatorio(impulsoMin, impulsoMax, impulsoHorizontal);

        spawnsRealizados++;
    }

    // --- Lógica de selección ---
    private SoulEnergyMovement ObtenerPrefabAleatorio()
    {
        if (posiblesOrbes.Count == 0) return null;

        // --- NUEVO: Comprobar si estamos en la fase de spawns garantizados ---
        if (spawnsRealizados < spawnsGarantizadosDelMasPeso)
        {
            return ObtenerPrefabConMasPeso();
        }

        // --- Lógica de probabilidad normal ---
        float pesoTotal = 0f;
        foreach (var drop in posiblesOrbes)
        {
            if (drop.prefab != null && drop.peso > 0)
                pesoTotal += drop.peso;
        }

        if (pesoTotal <= 0f)
        {
            Debug.LogWarning("Todos los pesos son 0 o nulos. Devolviendo el primer prefab válido.");
            return posiblesOrbes[0].prefab;
        }

        float randomValue = Random.Range(0f, pesoTotal);
        float pesoAcumulado = 0f;

        foreach (var drop in posiblesOrbes)
        {
            if (drop.prefab == null || drop.peso <= 0) continue;

            pesoAcumulado += drop.peso;
            if (randomValue <= pesoAcumulado)
            {
                return drop.prefab;
            }
        }

        return posiblesOrbes[0].prefab;
    }

    // --- NUEVO: Método para encontrar el prefab con el peso más alto ---
    private SoulEnergyMovement ObtenerPrefabConMasPeso()
    {
        SoulEnergyMovement mejorPrefab = null;
        float mayorPeso = -1f;

        foreach (var drop in posiblesOrbes)
        {
            if (drop.prefab != null && drop.peso > mayorPeso)
            {
                mayorPeso = drop.peso;
                mejorPrefab = drop.prefab;
            }
        }

        // Si por alguna razón no encontró ninguno válido, devolvemos el primero (aunque sea nulo)
        if (mejorPrefab == null && posiblesOrbes.Count > 0)
        {
            mejorPrefab = posiblesOrbes[0].prefab;
        }

        return mejorPrefab;
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