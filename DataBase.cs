using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using System.Threading.Tasks;
using Npgsql;
using NpgsqlTypes;

namespace ManagementAccounting
{
    public class DataBase : IDataBase
    {
        public NpgsqlConnection Connection { get; }
        private object locker { get; }

        public DataBase()
        {
            Connection = new NpgsqlConnection();
            locker = new object();
        }

        public Task<List<T>> ExecuteReaderAsync<T>(Func<DbDataRecord, T> getItem, string commandText)
        {
            var task = new Task<List<T>>(() =>
            {
                lock (locker)
                {
                    var items = new List<T>();
                    Connection.Open();
                    using (var cmd = new NpgsqlCommand())
                    {
                        cmd.Connection = Connection;

                        cmd.CommandText = commandText;
                        var reader = cmd.ExecuteReader();
                        foreach (DbDataRecord item in reader)
                        {
                            items.Add(getItem(item));
                            //items.Add(block.GetBlockItemFromDataBase(item));
                        }
                    }
                    Connection.Close();
                    return items;
                }
            });
            task.Start();
            return task;
        }
        
        public Task ExecuteNonQueryAsync(IBlockItem item, string commandText, string exceptionMessage, Action<NpgsqlCommand> assignItemParameters = null)
        {
            var task = new Task(() =>
            {
                Connection.Open();
                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = Connection;
                    cmd.CommandText = commandText;
                    //if (hasParameters)
                    //{
                    //    if (typeOfCommand == "add")
                    //        item.AssignParametersToAddCommand(cmd);
                    //    else if (typeOfCommand == "edit")
                    //        item.AssignParametersToEditCommand(cmd);
                    //}

                    assignItemParameters?.Invoke(cmd); //if (assignItemParameters != null) assignItemParameters(cmd);
                    try
                    {
                        cmd.ExecuteNonQuery();
                    }
                    catch (Exception e)
                    {
                        item.ExceptionEvent += () => throw new Exception(exceptionMessage);
                    }
                }
                Connection.Close();
            });
            task.Start();
            return task;
        }

        public Task<decimal> GetSum(string commandText)
        {
            var task = new Task<decimal>(() =>
            {
                var result = 0m;
                Connection.Open();
                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = Connection;

                    cmd.CommandText = commandText;
                    var reader = cmd.ExecuteReader();
                    foreach (DbDataRecord item in reader)
                    {
                        result = item["Sum"] is DBNull ? result : (decimal) item["Sum"];
                    }
                }
                Connection.Close();
                return result;
            });
            task.Start();
            return task;
        }

        public async Task SignIn(string login, string password)
        {
            var cStringPostgres = $"Host=localhost;Port=5432;Database=postgres;Username={login};Password={password}";
            var isDBExist = true;
            var cStringMA = $"Host=localhost;Port=5432;Database=managementaccounting;Username={login};Password={password}";

            await using (var connectionPostgres = new NpgsqlConnection(cStringPostgres))
            {
                await connectionPostgres.OpenAsync();

                await using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = connectionPostgres;

                    cmd.CommandText = "SELECT datname FROM pg_database WHERE datname = 'managementaccounting';";
                    var reader = await cmd.ExecuteReaderAsync();
                    if (!reader.HasRows)
                    {
                        isDBExist = false;
                        await reader.CloseAsync();
                        cmd.CommandText = "CREATE DATABASE managementaccounting;";
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
            }

            if (!isDBExist)
            {
                await using (var connectionMA = new NpgsqlConnection(cStringMA))
                {
                    await connectionMA.OpenAsync();

                    await using (var cmd = new NpgsqlCommand())
                    {
                        cmd.Connection = connectionMA;
                        cmd.CommandText = $"CREATE TABLE remainders (IdM SERIAL PRIMARY KEY, MaterialTypeM INTEGER, MaterialNameM CHARACTER VARYING(50) UNIQUE)";
                        await cmd.ExecuteNonQueryAsync();

                        cmd.CommandText = "CREATE INDEX MaterialTypeIndex ON remainders (MaterialTypeM);";
                        await cmd.ExecuteNonQueryAsync();

                        cmd.CommandText =
                            "CREATE TABLE materialreceiving (IdMR SERIAL PRIMARY KEY, MaterialIdMR INTEGER REFERENCES remainders (IdM) ON DELETE CASCADE, QuantityMR NUMERIC CHECK(QuantityMR > 0), ReceiveDateMR DATE, TotalCostMR NUMERIC CHECK(TotalCostMR >= 0), RemainderMR NUMERIC CHECK(RemainderMR >= 0), NoteMR CHARACTER VARYING(50), SearchNameMR CHARACTER VARYING(10))";
                        await cmd.ExecuteNonQueryAsync();

                        cmd.CommandText = "CREATE INDEX MaterialIdIndex ON materialreceiving (MaterialIdMR);";
                        await cmd.ExecuteNonQueryAsync();

                        cmd.CommandText = "CREATE INDEX RemainderIndex ON materialreceiving (RemainderMR);";
                        await cmd.ExecuteNonQueryAsync();

                        cmd.CommandText = "CREATE INDEX SearchNameIndex ON materialreceiving (SearchNameMR);";
                        await cmd.ExecuteNonQueryAsync();

                        cmd.CommandText = "CREATE INDEX ReceiveDateIndex ON materialreceiving (ReceiveDateMR);";
                        await cmd.ExecuteNonQueryAsync();

                        cmd.CommandText =
                            "CREATE TABLE calculations (IdC SERIAL PRIMARY KEY, CalculationNameC CHARACTER VARYING(50) UNIQUE)";
                        await cmd.ExecuteNonQueryAsync();

                        cmd.CommandText =
                            "CREATE TABLE calculationitems (IdCI SERIAL PRIMARY KEY, CalculationIdCI INTEGER REFERENCES calculations (IdC) ON DELETE CASCADE, MaterialIdCI INTEGER REFERENCES remainders (IdM) ON DELETE RESTRICT, QuantityCI NUMERIC CHECK(QuantityCI > 0))";
                        await cmd.ExecuteNonQueryAsync();

                        cmd.CommandText = "CREATE INDEX CalculationIdIndex ON calculationitems (CalculationIdCI);";
                        await cmd.ExecuteNonQueryAsync();





                        cmd.CommandText =
                            "CREATE TABLE orders (IdO SERIAL PRIMARY KEY, CreationDateO DATE, CalculationIdO INTEGER REFERENCES calculations (IdC) ON DELETE RESTRICT, QuantityO NUMERIC CHECK(QuantityO > 0))";
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
            }

            Connection.ConnectionString = cStringMA;
        }
    }
}
