using MySql.Data.MySqlClient;
using System;
using System.Configuration;
using System.Data;
using System.IO;

namespace DatabaseDeploy.Impl
{
    public class DataExecutor
    {
        private const string TrackingTableName = "DatabaseTrack";
        public string ConnectionString = ConfigurationManager.ConnectionStrings["DefaultString"].ToString();
        public string DbName = ConfigurationManager.AppSettings["DBName"];
        private MySqlConnection _connection;

        public DataExecutor()
        {
            _connection = new MySqlConnection(ConnectionString);
        }

        public void ExecuteFile(string file)
        {
            var fileStream = new FileStream(file, FileMode.Open, FileAccess.Read);
            var reader = new StreamReader(fileStream);

            try
            {
                var text = reader.ReadToEnd();
                ExceuteScriptData(text);
            }
            finally
            {
                reader.Close();
                reader.Dispose();
                fileStream.Close();
                fileStream.Dispose();
            }
        }

        public void SetDatabaseBlueJay()
        {
            ConnectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ToString();
            _connection = new MySqlConnection(ConnectionString);
        }

        private void ExceuteScriptData(string text)
        {
            MySqlTransaction trans = null;

            try
            {
                _connection.Open();
                trans = _connection.BeginTransaction();
                var script = new MySqlScript(_connection, text);

                script.Execute();

                trans.Commit();
            }
            catch (MySqlException)
            {
                if (trans != null)
                {
                    trans.Rollback();
                }

                throw;
            }
            finally
            {
                if (_connection.State != ConnectionState.Closed)
                {
                    if (trans != null)
                    {
                        trans.Dispose();
                    }

                    _connection.Close();
                }
            }
        }

        private int ExecuteNonQuery(string text, params MySqlParameter[] parameters)
        {
            MySqlTransaction trans = null;

            try
            {
                var cmd = new MySqlCommand(text, _connection);
                _connection.Open();
                trans = _connection.BeginTransaction();

                if (parameters != null && parameters.Length > 0)
                {
                    cmd.Parameters.AddRange(parameters);
                }

                var count = cmd.ExecuteNonQuery();

                trans.Commit();

                return count;
            }
            catch (MySqlException)
            {
                if (trans != null)
                {
                    trans.Rollback();
                }

                throw;
            }
            finally
            {
                if (_connection.State != ConnectionState.Closed)
                {
                    if (trans != null)
                    {
                        trans.Dispose();
                    }

                    _connection.Close();
                }
            }
        }

        private object ExecuteScalar(string text, params MySqlParameter[] parameters)
        {
            MySqlTransaction trans = null;

            try
            {
                var cmd = new MySqlCommand(text, _connection);
                _connection.Open();
                trans = _connection.BeginTransaction();

                if (parameters != null && parameters.Length > 0)
                {
                    cmd.Parameters.AddRange(parameters);
                }

                var obj = cmd.ExecuteScalar();

                trans.Commit();
                return obj;
            }
            catch (MySqlException)
            {
                if (trans != null)
                {
                    trans.Rollback();
                }

                throw;
            }
            finally
            {
                if (_connection.State != ConnectionState.Closed)
                {
                    if (trans != null)
                    {
                        trans.Dispose();
                    }

                    _connection.Close();
                }
            }
        }

        public bool DatabaseExists()
        {
            var query = "SELECT SCHEMA_NAME FROM INFORMATION_SCHEMA.SCHEMATA WHERE SCHEMA_NAME = '" + DbName + "'";
            var data = ExecuteScalar(query);
            return data != null && data != DBNull.Value;
        }

        private void CreateDatabaseTrackingTable()
        {
            string createQuery = "Use " + DbName + "; Create Table " + TrackingTableName + " (Id bigint not null AUTO_INCREMENT, FolderName varchar(100) not null, LastExecutedFile varchar(500) not null, LastExecutedOn DateTime not null, Primary Key (Id));";
            ExecuteNonQuery(createQuery);
        }

        public bool TrackingTableExists()
        {
            var query = string.Format("SELECT Table_Schema FROM INFORMATION_SCHEMA.Tables WHERE Table_Schema = '{0}' and Table_Name = '{1}' Limit 1;", _connection.Database, TrackingTableName);
            var data = ExecuteScalar(query);
            return data != null && data != DBNull.Value;
        }

        public string GetLastExecutedScriptfortheFolder(string folder)
        {
            if (!TrackingTableExists())
            {
                CreateDatabaseTrackingTable();
            }

            var query =
                string.Format(
                    "Select LastExecutedFile from {0} where FolderName = '{1}' order by LastExecutedOn desc Limit 1;",
                    TrackingTableName, folder);
            
            var obj = ExecuteScalar(query);
            if (obj == null || obj == DBNull.Value) return string.Empty;
            return obj.ToString();
        }

        public void UpdateTablewithLastPoint(string folderName, string fileExecuted)
        {
            if (!TrackingTableExists())
            {
                CreateDatabaseTrackingTable();
            }

            var query = string.Format("Use " + DbName + "; Insert Into {0} (FolderName, LastExecutedFile, LastExecutedOn) values (@folderName, @lastFileExecuted, @lastExecutedOn); ", TrackingTableName);

            var parameterFolderName = new MySqlParameter("@folderName", MySqlDbType.VarChar) { Value = folderName };
            var parameterFileExecuted = new MySqlParameter("@lastFileExecuted", MySqlDbType.VarChar) { Value = fileExecuted };
            var parameterExecutedOn = new MySqlParameter("@lastExecutedOn", MySqlDbType.DateTime) { Value = DateTime.Now };

            ExecuteNonQuery(query, parameterFolderName, parameterFileExecuted, parameterExecutedOn);
        }
    }
}
