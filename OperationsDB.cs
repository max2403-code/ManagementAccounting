//using System;
//using System.Collections.Generic;
//using System.Data;
//using System.Data.Common;
//using System.Text;
//using System.Threading.Tasks;
//using Npgsql;
//using System.Linq;
//using NpgsqlTypes;

//namespace ManagementAccounting
//{
//    public class OperationsDB
//    {
//        public NpgsqlConnection Connection { get; }
//        private object locker { get; }

//        public OperationsDB(string login, string password)
//        {
//            Connection = new NpgsqlConnection($"Host=localhost;Port=5432;Database=managementaccounting;Username={login};Password={password}");
//            locker = new object();
//        }

//        public Task GetShowItemsAsync(string cmdText, TypeOfItem blockType)
//        {
//            var task = new Task(() =>
//            {
//                var items = WorkSpace.GetWorkSpace().ShowListItems;
//                lock (locker)
//                {
//                    Connection.Open();
//                    using (var cmd = new NpgsqlCommand())
//                    {
//                        cmd.Connection = Connection;

//                        cmd.CommandText = cmdText;
//                        var reader = cmd.ExecuteReader();
//                        foreach (DbDataRecord item in reader)
//                        {
//                            var objectableItem = GetObjectableItem(blockType, item);
//                            items.Add(objectableItem);
//                        }
//                    }
//                    Connection.Close();
//                }
//            });
//            task.Start();
//            return task;
//        }

//        public static IObjectable GetObjectableItem(TypeOfItem blockType, DbDataRecord item)
//        {
//            if (blockType == TypeOfItem.Material)
//            {
//                var mType = (MaterialType)(int)item["MaterialType"];
//                var mName = (string)item["MaterialName"];
//                var remainders = (double)item["Remainder"];
//                var index = (int)item["Id"];
//                return new Material(mType, mName, remainders, index);
//            }

//            return null;
//        }

//        //public async Task AddNewItemAsync(string cmdText, IObjectable obj)
//        //{
//        //    await Connection.OpenAsync();
//        //    await using (var cmd = new NpgsqlCommand())
//        //    {
//        //        cmd.Connection = Connection;
//        //        cmd.CommandText = cmdText;
//        //        AssignParametersToCommand(cmd, obj);
//        //        try
//        //        {
//        //            await cmd.ExecuteNonQueryAsync();
//        //        }
//        //        catch (Exception e)
//        //        {
//        //            await Connection.CloseAsync();
//        //            throw;
//        //        }
//        //    }
//        //    await Connection.CloseAsync();
//        //}
//        public Task AddNewItemAsync(string cmdText, IObjectable obj, string exStr)
//        {
//            var task = new Task(() =>
//            {
//                Connection.Open();
//                using (var cmd = new NpgsqlCommand())
//                {
//                    cmd.Connection = Connection;
//                    cmd.CommandText = cmdText;
//                    AssignParametersToCommand(cmd, obj);
//                    try
//                    {
//                        cmd.ExecuteNonQuery();
//                    }
//                    catch (Exception e)
//                    {
//                        WorkSpace.GetWorkSpace().ExEvent += () => throw new Exception(exStr);
//                    }
//                }
//                Connection.Close();
//            });
//            task.Start();
//            return task;
//        }

//        public Task RemoveUpdateItemWithoutParametersAsync(string cmdText, string exStr)
//        {
//            var task = new Task(() =>
//            {
//                Connection.Open();
//                using (var cmd = new NpgsqlCommand())
//                {
//                    cmd.Connection = Connection;
//                    cmd.CommandText = cmdText;
//                    try
//                    {
//                        cmd.ExecuteNonQuery();
//                    }
//                    catch (Exception e)
//                    {
//                        WorkSpace.GetWorkSpace().ExEvent += () => throw new Exception(exStr);
//                    }
//                }
//                Connection.Close();
//            });
//            task.Start();
//            return task;
//        }

//        private void AssignParametersToCommand(NpgsqlCommand cmd, IObjectable obj)
//        {
//            if (obj is Material material)
//            {
//                cmd.Parameters.AddWithValue("materialtype", NpgsqlDbType.Integer, (int) material.MType);
//                cmd.Parameters.AddWithValue("materialname", NpgsqlDbType.Varchar, 50, material.Name);
//                cmd.Parameters.AddWithValue("remainder", NpgsqlDbType.Double, material.Remainder);
//            }

//        }

//        public static async Task SignInAsync(string login, string password)
//        {
//            var cStringPostgres = $"Host=localhost;Port=5432;Database=postgres;Username={login};Password={password}";
//            var isDBExist = true;
//            var cStringMA = $"Host=localhost;Port=5432;Database=managementaccounting;Username={login};Password={password}";
            
//            await using (var connectionPostgres = new NpgsqlConnection(cStringPostgres))
//            {
//                await connectionPostgres.OpenAsync();

//                await using (var cmd = new NpgsqlCommand())
//                {
//                    cmd.Connection = connectionPostgres;

//                    cmd.CommandText = "SELECT datname FROM pg_database WHERE datname = 'managementaccounting';";
//                    var reader = await cmd.ExecuteReaderAsync();
//                    if (!reader.HasRows)
//                    {
//                        isDBExist = false;
//                        await reader.CloseAsync();
//                        cmd.CommandText = "CREATE DATABASE managementaccounting;";
//                        await cmd.ExecuteNonQueryAsync();
//                    }
//                }
//            }

//            if (!isDBExist)
//            {
//                await using (var connectionMA = new NpgsqlConnection(cStringMA))
//                {
//                    await connectionMA.OpenAsync();

//                    await using (var cmd = new NpgsqlCommand())
//                    {
//                        cmd.Connection = connectionMA;
//                        cmd.CommandText = $"CREATE TABLE remainders (Id SERIAL PRIMARY KEY, MaterialType INTEGER, MaterialName CHARACTER VARYING({FactoryClass.GetNameItemLength()}) UNIQUE, Remainder DOUBLE PRECISION)";
//                        await cmd.ExecuteNonQueryAsync();

//                        cmd.CommandText = "CREATE INDEX MaterialTypeIndex ON remainders (MaterialType);";
//                        await cmd.ExecuteNonQueryAsync();

//                        cmd.CommandText =
//                            "CREATE TABLE materialflow (Id SERIAL PRIMARY KEY, MaterialId INTEGER REFERENCES remainders (Id) ON DELETE CASCADE, OperationType INTEGER, Quantity DOUBLE PRECISION, FlowDate DATE, TotalCost DOUBLE PRECISION, Remainder DOUBLE PRECISION, Note CHARACTER VARYING(50))";
//                        await cmd.ExecuteNonQueryAsync();

//                        cmd.CommandText = "CREATE INDEX MaterialIdIndex ON materialflow (MaterialId);";
//                        await cmd.ExecuteNonQueryAsync();
//                    }
//                }
//            }
//        }
//    }
//}
