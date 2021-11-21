using System;
using System.Linq;
using System.Text;
using Abilities;
using TenetStatuses;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Editor
{
    public class AbilityListToolWindow : EditorWindow
    {
        private Label abilityCountText;

        private readonly string[] defaultPathFilter = new []
        {
            "Abilities/Demo Experiments",
            "Abilities/Joy",
            "Abilities/Humility",
            "Abilities/Passion",
            "Abilities/Sorrow",
            "Abilities/Apathy",
            "Abilities/Pride",
        };
        private string[] pathFilter;

        private Label abilityFilteredCountText;

        public AbilityListToolWindow()
        {
            pathFilter = defaultPathFilter.ToArray();
        }

        [MenuItem("Window/Ability List Tool")]
        private static void ShowWindow()
        {
            var window = GetWindow<AbilityListToolWindow>();
            window.minSize = new Vector2(500, 560);
            window.titleContent = new GUIContent("Ability List Tool");
            window.Show();
        }

        private void CreateGUI()
        {
            VisualElement container = new VisualElement()
            {
                style =
                {
                    marginBottom = 10,
                    marginLeft = 10,
                    marginRight = 10,
                    marginTop = 10
                }
            };

            ScrollView scrollView = new ScrollView(ScrollViewMode.Vertical)
            {
                style =
                {
                    maxHeight = Length.Percent(55)
                }
            };
            
            Label title = new Label(
                "You can use this tool to copy all the information about abilities and put it in a google doc somewhere"
            )
            {
                style = { fontSize = 15, unityFontStyleAndWeight = FontStyle.Bold, whiteSpace = WhiteSpace.Normal } 
            };

            TextField generatedText = new TextField()
            {
                multiline = true,
                style =
                {
                    minHeight = 100
                }
            };

            VisualElement buttons = new VisualElement()
            {
                style =
                {
                    marginBottom = 5,
                    marginLeft = 5,
                    marginRight = 5,
                    marginTop = 5,
                    flexDirection = FlexDirection.Row,
                    minHeight = 20
                }
            };

            Button generateButton = new Button(() =>
            {
                generatedText.value = GenerateDetails(true);
            })
            {
                text = "Preview"
            };
            Button clipboardButton = new Button(() =>
            {
                generatedText.value = GenerateDetails(true);
                CopyDetailsToClipboard(GenerateDetails());
            })
            {
                text = "Generate to Clipboard"
            };
            Button clearButton = new Button(() =>
            {
                generatedText.value = string.Empty;
            })
            {
                text = "Clear"
            };

            buttons.Add(generateButton);
            buttons.Add(clipboardButton);
            buttons.Add(clearButton);


            container.Add(title);
            container.Add(CreateFilterElement());
            container.Add(CreateInfoElement());
            container.Add(buttons);
            scrollView.Add(generatedText);
            container.Add(scrollView);
            rootVisualElement.Add(container);
        }

        private VisualElement CreateInfoElement()
        {
            VisualElement visualElement = new VisualElement();
            
            abilityCountText = new Label();
            abilityFilteredCountText = new Label();
            SetAbilityCount(0);
            SetFilteredAbilityCount(0);
            
            visualElement.Add(abilityCountText);
            visualElement.Add(abilityFilteredCountText);
            return visualElement;
        }

        private VisualElement CreateFilterElement()
        {
            VisualElement visualElement = new VisualElement()
            {
                style =
                {
                    marginBottom = 10,
                    marginLeft = 10,
                    marginRight = 10,
                    marginTop = 10,
                    minHeight = 120,
                }
            };

            Label subtitle = new Label("Folder/Directory Path Filter")
            {
                style = { fontSize = 15 }
            };

            TextField filterText = new TextField()
            {
                multiline = true,
                value = string.Join(Environment.NewLine, pathFilter)
            };
            filterText.RegisterValueChangedCallback((changeEvent) => 
                pathFilter = changeEvent.newValue
                    .Split(new string[] { Environment.NewLine}, StringSplitOptions.None)
                    .Where(s => !string.IsNullOrEmpty(s) && !string.IsNullOrWhiteSpace(s))
                    .ToArray());
            
            Button resetButton = new Button(() =>
            {
                pathFilter = defaultPathFilter.ToArray();
                filterText.value = string.Join(Environment.NewLine, pathFilter);
            })
            {
                text = "Reset Path Filter"
            };
            
            visualElement.Add(subtitle);
            visualElement.Add(resetButton);
            visualElement.Add(filterText);
            return visualElement;
        }

        private void SetAbilityCount(int count)
        {
            if (abilityCountText == null)
                return;

            abilityCountText.text = $"Found {count} Abilities";
        }
        
        private void SetFilteredAbilityCount(int count)
        {
            if (abilityCountText == null)
                return;

            abilityFilteredCountText.text = $"Filtered {count} Abilities";
        }

        public string GenerateDetails(bool trimWhenLong = false)
        {
            const string divider = ">============================================<";
            const string bigDivider = ")#@#@#@#@#@#@#@#@#@#@#@#@#@#@#@#@#@#@#@#@#@#@#@#@#(";
            
            StringBuilder stringBuilder = new StringBuilder();
            var abilityGuids = AssetDatabase.FindAssets($"t:{nameof(Ability)}");
            
            var abilityPaths = abilityGuids
                .Select(AssetDatabase.GUIDToAssetPath)
                .ToArray();
            SetAbilityCount(abilityPaths.Count());

            var filteredAbilityPaths = abilityPaths
                .Where(path => pathFilter.Any(path.Contains))
                .ToArray();
            SetFilteredAbilityCount(filteredAbilityPaths.Count());

            var abilities = filteredAbilityPaths
                .Select(AssetDatabase.LoadAssetAtPath<Ability>);

            var orderedAbilities = abilities
                .OrderBy(a => a.RepresentedTenet);

            TenetType lastTenet = TenetType.Pride;
            stringBuilder.AppendLine(divider);

            foreach (Ability ability in orderedAbilities)
            {
                if (ability.RepresentedTenet != lastTenet)
                {
                    stringBuilder.AppendLine();
                    stringBuilder.AppendLine();
                    stringBuilder.AppendLine();
                    stringBuilder.AppendLine(bigDivider);
                    stringBuilder.AppendLine();
                    stringBuilder.AppendLine();
                    stringBuilder.AppendLine();
                    lastTenet = ability.RepresentedTenet;
                    stringBuilder.AppendLine(divider);
                }
                
                stringBuilder.Append(ability.name);
                stringBuilder.Append(" (");
                stringBuilder.Append(ability.RepresentedTenet);
                stringBuilder.Append(")");
                stringBuilder.AppendLine();
                
                stringBuilder.AppendLine(ability.Description);
                stringBuilder.AppendLine(divider);
            }

            const int characterLimit = 11000;
            string text = stringBuilder.ToString();
            return trimWhenLong && text.Length > characterLimit 
                ? text.Substring(0, characterLimit) + "......"
                : text;
        }

        public void CopyDetailsToClipboard(string text)
        {
            GUIUtility.systemCopyBuffer = text;
        }
    }
}
