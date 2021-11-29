using UnityEngine;

namespace UI.MainMenu.ExitConfirmation
{
    // TODO: Not technically inheriting from the right class.
    public class ExitGameButtonComponent : MainMenuButton
    {
        #region UIComponent

        protected override void Subscribe() {}

        protected override void Unsubscribe() {}

        #endregion

        #region ButtonHandling

        protected override void OnSelected()
        {
            Application.Quit();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }

        #endregion
    }
}
