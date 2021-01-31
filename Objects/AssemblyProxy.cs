using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TCAdmin.Interfaces.Database;
using TCAdmin.SDK.Objects;

namespace Alexr03.Common.TCAdmin.Objects
{
    public class AssemblyProxy : ObjectBase
    {
        public AssemblyProxy()
        {
            this.TableName = "ar_common_proxies";
            this.KeyColumns = new[] {"id"};
            this.UseApplicationDataField = true;
        }

        public int Id => this.GetIntegerValue("id");

        private string AssemblyStr => this.GetStringValue("assembly");

        public Assembly Assembly => AssemblyStr.ToLower().EndsWith(".dll") ? Assembly.LoadFile(AssemblyStr) : Assembly.Load(AssemblyStr);

        public static IList<AssemblyProxy> GetAssemblyProxies()
        {
            return new AssemblyProxy().GetObjectList(new WhereList()).Cast<AssemblyProxy>().ToList();
        }
    }
}