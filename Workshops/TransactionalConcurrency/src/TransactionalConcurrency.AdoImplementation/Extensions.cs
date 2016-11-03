using System.Data;

namespace TransactionalConcurrency.AdoImplementation
{
    public static class Extensions
    {
        public static IDataParameter GetParameter(this IDbCommand command, string name, object value)
        {
            var parameter = command.CreateParameter();
            parameter.ParameterName = string.Format("@{0}", name);
            parameter.Value = value;

            return parameter;
        }
    }
}