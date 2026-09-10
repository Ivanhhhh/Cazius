using UnityEngine;
using UnityEngine.VFX;
using System.Collections;


class ShaderLogo : MonoBehaviour
{
    [SerializeField] GameObject _GameObjectWShader;

    [SerializeField] GameObject _GameObjectWithParticles;



     Material _SafeLogoShader;
     VisualEffect _SafeLogoParticles;

    
    void OnEnable()
    {
        StartCoroutine(SubscribeWhenReady());
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
        _GameObjectWShader.SetActive(false);
         _SafeLogoShader = _GameObjectWShader.GetComponent<Renderer>().material;            
           
         _SafeLogoParticles = _GameObjectWithParticles.GetComponent<VisualEffect>();    
         
          _SafeLogoParticles.enabled = false;
    }

    public void EnableLogoVisuals()
    {
        _SafeLogoShader.SetFloat("_EmissionMultiply",7f);
        _SafeLogoParticles.enabled = true;
        _GameObjectWShader.SetActive(true);
        print ("7SAHDER");
    }

     public void DisableLogoVisuals()
    {
        _SafeLogoShader.SetFloat("_EmissionMultiply",0f);
        _SafeLogoParticles.enabled = false;
        _GameObjectWShader.SetActive(false);

        print ("7SAHDERoff");
    }


    private IEnumerator SubscribeWhenReady()
    {
        while (WorldScanManager.Instance == null)
            yield return null; // espera un frame y reintenta

        WorldScanManager.Instance.ScanActive += EnableLogoVisuals;
        WorldScanManager.Instance.ScanDeactivate += DisableLogoVisuals;

        if (WorldScanManager.Instance != null) print ("Suscrito");
    }
}
