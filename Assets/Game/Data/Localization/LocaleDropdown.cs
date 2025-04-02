using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Assets.Game.Data.Localization
{
    [RequireComponent(typeof(TMP_Dropdown))]

    public class LocaleDropdown : MonoBehaviour
    {
        TMP_Dropdown m_LocaleDropdown;

        void OnLocaleChangedFromUnity(Locale locale)
        {
            m_LocaleDropdown.SetValueWithoutNotify(LocalizationSettings.AvailableLocales.Locales.IndexOf(locale));
        }

        void OnLocaleChangedFromGame(int localeIndex)
        {
            LocalizationSettings.SelectedLocaleChanged -= OnLocaleChangedFromUnity;

            LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[localeIndex];

            LocalizationSettings.SelectedLocaleChanged += OnLocaleChangedFromUnity;
        }

        IEnumerator InitializeLocaleDropdown()
        {
            m_LocaleDropdown = GetComponent<TMP_Dropdown>();

            m_LocaleDropdown.interactable = false;

            m_LocaleDropdown.ClearOptions();
            m_LocaleDropdown.options.Add(new TMP_Dropdown.OptionData("Loading..."));

            yield return LocalizationSettings.InitializationOperation;

            List<Locale> locales = LocalizationSettings.AvailableLocales.Locales;
            Locale selectedLocale = LocalizationSettings.SelectedLocale;
            int selectedLocaleIndex = -1;
            List<TMP_Dropdown.OptionData> options = new();
            int localesCount = locales.Count;

            for (int i = 0; i < localesCount; i++)
            {
                Locale locale = locales[i];

                if (locale == selectedLocale)
                    selectedLocaleIndex = i;
                
                AsyncOperationHandle<Sprite> asyncGetLocaleImageHandle = LocalizationSettings.AssetDatabase.GetLocalizedAssetAsync<Sprite>("AssetTableCollection_0", "LocaleImage", locale);
                yield return asyncGetLocaleImageHandle;

                CultureInfo cultureInfo = locale.Identifier.CultureInfo;
                options.Add(new TMP_Dropdown.OptionData(cultureInfo != null ? cultureInfo.NativeName : locale.name, asyncGetLocaleImageHandle.Result));
            }

            if (options.Count == 0)
                options.Add(new TMP_Dropdown.OptionData("No locales available"));
            
            m_LocaleDropdown.ClearOptions();
            m_LocaleDropdown.AddOptions(options);

            if (selectedLocaleIndex >= 0)
                m_LocaleDropdown.SetValueWithoutNotify(selectedLocaleIndex);
            
            LocalizationSettings.SelectedLocaleChanged += OnLocaleChangedFromUnity;

            m_LocaleDropdown.onValueChanged.AddListener(OnLocaleChangedFromGame);

            m_LocaleDropdown.interactable = true;
        }

        void Start()
        {
            StartCoroutine(InitializeLocaleDropdown());
        }
    }
}
