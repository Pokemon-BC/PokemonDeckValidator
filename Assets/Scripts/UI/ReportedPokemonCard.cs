using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using PKMN.Cards;

public class ReportedPokemonCard : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Image cardImage;
    public Text quantity;
    public Text tempName;
    public Text tempId;
    public Image icon;

    private CardInDeck source;
    private string image;
    private List<FormatedNote> issues;
    private ReportedCardInfo rci;
    private bool mouseIn = false;

    public GameObject helpRoot;
    public Transform helpTextRoot;
    private bool initializedHelp = false;

    public GameObject textPrefab;

    //image, report.start.quantity, status
    public void Configure(CardInDeck rpc)
    {
        source = rpc;
        int status;
        image = "";
        if (rpc.reference.Supertype == CardSupertype.UNKNOWN)
        {
            status = 1;
        }
        else
        {
            status = 0;
            for(int i = 0, count = rpc.notes.Count; i < count; i++)
            {
                FormatedNote current = rpc.notes[i];
                if(current.severity == NoteType.INVALID)
                {
                    status = 2;
                    // Once one is invalid it doesn't matter if more are.
                    break;
                }
            }
            image = rpc.reference.Images.Small;
        }

        quantity.text = rpc.quantity.ToString();
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
        issues = rpc.notes;
    }

    private void Start()
    {
        helpRoot.SetActive(false);
        if (cardImage == null)
        {
            cardImage = GetComponent<Image>();
        }
        tempName.text = source.reference.Name;
        tempId.text = string.Format("{0} {1}", source.setId, source.reference.ID);
        if (image != "" || image == null)
        {
            StartCoroutine(DownloadImage(image));
            tempName.gameObject.SetActive(false);
            tempId.gameObject.SetActive(false);
        }
        else
        {
            tempName.gameObject.SetActive(true);
            tempId.gameObject.SetActive(true);
        }
        rci = helpRoot.GetComponent<ReportedCardInfo>();
        rci.controllerReference = this;
    }

    private IEnumerator DownloadImage(string mediaUrl)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(mediaUrl);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.DataProcessingError)
        {
            Debug.Log(request.error);
            tempName.gameObject.SetActive(true);
        }
        else
        {
            Texture2D cardTexture = DownloadHandlerTexture.GetContent(request);
            cardImage.sprite = Sprite.Create(cardTexture, new Rect(0, 0, cardTexture.width, cardTexture.height), new Vector2());
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        mouseIn = true;
        if(issues.Count != 0)
        {
            helpRoot.SetActive(true);
            if (!initializedHelp)
            {
                initializedHelp = true;
                for (int i = 0, count = issues.Count; i < count; i++)
                {
                    GameObject newText = GameObject.Instantiate(textPrefab);
                    newText.transform.SetParent(helpTextRoot, false);
                    newText.transform.localScale = Vector3.one;
                    if (newText.TryGetComponent<Text>(out Text actualText))
                    {
                        actualText.text = issues[i].text;
                        actualText.color = MapIssueToColor(issues[i].severity);
                    }
                }
            }
            if (helpRoot.transform.position.x > 1600)
            {
                helpRoot.transform.position = new Vector3(transform.position.x - 335f, transform.position.y);
            }
        }
    }

    private Color MapIssueToColor(NoteType nt)
    {
        switch (nt)
        {
            case NoteType.NOTE: 
                return Color.white;
            case NoteType.ERROR:
                return Color.yellow;
            case NoteType.INVALID:
                return Color.red;
            default:
                return Color.red;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        mouseIn = false;
        if(!rci.mouseOver)
        {
            helpRoot.SetActive(false);
        }
    }

    public void ReportPointerMove(bool pointerIn)
    {
        if(!mouseIn && !pointerIn)
        {
            helpRoot.SetActive(false);
        }
    }
}
