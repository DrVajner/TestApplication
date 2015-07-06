using Oracle.DataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OracleConn
{
    public class OraConnection
    {

        /*
File "Mandatory/
Optional" Description
oci.dll Mandatory Client code library.
ociw32.dll Optional Necessary for Microsoft ODBC or OLEDB (both are not actively supported anymore).
Oracle.DataAccess.dll Mandatory .NET managed Oracle ODP.NET client.
orannzsbb12.dll Mandatory Security library
oraocci12.dll Optional Necessary for OCCI usage.
oraociei12.dll Swapable Data and Code library - all code pages. Swapable with oraociicus12.dll.
oraociicus12.dll Swapable Data and Code library - Unicode code pages. Swapable with oraociei12.dll.
oraons.dll Mandatory Oracle Notification System library used by OCI internally.
OraOps12.dll Mandatory Oracle Provider Services library - user by .NET ODP.NET client.
         */
        private OracleConnection _connection;
        private OracleCommand _command;
        private string _connectionString;

        public event Action<OraLoggingEventArgs> OraLogging;

        public OraConnection()
            : this(null)
        {

        }

        public OraConnection(string connectionString)
        {
            if (!string.IsNullOrWhiteSpace(connectionString))
            {
                _connectionString = connectionString;
            }
        }

        public void CreateNewConnection()
        {
            if (string.IsNullOrWhiteSpace(_connectionString))
            {
                throw new OraConnectionException("Connection string is not set!", null, null, null);
            }

            _connection = new OracleConnection();
            _connection.ConnectionString = _connectionString;
            _connection.Open();
        }

        public void CreateNewConnection(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new ArgumentNullException("connectionString", "Connection string has not been set!");
            }

            _connectionString = connectionString;

            CreateNewConnection();
        }

        public void Reconnect(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new ArgumentNullException("connectionString", "Connection string has not been set!");
            }

            _connectionString = connectionString;

            if (_connection.State == ConnectionState.Open ||
                _connection.State == ConnectionState.Executing ||
                _connection.State == ConnectionState.Fetching)
            {
                _connection.Close();
            }

            CreateNewConnection();
        }

        public void CloseConnection()
        {
            _connection.Close();
        }

        public void ExecuteNonQuery(string statement, params object[] parameters)
        {
            SetCommand(statement, parameters);
            ExecuteNonQuery();
        }
        public void ExecuteNonQuery(string statement)
        {
            SetCommand(statement, null);
            ExecuteNonQuery();
        }

        public void ExecuteNonQuery()
        {
            if (_command == null)
            {
                throw new OraCommandNotSetException("Oracle statement has not been provided", _connectionString, null, null);
            }

            if (OraLogging != null)
            {
                OraLogging(new OraLoggingEventArgs(_connectionString, _command.CommandText));
            }

            try
            {
                _command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                ex.Data.Add("SqlStatement", _command.CommandText.ToString());
                throw ex;
            }
            finally
            {
                _command = null;
            }
        }

        public object GetScalar(string statement, params object[] parameters)
        {
            SetCommand(statement, parameters);
            return GetScalar();
        }

        public object GetScalar(string statement)
        {
            SetCommand(statement, null);
            return GetScalar();
        }

        public object GetScalar()
        {
            if (_command == null)
            {
                throw new OraCommandNotSetException("Oracle statement has not been provided", _connectionString, null, null);
            }

            if (OraLogging != null)
            {
                OraLogging(new OraLoggingEventArgs(_connectionString, _command.CommandText));
            }

            try
            {
                return _command.ExecuteScalar();
            }
            catch (Exception ex)
            {
                ex.Data.Add("SqlStatement", _command.CommandText.ToString());
                throw ex;
            }
            finally
            {
                _command = null;
            }
        }

        public string GetFullPrecisionScalar(string statement, params object[] parameters)
        {
            SetCommand(statement, parameters);
            return GetFullPrecisionScalar();
        }

        public string GetFullPrecisionScalar()
        {
            if (_command == null)
            {
                throw new OraCommandNotSetException("Oracle statement has not been provided", _connectionString, null, null);
            }

            if (OraLogging != null)
            {
                OraLogging(new OraLoggingEventArgs(_connectionString, _command.CommandText));
            }

            try
            {
                var dataAdapter = new OracleDataAdapter(_command);
                dataAdapter.SafeMapping.Add("*", typeof(string));
                var dataSet = new DataSet();
                dataAdapter.Fill(dataSet);
                if (dataSet.Tables != null && dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows != null && dataSet.Tables[0].Rows.Count > 0)
                {
                    return dataSet.Tables[0].Rows[0][0].ToString();
                }

                return null;
            }
            catch (Exception ex)
            {
                ex.Data.Add("SqlStatement", _command.CommandText.ToString());
                throw ex;
            }
            finally
            {
                _command = null;
            }
        }

        public DataTable GetDataTable(string statement, params object[] parameters)
        {
            SetCommand(statement, parameters);
            return GetDataTable();
        }

        public DataTable GetDataTable(string statement)
        {
            SetCommand(statement, null);
            return GetDataTable();
        }

        public DataTable GetDataTable()
        {
            if (_command == null)
            {
                throw new OraCommandNotSetException("Oracle statement has not been provided", _connectionString, null, null);
            }

            if (OraLogging != null)
            {
                OraLogging(new OraLoggingEventArgs(_connectionString, _command.CommandText));
            }

            try
            {
                var adapter = new OracleDataAdapter(_command);
                var dataTable = new DataTable();
                adapter.Fill(dataTable);

                return dataTable;
            }
            catch (Exception ex)
            {
                ex.Data.Add("SqlStatement", _command.CommandText.ToString());
                throw ex;
            }
            finally
            {
                _command = null;
            }
        }

        public void SetCommand(string statement, params object[] parameters)
        {
            if (string.IsNullOrWhiteSpace(statement))
            {
                throw new ArgumentNullException("statement", "Statement has not been provided!");
            }

            _command = new OracleCommand(statement, _connection);
            _command.BindByName = true;
            if (parameters != null)
            {
                for (int i = 0; i < parameters.Length; i++)
                {
                    _command.CommandText = _command.CommandText.Replace("{" + i.ToString() + "}", ":par" + i.ToString());
                    _command.Parameters.Add(new OracleParameter("par" + i.ToString(), GetOracleType(parameters[i]))
                    {
                        Value = parameters[i],
                        Direction = ParameterDirection.Input
                    });
                    //_command.Parameters.Add("par" + i.ToString(), GetOracleType(parameters[i])).Value = parameters[i];
                }
            }
        }

        private OracleDbType GetOracleType(object value)
        {
            if (value == null)
            {
                return OracleDbType.Raw;
            }

            var type = value.GetType();
            if (type == typeof(DateTime))
            {
                return OracleDbType.Date;
            }
            else if (type == typeof(byte))
            {
                return OracleDbType.Byte;
            }
            else if (type == typeof(short))
            {
                return OracleDbType.Int16;
            }
            else if (type == typeof(int))
            {
                return OracleDbType.Int32;
            }
            else if (type == typeof(long))
            {
                return OracleDbType.Int64;
            }
            else if (type == typeof(char))
            {
                return OracleDbType.Char;
            }
            else if (type == typeof(string))
            {
                return OracleDbType.Varchar2;
            }
            else if (type == typeof(float))
            {
                return OracleDbType.Decimal;
            }
            else if (type == typeof(decimal))
            {
                return OracleDbType.Decimal;
            }
            else if (type == typeof(double))
            {
                return OracleDbType.Double;
            }
            else
            {
                throw new NotSupportedException("Unsupported parameter type.");
            }
        }
    }
}
