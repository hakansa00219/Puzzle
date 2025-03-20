using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public static class Helper
{
    public static float FindMaxXValuedVector(Vector3[] vectors)
    {
        Vector3 maxValue = new Vector3(0,0);
        foreach(Vector3 v in vectors)
        {
            maxValue = v.x > maxValue.x ? v : maxValue;
        }
        return maxValue.x;
    }
    public static float FindMaxYValuedVector(Vector3[] vectors)
    {
        Vector3 maxValue = new Vector3(0, 0);
        foreach (Vector3 v in vectors)
        {
            maxValue = v.y > maxValue.y ? v : maxValue;
        }
        return maxValue.y;
    }
    public static Vector2[] CalculateUVsFromVertices(Vector3[] vertices,int puzzleSize, int pieceIndex, int width, int height)
    {
        Vector2[] uvs = new Vector2[vertices.Length];

        int xResolution = width / puzzleSize; // xRes = 2000 / 50 = 40
        int yResolution = height / puzzleSize; // yRes = 1000 / 50 = 20
        int xAmount = pieceIndex % xResolution == 0 ? 0 : xResolution - (pieceIndex % xResolution); // xAmount = 800 % 40 = 0, 40 - (800 % 40)
        int yAmount = Mathf.FloorToInt((pieceIndex - 1) / xResolution); //yAmount = 799 / 40  = 19 

        int currentXX = xAmount * puzzleSize;// 0 * 50  = 0
        int currentXY = yAmount * puzzleSize;// 19 * 50 = 950

        for (int i = 0; i < uvs.Length; i++)
        {
            uvs[i] = ConvertPixelsToUVCoordinates(currentXX + vertices[i].x, currentXY + vertices[i].y, width, height);//(0 + x, 950 + y, 2000,1000)
        }
        return uvs;
    }
    public static Vector2 ConvertPixelsToUVCoordinates(float x,float y,int textureWidth, int textureHeight)
    {
        return new Vector2((float)x / textureWidth, (float)y / textureHeight);
    }
    public static float Remap(float from, float fromMin, float fromMax, float toMin, float toMax)
    {
        var fromAbs = from - fromMin;
        var fromMaxAbs = fromMax - fromMin;

        var normal = fromAbs / fromMaxAbs;

        var toMaxAbs = toMax - toMin;
        var toAbs = toMaxAbs * normal;

        var to = toAbs + toMin;

        return to;
    }
    public static int[] FindAllDivisors(int n)
    {
        List<int> divisors = new List<int>();

        if (n < 2)
        {
            return null;
        }
        else if (IsPrime(n))
        {
            return null;
        }
        else
        {
            for (int i = 2; i < n; i++)
                if (n % i == 0)
                    divisors.Add(i);
        }

        return divisors.ToArray();
    }
    public static bool IsPrime(int n)
    {
        if (n == 2) return true;
        if (n % 2 == 0) return false;

        for (int x = 3; x * x <= n; x += 2)
            if (n % x == 0)
                return false;

        return true;
    }
    public static List<Divisor> FindPieceAmount(int x,int y)
    {
        List<Divisor> divisors = new List<Divisor>();
        int divide = PuzzleGenerator.PIECE_MINIMUM_SIZE;
        while (true)
        {
            int xPieces = x >= divide ? x / divide : 0;
            int yPieces = y >= divide ? y / divide : 0;

            if (xPieces == 0 || yPieces == 0) return divisors;

            if(xPieces != 1 && yPieces != 1)
            {
                if (divisors.Find(x => x.pieceAmount == xPieces * yPieces) == null)
                {
                    divisors.Add(new Divisor() { pieceAmount = xPieces * yPieces, xyPieces = (xPieces, yPieces), resolution = (xPieces * divide, yPieces * divide) });
                }
                else
                {
                    if (divisors[divisors.Count - 1].resolution.Item1 < xPieces * divide)
                    {
                        divisors.RemoveAt(divisors.Count - 1);
                        divisors.Add(new Divisor() { pieceAmount = xPieces * yPieces, xyPieces = (xPieces, yPieces), resolution = (xPieces * divide, yPieces * divide) });
                    }
                }
            }
            divide += PuzzleGenerator.PIECE_MINIMUM_SIZE;
        }
        
    }
    public class Divisor
    {
        public int pieceAmount;
        public (int, int) xyPieces;
        public (int, int) resolution;
    }

    public static Texture2D ConvertToTexture(this Sprite sprite)
    {
        if (sprite.rect.width != sprite.texture.width)
        {
            Texture2D newText = new Texture2D((int)sprite.rect.width, (int)sprite.rect.height);
            Color[] newColors = sprite.texture.GetPixels((int)sprite.textureRect.x,
                                                         (int)sprite.textureRect.y,
                                                         (int)sprite.textureRect.width,
                                                         (int)sprite.textureRect.height);
            newText.SetPixels(newColors);
            newText.Apply();
            return newText;
        }
        else
            return sprite.texture;
    }
    public static Texture2D DuplicateTexture(Texture2D source)
    {
        RenderTexture renderTex = RenderTexture.GetTemporary(
                    source.width,
                    source.height,
                    0,
                    RenderTextureFormat.Default,
                    RenderTextureReadWrite.Linear);

        Graphics.Blit(source, renderTex);
        RenderTexture previous = RenderTexture.active;
        RenderTexture.active = renderTex;
        Texture2D readableText = new Texture2D(source.width, source.height);
        readableText.ReadPixels(new Rect(0, 0, renderTex.width, renderTex.height), 0, 0);
        readableText.Apply();
        RenderTexture.active = previous;
        RenderTexture.ReleaseTemporary(renderTex);
        return readableText;
    }
}