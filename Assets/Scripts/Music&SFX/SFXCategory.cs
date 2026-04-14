using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SFXCategory", menuName = "Audio/SFXCategory")]
public class SFXCategory : ScriptableObject
{
    public string categoryName;  // Category name (JumpSFX, AttackSFX, etc.)
    public List<AudioClip> clips;  // Clips for that Category
}
