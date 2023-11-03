using UnityEngine;
using UnityEngine.UI;

public class ContentDetails : MonoBehaviour
{
    public Text nameText;
    public Text descriptionText;

    public void SetDetails(string name, string description)
    {
        nameText.text = name;
        descriptionText.text = description;
    }
}
