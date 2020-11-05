using System.Data;
using System.Linq;
using System.Web.Mvc;
using Alexr03.Common.TCAdmin.Objects;
using Alexr03.Common.Web.Attributes.ActionFilters;
using Alexr03.Common.Web.Extensions;
using Newtonsoft.Json.Linq;
using TCAdmin.SDK.Web.MVC.Controllers;
using TCAdmin.Web.MVC;

namespace Alexr03.Common.TCAdmin.Web.Controllers
{
    [ExceptionHandler]
    [RoutePrefix("ArCommon")]
    public class ArCommonController : BaseController
    {
        public ActionResult Index()
        {
            return View();
        }

        [ParentAction("Index")]
        public ActionResult Configure(string configName)
        {
            var config = ModuleConfiguration.GetModuleConfiguration(Globals.ModuleId, configName);
            var jObject = config.Parse<JObject>();
            ViewData.TemplateInfo = new TemplateInfo
            {
                HtmlFieldPrefix = config.Type.Name,
            };
            return PartialView(config.View, jObject.ToObject(config.Type));
        }

        [HttpPost]
        public ActionResult Configure(string configName, FormCollection model)
        {
            var config = ModuleConfiguration.GetModuleConfiguration(Globals.ModuleId, configName);
            var o = model.Parse(ControllerContext, config.Type);
            config.SetConfiguration(o);
            return PartialView(config.View, o);
        }

        [ParentAction("Index")]
        [DisallowDirect]
        public ActionResult Sql()
        {
            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Sql(string sqlScript)
        {
            var databaseProvider = global::TCAdmin.SDK.Database.DatabaseManager.CreateDatabaseManager(true);
            if (sqlScript.ToLower().StartsWith("select"))
            {
                var dataTable = databaseProvider.Execute(sqlScript);
                databaseProvider.Disconnect();
                var convertDataTableToString = ConvertDataTableToHtml(dataTable);
                return Json(new
                {
                    table = convertDataTableToString
                });
            }
            var executeNonQuery = databaseProvider.ExecuteNonQuery(sqlScript);
            databaseProvider.Disconnect();
            return Json(new
            {
                table = $"Query affected <strong>{executeNonQuery}</strong> rows."
            });
        }

        [Route("Sql/Save")]
        public ActionResult SaveSqlScript(string name, string contents)
        {
            var sqlScript = new SqlScript
            {
                Name = name,
                Contents = contents
            };
            sqlScript.GenerateKey();
            sqlScript.Save();

            return Json(new
            {
                Message = $"Successfully saved script as <strong>{name}</strong>"
            });
        }

        [Route("Sql/Load")]
        public ActionResult LoadSqlScript(int id)
        {
            var sqlScript = new SqlScript(id);

            return Json(new
            {
                sqlScript.Id,
                sqlScript.Name,
                sqlScript.Contents,
            }, JsonRequestBehavior.AllowGet);
        }

        [Route("Sql/Delete")]
        public ActionResult DeleteSqlScript(int id)
        {
            var sqlScript = new SqlScript(id);
            sqlScript.Delete();

            return Json(new
            {
                Message = $"Successfully deleted script with Id <strong>{id}</strong>."
            }, JsonRequestBehavior.AllowGet);
        }

        [Route("Sql/GetAllScripts")]
        public ActionResult GetSqlScripts()
        {
            var sqlScripts = SqlScript.GetSqlScripts();
            return Json(sqlScripts.Select(x => new
            {
                x.Id,
                x.Name,
                x.Contents
            }), JsonRequestBehavior.AllowGet);
        }

        public static string ConvertDataTableToHtml(DataTable dt)
        {
            var html = "<table id='sqltable'>";
            //add header row
            html += "<tr>";
            for (var i = 0; i < dt.Columns.Count; i++)
                html += "<th>" + dt.Columns[i].ColumnName + "</th>";
            html += "</tr>";
            //add rows
            for (var i = 0; i < dt.Rows.Count; i++)
            {
                html += "<tr>";
                for (var j = 0; j < dt.Columns.Count; j++)
                    html += "<td>" + System.Web.HttpUtility.HtmlEncode(dt.Rows[i][j]) + "</td>";
                html += "</tr>";
            }

            html += "</table>";
            return html;
        }
    }
}