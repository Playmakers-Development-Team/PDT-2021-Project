using UI.Core;
using UnityEngine;

namespace UI.Credits
{
    public class ScrollingText : DialogueComponent<CreditsDialogue>
    {
        [SerializeField] private float scrollSpeed;

        private new Camera camera;

        private float height;
        
        protected override void Subscribe() {}

        protected override void Unsubscribe() {}

        protected override void OnComponentStart()
        {
            base.OnComponentStart();

            camera = Camera.main;

            height = GetComponent<RectTransform>().rect.height;

            transform.position = new Vector3(camera.pixelWidth / 2.0f, -height / 2.0f, 0);
        }

        private void Update()
        {
            transform.Translate(Vector3.up * (scrollSpeed * Time.deltaTime));
            
            if (transform.position.y > camera.pixelHeight + height / 2.0f)
                manager.Pop();
        }
    }
}
