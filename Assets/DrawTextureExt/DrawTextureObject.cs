//------------------------------------
//              DrawTextureExt
//     Copyright © 2013-2015 White Dev
//------------------------------------

using UnityEngine;
using System;
using System.Collections;

namespace DrawTextureExt
{

public enum AlignMode {
	LeftTop,
	CenterTop,
	RightTop,
	LeftCenter,
	Center,
	RightCenter,
	LeftBottom,
	CenterBottom,
	RightBottom
};

// Interface of DrawTextureObject
public abstract class Interface : MonoBehaviour {

	public Color color = Color.white;
	public const float z = 0.0f;		// Depth is not support

	// Alignments
	public AlignMode alignMode = AlignMode.LeftTop;

	// DrawTexture
	public abstract void DrawTexture (Rect position);
	public abstract void DrawTexture (Rect position, float colorAlpha);		// Color = color.rgb, colorAlpha.  (Without DrawTextureExt.color changes)
	public abstract void DrawTexture (Rect position, Color _color);				// Color = _color.                          (Without DrawTextureExt.color changes)
	public abstract void DrawTexture (Rect position, Rect texCoords, float colorAlpha, AlignMode alignMode);
	public abstract void DrawTexture (Rect position, Rect texCoords, Color _color, AlignMode alignMode);

	// DrawTextureWithTexCoords
	public abstract void DrawTexture (Rect position, Rect texCoords);
	public abstract void DrawTexture (Rect position, Rect texCoords, float colorAlpha);		// Color = color.rgb, colorAlpha.  (Without DrawTextureExt.color changes)
	public abstract void DrawTexture (Rect position, Rect texCoords, Color _color);			// Color = _color.                          (Without DrawTextureExt.color changes)

	// DrawTexture with TexRect
	public abstract void DrawTexture (TexRect texRect);
	public abstract void DrawTexture (TexRect texRect, float colorAlpha);	// Color = color.rgb, colorAlpha.  (Without DrawTextureExt.color changes)
	public abstract void DrawTexture (TexRect texRect, Color _color);			// Color = _color.                          (Without DrawTextureExt.color changes)
	public abstract void DrawTexture (float x, float y, TexRect texRect);
	public abstract void DrawTexture (float x, float y, TexRect texRect, float colorAlpha);	// Color = color.rgb, colorAlpha.  (Without DrawTextureExt.color changes)
	public abstract void DrawTexture (float x, float y, TexRect texRect, Color _color);			// Color = _color.                          (Without DrawTextureExt.color changes)
	public abstract void DrawTexture (Vector2 position, TexRect texRect);
	public abstract void DrawTexture (Vector2 position, TexRect texRect, float colorAlpha);	// Color = color.rgb, colorAlpha.  (Without DrawTextureExt.color changes)
	public abstract void DrawTexture (Vector2 position, TexRect texRect, Color _color);			// Color = _color.                          (Without DrawTextureExt.color changes)

	// DrawTextureWithTexCoords with Rotate & Scale (**** Support for AlignMode.Center only)
	public abstract void DrawTexture (Rect position, Rect texCoords, Color _color, float scale, float rotation);

	// Draw Number
	public abstract void DrawNumber (Rect position, Rect numberTexCoords, long number);
	public abstract void DrawNumber (Rect position, Rect numberTexCoords, long number, float padding);

	// Draw Text
	public abstract void DrawText (Rect position, Rect textTexCoords, string text);
	public abstract void DrawText (Rect position, Rect textTexCoords, string text, Vector2 padding);
}


//
public class DrawMgr : Interface {
	
	Material material;
//	Vector2 texST;
	
	bool forScreen;
	int maxCount;
	int count;
	bool updated;

	static int _orthoLayer = -1;
	static int orthoLayer {
		get {
			if (_orthoLayer == -1) {
				int layer = DrawTextureExt.Util.SearchEmptyLayer();
				if (layer == 0) {
					Debug.LogError("DrawTextureExt needs an empty Layer!");
					layer = 31;  // Last Layer
				}
				_orthoLayer = layer;
			}
			return _orthoLayer;
		}
	}
	
	
	// Mesh
	MeshFilter meshFilter;
	MeshRenderer meshRenderer;
	Mesh mesh;

	[System.Serializable]
	class MeshData {
		public Vector3[] vertices;
		public Vector3[] normals;
		public Vector2[] texCoords;
		public Color32[] colors;
		public int[] indices;
		public bool haveNormals;
	}
	MeshData meshData = new MeshData();


	static Rect _rect = new Rect();	
	//static Color whiteColor = Color.white;
	static Rect fullTexCoord = new Rect(0,0,1,1);
	static Vector3 facedNormal = new Vector3(0,0,-1);
	
	
	// Camera
	//	[System.Serializable]
	struct MyOrthoCamera {
		public GameObject gameObject;
		public Camera camera;
	}
	static MyOrthoCamera orthoCamera;
	
	static GameObject GetOrthoCamera () {
		if (orthoCamera.gameObject == null) {
			// Layer
			foreach (Camera camera in Camera.allCameras) {
				camera.cullingMask &= ~(1 << orthoLayer);
			}
			//
			orthoCamera.gameObject = new GameObject("2D Camera");
			orthoCamera.camera = orthoCamera.gameObject.AddComponent<Camera>();
			orthoCamera.camera.clearFlags = CameraClearFlags.Nothing;
			orthoCamera.camera.cullingMask = (1 << orthoLayer);
			orthoCamera.camera.orthographic = true;
			orthoCamera.camera.orthographicSize = 1;
//			orthoCamera.camera.near = -1;
//			orthoCamera.camera.far = 1;
			orthoCamera.camera.nearClipPlane = -1;
			orthoCamera.camera.farClipPlane = 1;
			orthoCamera.camera.transform.position = Camera.main.transform.position;
		}
		return orthoCamera.gameObject;
	}

#if UNITY_EDITOR
	static void ResumeOrthoCamera (GameObject go) {
		GameObject cam = go.transform.parent.gameObject;
		if ((cam != null) && (cam.GetComponent<Camera>())) {
			orthoCamera.gameObject = cam;
			orthoCamera.camera = cam.GetComponent<Camera>();
		}
	}
#endif

	
	// Create
	public static DrawMgr CreateMgr (Texture texture, int maxCount) {
		return _CreateMgr(texture, maxCount, /*forScreen=*/true, /*parent=*/null);
	}

	public static DrawMgr CreateMgr_For3D (Texture texture, int maxCount, GameObject parent=null) {
		return _CreateMgr(texture, maxCount, /*forScreen=*/false, parent);
	}
	
	static DrawMgr _CreateMgr (Texture texture, int maxCount, bool forScreen=true, GameObject parent=null) {
		GameObject gameObject = new GameObject("DrawMgr");
		if (forScreen) {
			parent = GetOrthoCamera();
			gameObject.layer = orthoLayer;
		}
		if (parent != null) {
			gameObject.transform.parent = parent.transform;
		}
		gameObject.transform.localPosition = Vector3.zero;
		gameObject.transform.rotation = Quaternion.identity;
		gameObject.transform.localScale = Vector3.one;
		DrawMgr objDrawTex = gameObject.AddComponent<DrawMgr>();
		objDrawTex.Create(texture, maxCount, forScreen);
		return objDrawTex;
	}
		

	void Create(Texture texture, int maxCount, bool forScreen=true) {
		
		this.forScreen = forScreen;
		this.maxCount = maxCount;

		material = new Material(Shader.Find("DrawTextureExt/AlphaBlend"));
		material.mainTexture = texture;
//		texST = new Vector2(texture.width*texture.texelSize.x, texture.height*texture.texelSize.y);
//		texST = new Vector2(1,1);

		meshFilter = gameObject.AddComponent(typeof(MeshFilter)) as MeshFilter;
		meshRenderer = gameObject.AddComponent(typeof(MeshRenderer)) as MeshRenderer;
		if (forScreen) {
				#if ! UNITY_5
				meshRenderer.castShadows = false;
				#endif
			meshRenderer.receiveShadows = false;
		}
		mesh = new Mesh();
#if ! UNITY_3_5
		mesh.MarkDynamic();
#endif
		meshFilter.mesh = mesh;
		meshRenderer.GetComponent<Renderer>().material = material;
		
		// Vertex Data
		meshData.haveNormals = false;
		int vertexCount = 1+(maxCount*4);
		int indexCount = (maxCount*6);
		meshData.vertices = new Vector3[vertexCount];
		if (meshData.haveNormals) {
			meshData.normals = new Vector3[vertexCount];
		}
		meshData.texCoords = new Vector2[vertexCount];
		meshData.colors = new Color32[vertexCount];
		meshData.indices = new int[indexCount];
		// Initialize Empty Vertex
		meshData.vertices[0] = Vector3.zero;
		if (meshData.haveNormals) {
			meshData.normals[0] = Vector3.zero;
		}
		meshData.texCoords[0] = Vector2.zero;
		meshData.colors[0] = Color.clear;
		
		count = 0;
		updated = false;
	}

	void OnDestroy () {
		ResetMeshData();
		meshData.vertices = null;
		meshData.normals = null;
		meshData.texCoords = null;
		meshData.colors = null;
		meshData.indices = null;
		Destroy(mesh);
		Destroy(material);
		Destroy(gameObject);
		if (orthoCamera.gameObject != null) {
			Destroy(orthoCamera.gameObject);
			orthoCamera.gameObject = null;
			orthoCamera.camera = null;
		}
	}
	
	
	// Mesh
	void ResetMeshData () {
		if (count > 0) {
			Array.Clear(meshData.indices, 0, count*6);
		}
		count = 0;
		updated = false;
	}

	void ClearMesh () {
		mesh.Clear();
	}
	void UpdateMesh () {
		mesh.Clear();
		mesh.vertices = meshData.vertices;
		if (meshData.haveNormals) {
			mesh.normals = meshData.normals;
		}
		mesh.uv = meshData.texCoords;
		mesh.colors32 = meshData.colors;
#if UNITY_3_5
		mesh.triangles = meshData.indices;
#else
		mesh.SetIndices(meshData.indices, MeshTopology.Triangles, 0);
#endif
//		mesh.RecalculateNormals();
	}


	void Update () {
#if UNITY_EDITOR
		if (orthoCamera.camera == null) {
			ResumeOrthoCamera(gameObject);
		}
#endif
		if (forScreen && orthoCamera.camera && (Camera.main != null)) {
			orthoCamera.camera.transform.position = Camera.main.transform.position;
		}
		
		if (updated) {
			ResetMeshData();
			ClearMesh();
			meshRenderer.enabled = false;
			//updated = false;
		} else {
			UpdateMesh();
			meshRenderer.enabled = true;
		}
		
		if (forScreen) {
			// Transform
			float size = orthoCamera.camera.orthographicSize;
			float aspect = orthoCamera.camera.aspect;
			Vector3 scale = new Vector3((size*2.0f/Screen.width)*aspect, -(size*2.0f/(float)Screen.height), 1.0f);
			gameObject.transform.localScale = scale;
			gameObject.transform.localPosition = new Vector3(-size*aspect, size, z);
//		} else {
//			gameObject.transform.localPosition = Vector3.zero;
		}

		updated = true;	
	}

#if false
	void OnGUI () {		// After Render
		if (count > 0) {
			mesh.Clear();
			ResetMeshData();
		}
	}
#endif

	
	// DrawTexture
	public override void DrawTexture (Rect position) {
		_DrawTexture(position, fullTexCoord, color, alignMode);
	}
	public override void DrawTexture (Rect position, float colorAlpha) {
		Color _color = color;
		_color.a = colorAlpha;
		_DrawTexture(position, fullTexCoord, _color, alignMode);
	}
	public override void DrawTexture (Rect position, Color _color) {
		_DrawTexture(position, fullTexCoord, _color, alignMode);
	}

	public override void DrawTexture (Rect position, Rect texCoords) {
		_DrawTexture(position, texCoords, color, alignMode);
	}
	public override void DrawTexture (Rect position, Rect texCoords, float colorAlpha) {
		Color _color = color;
		_color.a = colorAlpha;
		_DrawTexture(position, texCoords, _color, alignMode);
	}
	public override void DrawTexture (Rect position, Rect texCoords, Color _color) {
		_DrawTexture(position, texCoords, _color, alignMode);
	}
	public override void DrawTexture (Rect position, Rect texCoords, float colorAlpha, AlignMode alignMode) {
		Color _color = color;
		_color.a = colorAlpha;
		_DrawTexture(position, texCoords, _color, alignMode);
	}
	public override void DrawTexture (Rect position, Rect texCoords, Color _color, AlignMode alignMode) {
		_DrawTexture(position, texCoords, _color, alignMode);
	}
	
	public override void DrawTexture (TexRect texRect) {
		_DrawTexture(texRect.rect, texRect.texCoord, color, alignMode);
	}
	public override void DrawTexture (TexRect texRect, float colorAlpha) {
		Color _color = color;
		_color.a = colorAlpha;
		_DrawTexture(texRect.rect, texRect.texCoord, _color, alignMode);
	}
	public override void DrawTexture (TexRect texRect, Color _color) {
		_DrawTexture(texRect.rect, texRect.texCoord, _color, alignMode);
	}

	public override void DrawTexture (float x, float y, TexRect texRect) {
		_rect.Set(x, y, texRect.width, texRect.height);
		_DrawTexture(_rect, texRect.texCoord, color, alignMode);
	}
	public override void DrawTexture (float x, float y, TexRect texRect, float colorAlpha) {
		_rect.Set(x, y, texRect.width, texRect.height);
		Color _color = color;
		_color.a = colorAlpha;
		_DrawTexture(_rect, texRect.texCoord, _color, alignMode);
	}
	public override void DrawTexture (float x, float y, TexRect texRect, Color _color) {
		_rect.Set(x, y, texRect.width, texRect.height);
		_DrawTexture(_rect, texRect.texCoord, _color, alignMode);
	}

	public override void DrawTexture (Vector2 position, TexRect texRect) {
		_rect.Set(position.x, position.y, texRect.width, texRect.height);
		_DrawTexture(_rect, texRect.texCoord, color, alignMode);
	}
	public override void DrawTexture (Vector2 position, TexRect texRect, float colorAlpha) {
		_rect.Set(position.x, position.y, texRect.width, texRect.height);
		Color _color = color;
		_color.a = colorAlpha;
		_DrawTexture(_rect, texRect.texCoord, _color, alignMode);
	}
	public override void DrawTexture (Vector2 position, TexRect texRect, Color _color) {
		_rect.Set(position.x, position.y, texRect.width, texRect.height);
		_DrawTexture(_rect, texRect.texCoord, _color, alignMode);
	}

	void _DrawTexture (Rect position, Rect texCoords, Color _color, AlignMode alignMode) {
		if (updated) {
			ResetMeshData();
			updated = false;
		}

		if (count >= maxCount) {
			Debug.LogError("DrawTextureObject: Not enough maxCount! (maxCount="+maxCount+")");
		} else {
			Alignment (ref position, position.width, position.height, alignMode);

			int vertexIndex = 1+count*4;
			int indexIndex = count*6;
			meshData.indices[indexIndex+0] = vertexIndex+0;
			meshData.indices[indexIndex+1] = vertexIndex+1;
			meshData.indices[indexIndex+2] = vertexIndex+2;
			meshData.indices[indexIndex+3] = vertexIndex+3;
			meshData.indices[indexIndex+4] = vertexIndex+2;
			meshData.indices[indexIndex+5] = vertexIndex+1;
			meshData.vertices[vertexIndex+0].Set(position.x, position.y, z);
			meshData.vertices[vertexIndex+1].Set(position.x+position.width, position.y, z);
			if (forScreen) {
				meshData.vertices[vertexIndex+2].Set(position.x, position.y+position.height, z);
				meshData.vertices[vertexIndex+3].Set(position.x+position.width, position.y+position.height, z);
			} else {
				meshData.vertices[vertexIndex+2].Set(position.x, position.y-position.height, z);
				meshData.vertices[vertexIndex+3].Set(position.x+position.width, position.y-position.height, z);
			}
			if (meshData.haveNormals) {
				meshData.normals[vertexIndex+0] = 
				meshData.normals[vertexIndex+1] = 
				meshData.normals[vertexIndex+2] = 
				meshData.normals[vertexIndex+3] = facedNormal;
			}
#if false
//			meshData.texCoords[vertexIndex+0].Set((texCoords.x)*texST.x, (texCoords.y+texCoords.height)*texST.y);
//			meshData.texCoords[vertexIndex+1].Set((texCoords.x+texCoords.width)*texST.x, (texCoords.y+texCoords.height)*texST.y);
//			meshData.texCoords[vertexIndex+2].Set((texCoords.x)*texST.x, (texCoords.y)*texST.y);
//			meshData.texCoords[vertexIndex+3].Set((texCoords.x+texCoords.width)*texST.x, (texCoords.y)*texST.y);
#else
			meshData.texCoords[vertexIndex+0].Set((texCoords.x), (texCoords.y+texCoords.height));
			meshData.texCoords[vertexIndex+1].Set((texCoords.x+texCoords.width), (texCoords.y+texCoords.height));
			meshData.texCoords[vertexIndex+2].Set((texCoords.x), (texCoords.y));
			meshData.texCoords[vertexIndex+3].Set((texCoords.x+texCoords.width), (texCoords.y));
#endif
#if false
//			meshData.colors[vertexIndex+0] = new Color32(255,0,0,255);
//			meshData.colors[vertexIndex+1] = new Color32(0,255,0,255);
//			meshData.colors[vertexIndex+2] = new Color32(0,0,255,255);
//			meshData.colors[vertexIndex+3] = new Color32(255,255,255,128);
#else
			meshData.colors[vertexIndex+0] = 
			meshData.colors[vertexIndex+1] = 
			meshData.colors[vertexIndex+2] = 
			meshData.colors[vertexIndex+3] = _color;
#endif
			++count;
		}
	}


	// DrawTextureWithTexCoords with Rotate & Scale
	public override void DrawTexture (Rect position, Rect texCoords, Color _color, float scale, float rotation) {
		if (updated) {
			ResetMeshData();
			updated = false;
		}

		if (count >= maxCount) {
			Debug.LogError("DrawTextureObject: Not enough maxCount! (maxCount="+maxCount+")");
		} else {
			int vertexIndex = 1+count*4;
			int indexIndex = count*6;
			meshData.indices[indexIndex+0] = vertexIndex+0;
			meshData.indices[indexIndex+1] = vertexIndex+1;
			meshData.indices[indexIndex+2] = vertexIndex+2;
			meshData.indices[indexIndex+3] = vertexIndex+3;
			meshData.indices[indexIndex+4] = vertexIndex+2;
			meshData.indices[indexIndex+5] = vertexIndex+1;
			//
			float wh = (position.width * 0.5f) * scale;
			float hh = (position.height * 0.5f) * scale;
			float rs = Mathf.Sin(rotation);
			float rc = Mathf.Cos(rotation);
			float ax = (1.0f * rc) + (1.0f * -rs);
			float ay = (1.0f * rs) + (1.0f * rc);
			//
			if (forScreen) {
				meshData.vertices[vertexIndex+0].Set(position.x-ax*wh, position.y-ay*hh, z);
				meshData.vertices[vertexIndex+1].Set(position.x+ay*wh, position.y-ax*hh, z);
				meshData.vertices[vertexIndex+2].Set(position.x-ay*wh, position.y+ax*hh, z);
				meshData.vertices[vertexIndex+3].Set(position.x+ax*wh, position.y+ay*hh, z);
			} else {
				meshData.vertices[vertexIndex+0].Set(position.x-ax*wh, position.y+ay*hh, z);
				meshData.vertices[vertexIndex+1].Set(position.x+ay*wh, position.y+ax*hh, z);
				meshData.vertices[vertexIndex+2].Set(position.x-ay*wh, position.y-ax*hh, z);
				meshData.vertices[vertexIndex+3].Set(position.x+ax*wh, position.y-ay*hh, z);
			}
			if (meshData.haveNormals) {
				meshData.normals[vertexIndex+0] = 
				meshData.normals[vertexIndex+1] = 
				meshData.normals[vertexIndex+2] = 
				meshData.normals[vertexIndex+3] = facedNormal;
			}
#if false
//			meshData.texCoords[vertexIndex+0].Set((texCoords.x)*texST.x, (texCoords.y+texCoords.height)*texST.y);
//			meshData.texCoords[vertexIndex+1].Set((texCoords.x+texCoords.width)*texST.x, (texCoords.y+texCoords.height)*texST.y);
//			meshData.texCoords[vertexIndex+2].Set((texCoords.x)*texST.x, (texCoords.y)*texST.y);
//			meshData.texCoords[vertexIndex+3].Set((texCoords.x+texCoords.width)*texST.x, (texCoords.y)*texST.y);
#else
			meshData.texCoords[vertexIndex+0].Set((texCoords.x), (texCoords.y+texCoords.height));
			meshData.texCoords[vertexIndex+1].Set((texCoords.x+texCoords.width), (texCoords.y+texCoords.height));
			meshData.texCoords[vertexIndex+2].Set((texCoords.x), (texCoords.y));
			meshData.texCoords[vertexIndex+3].Set((texCoords.x+texCoords.width), (texCoords.y));
#endif
#if false
//			meshData.colors[vertexIndex+0] = new Color32(255,0,0,255);
//			meshData.colors[vertexIndex+1] = new Color32(0,255,0,255);
//			meshData.colors[vertexIndex+2] = new Color32(0,0,255,255);
//			meshData.colors[vertexIndex+3] = new Color32(255,255,255,128);
#else
			meshData.colors[vertexIndex+0] = 
			meshData.colors[vertexIndex+1] = 
			meshData.colors[vertexIndex+2] = 
			meshData.colors[vertexIndex+3] = _color;
#endif
			++count;
		}
	}


	// Alignment
	float AlignmentWidth (float x, float width, AlignMode alignMode) {
		int alignX = ((int)alignMode % 3) - 1;
		if (alignX < 0) {
		} else if (alignX == 0) {
			x -= width * 0.5f;
		} else {
			x -= width;
		}
		return x;
	}
	void Alignment (ref Rect rect, float width, float height, AlignMode alignMode) {
		rect.x = AlignmentWidth(rect.x, width, alignMode);

		int alignY = ((int)alignMode / 3) - 1;
		if (forScreen) {
			if (alignY < 0) {
			} else if (alignY == 0) {
				rect.y -= height * 0.5f;
			} else {
				rect.y -= height;
			}
		} else {
			if (alignY < 0) {
			} else if (alignY == 0) {
				rect.y += height * 0.5f;
			} else {
				rect.y += height;
			}
		}
	}


	// Draw Number
	public override void DrawNumber (Rect position, Rect numberTexCoords, long number) {
		DrawNumber(position, numberTexCoords, number, /*padding=*/0);
	}
	public override void DrawNumber (Rect position, Rect numberTexCoords, long number, float padding) {
		Rect rect = position;
		Rect texCoord = numberTexCoords;

		// Alignment
		int count = 0;
		float width = 0;
		long tmp = number;
		do {
			width += rect.width;
			tmp /= 10;
			++count;
		} while (tmp > 0);
		width += padding * (count - 1);
		//
		Alignment(ref rect, width, rect.height, alignMode);
		rect.x += width;	// Alignment to the Right
		
		// Draw
		tmp = number;
		do {
			long n = tmp % 10;
			rect.x -= rect.width;
			texCoord.x = numberTexCoords.x + (n * numberTexCoords.width);
			DrawTexture(rect, texCoord, color, AlignMode.LeftTop);
			rect.x -= padding;
			tmp /= 10;
		} while (tmp > 0);
	}


	// Draw Text
	const int linesWidthTmpCount = 100;
	static float[] linesWidthTmp = new float[linesWidthTmpCount];

	public override void DrawText (Rect position, Rect textTexCoords, string text) {
		DrawText(position, textTexCoords, text, /*padding=*/Vector2.zero);
	}
	public override void DrawText (Rect position, Rect textTexCoords, string text, Vector2 padding) {
		Rect rect = position;
		Rect texCoord = textTexCoords;

		char[] charArray = text.ToCharArray();
		if (charArray.Length == 0) {
			return;
		}

		// Alignment (Supported multi-line)
		int lines = 1;
		foreach (char c in charArray) {
			if (c == '\n') {
				++lines;
			}
		}
		float height = (rect.height * lines) + (padding.y * (lines - 1));
		//
		float[] linesWidth = (lines <= linesWidthTmpCount) ? linesWidthTmp : (new float[lines]);
		int line = 0;
		int count = 0;
		foreach (char c in charArray) {
			if (c == '\n') {
				if (count > 0) {
					linesWidth[line] = (rect.width * count) + (padding.x * (count - 1));
				}
				count = 0;
				++line;
			} else if ((c >= 0x20) && (c <= 0x80)) {
				++count;
			}
		}
		if (count > 0) {
			linesWidth[line] = (rect.width * count) + (padding.x * (count - 1));
		}
		
		// Draw
		line = 0;
		Alignment(ref rect, linesWidth[line], height, alignMode);
		foreach (char c in charArray) {
			if (c == '\n') {
				++line;
				rect.x = AlignmentWidth(position.x, linesWidth[line], alignMode);
				rect.y += rect.height + padding.y;
			} else if ((c >= 0x20) && (c <= 0x80)) {
				char n = (char)(c - 0x20);
				texCoord.x = textTexCoords.x + ((float)(n & 0xF) * textTexCoords.width);
				texCoord.y = textTexCoords.y - ((float)((n >> 4) & 0xF) * textTexCoords.height);
				DrawTexture(rect, texCoord, color, AlignMode.LeftTop);
				rect.x += rect.width + padding.x;
			}
		}
	}
}

} //namespace DrawTextureExt
