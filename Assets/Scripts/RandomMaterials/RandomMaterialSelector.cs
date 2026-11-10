
using System.Collections.Generic;
using UnityEngine;

public class RandomMaterialSelector : MonoBehaviour
{
    [System.Serializable]
    public class MaterialConfiguration
    {
        public Renderer renderer;
        [Min(0)] public int materialIndex;
        public List<Material> possibleMaterials = new();
    }

    [SerializeField] private bool _randomizeOnStart = true;

    [SerializeField] private List<MaterialConfiguration> _materialConfigurations = new();

    private void Start()
    {
        if (_randomizeOnStart)
            RandomizeMaterials();
    }

    public void RandomizeMaterials()
    {
        foreach (MaterialConfiguration config in _materialConfigurations)
        {
            if (config.renderer == null || config.possibleMaterials == null || config.possibleMaterials.Count == 0)
                continue;

            Material[] materials = config.renderer.sharedMaterials;

            if (config.materialIndex < 0 || config.materialIndex >= materials.Length)
                continue;

            Material randomMaterial = config.possibleMaterials[Random.Range(0, config.possibleMaterials.Count)];

            if (randomMaterial == null)
                continue;

            materials[config.materialIndex] = randomMaterial;
            config.renderer.sharedMaterials = materials;
        }
    }
}
