using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;


public class DynamicScrollView : MonoBehaviour
{
    public GameObject contentPanel;
    public GameObject imagePrefab;
    public GameObject contentDetails;
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI nameText;
    public Button closeButton;
    public TextAsset jsonFile;
    public float scrollViewWidth = 1200f;
    public float imageHeight = 200f;
    public float contentPadding = 200f;

    [Serializable]
    public class ItemData
    {
        public string name;
        public string description;
        public string imageUrl; // Add this field for image URLs
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
        // contentRect.sizeDelta = new Vector2(contentRect.sizeDelta.x, rows * (imageHeight + spacing) + contentPadding);

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

                imageRect.anchoredPosition = new Vector2(x, y);

                // Start coroutine to load image from URL
                StartCoroutine(SetImageFromUrl(imageObject, items[index].imageUrl));

                Button button = imageObject.GetComponent<Button>();
                int itemIndex = index;
                button.onClick.AddListener(() => ShowDetails(items[itemIndex]));

                index++;
            }
        }

        closeButton.onClick.AddListener(() => contentDetails.SetActive(false));
    }

    private IEnumerator SetImageFromUrl(GameObject imageObject, string imageUrl)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(imageUrl);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Texture2D texture = DownloadHandlerTexture.GetContent(request);
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            Image imageComponent = imageObject.GetComponent<Image>();
            imageComponent.sprite = sprite;
        }
        else
        {
            Debug.LogError("Error downloading image: " + request.error);
        }
    }

    void ShowDetails(ItemData item)
    {
        descriptionText.text = item.description;
        nameText.text = item.name;
        contentDetails.SetActive(true);
    }
}
