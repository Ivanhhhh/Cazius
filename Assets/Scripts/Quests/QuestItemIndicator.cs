using UnityEngine;

public class QuestItemIndicator : MonoBehaviour
{
    [Header("Quest Settings")]
    [Tooltip("El ID de la misión que requiere que recojas este ítem")]
    [SerializeField] private string _questID;

    [Header("UI Reference")]
    [Tooltip("Arrastra aquí el GameObject hijo que contiene el Sprite 2D")]
    [SerializeField] private GameObject _indicator2D;

    private void Start()
    {
        // 1. Por seguridad, apagamos la imagen apenas carga el nivel
        if (_indicator2D != null)
            _indicator2D.SetActive(false);
    }

    private void Update()
    {
        // Si no hay Manager o no asignaste la imagen, no hacemos nada
        if (QuestManager.Instance == null || _indicator2D == null) return;

        // 2. Le preguntamos al Manager el estado actual de LA MISIÓN, no del ítem
        QuestStatus currentStatus = QuestManager.Instance.GetStatus(_questID);

        // 3. ¿Debería mostrarse la imagen flotante? (Solo si la quest está Activa)
        bool shouldShow = (currentStatus == QuestStatus.Active);

        // 4. Optimización de hardware: Solo ejecutamos SetActive si el estado necesita cambiar
        if (_indicator2D.activeSelf != shouldShow)
        {
            _indicator2D.SetActive(shouldShow);
        }
    }
}