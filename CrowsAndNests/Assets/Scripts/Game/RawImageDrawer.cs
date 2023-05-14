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
        /// <returns>Vygenerovana 2D Textura</returns>
        public static Texture2D CreateTexture(int width, int height, Color color)
        {
            Texture2D texture = new Texture2D(width, height);

            Color[] pixels = new Color[width * height];
            for (int i = 0; i < pixels.Length; i++)
            {
                pixels[i] = color;
            }
            texture.SetPixels(pixels);
            texture.Apply(true);

            return texture;
        }

        /// <summary>
        /// Vykresli elipsu
        /// </summary>
        /// <param name="texture">Reference na 2D texturu</param>
        /// <param name="position">Pozice elipsy</param>
        /// <param name="width">Sirka elipsy</param>
        /// <param name="height">Vyska elipsy</param>
        /// <param name="colorTop">Barva v horni casti</param>
        /// <param name="colorBottom">Barva ve spodni casti</param>
        public static void DrawEllipse(Texture2D  texture, Vector2 position, 
                                       float width, float height, Color colorTop, Color colorBottom)
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
                        texture.SetPixel(x, y, Gradient(colorBottom, minY, colorTop, maxY, y));
                    }
                }
            }

            texture.Apply(true);
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
            texture.Apply(true);
        }

        /// <summary>
        /// Navrati tmavejsi barvu
        /// </summary>
        /// <param name="c">Puvodni barva</param>
        /// <param name="factor">Faktor ztmaveni barvy</param>
        /// <returns></returns>
        public static Color Darker(Color color, float factor) {
            return Color.Lerp(color, Color.black, factor);
        }

        /// <summary>
        /// Vypocita barvu pixelu v 1D gradientu
        /// </summary>
        /// <param name="start">Pocatecni barva</param>
        /// <param name="x_start">Pozice pocatecni barvy</param>
        /// <param name="end">Koncova barva</param>
        /// <param name="x_end">Pozice koncove barvy</param>
        /// <param name="x">Pozadovany pixel</param>
        /// <returns>Vysledna barva pixelu v gradientu</returns>
        public static Color Gradient(Color start, float x_start, Color end, float x_end, float x) {
            if (x <= x_start) {
                return start;
            } else if (x >= x_end) {
                return end;
            } else {
                float t = (x - x_start) / (x_end - x_start);
                return Color.Lerp(start, end, t);
            }
        }

    }

}