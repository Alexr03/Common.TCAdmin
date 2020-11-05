using System.Collections.Generic;
using System.Linq;
using TCAdmin.Interfaces.Database;
using TCAdmin.SDK.Objects;

namespace Alexr03.Common.TCAdmin.Objects
{
    public class SqlScript : ObjectBase 
    {
        public SqlScript()
        {
            this.TableName = "ar_common_sql_scripts";
            this.KeyColumns = new[] {"id"};
            this.SetValue("id", -1);
            this.UseApplicationDataField = true;
        }

        public SqlScript(int id) : this()
        {
            this.SetValue("id", id);
            this.ValidateKeys();
            if (!this.Find())
            {
                throw new KeyNotFoundException("Could not find SQL Script with ID " + id);
            }
        }

        public int Id => this.GetIntegerValue("id");
        
        public string Name
        {
            get => this.GetStringValue("name");
            set => this.SetValue("name", value);
        }

        public string Contents
        {
            get => this.GetStringValue("contents");
            set => this.SetValue("contents", value);
        }

        public static List<SqlScript> GetSqlScripts()
        {
            return new SqlScript().GetObjectList(new WhereList()).Cast<SqlScript>().ToList();
        }
    }
}