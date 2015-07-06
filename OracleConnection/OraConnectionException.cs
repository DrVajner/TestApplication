using System;

namespace OracleConn
{
    public class OraConnectionException : Exception
    {
        private readonly string _connectionString;
        private readonly string _statement;

        public string ConnectionString
        {
            get { return _connectionString; }
        }
        public string Statement
        {
            get { return _statement; }
        }

        public OraConnectionException(string message, string connectionString, string statement, Exception innerException)
            : base(message, innerException)
        {
            _connectionString = connectionString;
            _statement = statement;
        }
    }
}
