using aberonServices.Helpers;
using aberonServices.Models;
using aberonServices.Utils;
using Newtonsoft.Json;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;

namespace aberonServices.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class PickingListController : ApiController
    {

        // GET: api/PickingList/5
        public IHttpActionResult Get(decimal id)
        {
            Log.Write("Picking List Request.Id=" + id);
            var data = SqlHelper.GetData("PickingList", 
                new OracleParameter("OrderId", id));
            if (data == null || data.Count==0) return NotFound();
            PickingList pl = new PickingList();
            int i = 0;
            foreach (var r in data) {
                if (pl.Lines == null) {
                    pl.Id = id;
                    pl.CusCode = r["CUSTOMERCODE"].ToString().TrimEnd();
                    pl.CusName = r["CUS_NAME1"].ToString().TrimEnd();
                    pl.Lines = new PickingListLine[data.Count];
                }                
                PickingListLine plsLine = new PickingListLine();
                plsLine.Id = r["LINEID"].ToString();
                plsLine.Aisle = r["AISLE"].ToString().TrimEnd();
                plsLine.Bay = Convert.ToInt32(r["BAY"]);
                plsLine.Layer = Convert.ToInt32(r["LAYER"]);
                plsLine.RelativeBay = Convert.ToInt32(r["RELATIVEBAY"]);
                if (r["COLUMNS_PER_LAYER"].ToString() != "") {
                    plsLine.ColumnsPerLayer = r["COLUMNS_PER_LAYER"].ToString().Split(',').
                        Select(e => Convert.ToInt32(e)).ToArray();
                }
                string[] layerInfo = r["MARTIX_LAYER"].ToString().Split('-');
                plsLine.MatrixLayer = Convert.ToInt32(layerInfo[0]);
                if (layerInfo.Length == 2 && !string.IsNullOrEmpty(layerInfo[1])) {
                    plsLine.DistanceFromFloor= Convert.ToInt32(layerInfo[1]);
                }
                string[] columnInfo= r["MARTIX_COLUMN"].ToString().Split('-');
                plsLine.MatrixColumn = Convert.ToInt32(columnInfo[0]);
                if (columnInfo.Length == 2 && !string.IsNullOrEmpty(columnInfo[1])) {
                    plsLine.DistanceFromMarker = Convert.ToInt32(columnInfo[1]);
                }
                plsLine.Width = Convert.ToInt32(r["WIDTH"]);
                plsLine.Height = Convert.ToInt32(r["HEIGHT"]);
                plsLine.LocAddress = r["LOCADDRESS"].ToString().TrimEnd();
                plsLine.ItemCode = r["ITEMCODE"].ToString().TrimEnd();
                plsLine.ItmDescr = r["ITM_DESCR1"].ToString().TrimEnd();
                plsLine.ImgUrl= r["IMAGEPATH"].ToString().TrimEnd();
                plsLine.SerialScanRequired = Convert.ToInt32(r["SERIALNOHANDLINGFLG"]) == 0 ? false : true;
                plsLine.SerialLength= r["SERIALLENGTH"].ToString().TrimEnd();
                plsLine.SerialPrefix= r["SERIALPREFIX"].ToString().TrimEnd();
                plsLine.LutDescr= r["LUT_DESCR"].ToString().TrimEnd();
                if (r["ISDUAL"].ToString().TrimEnd() != "") {
                    plsLine.IsDual = true;
                }
                plsLine.TotalQty = Convert.ToDecimal(r["PICKINGQTY"]);
                plsLine.UnitsPerBox = Convert.ToDecimal(r["LU1PERLU2"]);
                if (plsLine.UnitsPerBox != 0) {
                    plsLine.BoxQty = Math.Floor(plsLine.TotalQty / plsLine.UnitsPerBox);
                    plsLine.Qty = Decimal.Remainder(plsLine.TotalQty, plsLine.UnitsPerBox);
                } else {
                    plsLine.BoxQty = 0;
                    plsLine.Qty = plsLine.TotalQty;
                }
                plsLine.ValidBarcodes =r["BARCODES"].ToString().TrimEnd().Split(',').
                    Where(e=>e.Trim()!="").
                    GroupBy(e=>e).
                    Select(g=>g.Key).
                    ToArray();
                pl.Lines[i] = plsLine;
                i++;
            }
            return Ok(pl);
        }
        // POST: api/PickingList
        public async Task<IHttpActionResult> Post([FromBody]PickingConfirmation pickingConfirmation)
        {
            string errorPoint = "start";
            try
            {          
                string body = await HttpHelper.GetBodyContentAsStringAsync(Request);
                string fileName = Log.WriteFile("", "PickingCnf", body);
                if (pickingConfirmation == null)
                {
                    return BadRequest("PickingConfirmation object not defined");
                }
                if (ConfigurationManager.AppSettings["PickingConfirmation"] != "true") {
                    /*if (pickingConfirmation.Qty == 1115) {
                        return BadRequest("Invalid Qty");
                    }
                    if (pickingConfirmation.SerialNumbers != null &&
                        pickingConfirmation.SerialNumbers.Length > 0 &&
                        pickingConfirmation.SerialNumbers[0].EndsWith("1")) {
                        return BadRequest("Invalid SerialNumber");
                    }*/
                    return Ok();
                }
                #region lineId decomposition
                if (string.IsNullOrEmpty(pickingConfirmation.LineId)) {
                    return BadRequest("Line Id not provided");
                }
                string[] lineIdArray = pickingConfirmation.LineId.Split('|');
                int idx = 0;
                decimal taskId = GetLnDecItem(lineIdArray, idx); idx++;
                decimal ordanalId = GetLnDecItem(lineIdArray, idx); idx++;
                decimal pickingType = GetLnDecItem(lineIdArray, idx); idx++;
                decimal prepackageid = GetLnDecItem(lineIdArray, idx); idx++;
                decimal warehouseid = GetLnDecItem(lineIdArray, idx); idx++;
                decimal locationid = GetLnDecItem(lineIdArray, idx); idx++;
                string ordersgroup = GetLnStrItem(lineIdArray, idx); idx++;
                string lu3barcode = GetLnStrItem(lineIdArray, idx); idx++;
                decimal itemid = GetLnDecItem(lineIdArray, idx); idx++;
                decimal stockstatusid = GetLnDecItem(lineIdArray, idx); idx++;
                decimal wmsstatusid = GetLnDecItem(lineIdArray, idx); idx++;
                decimal reqlotid = GetLnDecItem(lineIdArray, idx); idx++;
                decimal lotid = GetLnDecItem(lineIdArray, idx); idx++;

                #endregion

                #region compose SerialNumber Array
                string serialNoPlSql = "";
                for (int i = 0; i < pickingConfirmation.SerialNumbers?.Length; i++)
                {
                    serialNoPlSql += string.Format("\tPT_STD({0}).SERIALNO:= '{1}';\n", i, pickingConfirmation.SerialNumbers[i]);
                }
                #endregion

                #region call Procedure
                string sql = ResourseHelper.GetEmbeddedFileContent("Queries.PickingCnf.sql");
                sql = sql.Replace("/*SrnUpdate*/", serialNoPlSql);
                SqlHelper.ExecuteCommandStr(sql,
                    new OracleParameter("pTaskId", taskId),
                    new OracleParameter("pOrdanalId", ordanalId),
                    new OracleParameter("pPickingType", pickingType),
                    new OracleParameter("pOrdersgroup", ordersgroup),
                    new OracleParameter("pPrepackageid", prepackageid),
                    new OracleParameter("pWarehouseid", warehouseid),
                    new OracleParameter("pLocationid", locationid),
                    new OracleParameter("pLu3barcode", lu3barcode),
                    new OracleParameter("pItemid", itemid),
                    new OracleParameter("pStockstatusid", stockstatusid),
                    new OracleParameter("pWmsstatusid", wmsstatusid),
                    new OracleParameter("pReqLotid", reqlotid),
                    new OracleParameter("pLotid", lotid),
                    new OracleParameter("pQty", pickingConfirmation.Qty),
                    new OracleParameter("pPickerId", pickingConfirmation.PickerId),
                    new OracleParameter("pScannedBarcode", pickingConfirmation.ScannedBarcode)
                    );
                #endregion

                Log.Write("Success confirmation for line id " + pickingConfirmation.LineId);
                return Ok();
            }catch (Exception ex)
            {
                string errorMsg = "Error while confirming picking " + " at (" + errorPoint + ")" + ex.Message;
                Log.Write(errorMsg);
                //return InternalServerError(new Exception(errorMsg, ex));
                return BadRequest(errorMsg);
            }
        }
        private decimal GetLnDecItem(string[] lineIdArray, int idx)
        {
            string id = GetLnStrItem(lineIdArray, idx);
            if (string.IsNullOrEmpty(id)) return 0;
            return Convert.ToDecimal(id);
        }
        private string GetLnStrItem(string[] lineIdArray,int idx)
        {
            string id= (lineIdArray.Length >= idx + 1) ? lineIdArray[idx] : "";
            return id;
        }
    }
}
