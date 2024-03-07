using System.Collections;
using System.Collections.Generic;
using PlanetGenerator2D;
using UnityEngine;

public static class FishColorReplacer
{
	public static Texture2D ReplaceFishColors(Texture2D tex, Color[] newColors)
	{
		Color[] mainColors = { Color.green, Color.red, Color.blue };
		Color[] textureColors = tex.GetPixels();
		
		for(int i = 0; i < textureColors.Length; i++)
		{
			if(textureColors[i].a == 0f)
			{
				continue;
			}
			string textureColorHex = ColorUtility.ToHtmlStringRGB(textureColors[i]);
			int index = -1;
			for (int j = 0; j < mainColors.Length; j++)
			{
				string mainColorHex = ColorUtility.ToHtmlStringRGB(mainColors[j]);
				if(textureColorHex == mainColorHex)
				{
					index = j;
					break;
				}
			}
			if(index == -1)
			{
				continue;
			}
			//Debug.Log(index);
			//Debug.Log(newColors.Length);
			textureColors[i] = newColors[index];
		}
		int width = tex.width;
		int height = tex.height;

		tex = TextureGenerator.TextureFromColorMap(textureColors, width, height);

		return tex;
	}
}
