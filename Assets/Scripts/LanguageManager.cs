using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

public class LanguageManager : MonoBehaviour
{
    public void SetEnglishLanguage()
    {
        SetLanguage("en");
    }

    public void SetRussianLanguage()
    {
        SetLanguage("ru");
    }

    public void SetSpanishLanguage()
    {
        SetLanguage("es");
    }

    private void SetLanguage(string languageCode)
    {
        var locale = LocalizationSettings.AvailableLocales.GetLocale(languageCode);
        
        if (locale != null)
        {
            LocalizationSettings.SelectedLocale = locale;
            PlayerPrefs.SetString("SelectedLanguage", languageCode);
            Debug.Log($"Language changed to {locale.name}");
        }
        else
        {
            Debug.LogError($"Locale for language {languageCode} not found!");
        }
    }

    private void Start()
    {
        if (PlayerPrefs.HasKey("SelectedLanguage"))
        {
            SetLanguage(PlayerPrefs.GetString("SelectedLanguage"));
        }
        else
        {
            var defaultLocale = LocalizationSettings.SelectedLocale;
            PlayerPrefs.SetString("SelectedLanguage", defaultLocale.Identifier.Code);
        }
    }
}