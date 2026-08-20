using UnityEngine;

public class ShaderLogo : MonoBehaviour
{
    public Material _SafeLogoShader;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

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
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void EnableLogoVisuals()
    {
        _SafeLogoShader.SetFloat("_EmissionMultiply",7f);
        print ("7SAHDER");
    }

     public void DisableLogoVisuals()
    {
        _SafeLogoShader.SetFloat("_EmissionMultiply",1f);
        print ("7SAHDERoff");
    }
}
