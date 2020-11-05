using System;
using System.Globalization;
using System.Web.Mvc;

namespace Alexr03.Common.TCAdmin.Web.Binders
{
    public class DynamicTypeBaseBinderAttribute : CustomModelBinderAttribute
    {
        public Type Type = typeof(DynamicTypeBaseBinder);
        
        public DynamicTypeBaseBinderAttribute()
        {
            if (!typeof(IModelBinder).IsAssignableFrom(Type))
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture,
                    "An error occurred when trying to create the IModelBinder '{0}'. Make sure that the binder has a public parameterless constructor.",
                    Type.FullName), nameof(Type));
            }
        }

        /// <summary>Retrieves an instance of the model binder.</summary>
        /// <returns>A reference to an object that implements the <see cref="T:System.Web.Mvc.IModelBinder" /> interface.</returns>
        /// <exception cref="T:System.InvalidOperationException">An error occurred while an instance of the model binder was being created.</exception>
        public override IModelBinder GetBinder()
        {
            try
            {
                return (IModelBinder) Activator.CreateInstance(Type);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture,
                    "An error occurred when trying to create the IModelBinder '{0}'. Make sure that the binder has a public parameterless constructor.",
                    Type.FullName), ex);
            }
        }
    }
}