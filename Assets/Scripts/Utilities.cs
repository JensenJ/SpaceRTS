using UnityEngine;
using System.Collections.Generic;
using System.Text;
using System.IO;

public class Utilities
{
    private static List<string[]> rowData = new List<string[]>();

    public static void WriteToCSV(string fileName, string[] headings, string[,] data)
    {
        //Headings
        rowData.Add(headings);

        //Data for CSV
        //for every row
        for (int i = 0; i < data.GetLength(1); i++)
        {
            //Create row array
            string[] dataPerRow = new string[data.GetLength(0)];
            //Get all data for row / data in each column
            for (int j = 0; j < data.GetLength(0); j++)
            {
                dataPerRow[j] = data[j, i];
            }
            //Add row to rowData list.
            rowData.Add(dataPerRow);
        }

        string[][] output = new string[rowData.Count][];

        for (int i = 0; i < output.Length; i++)
        {
            output[i] = rowData[i];
        }

        int length = output.GetLength(0);
        string delimiter = ",";

        //Build string
        StringBuilder sb = new StringBuilder();

        for (int index = 0; index < length; index++)
            sb.AppendLine(string.Join(delimiter, output[index]));


        //Get file path
        string filePath = getPath(fileName);

        //Write to file
        StreamWriter outStream = System.IO.File.CreateText(filePath);
        outStream.WriteLine(sb);
        outStream.Close();
    }

    // Gets the file path
    private static string getPath(string fileName)
    {
        return Application.dataPath + "/Logs/" + fileName + ".csv";
    }
}