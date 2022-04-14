using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace aberonServices.Models
{
    public class PickingListLine
    {
        public string Id;
        public string Aisle;
        public int Bay;
        public int Layer;
        public int RelativeBay;
        public int[] ColumnsPerLayer;
        public int MatrixLayer;
        public int MatrixColumn;
        public int DistanceFromFloor;
        public int DistanceFromMarker;
        public int Width;
        public int Height;
        public string LocAddress;
        public string ItemCode;
        public string ItmDescr;
        public Boolean SerialScanRequired;
        public Boolean IsDual;
        public string SerialLength;
        public string SerialPrefix;
        public decimal BoxQty;
        public decimal Qty;
        public decimal TotalQty;
        public decimal UnitsPerBox;
        public string ImgUrl;
        public string LutDescr;
        public string[] ValidBarcodes;
    }
}