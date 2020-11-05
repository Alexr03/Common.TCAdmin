using TCAdmin.SDK.Objects;

namespace Alexr03.Common.TCAdmin.Misc.Strings
{
    public static class VariableReplacement
    {
        public static string ReplaceVariables(this string query, global::TCAdmin.GameHosting.SDK.Objects.Service service = null, User user = null, Server server = null, Datacenter datacenter = null)
        {
            var input = new global::TCAdmin.SDK.Scripting.InputParser(query);
            service?.ReplacePropertyValues(input);
            service?.ReplaceCustomVariables(new global::TCAdmin.GameHosting.SDK.Objects.Game(service.GameId).CustomVariables, service.Variables, input);
            user?.ReplacePropertyValues(input);
            server?.ReplacePropertyValues(input);
            datacenter?.ReplacePropertyValues(input);

            var output = input.GetOutput();
            return output;
        }
        
        public static string ReplaceVariables(this string query, ObjectBase objectBase)
        {
            var input = new global::TCAdmin.SDK.Scripting.InputParser(query);
            objectBase?.ReplacePropertyValues(input);
            var output = input.GetOutput();
            return output;
        }
    }
}