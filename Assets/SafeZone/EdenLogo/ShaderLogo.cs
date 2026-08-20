using UnityEngine;
using UnityEngine.VFX;

class ShaderLogo : MonoBehaviour
{
    [SerializeField] Material _SafeLogoShader;
    [SerializeField] VisualEffect _SafeLogoParticles;


    // Start is Xcalled once before the first execution of Update after the MonoBehaviour is created

    void OnEnable()
    {
        WorldScanManager.Instance.ScanActive += EnableLogoVisuals;
        WorldScanManager.Instance.ScanDeactivate += DisableLogoVisuals;
    }   



    void OnDisable()
    {
       WorldScanManager.Instance.ScanActive -= EnableLogoVisuals;
       WorldScanManager.Instance.ScanDeactivate -= DisableLogoVisuals;

    }

    void Start()
    {
        _SafeLogoParticles.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void EnableLogoVisuals()
    {
        _SafeLogoShader.SetFloat("_EmissionMultiply",7f);
        _SafeLogoParticles.enabled = true;
        print ("7SAHDER");
    }

     public void DisableLogoVisuals()
    {
        _SafeLogoShader.SetFloat("_EmissionMultiply",1f);
        _SafeLogoParticles.enabled = false;
        print ("7SAHDERoff");
    }
}
