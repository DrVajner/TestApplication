using System;

namespace OracleConn
{
    public class OraLoggingEventArgs : EventArgs
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

        public OraLoggingEventArgs(string connectionString, string statement)
        {
            _connectionString = connectionString;
            _statement = statement;
        }
    }
}
