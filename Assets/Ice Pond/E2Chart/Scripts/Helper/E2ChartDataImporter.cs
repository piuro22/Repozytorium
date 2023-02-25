using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using System;

namespace E2C
{
    public class E2ChartDataImporter : MonoBehaviour
    {
        [System.Serializable]
        public struct CSVSeries
        {
            public string dataName;
            public string dataShow;
            public string dataX;
            public string dataY;
            public string dateTimeTick;
            public string dateTimeString;
        }

        [System.Serializable]
        public struct CSVImportSettings
        {
            public char delimiter;
            public List<CSVSeries> series;
        }

        public E2Chart chart;
        public string fileName;
        public bool isStreamingAsset;
        public bool runtimeImport;
        public CSVImportSettings csvImportSettings;


        private void Start()
        {
            if (runtimeImport)
            {
                if (isStreamingAsset)
                {
                    ImportStreamingAssetRuntime();
                }
                else
                {
                    ImportData();
                    chart.UpdateChart();
                }
            }
        }

        private void Reset()
        {
            if (chart == null) chart = GetComponent<E2Chart>();
        }

        public void ClearData()
        {
            if (chart == null || chart.chartData == null) return;
            chart.chartData.series.Clear();
        }

        public void ImportData()
        {
            if (chart == null || chart.chartData == null) return;
            string filePath = isStreamingAsset ? Path.Combine(Application.streamingAssetsPath, fileName) : fileName;
            if (!File.Exists(filePath)) { Debug.Log("File not exists"); return; }
            string dataStr = File.ReadAllText(filePath);
            LoadData(dataStr);
        }

        void ImportStreamingAssetRuntime()
        {
            if (chart == null || chart.chartData == null) return;
            string filePath = isStreamingAsset ? Path.Combine(Application.streamingAssetsPath, fileName) : fileName;
            if (!File.Exists(filePath)) { Debug.Log("File not exists"); return; }
            StartCoroutine(ImportStreamingAssetCoroutine(filePath));
        }

        IEnumerator ImportStreamingAssetCoroutine(string filePath)
        {
            string dataStr = "";
            using (UnityWebRequest req = UnityWebRequest.Get(filePath))
            {
                yield return req.SendWebRequest();
                if (!string.IsNullOrEmpty(req.error)) Debug.LogError("Error: " + req.error);
                else dataStr = req.downloadHandler.text;
            }
            LoadData(dataStr);
            chart.UpdateChart();
        }

        public void LoadData(string dataStr)
        {
            ClearData();
            Dictionary<string, List<string>> csv = ReadCsv(dataStr, csvImportSettings.delimiter);
            for (int i = 0; i < csvImportSettings.series.Count; ++i)
            {
                CSVSeries csvSeries = csvImportSettings.series[i];
                E2ChartData.Series dataSeries = new E2ChartData.Series();

                if (!string.IsNullOrEmpty(csvSeries.dataName) && csv.ContainsKey(csvSeries.dataName))
                {
                    List<string> dataList = csv[csvSeries.dataName];
                    dataSeries.dataName = dataList;
                }

                if (!string.IsNullOrEmpty(csvSeries.dataShow) && csv.ContainsKey(csvSeries.dataShow))
                {
                    List<bool> dataList = new List<bool>();
                    try { foreach (string str in csv[csvSeries.dataShow]) dataList.Add(bool.Parse(str)); }
                    catch (Exception e) { Debug.LogError(e); }
                    dataSeries.dataShow = dataList;
                }

                if (!string.IsNullOrEmpty(csvSeries.dataX) && csv.ContainsKey(csvSeries.dataX))
                {
                    List<float> dataList = new List<float>();
                    try { foreach (string str in csv[csvSeries.dataX]) dataList.Add(float.Parse(str)); }
                    catch (Exception e) { Debug.LogError(e); }
                    dataSeries.dataX = dataList;
                }

                if (!string.IsNullOrEmpty(csvSeries.dataY) && csv.ContainsKey(csvSeries.dataY))
                {
                    List<float> dataList = new List<float>();
                    try { foreach (string str in csv[csvSeries.dataY]) dataList.Add(float.Parse(str)); }
                    catch (Exception e) { Debug.LogError(e); }
                    dataSeries.dataY = dataList;
                }

                if (!string.IsNullOrEmpty(csvSeries.dateTimeTick) && csv.ContainsKey(csvSeries.dateTimeTick))
                {
                    List<long> dataList = new List<long>();
                    try { foreach (string str in csv[csvSeries.dateTimeTick]) dataList.Add(long.Parse(str)); }
                    catch (Exception e) { Debug.LogError(e); }
                    dataSeries.dateTimeTick = dataList;
                }

                if (!string.IsNullOrEmpty(csvSeries.dateTimeString) && csv.ContainsKey(csvSeries.dateTimeString))
                {
                    List<string> dataList = csv[csvSeries.dateTimeString];
                    dataSeries.dateTimeString = dataList;
                }

                chart.chartData.series.Add(dataSeries);
            }
        }

        public static Dictionary<string, List<string>> ReadCsv(string data, char delimiter = ',')
        {
            Dictionary<string, List<string>> csv = new Dictionary<string, List<string>>();
            string[] rows = data.Split('\n');
            if (rows.Length == 0) return null;

            string[] columns = rows[0].Split(delimiter);
            for (int i = 0; i < columns.Length; ++i)
            {
                columns[i] = columns[i].Trim();
                csv[columns[i]] = new List<string>();
            }

            List<string> items = new List<string>();
            for (int rowIndex = 1; rowIndex < rows.Length; ++rowIndex)
            {
                string row = rows[rowIndex].Trim();
                if (row == "") continue;

                int lastIndex = 0;
                int delimiterCount = 0;
                if (items.Count == 0)
                {
                    for (int j = 0; j < row.Length; ++j)
                    {
                        if (row[j] == delimiter)
                        {
                            delimiterCount++;
                            string item = row.Substring(lastIndex, j - lastIndex);
                            items.Add(item);
                            lastIndex = j + 1;
                        }
                    }
                    string lastItem = row.Substring(lastIndex, row.Length - lastIndex);
                    items.Add(lastItem);
                }
                else
                {
                    for (int j = 0; j < row.Length; ++j)
                    {
                        if (row[j] == delimiter)
                        {
                            delimiterCount++;
                            string item = row.Substring(lastIndex, j - lastIndex);
                            if (delimiterCount == 1) items[items.Count - 1] = items[items.Count - 1] + "\n" + item;
                            else items.Add(item);
                            lastIndex = j + 1;
                        }
                    }
                    string lastItem = row.Substring(lastIndex, row.Length - lastIndex);
                    if (delimiterCount == 0) items[items.Count - 1] = items[items.Count - 1] + "\n" + lastItem;
                    else items.Add(lastItem);
                }

                if (items.Count >= columns.Length)
                {
                    for (int i = 0; i < columns.Length; ++i)
                    {
                        csv[columns[i]].Add(items[i]);
                    }
                    items.Clear();
                }
            }

            return csv;
        }
    }
}