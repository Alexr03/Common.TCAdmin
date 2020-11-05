using System.Web.Mvc;
using Alexr03.Common.TCAdmin.Objects;
using Alexr03.Common.Web.Extensions;
using Newtonsoft.Json.Linq;

namespace Alexr03.Common.TCAdmin.Extensions
{
    public static class DynamicTypeBaseExtensions
    {
        public static bool UpdateConfigurationFromCollection(this DynamicTypeBase dynamicTypeBase, FormCollection formCollection,
            ControllerContext controllerContext)
        {
            var bindModel = formCollection.Parse(controllerContext, dynamicTypeBase.Configuration.Type,
                dynamicTypeBase.Type.Name);
            return dynamicTypeBase.Configuration.SetConfiguration(bindModel);
        }

        public static ActionResult GetConfigurationResultView(this DynamicTypeBase dynamicTypeBase,
            ControllerBase controllerBase)
        {
            return GetConfigurationResultView(dynamicTypeBase, controllerBase, dynamicTypeBase.Type.Name);
        }

        public static ActionResult GetConfigurationResultView(this DynamicTypeBase dynamicTypeBase,
            ControllerBase controllerBase, string htmlFieldPrefix)
        {
            var config = dynamicTypeBase.Configuration;
            var jObject = config.Parse<JObject>();
            controllerBase.ViewData.Model = jObject.ToObject(config.Type);
            controllerBase.ViewData.TemplateInfo = new TemplateInfo
            {
                HtmlFieldPrefix = htmlFieldPrefix,
            };
            return new PartialViewResult
            {
                ViewName = config.View,
                ViewData = controllerBase.ViewData,
                TempData = controllerBase.TempData,
            };
        }
    }
}