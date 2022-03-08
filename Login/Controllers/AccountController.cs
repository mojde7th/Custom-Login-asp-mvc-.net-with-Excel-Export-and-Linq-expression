using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Login.Models;
using System.Data.SqlClient;
using System.Data;
using OfficeOpenXml;
using System.Drawing;
using System.IO;
using Microsoft.Win32;

namespace Login.Controllers
{
  
    public class AccountController : Controller
    {
        SqlConnection con = new SqlConnection();
        SqlCommand com = new SqlCommand();
        SqlDataReader dr;
       
        // GET: Account
        [HttpGet]
       
        public ActionResult Login()
        {
            return View();
        }
        void connectionString()
        {
            con.ConnectionString = "Data Source=(local);Initial Catalog=of1;Integrated Security=True";
        }
        [HttpPost]
        public ActionResult verify(UserAccounts acc)
        {
            DataTable dt = new DataTable();
            connectionString();
            con.Open();
            com.Connection = con;
            var ff = acc.Username;
            TempData["idd"] = ff;
            com.CommandText = "SELECT * FROM [of1].[dbo].[User] where Username='"+acc.Username+"' and Pass='"+acc.Pass+"'";
            dr = com.ExecuteReader();
            if (dr.Read())
            {
                con.Close();
                return View("Create");
            }
            else
            {
                con.Close();
                return View("Error");
            }
            
        }
        Entities2 db = new Entities2();
        public ActionResult but()
        {
            var Userid = TempData["idd"];
            DataTable dt = new DataTable();
       
            List<Employee> query = (from Employees in db.Employees
                        where
                              (from Users in db.Users
                               where
                     Users.Username == "admin"
                               select new
                               {
                                   Users.CompanyCode
                               }).Contains(new { CompanyCode = Employees.CompanyCodee })
                        select Employees).ToList();
            //var nel = sd.UserAccounts.Where(x => x.Username == Userid).ToList();
            return View(query);
            //sda.Fill(dt);
          
        }

        public void ExporttoExcel()
        {
            List<Employee> query = (from Employees in db.Employees
                                    where
                                          (from Users in db.Users
                                           where
                                 Users.Username == "admin"
                                           select new
                                           {
                                               Users.CompanyCode
                                           }).Contains(new { CompanyCode = Employees.CompanyCodee })
                                    select Employees).ToList();
            ExcelPackage p1 = new ExcelPackage();
            ExcelWorksheet ew = p1.Workbook.Worksheets.Add("Report");
            ew.Cells["A1"].Value= "Communication";
            ew.Cells["B1"].Value = "Com1";

            ew.Cells["A2"].Value = "Report";
            ew.Cells["B2"].Value = "Report1";

            ew.Cells["A3"].Value = "Date";
            ew.Cells["B3"].Value = string.Format("{0:dd MMMM yyyy} at {0:H: mm tt}",DateTimeOffset.Now);

            ew.Cells["A6"].Value = "Reg";
            ew.Cells["B6"].Value = "CompanyCodee";
            ew.Cells["C6"].Value = "PayrollCodee";
            ew.Cells["D6"].Value = "Statement";
            ew.Cells["E6"].Value = "Personel";
            foreach (var item in query)
            {
                int rowStart = 7;
                if (item.CompanyCodee =="0")
                {
                    ew.Row(rowStart).Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    ew.Row(rowStart).Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml(string.Format("pink")));
                }
                ew.Cells[String.Format("A{0}", rowStart)].Value = item.Reg;
                ew.Cells[String.Format("B{0}", rowStart)].Value = item.CompanyCodee;
                ew.Cells[String.Format("C{0}", rowStart)].Value = item.PayrollCodee;
                ew.Cells[String.Format("D{0}", rowStart)].Value = item.Statement;
                ew.Cells[String.Format("E{0}", rowStart)].Value = item.Personel;
                rowStart++;
            }
            ew.Cells["A:AZ"].AutoFitColumns();
            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content.disposition","attachment: filename="+"ExcelReport.xlsx");
            Response.BinaryWrite(p1.GetAsByteArray());
            Response.End();
        }

    }
}