using UnityEngine;
using System.Collections;


[System.Serializable]
public class TileData
{
    [System.Serializable]
    public struct rowData
    {
        public bool[] row;
        private int count;
        public int Count
        {
            get { return count; }
        }
    }

    public rowData[] rows = new rowData[16];
}