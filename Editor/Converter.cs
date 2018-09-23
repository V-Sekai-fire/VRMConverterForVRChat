﻿using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using VRM;
using VRCSDK2;

namespace Esperecyan.Unity.VRMConverterForVRChat
{
    /// <summary>
    /// アバターの変換を行うパブリックAPI。
    /// </summary>
    public class Converter
    {
        /// <summary>
        /// <see cref="EditorGUILayout.HelpBox"/>で表示するメッセージ。
        /// </summary>
        public struct Message
        {
            public string message;
            public MessageType type;
        }

        /// <summary>
        /// 変換元のアバターのルートに設定されている必要があるコンポーネント。
        /// </summary>
        public static readonly Type[] RequiredComponents = { typeof(Animator), typeof(VRMMeta), typeof(VRMFirstPerson) };

        /// <summary>
        /// Hierarchy上のアバターをVRChatへアップロード可能な状態にします。
        /// </summary>
        /// <param name="avatar"><see cref="Converter.RequiredComponents"/>が設定されたインスタンス。</param>
        /// <param name="defaultAnimationSet"></param>
        /// <param name="swayingParametersConverter"></param>
        /// <param name="assetsPath">「Assets/」から始まるVRMプレハブのパス。</param>
        /// <returns>変換中に発生したメッセージ。</returns>
        public static IEnumerable<Converter.Message> Convert(
            GameObject avatar,
            VRC_AvatarDescriptor.AnimationSet defaultAnimationSet,
            ComponentsReplacer.SwayingParametersConverter swayingParametersConverter = null,
            string assetsPath = ""
        ) {
#pragma warning disable 618
            avatar.SetActiveRecursively(state: true); // GameObject.setActive() は子孫の有効・無効を切り替えない
#pragma warning restore 618
            IEnumerable<Converter.Message> messages = GeometryCorrector.Apply(avatar: avatar);
            BlendShapeReplacer.Apply(avatar: avatar, assetsPath: assetsPath);
            ComponentsReplacer.Apply(avatar: avatar, defaultAnimationSet: defaultAnimationSet, swayingParametersConverter: swayingParametersConverter);
            ComponentsRemover.Apply(avatar: avatar);
            return messages;
        }
    }
}
