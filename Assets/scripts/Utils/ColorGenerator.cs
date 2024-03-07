using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ColorGenerator
{
    public static Color[] GetFishColors(bool rich = false, float highlightIntensity = 0.5f, float shadowIntensity = 0.5f)
	{
		if (rich)
		{
			Color[] colors = new Color[6];

			Color baseColor = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), 1);
			Color highlight = Color.Lerp(baseColor, Color.white, highlightIntensity);
			Color shadow = Color.Lerp(baseColor, Color.black, shadowIntensity);

			colors[0] = baseColor;
			colors[1] = highlight;
			colors[2] = shadow;

			baseColor = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), 1);
			highlight = Color.Lerp(baseColor, Color.white, highlightIntensity);
			shadow = Color.Lerp(baseColor, Color.black, shadowIntensity);

			colors[3] = baseColor;
			colors[4] = highlight;
			colors[5] = shadow;

			return colors;
		}
		else
		{
			Color[] colors = new Color[3];

			Color baseColor = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), 1);
			Color highlight = Color.Lerp(baseColor, Color.white, highlightIntensity);
			Color shadow = Color.Lerp(baseColor, Color.black, shadowIntensity);

			colors[0] = baseColor;
			colors[1] = highlight;
			colors[2] = shadow;

			return colors;
		}
		
	}
}
