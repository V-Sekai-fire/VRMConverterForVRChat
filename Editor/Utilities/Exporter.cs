using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Diagnostics;
using UnityEngine;
using UnityEditor;
using UniGLTF;
using VRM;

namespace Esperecyan.Unity.VRMConverterForVRChat.Utilities
{
    internal class Exporter
    {
        private static readonly Regex FilePathPattern
            = new Regex(pattern: "^" + Regex.Escape(Converter.RootFolderPath) + @"/(?!Editor/Utilities/Exporter\.cs$)");

        private static readonly Regex ExcludedFilePathPatternInUniVRM = new Regex(pattern: @"/[^/]+-[^/]+\.shader$");

        private static readonly string PackageName = Converter.Name + "-" + Converter.Version;

        [MenuItem("Assets/Export " + Converter.Name, false, 30)]
        private static void Export()
        {
            string[] allAssetPathNames = AssetDatabase.GetAllAssetPaths();

            IEnumerable<string> assetPathNames
                = allAssetPathNames.Where(path => Exporter.FilePathPattern.IsMatch(input: path));
            
            IEnumerable<string> packagePaths = new[] { false, true }.Select(withUniVRM => {
                string name = Exporter.PackageName;

                if (withUniVRM) {
                    name += " + " + VRMVersion.VRM_VERSION;
                    assetPathNames = assetPathNames.Concat(
                        allAssetPathNames.Where(path => path.StartsWith("Assets/VRM/") && !Exporter.ExcludedFilePathPatternInUniVRM.IsMatch(input: path))
                    );
                }

                var packagePath = Path.Combine(Application.temporaryCachePath, name + ".unitypackage");
                AssetDatabase.ExportPackage(assetPathNames.ToArray(), packagePath);
                return packagePath;
            });

            Process.Start(fileName: "PowerShell", arguments: "-Command \"Compress-Archive"
                + " -Path @(" + string.Join(separator: ",", value: packagePaths.Select(path => "'" + path + "'").ToArray()) + ")"
                + " -DestinationPath '" + Path.Combine(Environment.GetFolderPath(folder: Environment.SpecialFolder.DesktopDirectory), Exporter.PackageName + ".zip") + "'\"")
                .WaitForExit();

            foreach (string packagePath in packagePaths) {
                File.Delete(path: packagePath);
            }
        }
    }
}