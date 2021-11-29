using System.IO;
using TenetStatuses;
using UnityEditor;
using UnityEngine;

namespace UI
{
    public class Settings : ScriptableObject
    {
        [SerializeField] private Sprite attackIcon;
        [SerializeField] private Sprite defenceIcon;
        
        [SerializeField] private Sprite passionIcon;
        [SerializeField] private Sprite apathyIcon;
        [SerializeField] private Sprite prideIcon;
        [SerializeField] private Sprite humilityIcon;
        [SerializeField] private Sprite joyIcon;
        [SerializeField] private Sprite sorrowIcon;
        [SerializeField] private Sprite neutralIcon;
        
        [SerializeField, ColorUsage(true, true)] private Color attackColour;
        [SerializeField, ColorUsage(true, true)] private Color defenceColour;
        
        [SerializeField, ColorUsage(true, true)] private Color passionColour;
        [SerializeField, ColorUsage(true, true)] private Color apathyColour;
        [SerializeField, ColorUsage(true, true)] private Color prideColour;
        [SerializeField, ColorUsage(true, true)] private Color humilityColour;
        [SerializeField, ColorUsage(true, true)] private Color joyColour;
        [SerializeField, ColorUsage(true, true)] private Color sorrowColour;
        [SerializeField, ColorUsage(true, true)] private Color neutralColor;
        
        
        #region Singleton
        
        private const string instanceDirectory = "UI/Settings/";
        private const string instanceFileName = "settingsInstance";

        private static Settings instance;

        public static Settings Instance
        {
            get
            {
                if (instance)
                    return instance;

                if (instance = Resources.Load<Settings>(instanceDirectory + instanceFileName))
                    return instance;
                
#if UNITY_EDITOR
                instance = CreateInstance<Settings>();
                Directory.CreateDirectory(Application.dataPath + "/Resources/" + instanceDirectory);
                AssetDatabase.CreateAsset(instance,
                    "Assets/Resources/" + instanceDirectory + instanceFileName + ".asset");
#endif

                return instance;
            }
        }
        
        #endregion

        
        public static Sprite AttackIcon => Instance.attackIcon;
        
        public static Sprite DefenceIcon => Instance.defenceIcon;

        public static Sprite GetTenetIcon(TenetType tenet)
        {
            return tenet switch
            {
                TenetType.Pride => Instance.prideIcon,
                TenetType.Humility => Instance.humilityIcon,
                TenetType.Passion => Instance.passionIcon,
                TenetType.Apathy => Instance.apathyIcon,
                TenetType.Joy => Instance.joyIcon,
                TenetType.Sorrow => Instance.sorrowIcon,
                TenetType.Neutral => Instance.neutralIcon,
                _ => null
            };
        }
        
        public static Color AttackColor => Instance.attackColour;
        
        public static Color DefenceColor => Instance.defenceColour;
        
        public static Color GetTenetColour(TenetType tenet)
        {
            return tenet switch
            {
                TenetType.Pride => Instance.prideColour,
                TenetType.Humility => Instance.humilityColour,
                TenetType.Passion => Instance.passionColour,
                TenetType.Apathy => Instance.apathyColour,
                TenetType.Joy => Instance.joyColour,
                TenetType.Sorrow => Instance.sorrowColour,
                TenetType.Neutral => Instance.neutralColor,
                _ => Color.black
            };
        }
    }
}
