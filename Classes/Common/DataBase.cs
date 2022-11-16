using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;
using System.Threading.Tasks;
using ManagementAccounting.Classes.Abstract;
using ManagementAccounting.Interfaces.Common;
using Npgsql;
using NpgsqlTypes;

namespace ManagementAccounting
{
    public class DataBase : IDataBase
    {
        public NpgsqlConnection Connection { get; }
        private object Locker { get; }

        public DataBase()
        {
            Connection = new NpgsqlConnection();
            Locker = new object();
        }

        public Task<List<T>> ExecuteReaderAsync<T>(IExceptionChecker exceptionChecker, Func<DbDataRecord, T> getItem, string commandText)
        {
            var task = new Task<List<T>>(() =>
            {
                lock (Locker)
                {
                    var items = new List<T>();
                    Connection.Open();
                    using (var cmd = new NpgsqlCommand())
                    {
                        cmd.Connection = Connection;
                        cmd.CommandText = commandText;

                        try
                        {
                            var reader = cmd.ExecuteReader();
                            foreach (DbDataRecord item in reader)
                            {
                                items.Add(getItem(item));
                            }
                        }
                        catch (NpgsqlException)
                        {
                            if (Connection.State != ConnectionState.Closed) Connection.Close();
                            exceptionChecker.IsExceptionHappened = true;
                            return items;
                        }


                    }
                    Connection.Close();
                    return items;
                }
            });
            task.Start();
            return task;
        }

        public Task ExecuteUpdaterAsync(IExceptionChecker exceptionChecker, Action<DbDataRecord> updateItem, string commandText)
        {
            var task = new Task(() =>
            {
                lock (Locker)
                {
                    Connection.Open();
                    using (var cmd = new NpgsqlCommand())
                    {
                        cmd.Connection = Connection;
                        cmd.CommandText = commandText;

                        try
                        {
                            var reader = cmd.ExecuteReader();
                            foreach (DbDataRecord item in reader)
                            {
                                updateItem(item);
                            }
                        }
                        catch (NpgsqlException)
                        {
                            if (Connection.State != ConnectionState.Closed) Connection.Close();
                            exceptionChecker.IsExceptionHappened = true;
                            return;
                        }
                        
                    }
                    Connection.Close();
                }
            });
            task.Start();
            return task;
        }

        public Task ExecuteNonQueryAndReaderAsync(IExceptionChecker exceptionChecker, string commandText, Action<NpgsqlCommand> assignItemParameters = null, Action<DbDataRecord> assignId = null)
        {
            var task = new Task(() =>
            {
                lock (Locker)
                {
                    Connection.Open();
                    using (var cmd = new NpgsqlCommand())
                    {
                        cmd.Connection = Connection;
                        cmd.CommandText = commandText;

                        assignItemParameters?.Invoke(cmd); //if (assignItemParameters != null) assignItemParameters(cmd);
                        try
                        {
                            if (assignId == null)
                                cmd.ExecuteNonQuery();
                            else
                            {
                                var reader = cmd.ExecuteReader();
                                foreach (DbDataRecord id in reader)
                                    assignId(id);
                            }
                        }
                        catch (NpgsqlException)
                        {
                            if (Connection.State != ConnectionState.Closed) Connection.Close();
                            exceptionChecker.IsExceptionHappened = true;
                            return;
                        }
                    }
                    Connection.Close();
                }
            });
            task.Start();
            return task;
        }

        //public Task ExecuteNonQueryAndReaderAsync(IExceptionable item, string commandText, string exceptionMessage, Action<NpgsqlCommand> assignItemParameters = null, Action<DbDataRecord> assignId = null)
        //{
        //    var task = new Task(() =>
        //    {
        //        Connection.Open();
        //        using (var cmd = new NpgsqlCommand())
        //        {
        //            cmd.Connection = Connection;
        //            cmd.CommandText = commandText;

        //            assignItemParameters?.Invoke(cmd); //if (assignItemParameters != null) assignItemParameters(cmd);
        //            try
        //            {
        //                if (assignId == null)
        //                    cmd.ExecuteNonQuery();
        //                else
        //                {
        //                    var reader = cmd.ExecuteReader();
        //                    foreach (DbDataRecord id in reader)
        //                        assignId(id);
        //                }
        //            }
        //            catch (Exception e)
        //            {
        //                item.ExceptionEvent += () => throw new Exception(exceptionMessage);
        //            }
        //        }
        //        Connection.Close();
        //    });
        //    task.Start();
        //    return task;
        //}

        //public Task<decimal> GetSum(string commandText)
        //{
        //    var task = new Task<decimal>(() =>
        //    {
        //        var result = 0m;
        //        Connection.Open();
        //        using (var cmd = new NpgsqlCommand())
        //        {
        //            cmd.Connection = Connection;

        //            cmd.CommandText = commandText;
        //            var reader = cmd.ExecuteReader();
        //            foreach (DbDataRecord item in reader)
        //            {
        //                result = item["Sum"] is DBNull ? result : (decimal) item["Sum"];
        //            }
        //        }
        //        Connection.Close();
        //        return result;
        //    });
        //    task.Start();
        //    return task;
        //}

        public Task SignInAsync(IExceptionChecker exceptionChecker, string login, string password)
        {
            var task = new Task(() =>
            {
                lock (Locker)
                {
                    var cStringPostgres =
                    $"Host=localhost;Port=5432;Database=postgres;Username={login};Password={password}";
                    var isDBExist = true;
                    var cStringMA =
                        $"Host=localhost;Port=5432;Database=managementaccounting;Username={login};Password={password}";

                    using (var connectionPostgres = new NpgsqlConnection(cStringPostgres))
                    {
                        try
                        {
                            connectionPostgres.Open();
                            using (var cmd = new NpgsqlCommand())
                            {
                                cmd.Connection = connectionPostgres;

                                cmd.CommandText = "SELECT datname FROM pg_database WHERE datname = 'managementaccounting';";
                                var reader = cmd.ExecuteReader();
                                if (!reader.HasRows)
                                {
                                    isDBExist = false;
                                    reader.Close();
                                    cmd.CommandText = "CREATE DATABASE managementaccounting;";
                                    cmd.ExecuteNonQuery();
                                }
                            }
                        }
                        catch (NpgsqlException)
                        {
                            if (connectionPostgres.State != ConnectionState.Closed) connectionPostgres.Close();
                            exceptionChecker.IsExceptionHappened = true;
                            return;
                        }
                    }

                    if (!isDBExist)
                    {
                        using (var connectionMA = new NpgsqlConnection(cStringMA))
                        {
                            try
                            {
                                connectionMA.Open();

                                using (var cmd = new NpgsqlCommand())
                                {
                                    cmd.Connection = connectionMA;
                                    cmd.CommandText =
                                        $"CREATE TABLE materials (IdM SERIAL PRIMARY KEY, MaterialTypeM INTEGER, MaterialNameM CHARACTER VARYING(50) UNIQUE, UnitM INTEGER)";
                                    cmd.ExecuteNonQuery();

                                    cmd.CommandText = "CREATE INDEX MaterialTypeMIndex ON materials (MaterialTypeM);";
                                    cmd.ExecuteNonQuery();

                                    cmd.CommandText =
                                        "CREATE TABLE materialreceiving (IdMR SERIAL PRIMARY KEY, MaterialIdMR INTEGER REFERENCES materials (IdM) ON DELETE CASCADE, QuantityMR NUMERIC CHECK(QuantityMR > 0), ReceiveDateMR DATE, TotalCostMR NUMERIC CHECK(TotalCostMR >= 0), RemainderMR NUMERIC CHECK(RemainderMR >= 0), NoteMR CHARACTER VARYING(50), SearchNameMR CHARACTER VARYING(10))";
                                    cmd.ExecuteNonQuery();

                                    cmd.CommandText = "CREATE INDEX MaterialIdMRIndex ON materialreceiving (MaterialIdMR);";
                                    cmd.ExecuteNonQuery();

                                    cmd.CommandText = "CREATE INDEX RemainderMRIndex ON materialreceiving (RemainderMR);";
                                    cmd.ExecuteNonQuery();

                                    cmd.CommandText = "CREATE INDEX SearchNameMRIndex ON materialreceiving (SearchNameMR);";
                                    cmd.ExecuteNonQuery();

                                    cmd.CommandText = "CREATE INDEX ReceiveDateMRIndex ON materialreceiving (ReceiveDateMR);";
                                    cmd.ExecuteNonQuery();

                                    cmd.CommandText =
                                        "CREATE TABLE calculations (IdC SERIAL PRIMARY KEY, CalculationNameC CHARACTER VARYING(50) UNIQUE)";
                                    cmd.ExecuteNonQuery();

                                    cmd.CommandText =
                                        "CREATE TABLE calculationitems (IdCI SERIAL PRIMARY KEY, CalculationIdCI INTEGER REFERENCES calculations (IdC) ON DELETE CASCADE, MaterialIdCI INTEGER REFERENCES materials (IdM) ON DELETE RESTRICT, ConsumptionCI NUMERIC CHECK(ConsumptionCI > 0), UNIQUE(CalculationIdCI, MaterialIdCI))";
                                    cmd.ExecuteNonQuery();

                                    cmd.CommandText =
                                        "CREATE INDEX CalculationIdCIIndex ON calculationitems (CalculationIdCI);";
                                    cmd.ExecuteNonQuery();

                                    cmd.CommandText =
                                        "CREATE TABLE preorders (IdPO SERIAL PRIMARY KEY, CalculationIdPO INTEGER REFERENCES calculations (IdC) ON DELETE RESTRICT, QuantityPO INTEGER CHECK(QuantityPO > 0), CreationDatePO DATE, SearchNamePO CHARACTER VARYING(64))";
                                    cmd.ExecuteNonQuery();

                                    cmd.CommandText = "CREATE INDEX CalculationIdPOIndex ON preorders (CalculationIdPO);";
                                    cmd.ExecuteNonQuery();

                                    cmd.CommandText = "CREATE INDEX SearchNamePOIndex ON preorders (SearchNamePO);";
                                    cmd.ExecuteNonQuery();

                                    cmd.CommandText =
                                        "CREATE TABLE orders (IdO SERIAL PRIMARY KEY, CreationDateO DATE, OrderNameO CHARACTER VARYING(50), QuantityO INTEGER CHECK(QuantityO > 0), SearchNameO CHARACTER VARYING(64))";
                                    cmd.ExecuteNonQuery();

                                    cmd.CommandText = "CREATE INDEX SearchNameOIndex ON orders (SearchNameO);";
                                    cmd.ExecuteNonQuery();

                                    cmd.CommandText = "CREATE INDEX CreationDateOIndex ON orders (CreationDateO);";
                                    cmd.ExecuteNonQuery();

                                    cmd.CommandText =
                                        "CREATE TABLE orderitems (IdOI SERIAL PRIMARY KEY, OrderIdOI INTEGER REFERENCES orders (IdO) ON DELETE CASCADE, MaterialIdOI INTEGER REFERENCES materials (IdM) ON DELETE RESTRICT, ConsumptionOI NUMERIC CHECK(ConsumptionOI >= 0), TotalConsumptionOI NUMERIC CHECK(TotalConsumptionOI > 0))";
                                    cmd.ExecuteNonQuery();

                                    cmd.CommandText = "CREATE INDEX ConsumptionOIIndex ON orderitems (ConsumptionOI);";
                                    cmd.ExecuteNonQuery();

                                    cmd.CommandText = "CREATE INDEX OrderIdOIIndex ON orderitems (OrderIdOI);";
                                    cmd.ExecuteNonQuery();

                                    cmd.CommandText = "CREATE INDEX MaterialIdOIIndex ON orderitems (MaterialIdOI);";
                                    cmd.ExecuteNonQuery();

                                    cmd.CommandText =
                                        "CREATE TABLE ordermaterialreceiving (IdOMR SERIAL PRIMARY KEY, OrderItemIdOMR INTEGER REFERENCES orderitems (IdOI) ON DELETE CASCADE, MaterialReceivingIdOMR INTEGER REFERENCES materialreceiving (IdMR) ON DELETE RESTRICT, ConsumptionOMR NUMERIC CHECK(ConsumptionOMR > 0))";
                                    cmd.ExecuteNonQuery();

                                    cmd.CommandText =
                                        "CREATE INDEX OrderItemIdOMRIndex ON ordermaterialreceiving (OrderItemIdOMR);";
                                    cmd.ExecuteNonQuery();

                                    cmd.CommandText =
                                        "CREATE INDEX MaterialReceivingIdOMRIndex ON ordermaterialreceiving (MaterialReceivingIdOMR);";
                                    cmd.ExecuteNonQuery();

                                    //cmd.CommandText = "CREATE TABLE temporderitems (IdTOI INTEGER)";
                                    //cmd.ExecuteNonQuery();
                                }
                            }
                            catch (NpgsqlException)
                            {
                                if (connectionMA.State != ConnectionState.Closed) connectionMA.Close();
                                exceptionChecker.IsExceptionHappened = true;
                                return;
                            }

                        }
                    }

                    Connection.ConnectionString = cStringMA;
                }
            });

            task.Start();
            return task;
        }

        //public async Task SignIn(string login, string password)
        //{
        //    var cStringPostgres = $"Host=localhost;Port=5432;Database=postgres;Username={login};Password={password}";
        //    var isDBExist = true;
        //    var cStringMA = $"Host=localhost;Port=5432;Database=managementaccounting;Username={login};Password={password}";

        //    await using (var connectionPostgres = new NpgsqlConnection(cStringPostgres))
        //    {
        //        await connectionPostgres.OpenAsync();

        //        await using (var cmd = new NpgsqlCommand())
        //        {
        //            cmd.Connection = connectionPostgres;

        //            cmd.CommandText = "SELECT datname FROM pg_database WHERE datname = 'managementaccounting';";
        //            var reader = await cmd.ExecuteReaderAsync();
        //            if (!reader.HasRows)
        //            {
        //                isDBExist = false;
        //                await reader.CloseAsync();
        //                cmd.CommandText = "CREATE DATABASE managementaccounting;";
        //                await cmd.ExecuteNonQueryAsync();
        //            }
        //        }
        //    }

        //    if (!isDBExist)
        //    {
        //        await using (var connectionMA = new NpgsqlConnection(cStringMA))
        //        {
        //            await connectionMA.OpenAsync();

        //            await using (var cmd = new NpgsqlCommand())
        //            {
        //                cmd.Connection = connectionMA;
        //                cmd.CommandText = $"CREATE TABLE materials (IdM SERIAL PRIMARY KEY, MaterialTypeM INTEGER, MaterialNameM CHARACTER VARYING(50) UNIQUE, UnitM INTEGER)";
        //                await cmd.ExecuteNonQueryAsync();

        //                cmd.CommandText = "CREATE INDEX MaterialTypeMIndex ON materials (MaterialTypeM);";
        //                await cmd.ExecuteNonQueryAsync();

        //                cmd.CommandText =
        //                    "CREATE TABLE materialreceiving (IdMR SERIAL PRIMARY KEY, MaterialIdMR INTEGER REFERENCES materials (IdM) ON DELETE CASCADE, QuantityMR NUMERIC CHECK(QuantityMR > 0), ReceiveDateMR DATE, TotalCostMR NUMERIC CHECK(TotalCostMR >= 0), RemainderMR NUMERIC CHECK(RemainderMR >= 0), NoteMR CHARACTER VARYING(50), SearchNameMR CHARACTER VARYING(10))";
        //                await cmd.ExecuteNonQueryAsync();

        //                cmd.CommandText = "CREATE INDEX MaterialIdMRIndex ON materialreceiving (MaterialIdMR);";
        //                await cmd.ExecuteNonQueryAsync();

        //                cmd.CommandText = "CREATE INDEX RemainderMRIndex ON materialreceiving (RemainderMR);";
        //                await cmd.ExecuteNonQueryAsync();

        //                cmd.CommandText = "CREATE INDEX SearchNameMRIndex ON materialreceiving (SearchNameMR);";
        //                await cmd.ExecuteNonQueryAsync();

        //                cmd.CommandText = "CREATE INDEX ReceiveDateMRIndex ON materialreceiving (ReceiveDateMR);";
        //                await cmd.ExecuteNonQueryAsync();

        //                cmd.CommandText =
        //                    "CREATE TABLE calculations (IdC SERIAL PRIMARY KEY, CalculationNameC CHARACTER VARYING(50) UNIQUE)";
        //                await cmd.ExecuteNonQueryAsync();

        //                cmd.CommandText =
        //                    "CREATE TABLE calculationitems (IdCI SERIAL PRIMARY KEY, CalculationIdCI INTEGER REFERENCES calculations (IdC) ON DELETE CASCADE, MaterialIdCI INTEGER REFERENCES materials (IdM) ON DELETE RESTRICT, ConsumptionCI NUMERIC CHECK(ConsumptionCI > 0), UNIQUE(CalculationIdCI, MaterialIdCI))";
        //                await cmd.ExecuteNonQueryAsync();

        //                cmd.CommandText = "CREATE INDEX CalculationIdCIIndex ON calculationitems (CalculationIdCI);";
        //                await cmd.ExecuteNonQueryAsync();

        //                cmd.CommandText =
        //                    "CREATE TABLE preorders (IdPO SERIAL PRIMARY KEY, CalculationIdPO INTEGER REFERENCES calculations (IdC) ON DELETE RESTRICT, QuantityPO INTEGER CHECK(QuantityPO > 0), CreationDatePO DATE, SearchNamePO CHARACTER VARYING(64))";
        //                await cmd.ExecuteNonQueryAsync();

        //                cmd.CommandText = "CREATE INDEX CalculationIdPOIndex ON preorders (CalculationIdPO);";
        //                await cmd.ExecuteNonQueryAsync();

        //                cmd.CommandText = "CREATE INDEX SearchNamePOIndex ON preorders (SearchNamePO);";
        //                await cmd.ExecuteNonQueryAsync();

        //                cmd.CommandText =
        //                    "CREATE TABLE orders (IdO SERIAL PRIMARY KEY, CreationDateO DATE, OrderNameO CHARACTER VARYING(50), QuantityO INTEGER CHECK(QuantityO > 0), SearchNameO CHARACTER VARYING(64))";
        //                await cmd.ExecuteNonQueryAsync();

        //                cmd.CommandText = "CREATE INDEX SearchNameOIndex ON orders (SearchNameO);";
        //                await cmd.ExecuteNonQueryAsync();

        //                cmd.CommandText = "CREATE INDEX CreationDateOIndex ON orders (CreationDateO);";
        //                await cmd.ExecuteNonQueryAsync();

        //                cmd.CommandText =
        //                    "CREATE TABLE orderitems (IdOI SERIAL PRIMARY KEY, OrderIdOI INTEGER REFERENCES orders (IdO) ON DELETE CASCADE, MaterialIdOI INTEGER REFERENCES materials (IdM) ON DELETE RESTRICT, ConsumptionOI NUMERIC CHECK(ConsumptionOI >= 0), TotalConsumptionOI NUMERIC CHECK(TotalConsumptionOI > 0))";
        //                await cmd.ExecuteNonQueryAsync();

        //                cmd.CommandText = "CREATE INDEX ConsumptionOIIndex ON orderitems (ConsumptionOI);";
        //                await cmd.ExecuteNonQueryAsync();

        //                cmd.CommandText = "CREATE INDEX OrderIdOIIndex ON orderitems (OrderIdOI);";
        //                await cmd.ExecuteNonQueryAsync();

        //                cmd.CommandText = "CREATE INDEX MaterialIdOIIndex ON orderitems (MaterialIdOI);";
        //                await cmd.ExecuteNonQueryAsync();

        //                cmd.CommandText =
        //                    "CREATE TABLE ordermaterialreceiving (IdOMR SERIAL PRIMARY KEY, OrderItemIdOMR INTEGER REFERENCES orderitems (IdOI) ON DELETE CASCADE, MaterialReceivingIdOMR INTEGER REFERENCES materialreceiving (IdMR) ON DELETE RESTRICT, ConsumptionOMR NUMERIC CHECK(ConsumptionOMR > 0))";
        //                await cmd.ExecuteNonQueryAsync();

        //                cmd.CommandText = "CREATE INDEX OrderItemIdOMRIndex ON ordermaterialreceiving (OrderItemIdOMR);";
        //                await cmd.ExecuteNonQueryAsync();

        //                cmd.CommandText = "CREATE INDEX MaterialReceivingIdOMRIndex ON ordermaterialreceiving (MaterialReceivingIdOMR);";
        //                await cmd.ExecuteNonQueryAsync();

        //                //cmd.CommandText = "CREATE TABLE temporderitems (IdTOI INTEGER)";
        //                //await cmd.ExecuteNonQueryAsync();
        //            }
        //        }
        //    }

        //    Connection.ConnectionString = cStringMA;
        //}
    }
}
