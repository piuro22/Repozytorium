using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if CHART_TMPRO
using TMPro;
using E2ChartText = TMPro.TextMeshProUGUI;
using E2ChartTextFont = TMPro.TMP_FontAsset;
#else
using E2ChartText = UnityEngine.UI.Text;
using E2ChartTextFont = UnityEngine.Font;
#endif

namespace E2C.ChartBuilder
{
    public class E2ChartThresholdLines : MonoBehaviour
    {
        [System.Serializable]
        public class ThresholdLineInfo
        {
            public float ratio;
            public string labelText;
        }

        public E2ChartOptions.AxisDirection axis = E2ChartOptions.AxisDirection.Y;
        public bool enableLine = true;
        public Color lineColor = new Color(0.3f, 0.3f, 0.3f, 1.0f);
        public float lineWidth = 2;
        public bool enableTick = true;
        public Color tickColor = new Color(0.3f, 0.3f, 0.3f, 1.0f);
        public Vector2 tickSize = new Vector2(2.0f, 4.0f);
        public bool enableLabel = true;
        public bool overlapAxisLabels = false;
        public E2ChartOptions.TextOptions labelTextOption = new E2ChartOptions.TextOptions(new Color(0.2f, 0.2f, 0.2f, 1.0f), null, 14);
        public E2ChartOptions.LabelRotation labelRotationMode = E2ChartOptions.LabelRotation.Auto;
        public ThresholdLineInfo[] thresholdInfo;

        public void CreateHorizontalLines(RectTransform axisRect, E2ChartGridAxis axis)
        {
            if (thresholdInfo == null) return;

            if (enableLine)
            {
                foreach (ThresholdLineInfo lineInfo in thresholdInfo)
                {
                    Image line = E2ChartBuilderUtility.CreateImage("Threshold Lines", axisRect);
                    line.color = lineColor;
                    line.rectTransform.anchorMin = new Vector2(0.0f, lineInfo.ratio);
                    line.rectTransform.anchorMax = new Vector2(1.0f, lineInfo.ratio);
                    line.rectTransform.offsetMin = new Vector2(0.0f, -lineWidth * 0.5f);
                    line.rectTransform.offsetMax = new Vector2(0.0f, lineWidth * 0.5f);
                }
            }

            if (enableTick)
            {
                foreach (ThresholdLineInfo lineInfo in thresholdInfo)
                {
                    Image tick = E2ChartBuilderUtility.CreateImage("Threshold Ticks", axisRect);
                    tick.color = tickColor;
                    if (axis.axisOptions.mirrored)
                    {
                        tick.rectTransform.anchorMin = new Vector2(1.0f, lineInfo.ratio);
                        tick.rectTransform.anchorMax = new Vector2(1.0f, lineInfo.ratio);
                        tick.rectTransform.offsetMin = new Vector2(0.0f, -tickSize.x * 0.5f);
                        tick.rectTransform.offsetMax = new Vector2(tickSize.y, tickSize.x * 0.5f);
                    }
                    else
                    {
                        tick.rectTransform.anchorMin = new Vector2(0.0f, lineInfo.ratio);
                        tick.rectTransform.anchorMax = new Vector2(0.0f, lineInfo.ratio);
                        tick.rectTransform.offsetMin = new Vector2(-tickSize.y, -tickSize.x * 0.5f);
                        tick.rectTransform.offsetMax = new Vector2(0.0f, tickSize.x * 0.5f);
                    }
                }
            }

            if (enableLabel)
            {
                float thWidth = enableTick ? tickSize.y : 0.0f;
                if (!overlapAxisLabels && thWidth < axis.axisWidth) thWidth = axis.axisWidth;
                float labelRotation = E2ChartBuilderUtility.GetLabelRotation(labelRotationMode);
                if (axis.axisOptions.mirrored) labelRotation = -labelRotation;
                float radian = Mathf.Abs(labelRotation) * Mathf.Deg2Rad;
                float cos = Mathf.Cos(radian);
                float maxWidth = 0.0f;
                float widthLimit = axisRect.rect.width * E2ChartGridAxis.AXIS_LABEL_AREA_LIMIT - axis.axisWidth;

                foreach (ThresholdLineInfo lineInfo in thresholdInfo)
                {
                    E2ChartText label = E2ChartBuilderUtility.CreateText("LabelTemp", axisRect, labelTextOption, axis.generalFont);
                    float labelOffset = thWidth + label.fontSize * 0.5f;
                    label.text = lineInfo.labelText;
                    label.rectTransform.sizeDelta = Vector2.zero;
                    if (axis.axisOptions.mirrored)
                    {
                        label.alignment = E2ChartBuilderUtility.ConvertAlignment(TextAnchor.MiddleLeft);
                        label.rectTransform.anchorMin = label.rectTransform.anchorMax = new Vector2(1.0f, lineInfo.ratio);
                        label.rectTransform.anchoredPosition = new Vector2(labelOffset, 0.0f);
                    }
                    else
                    {
                        label.alignment = E2ChartBuilderUtility.ConvertAlignment(TextAnchor.MiddleRight);
                        label.rectTransform.anchorMin = label.rectTransform.anchorMax = new Vector2(0.0f, lineInfo.ratio);
                        label.rectTransform.anchoredPosition = new Vector2(-labelOffset, 0.0f);
                    }
                    label.rectTransform.localRotation = Quaternion.Euler(0.0f, 0.0f, labelRotation);
                    float width = Mathf.Clamp(label.preferredWidth * cos, 0.0f, widthLimit);
                    if (width > maxWidth) maxWidth = width;
                }
                thWidth += labelTextOption.fontSize * 1.0f + maxWidth;
                if (thWidth > axis.axisWidth) axis.axisWidth = thWidth;
            }
        }

        public void CreateVerticalLines(RectTransform axisRect, E2ChartGridAxis axis)
        {
            if (thresholdInfo == null) return;

            if (enableLine)
            {
                foreach (ThresholdLineInfo lineInfo in thresholdInfo)
                {
                    Image line = E2ChartBuilderUtility.CreateImage("Threshold Lines", axisRect);
                    line.color = lineColor;
                    line.rectTransform.anchorMin = new Vector2(lineInfo.ratio, 0.0f);
                    line.rectTransform.anchorMax = new Vector2(lineInfo.ratio, 1.0f);
                    line.rectTransform.offsetMin = new Vector2(-lineWidth * 0.5f, 0.0f);
                    line.rectTransform.offsetMax = new Vector2(lineWidth * 0.5f, 0.0f);
                }
            }

            if (enableTick)
            {
                foreach (ThresholdLineInfo lineInfo in thresholdInfo)
                {
                    Image tick = E2ChartBuilderUtility.CreateImage("Threshold Ticks", axisRect);
                    tick.color = tickColor;
                    if (axis.axisOptions.mirrored)
                    {
                        tick.rectTransform.anchorMin = new Vector2(lineInfo.ratio, 1.0f);
                        tick.rectTransform.anchorMax = new Vector2(lineInfo.ratio, 1.0f);
                        tick.rectTransform.offsetMin = new Vector2(-tickSize.x * 0.5f, 0.0f);
                        tick.rectTransform.offsetMax = new Vector2(tickSize.x * 0.5f, tickSize.y);
                    }
                    else
                    {
                        tick.rectTransform.anchorMin = new Vector2(lineInfo.ratio, 0.0f);
                        tick.rectTransform.anchorMax = new Vector2(lineInfo.ratio, 0.0f);
                        tick.rectTransform.offsetMin = new Vector2(-tickSize.x * 0.5f, -tickSize.y);
                        tick.rectTransform.offsetMax = new Vector2(tickSize.x * 0.5f, 0.0f);
                    }
                }
            }

            if (enableLabel)
            {
                float thWidth = enableTick ? tickSize.y : 0.0f;
                if (!overlapAxisLabels && thWidth < axis.axisWidth) thWidth = axis.axisWidth;
                float labelRotation = E2ChartBuilderUtility.GetLabelRotation(labelRotationMode);
                if (axis.axisOptions.mirrored) labelRotation = -labelRotation;
                float radian = Mathf.Abs(labelRotation) * Mathf.Deg2Rad;
                float sin = Mathf.Sin(radian);
                float maxWidth = 0.0f;
                float widthLimit = axisRect.rect.height * E2ChartGridAxis.AXIS_LABEL_AREA_LIMIT - axis.axisWidth;

                foreach (ThresholdLineInfo lineInfo in thresholdInfo)
                {
                    E2ChartText label = E2ChartBuilderUtility.CreateText("LabelTemp", axisRect, labelTextOption, axis.generalFont);
                    float labelOffset = thWidth + label.fontSize * (0.5f + E2ChartGridAxis.AXIS_LABEL_HEIGHT_SPACING * 0.5f);
                    label.text = lineInfo.labelText;
                    label.rectTransform.sizeDelta = Vector2.zero;
                    if (axis.axisOptions.mirrored)
                    {
                        if (labelRotation > 0.0f) label.alignment = E2ChartBuilderUtility.ConvertAlignment(TextAnchor.MiddleLeft);
                        else if (labelRotation < 0.0f) label.alignment = E2ChartBuilderUtility.ConvertAlignment(TextAnchor.MiddleRight);
                        label.rectTransform.anchorMin = label.rectTransform.anchorMax = new Vector2(lineInfo.ratio, 1.0f);
                        label.rectTransform.anchoredPosition = new Vector2(0.0f, labelOffset);
                    }
                    else
                    {
                        if (labelRotation > 0.0f) label.alignment = E2ChartBuilderUtility.ConvertAlignment(TextAnchor.MiddleRight);
                        else if (labelRotation < 0.0f) label.alignment = E2ChartBuilderUtility.ConvertAlignment(TextAnchor.MiddleLeft);
                        label.rectTransform.anchorMin = label.rectTransform.anchorMax = new Vector2(lineInfo.ratio, 0.0f);
                        label.rectTransform.anchoredPosition = new Vector2(0.0f, -labelOffset);
                    }
                    label.rectTransform.localRotation = Quaternion.Euler(0.0f, 0.0f, labelRotation);
                    float width = Mathf.Clamp(label.preferredWidth * sin, 0.0f, widthLimit);
                    if (width > maxWidth) maxWidth = width;
                }
                thWidth += labelTextOption.fontSize * (1.0f + E2ChartGridAxis.AXIS_LABEL_HEIGHT_SPACING) + maxWidth;
                if (thWidth > axis.axisWidth) axis.axisWidth = thWidth;
            }
        }
    }
}