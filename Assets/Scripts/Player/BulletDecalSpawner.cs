using FactoryPool;
using UnityEngine;


public class BulletDecalSpawner : MonoBehaviour
{

   Pool<Decal> _pool;   // junto a los demás campos
[Header("Decal Prefabs")]
    [SerializeField] private GameObject _normalBulletDecalPrefab;
    [SerializeField] private GameObject _bloodyBulletDecalPrefab;

    [Header("Settings")]
    [SerializeField] private float _decalLifetime = 8f;
    [SerializeField] private float _surfaceOffset = 0.01f;
    [SerializeField] private Vector2 _randomSizeRange = new Vector2(0.08f, 0.14f);
    [SerializeField] private bool _randomRotation = true;

    public void SpawnNormalDecal(RaycastHit hit)
    {
        //_decalLifetime = 8f;
        //SpawnDecal(hit, _normalBulletDecalPrefab, null);
        Decal decal = DecalFactory.Instance.GetDecal();

        Vector3 spawnPosition = hit.point + hit.normal * _surfaceOffset;

        Quaternion spawnRotation = Quaternion.LookRotation(-hit.normal);

        decal.transform.position = spawnPosition;
        decal.transform.rotation = spawnRotation;
    }

    public void SpawnBloodyDecal(RaycastHit hit)
    {
        Transform boneParent = GetClosestBone(hit);
        _decalLifetime = 1.3f;

        SpawnDecal(hit, _bloodyBulletDecalPrefab, boneParent);
    }

    private void SpawnDecal(RaycastHit hit, GameObject decalPrefab, Transform parent)
    {
        if (decalPrefab == null)
            return;

        Vector3 spawnPosition = hit.point + hit.normal * _surfaceOffset;

        Quaternion spawnRotation = Quaternion.LookRotation(-hit.normal);

        if (_randomRotation)
        {
            spawnRotation *= Quaternion.Euler(0f, 0f, Random.Range(0f, 360f));
        }

        GameObject decal = Instantiate(
            decalPrefab,
           spawnPosition,
           spawnRotation
        );

        if (parent != null)
        {
            decal.transform.SetParent(parent, true);
        }

        float randomSize = Random.Range(_randomSizeRange.x, _randomSizeRange.y);
       decal.transform.localScale = Vector3.one * randomSize;

        Destroy(decal, _decalLifetime);
    }

    private Transform GetClosestBone(RaycastHit hit)
    {
        SkinnedMeshRenderer skinnedMesh = hit.collider.GetComponentInChildren<SkinnedMeshRenderer>();

        if (skinnedMesh == null)
        {
            skinnedMesh = hit.collider.GetComponentInParent<SkinnedMeshRenderer>();
        }

        if (skinnedMesh == null || skinnedMesh.bones == null || skinnedMesh.bones.Length == 0)
        {
            return hit.collider.transform;
        }

        Transform closestBone = null;
        float closestDistance = Mathf.Infinity;

        foreach (Transform bone in skinnedMesh.bones)
        {
            if (bone == null)
                continue;

            float distance = Vector3.Distance(hit.point, bone.position);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestBone = bone;
            }
        }

        return closestBone != null ? closestBone : hit.collider.transform;
    }
}