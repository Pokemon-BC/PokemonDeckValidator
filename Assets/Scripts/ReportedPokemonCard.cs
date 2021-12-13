using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ReportedPokemonCard : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Image cardImage;
    public Text quantity;
    public Image icon;

    private string image;
    private List<string> issues;

    public GameObject helpRoot;
    public Transform helpTextRoot;
    private bool initializedHelp = false;

    public GameObject textPrefab;

    //image, report.start.quantity, status
    public void Configure(CardReported rpc)
    {
        int status;
        image = "";
        if (rpc.start.reference == null)
        {
            status = 1;
        }
        else
        {
            status = rpc.issues.Count == 0 ? 0 : 2;
            image = rpc.start.reference.images.small;
        }

        quantity.text = rpc.start.quantity.ToString();
        if(status == 0)
        {
            icon.color = Color.green;
        }
        else if(status == 1)
        {
            icon.color = Color.yellow;
        }
        else
        {
            icon.color = Color.red;
        }
        issues = rpc.issues;
    }

    private void Start()
    {
        helpRoot.SetActive(false);
        if (cardImage == null)
        {
            cardImage = GetComponent<Image>();
        }
        if (image != "")
        {
            Debug.Log("Image not empty, downloading");
            StartCoroutine(DownloadImage(image));
        }
        else
        {
            Debug.Log("No image available, skip");
        }
    }

    private IEnumerator DownloadImage(string MediaUrl)
    {
        Debug.Log("Downloading image with url " + MediaUrl);
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(MediaUrl);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.DataProcessingError)
        {
            Debug.Log(request.error);
        }
        else
        {
            Texture2D cardTexture = DownloadHandlerTexture.GetContent(request);
            Debug.Log("Downloaded Image with width = " + cardTexture.width + ", height = " + cardTexture.height);
            cardImage.sprite = Sprite.Create(cardTexture, new Rect(0, 0, cardTexture.width, cardTexture.height), new Vector2());
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("Pointer Enter");
        if(issues.Count != 0)
        {
            helpRoot.SetActive(true);
            if (!initializedHelp)
            {
                initializedHelp = true;
                for (int i = 0, count = issues.Count; i < count; i++)
                {
                    GameObject newText = GameObject.Instantiate(textPrefab);
                    newText.transform.SetParent(helpTextRoot);
                    if (newText.TryGetComponent<Text>(out Text actualText))
                    {
                        actualText.text = issues[i];
                    }
                }
            }
            if (helpRoot.transform.position.x > 1600)
            {
                helpRoot.transform.position = new Vector3(transform.position.x - 335f, transform.position.y);
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("Pointer exit");
        helpRoot.SetActive(false);
    }
}
