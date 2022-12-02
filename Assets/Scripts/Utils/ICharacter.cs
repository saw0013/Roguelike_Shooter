using Cosmo;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class OnActiveRagdoll : UnityEngine.Events.UnityEvent<Damage> { }
public interface ICharacter : IHealthController
{
    OnActiveRagdoll onActiveRagdoll { get; }
    Animator animator { get; }
    bool ragdolled { get; set; }
    void EnableRagdoll();
    void ResetRagdoll();
}
