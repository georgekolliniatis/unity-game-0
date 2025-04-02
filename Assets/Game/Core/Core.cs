using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

namespace Assets.Game.Core
{
    static class Core
    {
        const string MENU_CUSTOM_ITEM_NAME = "Custom";
        const string GAME_DIRECTORY_PATH = "Assets/Game";
        const string GAME_DATA_DIRECTORY_PATH = GAME_DIRECTORY_PATH + "/Data";

        static AddAndRemoveRequest s_InstallUPMPackagesRequest;

        static KeyValuePair<Func<Dictionary<string, object>, object>, Dictionary<string, object>> s_OnInstallUPMPackagesCompletedSuccessCallbackFunction;

        static void OnInstallUPMPackagesCompleted()
        {
            if (s_InstallUPMPackagesRequest.IsCompleted)
            {
                if (s_InstallUPMPackagesRequest.Status == StatusCode.Success && !s_OnInstallUPMPackagesCompletedSuccessCallbackFunction.Equals(default))
                    s_OnInstallUPMPackagesCompletedSuccessCallbackFunction.Key(s_OnInstallUPMPackagesCompletedSuccessCallbackFunction.Value);

                s_OnInstallUPMPackagesCompletedSuccessCallbackFunction = default;

                EditorApplication.update -= OnInstallUPMPackagesCompleted;
            }
        }

        static void InstallUPMPackages()
        {
            s_InstallUPMPackagesRequest = Client.AddAndRemove(new[]
            {
                "com.unity.localization"
            });

            EditorApplication.update += OnInstallUPMPackagesCompleted;
        }

        static void ImportAssetPackages()
        {
            foreach (string assetPackagePath in new[]
            {
                "IgniteCoders/Textures MaterialsWater/Simple Water Shader URP.unitypackage"
            })
                AssetDatabase.ImportPackage($"{EditorPrefs.GetString("AssetStoreCacheRootPath_h2408304525")}/Asset Store-5.x/{assetPackagePath}", false);
        }
    }
}
