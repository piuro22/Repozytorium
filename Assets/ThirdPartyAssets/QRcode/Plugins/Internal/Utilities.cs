using System;
using System.Collections;
using UnityEngine;

namespace TBEasyWebCam
{
	public static class Utilities
	{
		public static void Dimensions(ResolutionMode preset, out int width, out int height)
		{
			width = height = 600;
			switch (preset)
			{
			case ResolutionMode.HD:
				width = 1280;
				height = 720;
				break;
			case ResolutionMode.FullHD:
				width = 1920;
				height = 1080;
				break;
			case ResolutionMode.HighestResolution:
				width = 1920;
				height = 1080;
				break;
			case ResolutionMode.MediumResolution:
				width = 640;
				height = 480;
				break;
			case ResolutionMode.LowestResolution:
				width = 50;
				height = 50;
				break;
			}
		}
	}
}
