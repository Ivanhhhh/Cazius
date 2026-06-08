using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SFXManager : MonoBehaviour
{
    public static SFXManager Instance;

    //[SerializeField] AudioSource sfxMixerGroup;

    // SFX categories
    public enum SFXCategoryType
    {
        EnemyGruntSFX,
        PlayerShootingSFX,
        CriticalHitSFX,
        MaleHeySFX,
        RechargingGun,
        Heal,
        Cat,
        HoverButton,
        ClickButton
        RockCrumble
        // Add Categories here
    }

    // Pair each Enum with an SFXCategory
    [System.Serializable]
    public class SFXCategoryPair
    {
        public SFXCategoryType categoryType; // Enum
        public SFXCategory sfxCategory; // ScriptableObject
    }

    // List of SFXCategoryPair (Enum + ScriptableObject)
    public List<SFXCategoryPair> sfxCategories = new List<SFXCategoryPair>();

    private Dictionary<SFXCategoryType, SFXCategory> sfxDict;

    private AudioSource audioSource;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogWarning("AudioSource not found. Adding a new one.");
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // Build the Dictionary from SFXCategory pairs
        sfxDict = new Dictionary<SFXCategoryType, SFXCategory>();

        foreach (var pair in sfxCategories)
        {
            if (pair.sfxCategory != null && pair.sfxCategory.clips != null && pair.sfxCategory.clips.Count > 0)
            {
                sfxDict[pair.categoryType] = pair.sfxCategory;
            }
            else
            {
                Debug.LogWarning($"SFXManager: No clips assigned for category {pair.categoryType}");
            }
        }
    }

    public void PlaySFX(SFXCategoryType category)
    {
        PlayRandomClip(category, audioSource.volume);
    }

    public void PlaySFX(SFXCategoryType category, float volume)
    {
        PlayRandomClip(category, volume);
    }

    //public void PlaySFXAtPosition(SFXCategoryType category, Vector3 position)
    //{
    //    if (sfxDict.TryGetValue(category, out SFXCategory sfxCategory))
    //    {
    //        int randomIndex = Random.Range(0, sfxCategory.clips.Count);
    //        AudioClip clip = sfxCategory.clips[randomIndex];
    //        AudioSource.PlayClipAtPoint(clip, position);
    //    }
    //    else
    //    {
    //        Debug.LogWarning($"SFXManager: Sound effect category '{category}' not found!");
    //    }
    //}


    public void PlaySFXAtPosition(SFXCategoryType category, Vector3 position)
    {
        if (sfxDict.TryGetValue(category, out SFXCategory sfxCategory))
        {
            int randomIndex = Random.Range(0, sfxCategory.clips.Count);
            AudioClip clip = sfxCategory.clips[randomIndex];

            audioSource.transform.position = position;
            audioSource.PlayOneShot(clip);
        }
        else
        {
            Debug.LogWarning($"SFXManager: Sound effect category '{category}' not found!");
        }
    }

    private void PlayRandomClip(SFXCategoryType category, float volume)
    {
        if (sfxDict.TryGetValue(category, out SFXCategory sfxCategory))
        {
            int randomIndex = Random.Range(0, sfxCategory.clips.Count);
            audioSource.PlayOneShot(sfxCategory.clips[randomIndex], volume);
        }
        else
        {
            Debug.LogWarning($"SFXManager: Sound effect category '{category}' not found!");
        }
    }
}
