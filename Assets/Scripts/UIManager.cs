using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public PuzzleData puzzleData;

    public Image panelImage;
    public Text puzzleResolutionText;
    public Text imageResolutionText;
    public Text puzzleTypeText;
    public Dropdown pieceAmountDropdown;

    public GameObject lineObject;

    private List<GameObject> lines = new List<GameObject>();

    public void SetLinesOff() => lines.ForEach((x) => x.SetActive(false));
    public void SetLinesOn() => lines.ForEach((x) => x.SetActive(true));

    private void Start()
    {
        puzzleData = Resources.Load<PuzzleData>("Data/PuzzleData/PuzzleData");
    }

    public void AfterImageSelected(Texture2D texture)
    {
        int width = texture.width;
        int height = texture.height;

        int widthDecrease = width % 50;
        int heightDecrease = height % 50;

        int realWidth = texture.width - widthDecrease;
        int realHeight = texture.height - heightDecrease;

        Texture2D duplicatedTexture = Helper.DuplicateTexture(texture);

        ResizeSprite(duplicatedTexture, new Vector2(realWidth, realHeight));

        imageResolutionText.text = "Image Resolution: " + duplicatedTexture.width + " x " + duplicatedTexture.height;
        imageResolutionText.gameObject.SetActive(true);

        //find same divisors

        pieceAmountDropdown.ClearOptions();
        pieceAmountDropdown.onValueChanged.RemoveAllListeners();

        List<Helper.Divisor> divisors = Helper.FindPieceAmount(realWidth, realHeight);
        List<Dropdown.OptionData> optionDatas = new List<Dropdown.OptionData>();

        foreach (var i in divisors)
        {
            optionDatas.Add(new Dropdown.OptionData(i.pieceAmount.ToString()));
        }
        pieceAmountDropdown.AddOptions(optionDatas);

        pieceAmountDropdown.onValueChanged.AddListener((val) => {

            puzzleTypeText.text = "Puzzle Type: " + divisors[val].xyPieces.Item1 + " x " + divisors[val].xyPieces.Item2;
            puzzleResolutionText.text = "Puzzle Resolution: " + divisors[val].resolution.Item1 + " x " + divisors[val].resolution.Item2;

            ResizeSprite(duplicatedTexture, new Vector2(divisors[val].resolution.Item1, divisors[val].resolution.Item2));

            CreateLines(divisors[val].xyPieces.Item1, divisors[val].xyPieces.Item2, panelImage.GetComponent<RectTransform>().sizeDelta.x, panelImage.GetComponent<RectTransform>().sizeDelta.y);

            puzzleData.variables = new PuzzleData.Variables
            {
                pieceAmount = divisors[val].pieceAmount,
                resolution = new Vector2(divisors[val].resolution.Item1, divisors[val].resolution.Item2),
                type = new Vector2(divisors[val].xyPieces.Item1, divisors[val].xyPieces.Item2)
            };

            puzzleData.selectedImage = panelImage.sprite;

        }
        );
        pieceAmountDropdown.value = 0;
        pieceAmountDropdown.transform.parent.gameObject.SetActive(true);

        puzzleTypeText.text = "Puzzle Type: " + divisors[0].xyPieces.Item1 + " x " + divisors[0].xyPieces.Item2;
        puzzleTypeText.gameObject.SetActive(true);

        puzzleResolutionText.text = "Puzzle Resolution: " + divisors[0].resolution.Item1 + " x " + divisors[0].resolution.Item2;
        puzzleResolutionText.gameObject.SetActive(true);

        CreateLines(divisors[0].xyPieces.Item1, divisors[0].xyPieces.Item2, panelImage.GetComponent<RectTransform>().sizeDelta.x, panelImage.GetComponent<RectTransform>().sizeDelta.y);

        puzzleData.variables = new PuzzleData.Variables
        {
            pieceAmount = divisors[0].pieceAmount,
            resolution = new Vector2(divisors[0].resolution.Item1, divisors[0].resolution.Item2),
            type = new Vector2(divisors[0].xyPieces.Item1, divisors[0].xyPieces.Item2)
        };


    }

    public void ResizeSprite(Texture2D texture, Vector2 to)
    {
        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, to.x, to.y), Vector2.zero);

        float sizeWidth = ((float)to.x / (float)to.y) * 720f;
        float sizeHeight = sizeWidth > 1280 ? (1280 / sizeWidth) * 720f : 720f;

        panelImage.GetComponent<RectTransform>().sizeDelta = new Vector2(Mathf.Clamp(sizeWidth, 0, 1280), sizeHeight);

        panelImage.sprite = sprite;
    }

    public void CreateLines(int xLoop, int yLoop, float spriteX, float spriteY) //40x20 foto x = 39 y = 19 line amount
    {
        foreach(GameObject line in lines)
        {
            Destroy(line);
        }
        lines.Clear();

        Transform path = lineObject.transform.parent;

        for (int i = 0; i < xLoop - 1; i++)
        {
            if(xLoop % 2 == 0)
            {
                //çift
                float offset = (spriteX / xLoop) * (i - ((xLoop / 2) - 1));
                RectTransform line = Instantiate(lineObject, path).GetComponent<RectTransform>();
                line.anchoredPosition = new Vector2(line.anchoredPosition.x + offset, line.anchoredPosition.y);
                line.sizeDelta = new Vector2(4, panelImage.GetComponent<RectTransform>().sizeDelta.y + 50);

                lines.Add(line.gameObject);
            }
            else
            {
                //tek
                float offset = (spriteX / xLoop) * (((float)xLoop - 2 - 2 * (float)i) / 2);
                RectTransform line = Instantiate(lineObject, path).GetComponent<RectTransform>();
                line.anchoredPosition = new Vector2(line.anchoredPosition.x + offset, line.anchoredPosition.y);
                line.sizeDelta = new Vector2(4, panelImage.GetComponent<RectTransform>().sizeDelta.y + 50);

                lines.Add(line.gameObject);
            }
        }
        for (int i = 0; i < yLoop - 1; i++)
        {
            if (yLoop % 2 == 0)
            {
                //çift
                float offset = (spriteY / yLoop) * (i - ((yLoop / 2) - 1));
                RectTransform line = Instantiate(lineObject, path).GetComponent<RectTransform>();
                line.anchoredPosition = new Vector2(line.anchoredPosition.x, line.anchoredPosition.y + offset);
                line.sizeDelta = new Vector2(panelImage.GetComponent<RectTransform>().sizeDelta.x + 50, 4);

                lines.Add(line.gameObject);
            }
            else
            {
                //tek
                float offset = (spriteY / yLoop) * (((float)yLoop - 2 - 2 * (float)i) / 2);
                RectTransform line = Instantiate(lineObject, path).GetComponent<RectTransform>();
                line.anchoredPosition = new Vector2(line.anchoredPosition.x, line.anchoredPosition.y + offset);
                line.sizeDelta = new Vector2(panelImage.GetComponent<RectTransform>().sizeDelta.x + 50, 4);

                lines.Add(line.gameObject);
            }
        }

        SetLinesOn();
    }

}
