using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace aberonServices.Models
{
    public class PickingList
    {
        public decimal Id;
        public string CusCode;
        public string CusName;
        public PickingListLine[] Lines;
    }
}