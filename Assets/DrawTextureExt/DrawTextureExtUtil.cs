//------------------------------------
//              DrawTextureExt
//     Copyright © 2013-2015 White Dev
//------------------------------------

using UnityEngine;

namespace DrawTextureExt
{

public class TexRect {

	public Rect rect;
	public Rect texCoord;

	public float width {
		get { return rect.width; }
	}
	public float height {
		get { return rect.height; }
	}
	public float aspect {
		get { return _aspect; }
	}

	Rect _paramRect;
	float _aspect;

	void _TexRect(Rect rect, float texWidth, float texHeight, float scale) {
		_paramRect = rect;
		this.rect = rect;
		UpdateTexCoord(texWidth, texHeight);
		RectScale(scale);
		_aspect = rect.width / rect.height;
	}
	public TexRect(Rect rect, float texWidth, float texHeight, float scale=1.0f) {
		_TexRect(rect, texWidth, texHeight, scale);
	}
	public TexRect(Rect rect, Texture texture, float scale=1.0f) {
		_TexRect(rect, texture.width, texture.height, scale);
	}

	public void RectScale (float scale) {
		rect.width = Mathf.Round(_paramRect.width * scale);
		rect.height = Mathf.Round(_paramRect.height * scale);
	}

	public void UpdateTexCoord (float texWidth, float texHeight) {
		texCoord.x = (_paramRect.x / texWidth);
		texCoord.y = (1.0f - ((_paramRect.y + _paramRect.height) / texHeight));
		texCoord.width = (_paramRect.width / texWidth);
		texCoord.height = (_paramRect.height / texHeight);
	}
	public void UpdateTexCoord (Texture texture) {
		UpdateTexCoord(texture.width, texture.height);
	}
}


public class Util {
	
	static public int SearchEmptyLayer () {
		int layer = 0;
		for (int i=31; i>=8; --i) {
			if (string.IsNullOrEmpty(LayerMask.LayerToName(i))) {
				layer = i;
				break;
			}
		}
		return layer;
	}

}

}//namespace DrawTextureExt
