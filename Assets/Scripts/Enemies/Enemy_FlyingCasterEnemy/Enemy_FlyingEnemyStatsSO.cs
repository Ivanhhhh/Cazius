using UnityEngine;

[CreateAssetMenu(fileName = "NewFlyingEnemyStats", menuName = "Enemy AI/Flying Stats")]
public class FlyingEnemyStatsSO : ScriptableObject
{
    [Header("Chasing & Attacking")]
    public float chaseSpeed = 6f;
    public float rotationSpeed = 5f;
    public float attackRange = 10f;
    public float shootCooldown = 2.5f;
    public LayerMask playerLayer;

    [Header("Advanced Movement")]
    public float aimOffset = 1f;
    public float hoverHeight = 4f;
    public float retreatMargin = 2f; 
    public float normalReactionSpeed = 5f; 
    public float evasionReactionSpeed = 15f; 
    public float heightCorrectionMultiplier = 2f; 
    
    [Header("Random Wander")]
    public float minWanderTime = 1.5f;
    public float maxWanderTime = 3f;
    [Range(0f, 2f)] public float wanderStrafeLimit = 1f;
    [Range(0f, 2f)] public float wanderForwardLimit = 0.5f;

    [Header("Visual Debugging (Rays)")]
    public bool showTargetingRay = true;     
    public bool showWanderRay = true;        
    public bool showIdealMovementRay = true; 
    public bool showAvoidanceRay = true;     
    public bool showFinalDirectionRay = true;
    public bool showVelocityRay = true;
}