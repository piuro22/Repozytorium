using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace E2C.ChartEditor
{
    [CustomEditor(typeof(E2ChartDataImporter))]
    [CanEditMultipleObjects]
    public class E2ChartDataImporterEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            if (GUILayout.Button("Import data"))
            {
                foreach (E2ChartDataImporter generator in targets)
                {
                    if (generator.gameObject.scene.name == null) continue;
                    generator.ImportData();
                    try { PrefabUtility.RecordPrefabInstancePropertyModifications(generator.chart.chartData); } catch { }
                }
            }

            if (GUILayout.Button("Clear data"))
            {
                foreach (E2ChartDataImporter generator in targets)
                {
                    if (generator.gameObject.scene.name == null) continue;
                    generator.ClearData();
                    try { PrefabUtility.RecordPrefabInstancePropertyModifications(generator.chart.chartData); } catch { }
                }
            }
        }
    }
}