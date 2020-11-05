using System;
using System.Web.Mvc;
using IModelBinder = System.Web.Mvc.IModelBinder;
using ModelBindingContext = System.Web.Mvc.ModelBindingContext;

namespace Alexr03.Common.TCAdmin.Web.Binders
{
    public class DynamicTypeBaseBinder : IModelBinder
    {
        public string IdProperty { get; set; } = "id";
        
        public DynamicTypeBaseBinder()
        {

        }

        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
            {
                throw new ArgumentNullException(nameof(bindingContext));
            }

            var idResult = bindingContext.ValueProvider.GetValue(IdProperty);
            if (idResult == null)
            {
                return null;
            }
            
            var id = (int) idResult.ConvertTo(typeof(int));
            
            return Activator.CreateInstance(bindingContext.ModelType, id);
        }
    }
}