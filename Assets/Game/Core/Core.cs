using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

namespace Assets.Game.Core
{
    static class Core
    {
        const string CUSTOM_MENU_PATH = "Custom";
        const string REFRESH_GAME_MENU_ITEM_PATH = CUSTOM_MENU_PATH + "/Refresh Game";
        const string ATTACH_MAIN_MODULE_MENU_ITEM_PATH = CUSTOM_MENU_PATH + "/Attach Main Module of Game";
        const string DETACH_MAIN_MODULE_MENU_ITEM_PATH = CUSTOM_MENU_PATH + "/Detach Main Module of Game";

        const string GAME_DIRECTORY_PATH = "Assets/Game";
        const string GAME_DATA_DIRECTORY_PATH = GAME_DIRECTORY_PATH + "/Data";

        const string ADDRESSABLE_ASSETS_DATA_DIRECTORY_PATH = "Assets/AddressableAssetsData";

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

        static SerializedProperty GetProjectSetting(string settingAssetPath, object[] settingKeyParts, out SerializedObject settingAsset, out SerializedProperty nextSetting)
        {
            settingAsset = new(AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(settingAssetPath));
            nextSetting = null;

            SerializedProperty setting = settingAsset.FindProperty((string)settingKeyParts[0]);

            for (int i = 1; i < settingKeyParts.Length; i++)
            {
                object settingKeyPart = settingKeyParts[i];

                if (settingKeyPart is string settingRelativeKey)
                    setting = setting.FindPropertyRelative(settingRelativeKey);
                else if (settingKeyPart is int settingRelativeArrayIndex)
                {
                    if (settingRelativeArrayIndex < 0)
                    {
                        setting.InsertArrayElementAtIndex(setting.arraySize);
                        setting = setting.GetArrayElementAtIndex(setting.arraySize - 1);
                    }
                    else
                    {
                        setting.arraySize = setting.arraySize < settingRelativeArrayIndex ? settingRelativeArrayIndex : setting.arraySize;
                        setting.InsertArrayElementAtIndex(settingRelativeArrayIndex);
                        setting = setting.GetArrayElementAtIndex(settingRelativeArrayIndex);

                        nextSetting = setting.GetArrayElementAtIndex(settingRelativeArrayIndex + 1);
                    }
                }
            }

            return setting;
        }

        static void SetProjectSetting(string settingAssetPath, object[] settingKeyParts, object settingValue, bool insertAndReplace = false)
        {
            SerializedProperty setting = GetProjectSetting(settingAssetPath, settingKeyParts, out SerializedObject settingAsset, out SerializedProperty nextSetting);

            if (settingValue == null)
                setting.DeleteCommand();

            if (insertAndReplace && nextSetting != null)
                nextSetting.DeleteCommand();

            if (setting != null)
                setting.boxedValue = settingValue;

            settingAsset.ApplyModifiedProperties();
        }

        static void DefineProjectSettings()
        {
            PlayerSettings.SplashScreen.animationMode = PlayerSettings.SplashScreen.AnimationMode.Static;
            PlayerSettings.SplashScreen.drawMode = PlayerSettings.SplashScreen.DrawMode.AllSequential;

            PlayerSettings.SplashScreenLogo[] splashScreenLogos = new PlayerSettings.SplashScreenLogo[2];
            splashScreenLogos[0] = PlayerSettings.SplashScreenLogo.CreateWithUnityLogo();
            splashScreenLogos[1] = PlayerSettings.SplashScreenLogo.Create(6.0f, AssetDatabase.LoadAssetAtPath<Sprite>($"{GAME_DATA_DIRECTORY_PATH}/SplashScreen/NoticeLogo/El/SplashScreenNoticeLogoEl.png"));
            PlayerSettings.SplashScreen.logos = splashScreenLogos;
        }

        static void DirectoryToZipFile(string directoryPath, string zipFilePath = null, bool deleteDirectory = false)
        {
            zipFilePath = zipFilePath ?? directoryPath;
            zipFilePath = zipFilePath.EndsWith(".zip", StringComparison.OrdinalIgnoreCase) ? zipFilePath : $"{zipFilePath}.zip";

            if (Directory.Exists(directoryPath))
            {
                ZipFile.CreateFromDirectory(directoryPath, zipFilePath);

                if (deleteDirectory)
                    Directory.Delete(directoryPath, true);
            }
        }

        static void ZipFileToDirectory(string zipFilePath, string directoryPath = null, bool deleteZipFile = false)
        {
            directoryPath = directoryPath ?? zipFilePath;
            directoryPath = directoryPath.EndsWith(".zip", StringComparison.OrdinalIgnoreCase) ? directoryPath.Substring(0, directoryPath.Length - 4) : directoryPath;

            if (File.Exists(zipFilePath))
            {
                ZipFile.ExtractToDirectory(zipFilePath, directoryPath);

                if (deleteZipFile)
                    File.Delete(zipFilePath);
            }
        }

        [MenuItem(REFRESH_GAME_MENU_ITEM_PATH, true)]
        static bool RefreshGameValidation()
        {
            return Directory.GetFileSystemEntries(GAME_DATA_DIRECTORY_PATH).Length > 0;
        }

        [MenuItem(REFRESH_GAME_MENU_ITEM_PATH, false, 0)]
        static void RefreshGameAction()
        {
            s_OnInstallUPMPackagesCompletedSuccessCallbackFunction = new((Dictionary<string, object> arguments) =>
            {
                ImportAssetPackages();
                DefineProjectSettings();

                return null;
            }, new());

            InstallUPMPackages();
        }

        [MenuItem(ATTACH_MAIN_MODULE_MENU_ITEM_PATH, true)]
        static bool AttachMainModuleValidation()
        {
            return !RefreshGameValidation();
        }

        [MenuItem(ATTACH_MAIN_MODULE_MENU_ITEM_PATH, false, 1)]
        static void AttachMainModuleAction()
        {
            ZipFileToDirectory(GAME_DATA_DIRECTORY_PATH, deleteZipFile: true);
            ZipFileToDirectory(ADDRESSABLE_ASSETS_DATA_DIRECTORY_PATH, deleteZipFile: true);

            if (RefreshGameValidation())
                RefreshGameAction();
        }

        [MenuItem(DETACH_MAIN_MODULE_MENU_ITEM_PATH, true)]
        static bool DetachMainModuleValidation()
        {
            return RefreshGameValidation();
        }

        [MenuItem(DETACH_MAIN_MODULE_MENU_ITEM_PATH, false, 2)]
        static void DetachMainModuleAction()
        {
            DirectoryToZipFile(ADDRESSABLE_ASSETS_DATA_DIRECTORY_PATH, deleteDirectory: true);
            DirectoryToZipFile(GAME_DATA_DIRECTORY_PATH, deleteDirectory: true);
        }
    }
}
