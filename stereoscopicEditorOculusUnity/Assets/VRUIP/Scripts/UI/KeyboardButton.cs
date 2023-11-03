using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace VRUIP
{
    public class KeyboardButton : A_ColorController
    {
        [Header("Button Type")]
        [SerializeField] private KeyboardButtonType buttonType;

        [Header("Button Properties")]
        [SerializeField] private string character;
        [SerializeField] private Keyboard keyboard;

        [Header("Button Components")]
        [SerializeField] private Image buttonBackground;
        [SerializeField] private TextMeshProUGUI buttonText;

        public string Character => character;
        public KeyboardButtonType Type => buttonType;

        private void Awake()
        {
            // Check if 'keyboard' is null
            if (keyboard == null)
            {
                // Instantiate a cube to indicate 'keyboard' is null
                GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cube.transform.position = new Vector3(0, 1, 0);

                // Assign a default keyboard
                // You might have a singleton or another way to get a default keyboard instance
                keyboard = FindObjectOfType<Keyboard>();  // For demonstration, find the first Keyboard component in the scene
            }

            // Add button click listener
            GetComponent<Button>().onClick.AddListener(OnClicked);
        }


        /// <summary>
        /// On this button clicked.
        /// </summary>
        private void OnClicked()
        {
            keyboard.ButtonPressed(this);
        }

        public enum KeyboardButtonType
        {
            Character,
            Space,
            Enter,
            Tab,
            Delete,
            Shift,
            Caps,
            Custom,
            Clear
        }

        /// <summary>
        /// Set the parent keyboard that this button belongs to.
        /// </summary>
        /// <param name="parentKeyboard"></param>
        public void SetKeyboard(Keyboard parentKeyboard)
        {
            keyboard = parentKeyboard;
        }

        protected override void SetColors(ColorTheme theme)
        {
            buttonBackground.color = theme.thirdColor;
            buttonText.color = theme.secondaryColor;
        }
    }
}
