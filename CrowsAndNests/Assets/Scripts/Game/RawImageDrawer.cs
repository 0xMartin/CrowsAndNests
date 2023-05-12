using UnityEngine;
using UnityEngine.UI;

namespace Game
{

    /// <summary>
    /// Staticka trida ulehcujici praci s vykreslovanim obrazku
    /// </summary>
    public static class RawImageDrawer
    {

        /// <summary>
        /// Vykresli texturu do RawImage Unity komponenty
        /// </summary>
        /// <param name="rawImage">Reference na RawImage komponentu</param>
        /// <param name="texture">2D Textura</param>
        public static void DrawTexture(RawImage rawImage, Texture2D texture)
        {
            rawImage.texture = texture;
            rawImage.rectTransform.sizeDelta = new Vector2(texture.width, texture.height);
        }

        /// <summary>
        /// Vytvori prazdnou texturu o pozadovanych parametrech
        /// </summary>
        /// <param name="width">Sirka v pixelech</param>
        /// <param name="height">Vyska v pixelech</param>
        /// <param name="color">Barva pozadi</param>
        /// <param name="transparentBackground">Transparentnost pozadni</param>
        /// <returns>Vygenerovana 2D Textura</returns>
        public static Texture2D CreateTexture(int width, int height, Color color, bool transparentBackground = false)
        {
            Texture2D texture = new Texture2D(width, height);

            Color[] pixels = new Color[width * height];
            for (int i = 0; i < pixels.Length; i++)
            {
                pixels[i] = color;
            }
            texture.SetPixels(pixels);

            if (transparentBackground)
            {
                Color transparentColor = new Color(0f, 0f, 0f, 0f);
                for (int i = 0; i < width; i++)
                {
                    texture.SetPixel(i, 0, transparentColor);
                    texture.SetPixel(i, height - 1, transparentColor);
                }
                for (int i = 0; i < height; i++)
                {
                    texture.SetPixel(0, i, transparentColor);
                    texture.SetPixel(width - 1, i, transparentColor);
                }
            }

            texture.Apply();

            return texture;
        }


        /// <summary>
        /// Vykresli elipsu
        /// </summary>
        /// <param name="texture">Reference na 2D texturu</param>
        /// <param name="position">Pozice elipsy</param>
        /// <param name="width">Sirka elipsy</param>
        /// <param name="height">Vyska elipsy</param>
        /// <param name="color">Barva elipsy</param>
        public static void DrawEllipse(Texture2D  texture, Vector2 position, float width, float height, Color color)
        {
            if (texture == null)
            {
                Debug.LogError("RawImage does not have a valid texture.");
                return;
            }

            int centerX = Mathf.RoundToInt(position.x);
            int centerY = Mathf.RoundToInt(position.y);

            int minX = Mathf.RoundToInt(centerX - width / 2f);
            int maxX = Mathf.RoundToInt(centerX + width / 2f);
            int minY = Mathf.RoundToInt(centerY - height / 2f);
            int maxY = Mathf.RoundToInt(centerY + height / 2f);

            minX = Mathf.Max(minX, 0);
            minY = Mathf.Max(minY, 0);
            maxX = Mathf.Min(maxX, texture.width);
            maxY = Mathf.Min(maxY, texture.height);

            for (int y = minY; y < maxY; y++)
            {
                for (int x = minX; x < maxX; x++)
                {
                    float dx = (float)(x - centerX) / (width / 2f);
                    float dy = (float)(y - centerY) / (height / 2f);
                    if (dx * dx + dy * dy <= 1f)
                    {
                        texture.SetPixel(x, y, color);
                    }
                }
            }

            texture.Apply();
        }

        /// <summary>
        /// Vykresli obdelnik
        /// </summary>
        /// <param name="texture">Reference na 2D texturu</param>
        /// <param name="rect">Paramety obdelniku</param>
        /// <param name="color">Barva obdelniku</param>
        public static void DrawRectangle(Texture2D texture, Rect rect, Color color)
        {
            int x = (int) rect.x;
            int y = (int) rect.y;
            int width = (int) rect.width;
            int height = (int) rect.height;

            Color[] colors = new Color[width * height];
            for (int i = 0; i < colors.Length; i++)
            {
                colors[i] = color;
            }

            texture.SetPixels(x, y, width, height, colors);
            texture.Apply();
        }

    }

}