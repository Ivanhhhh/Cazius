using UnityEngine;
public class Player_CameraRecoil : MonoBehaviour
{
    [Header("Recoil")]
    [SerializeField] private float _recoilX = 2f;
    [SerializeField] private float _recoilY = 1f;
    [SerializeField] private float _returnSpeed = 5f;
    [SerializeField] private float _recoilSpeed = 10f;

    private Vector3 _currentRotation;
    private Vector3 _targetRotation;
    public Vector3 CurrentRotation => _currentRotation;
    public System.Action OnRecoil; // ← el Action

    void Start()
    {
        OnRecoil += ApplyRecoil; // se suscribe a sí mismo
    }

    void Update()
    {
        _targetRotation = Vector3.Lerp(_targetRotation, Vector3.zero, _returnSpeed * Time.deltaTime);
        _currentRotation = Vector3.Lerp(_currentRotation, _targetRotation, _recoilSpeed * Time.deltaTime);
    }

    void ApplyRecoil()
    {
        Debug.Log("Recoil aplicado");
        _targetRotation += new Vector3(-_recoilX, Random.Range(-_recoilY, _recoilY), 0);
    }
}
