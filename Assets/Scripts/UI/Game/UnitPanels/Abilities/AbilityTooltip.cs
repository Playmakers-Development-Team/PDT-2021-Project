using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Abilities;
using TMPro;
using UI.Core;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Game.UnitPanels.Abilities
{
    public class AbilityTooltip : DialogueComponent<GameDialogue>
    {
        [SerializeField] private AbilityList abilityList;
        [SerializeField] private GameObject tooltipPanel;
        [SerializeField] private GameObject keywordPanel;
        [SerializeField] private GameObject speedPanel;
        [SerializeField] private TextMeshProUGUI tooltipDescription;
        [SerializeField] private TextMeshProUGUI keywordDescription;
        [SerializeField] private TextMeshProUGUI speedDescription;
        [SerializeField] private Image shapeIcon;

        [Header("Tooltip Sprites")]
        [SerializeField] private string spriteAssetPrefix;
        [SerializeField] private TMP_SpriteAsset spriteAsset;

        [Header("Configurable")]
        [SerializeField] private bool descriptiveIcons;
        [SerializeField] private bool showAbilitySpeed = true;
        [SerializeField] private bool showAverageAbilitySpeed;
        [SerializeField] private bool showKeywords = true;

        protected override void OnComponentAwake()
        {
            // Just include every darn text okay
            foreach (var textComponent in GetComponentsInChildren<TextMeshProUGUI>())
                textComponent.spriteAsset = spriteAsset;
            
            tooltipPanel.gameObject.SetActive(false);
            keywordPanel.gameObject.SetActive(false);
        }

        protected override void Subscribe()
        {
            dialogue.abilityHoverEnter.AddListener(OnAbilityHoverEnter);
            dialogue.abilityHoverExit.AddListener(OnAbilityHoverExit);
        }

        protected override void Unsubscribe()
        {
            dialogue.abilityHoverEnter.RemoveListener(OnAbilityHoverEnter);
            dialogue.abilityHoverExit.RemoveListener(OnAbilityHoverExit);
        }

        #region Listeners

        private void OnAbilityHoverEnter(AbilityCard card)
        {
            if (abilityList != null && !abilityList.Cards.Contains(card))
                return;

            DrawAbility(card.Ability);
        }

        private void OnAbilityHoverExit(AbilityCard card)
        {
            if (abilityList == null || !abilityList.Cards.Contains(card))
                return;

            HideAbilities();
        }
        
        #endregion

        internal void DrawAbility(Ability ability)
        {
            tooltipPanel.SetActive(true);
            tooltipDescription.text = PrettyAbilityDescription(ability);

            keywordPanel.SetActive(showKeywords && ability.AllKeywords.Any());
            keywordDescription.text = string.Empty;

            // Shape icons
            Sprite shapeSprite = ability.Shape.DisplayIcon;
            shapeIcon.gameObject.SetActive(shapeSprite != null);
            
            if (ability.Shape.DisplayIcon != null)
                shapeIcon.sprite = shapeSprite;
            
            // Ability speed
            speedPanel.SetActive(showAbilitySpeed && (ability.SpeedType != AbilitySpeedType.Average || showAverageAbilitySpeed));
            speedDescription.text = ability.SpeedType.DisplayName();

            foreach (Keyword keyword in ability.AllKeywords)
                keywordDescription.text += PrettyKeywordDescription(keyword);
        }

        internal void HideAbilities()
        {
            tooltipPanel.SetActive(false);
            keywordPanel.SetActive(false);
        }

        private string PrettyKeywordDescription(Keyword keyword)
        {
            StringBuilder descriptionBuilder = new StringBuilder(keyword.Description);
            IntegrateIcons(descriptionBuilder);
            return $"{PrettyKeywordTitle(keyword)}: {descriptionBuilder}\n\n";
        }

        private string PrettyKeywordTitle(Keyword keyword) =>
            $"<i><uppercase>{keyword.DisplayName}</uppercase></i>";

        private string PrettyAbilityDescription(Ability ability)
        {
            StringBuilder descriptionBuilder = new StringBuilder(ability.Description);
            IntegrateIcons(descriptionBuilder);
            string s = descriptionBuilder.ToString();
            IntegrateKeywords(ref s, ability.AllKeywords);
            return s;
        }

        private void IntegrateKeywords(ref string s, IEnumerable<Keyword> keywords)
        {
            foreach (Keyword keyword in keywords)
            {
                string replacement = PrettyKeywordTitle(keyword);
                s = Regex.Replace(s, keyword.DisplayName, replacement, RegexOptions.IgnoreCase);
            }
        }

        private void IntegrateIcons(StringBuilder builder)
        {
            // NOTE: This entire code can actually be converted and simplified using Regex. If someone wants to do it, you have my thanks.
            
            builder.Replace(" attack", ShowIcon("attack"));
            builder.Replace(" Attack", ShowIcon("attack"));
            if (descriptiveIcons)
                builder.Replace(ShowIcon("attack"), ShowIcon("attack") + " Attack");

            builder.Replace(" defence", ShowIcon("defence"));
            builder.Replace(" Defence", ShowIcon("defence"));
            if (descriptiveIcons)
                builder.Replace(ShowIcon("defence"), ShowIcon("defence") + " Defence");

            builder.Replace(" pride", ShowIcon("pride"));
            builder.Replace(" Pride", ShowIcon("pride"));
            if (descriptiveIcons)
                builder.Replace(ShowIcon("pride"), ShowIcon("pride") + " Pride");

            builder.Replace(" humility", ShowIcon("humility"));
            builder.Replace(" Humility", ShowIcon("humility"));
            if (descriptiveIcons)
                builder.Replace(ShowIcon("humility"), ShowIcon("humility") + " Humility");

            builder.Replace(" passion", ShowIcon("passion"));
            builder.Replace(" Passion", ShowIcon("passion"));
            if (descriptiveIcons)
                builder.Replace(ShowIcon("passion"), ShowIcon("passion") + " Passion");

            builder.Replace(" apathy", ShowIcon("apathy"));
            builder.Replace(" Apathy", ShowIcon("apathy"));
            if (descriptiveIcons)
                builder.Replace(ShowIcon("apathy"), ShowIcon("apathy") + " Apathy");

            builder.Replace(" joy", ShowIcon("joy"));
            builder.Replace(" Joy", ShowIcon("joy"));
            if (descriptiveIcons)
                builder.Replace(ShowIcon("joy"), ShowIcon("joy") + " Joy");

            builder.Replace(" sorrow", ShowIcon("sorrow"));
            builder.Replace(" Sorrow", ShowIcon("sorrow"));
            if (descriptiveIcons)
                builder.Replace(ShowIcon("sorrow"), ShowIcon("sorrow") + " Sorrow");
        }

        private string ShowIcon(string name) => $"<sprite name={spriteAssetPrefix}{name}>";
    }
}
