using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;  // Make sure to add this for TextMeshPro

public class DynamicScrollView : MonoBehaviour
{
    public GameObject contentPanel;
    public GameObject imagePrefab;
    public GameObject contentDetails;  // The GameObject that holds the content details UI
    public TextMeshProUGUI descriptionText;  // TextMeshProUGUI for description
    public TextMeshProUGUI nameText;  // Text for name
    public Button closeButton;  // Button to close the details
    public TextAsset jsonFile;
    public float scrollViewWidth = 1200f;
    public float imageHeight = 200f;
    public float contentPadding = 200f;

    [Serializable]
    public class ItemData
    {
        public string name;
        public string description;
    }

    [Serializable]
    public class ItemDataArray
    {
        public ItemData[] items;
    }

    void Start()
    {
        ItemDataArray itemDataArray = JsonUtility.FromJson<ItemDataArray>(jsonFile.text);
        ItemData[] items = itemDataArray.items;

        int columns = 4;
        int rows = Mathf.CeilToInt(items.Length / (float)columns);

        float spacing = imageHeight / 5;

        RectTransform contentRect = contentPanel.GetComponent<RectTransform>();
        contentRect.sizeDelta = new Vector2(contentRect.sizeDelta.x, rows * (imageHeight + spacing) + contentPadding);

        int index = 0;
        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < columns; c++)
            {
                if (index >= items.Length) break;

                GameObject imageObject = Instantiate(imagePrefab, contentPanel.transform, false);
                RectTransform imageRect = imageObject.GetComponent<RectTransform>();

                float x = (c - columns / 2.0f + 0.5f) * (imageHeight + spacing);
                float y = -(r * (imageHeight + spacing) + imageHeight / 2);
                // float y = -(r * (imageHeight + spacing) + imageHeight);

                imageRect.anchoredPosition = new Vector2(x, y);

                Button button = imageObject.GetComponent<Button>();
                int itemIndex = index;
                button.onClick.AddListener(() => ShowDetails(items[itemIndex]));

                index++;
            }
        }

        // Add a listener to the close button to disable the contentDetails GameObject
        closeButton.onClick.AddListener(() => contentDetails.SetActive(false));
    }

    void ShowDetails(ItemData item)
    {
        // Set the details
        descriptionText.text = item.description;
        nameText.text = item.name;

        // Enable the content details GameObject
        contentDetails.SetActive(true);
    }
}
