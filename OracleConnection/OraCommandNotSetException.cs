using System;

namespace OracleConn
{
    public sealed class OraCommandNotSetException : OraConnectionException
    {
        public OraCommandNotSetException(string message, string connectionString, string statement, Exception innerException)
            : base(message, connectionString, statement, innerException)
        {
            
        }
    }
}
