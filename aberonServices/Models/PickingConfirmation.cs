using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace aberonServices.Models
{
    public class DualImei
    {
        public string Imei1;
        public string Imei2;
    }
    public class PickingConfirmation
    {
        public string LineId;
        public string ScannedBarcode;
        public int PickerId;
        public decimal Qty;
        public string[] SerialNumbers;
        public DualImei[] DualImeis;
    }
}