﻿using UnityEngine;
using VRCSDK2;

namespace Esperecyan.Unity.VRMConverterForVRChat
{
    /// <summary>
    /// <see cref="AvatarValidation.RemoveIllegalComponents"/>が動作しないため、その代替。
    /// </summary>
    internal class ComponentsRemover
    {
        internal static void Apply(GameObject avatar)
        {
            foreach (Component component in AvatarValidation.FindIllegalComponents(Name: avatar.GetComponent<VRC_AvatarDescriptor>().Name, currentAvatar: avatar)) {
                Object.DestroyImmediate(obj: component);
            }
        }
    }
}