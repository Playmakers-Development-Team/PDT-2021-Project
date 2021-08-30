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
        
        [SerializeField, ColorUsage(true, true)] private Color attackColour;
        [SerializeField, ColorUsage(true, true)] private Color defenceColour;
        
        [SerializeField, ColorUsage(true, true)] private Color passionColour;
        [SerializeField, ColorUsage(true, true)] private Color apathyColour;
        [SerializeField, ColorUsage(true, true)] private Color prideColour;
        [SerializeField, ColorUsage(true, true)] private Color humilityColour;
        [SerializeField, ColorUsage(true, true)] private Color joyColour;
        [SerializeField, ColorUsage(true, true)] private Color sorrowColour;
        
        
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

        
        public static Sprite AttackIcon => instance.attackIcon;
        
        public static Sprite DefenceIcon => instance.defenceIcon;

        public static Sprite GetTenetIcon(TenetType tenet)
        {
            return tenet switch
            {
                TenetType.Pride => instance.prideIcon,
                TenetType.Humility => instance.humilityIcon,
                TenetType.Passion => instance.passionIcon,
                TenetType.Apathy => instance.apathyIcon,
                TenetType.Joy => instance.joyIcon,
                TenetType.Sorrow => instance.sorrowIcon,
                _ => null
            };
        }
        
        public static Color AttackColor => instance.attackColour;
        
        public static Color DefenceColor => instance.defenceColour;
        
        public static Color GetTenetColour(TenetType tenet)
        {
            return tenet switch
            {
                TenetType.Pride => instance.prideColour,
                TenetType.Humility => instance.humilityColour,
                TenetType.Passion => instance.passionColour,
                TenetType.Apathy => instance.apathyColour,
                TenetType.Joy => instance.joyColour,
                TenetType.Sorrow => instance.sorrowColour,
                _ => Color.black
            };
        }
    }
}
