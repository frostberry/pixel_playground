using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PixelText : MonoBehaviour
{
    public Color color;
    public Sprite font;
    public int pixelSize;
    public int kerning;
    public int pixelsPerUnit;
    [TextArea(5, 20)]
    public string characters;

    [TextArea(15, 20)]
    public string text;

    public int spaceDistance;
    public int verticalSpacing;
    

    private Sprite[] spriteArray;
    private int[] widthArray;

    private void Start()
    {
        int nCharacters = Mathf.FloorToInt(font.texture.width / pixelsPerUnit);
        spriteArray = new Sprite[nCharacters];
        widthArray = new int[nCharacters];
        GenerateSpriteArray();

        SetText(text);
    }

    private void GenerateSpriteArray()
    {
        for (int i = 0; i < Mathf.FloorToInt(font.texture.width / pixelsPerUnit); i++)
        {
            Color[] pixels = font.texture.GetPixels(
                pixelsPerUnit * i, 0, pixelsPerUnit, pixelsPerUnit);

            Texture2D characterTexture = new Texture2D(pixelsPerUnit, pixelsPerUnit);
            characterTexture.SetPixels(pixels);
            characterTexture.filterMode = FilterMode.Point;
            characterTexture.Apply();
            Sprite characterSprite = Sprite.Create(
                characterTexture,
                new Rect(0, 0, pixelsPerUnit, pixelsPerUnit),
                new Vector2(0f, 0f),
                16f
            );

            widthArray[i] = GetCharacterWidth(pixels);
            spriteArray[i] = characterSprite;
        }
    }

    private int GetCharacterWidth(Color[] pixels)
    {
        int width = pixelSize;

        bool flag = false;
        for (int i = pixelSize - 1; i > -1; i--)
        {
            for (int j = 0; j < pixelSize; j++)
            {
                if (pixels[i + j * pixelSize].a > 0f)
                {
                    width = i + 1;
                    flag = true;
                    break;
                }
            }
            if (flag) break;
        }
        return width;
    }

    public void SetText(string text)
    {
        foreach (Transform character in transform)
        {
            Destroy(character);
        }

        Vector2 position = Vector2.zero;
        for (int i = 0; i < text.Length; i++)
        {
            if (text[i].Equals(' '))
            {
                position += Vector2.right * spaceDistance;
            }
            else if (text[i].Equals('\n'))
            {
                position = Vector2.up * (position.y - verticalSpacing);
            }
            else if (!characters.Contains(text[i].ToString()))
            {
                Debug.Log("Character missing: " + text[i]);
                break;
            }
            else
            {
                int index = characters.IndexOf(text[i]);
                GameObject newCharacter = new GameObject("Character_" + text[i]);
                SpriteRenderer sr = newCharacter.AddComponent<SpriteRenderer>();
                sr.sprite = spriteArray[index];
                sr.color = color;

                newCharacter.transform.position = transform.position +
                    Vector3.right * ((float)position.x / (float)pixelsPerUnit) +
                    Vector3.up * ((float)position.y / (float)pixelsPerUnit);
                newCharacter.transform.SetParent(transform);
                position += Vector2.right * (widthArray[index] + kerning);
            }
            
        }
    }
}
