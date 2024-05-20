using UnityEngine;

public class OpenWebsiteButton : MonoBehaviour
{
    public string url = "https://www.github.com";

    public void OpenWebsite()
    {
        Application.OpenURL(url);
    }
}
