using Cinemachine.Editor;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public partial class MenuComponents
{
    [MenuItem("Tools/Cosmoground/Resources/New Ragdoll Generic Template")]
    static void RagdollGenericTemplate()
    {
        ScriptableObjectUtility.CreateAsset<RagdollGenericTemplate>();
    }
}
