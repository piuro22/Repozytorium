---------------------------------------------------------------------------
                     DrawTexture Ext
            Copyright © 2013-2015 White Dev
---------------------------------------------------------------------------

---------------------------------------------------------------------------
Description:
This plugin is able to draw with low drawcall draw. It has some interfaces like GUI.DrawTexture.

And, some extra-functions included.


Features:
- Play the Crazy Pong  [ NEW!! ]
- Interface like GUI.DrawTexture
- 1 Drawcall per Texture/Object
- Draw on 2D Screen or 3D World
- Draw Numbers/Text easily
- Alignments support


How to use (C# Script):

Use GUI:
	void OnGUI () {
		Rect pos = new Rect(0,0, 100,100);
		Rect texCoord = new Rect(0,0, 1,1);
		GUI.DrawTextureWithTexCoords(pos, texture, texCoord);
	}


Use DrawTextureExt:

	DrawMgr GUIext;

	void Start () {
		GUIext = DrawTextureExt.DrawMgr.CreateMgr(texture, /*maxCount=*/100);
	}

	void Update () {
		Rect pos = new Rect(0,0, 100,100);
		Rect texCoord = new Rect(0,0, 1,1);
		GUIext.DrawTexture(pos, texCoord);
	}


Use DrawTextureExt for 3D:

	DrawMgr GUIext3D;

	void Start () {
		GUIext3D = DrawTextureExt.DrawMgr.CreateMgr_For3D(texture, /*maxCount=*/100, parentObject);
	}

	void Update () {
		Rect pos = new Rect(-0.5f,-1, 1,1);
		Rect texCoord = new Rect(0,0, 1,1);
		GUIext3D.DrawTexture(pos, texCoord);
	}


Support: whitedev.support@gmail.com


---------------------------------------------------------------------------
Version Changes:
1.3:
	- Support for Unity 5
1.2:
	- Using namespace ‘DrawTextureExt’
	- Improved TexRect class
	- Added some API to easily draw
	- Support for draw with scale/rotation
1.1:
	- Added Pong Demo
	- Alignments for DrawTexture
1.0.1:
	- Fixed alignment for multi-line text.
1.0:
	- Initial version.

---------------------------------------------------------------------------
