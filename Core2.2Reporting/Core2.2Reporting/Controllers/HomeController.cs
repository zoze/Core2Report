using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Core2._2Reporting.Models;

namespace Core2._2Reporting.Controllers
{
    public class HomeController : Controller
    {
        private IHostingEnvironment _hostingEnvironment;
        private IConfiguration _configuration;

        public HomeController(IHostingEnvironment hostingEnvironment, IConfiguration configuration)
        {
            _hostingEnvironment = hostingEnvironment;
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            return View();
        }

        //if you want embeded file in view
        public IActionResult ReportViewer()
        {
            var reportFile = GenerateReport();
            string fileName = Path.GetFileName(reportFile);
            
            ViewData["Message"] = "Report Name";
            ViewData["ReportName"] = fileName;
            return View();
        }

        //If you want download file directly 
        public IActionResult ReportDownload()
        {
            var reportFile = GenerateReport();
            string fileName = Path.GetFileName(reportFile);
            
            byte[] fileBytes = System.IO.File.ReadAllBytes(reportFile);
            return File(fileBytes, "application/force-download", fileName);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private string GenerateReport()
        {

            #region GenerateTableInDataBase
            //CREATE TABLE [dbo].[Kinds]([kind] [bigint] IDENTITY(1,1) NOT NULL,[kindst] [nvarchar](450) NULL)
            //INSERT INTO[dbo].[Kinds] ([kindst]) VALUES('Kind1');
            //INSERT INTO[dbo].[Kinds] ([kindst]) VALUES('Kind2');
            //INSERT INTO[dbo].[Kinds] ([kindst]) VALUES('Kind3');
            //INSERT INTO[dbo].[Kinds] ([kindst]) VALUES('Kind4');
            #endregion

            var _webRootPath = _hostingEnvironment.WebRootPath;


            //string _sqlQuery = "SELECT * FROM Kinds where kind=1";
            string _sqlQuery = "SELECT * FROM Kinds";
            string _conectionString = _configuration["ConnectionStrings:MyAppConnection"];

            System.Data.DataTable _dataTable = new System.Data.DataTable();
            using (System.Data.Common.DbDataAdapter _dataAdapter = new System.Data.SqlClient.SqlDataAdapter(_sqlQuery, _conectionString))
            {
                _dataAdapter.Fill(_dataTable);
            }

            //string _reportPath = Path.Combine(webRootPath, "ReportsFiles", "ReportWithParameter.rdlc");
            string _reportPath = Path.Combine(_webRootPath, "ReportsFiles", "ReportWithoutParameter.rdlc");


            AspNetCore.Reporting.LocalReport _localReport = new AspNetCore.Reporting.LocalReport(_reportPath);


            System.Collections.Generic.Dictionary<string, string> parameters =new System.Collections.Generic.Dictionary<string, string>();
            #region Parameters
            //If You Need Pass Parameter to rdlc Report : 

            //parameters["ReportParameterKind"] = "1";
            //parameters["TEN_ID"] = "45";
            //parameters["START_DATE"] = "2018";
            //parameters["END_DATE"] = "2018";
            // parameters.Add("in_logo", "base64");
            #endregion


            _localReport.AddDataSource("DataSet1", _dataTable); // DataSet1 is the name of the DataSet in the report



            #region RenderType
            // var reportResult = _localReport.Execute(AspNetCore.Reporting.RenderType.Html, 1, null, "");
            // var reportResult = _localReport.Execute(AspNetCore.Reporting.RenderType.Rpl, 1, null, ""); // Kaboom 
            // var reportResult = _localReport.Execute(AspNetCore.Reporting.RenderType.Html, 2, null, "");
            // var reportResult = _localReport.Execute(AspNetCore.Reporting.RenderType.Pdf, 1, null, "");
            // var reportResult = _localReport.Execute(AspNetCore.Reporting.RenderType.Excel, 1, null, "");
            // var reportResult = _localReport.Execute(AspNetCore.Reporting.RenderType.ExcelOpenXml, 1, null, "");
            // var reportResult = _localReport.Execute(AspNetCore.Reporting.RenderType.Word, 1, null, "");
            // var reportResult = _localReport.Execute(AspNetCore.Reporting.RenderType.WordOpenXml, 1, null, "");
            // var reportResult = _localReport.Execute(AspNetCore.Reporting.RenderType.Atom, 1, null, "");
            // var reportResult = _localReport.Execute(AspNetCore.Reporting.RenderType.Xml, 1, null, "");
            // var reportResult = _localReport.Execute(AspNetCore.Reporting.RenderType.Json, 1, null, "");// KABOOM 
            // var reportResult = _localReport.Execute(AspNetCore.Reporting.RenderType.Csv, 1, null, "");
            #endregion


            var reportResult = _localReport.Execute(AspNetCore.Reporting.RenderType.Pdf, 1, parameters, "");

            var _totalPage= reportResult.TotalPages;


            string dir = System.IO.Path.Combine(_webRootPath, "Reports");
            string newGuid = Guid.NewGuid().ToString("N");
            string newFileName = $"{newGuid}.pdf";

            var filePath = System.IO.Path.Combine(dir, newFileName);
            System.IO.File.WriteAllBytes(filePath, reportResult.MainStream);
            return filePath;

            #region SaveAsType
            // System.IO.File.WriteAllBytes(System.IO.Path.Combine(dir, "foo.htm"), rr.MainStream);
            // System.IO.File.WriteAllBytes(System.IO.Path.Combine(dir, "foo.css"), rr.SecondaryStream);
            // System.IO.File.WriteAllBytes(System.IO.Path.Combine(dir, "foo.xls"), rr.MainStream);
            // System.IO.File.WriteAllBytes(System.IO.Path.Combine(dir, "foo.xlsx"), rr.MainStream);
            // System.IO.File.WriteAllBytes(System.IO.Path.Combine(dir, "foo.doc"), rr.MainStream);
            // System.IO.File.WriteAllBytes(System.IO.Path.Combine(dir, "foo.docx"), rr.MainStream);
            // System.IO.File.WriteAllBytes(System.IO.Path.Combine(dir, "foo.tiff"), rr.MainStream);
            // System.IO.File.WriteAllBytes(System.IO.Path.Combine(dir, "foo.atom.xml"), rr.MainStream);
            // System.IO.File.WriteAllBytes(System.IO.Path.Combine(dir, "foo.xml"), rr.MainStream);
            // System.IO.File.WriteAllBytes(System.IO.Path.Combine(dir, "foo.json"), rr.MainStream);
            // System.IO.File.WriteAllBytes(System.IO.Path.Combine(dir, "foo.csv"), rr.MainStream);
            // System.IO.File.WriteAllBytes(System.IO.Path.Combine(dir, "foo.rpl"), rr.MainStream); // BOOM 
            // System.IO.File.WriteAllBytes(System.IO.Path.Combine(dir, "foo.htm"), rr.MainStream);
            //System.IO.File.WriteAllBytes(System.IO.Path.Combine(dir, "foo.pptx"), rr.MainStream);
            #endregion



        }
    }
}
