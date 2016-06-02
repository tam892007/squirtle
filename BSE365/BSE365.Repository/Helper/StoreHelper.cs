using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BSE365.Repository.DataContext;

namespace BSE365.Repository.Helper
{
    public class StoreHelper
    {
        public static void MapGiversAndReceivers()
        {
            using (var context = new BSE365Context())
            {
                context.Database.ExecuteSqlCommand("EXEC MapGiversAndReceivers");
            }
        }


        public static void UpdateTransactions()
        {
            using (var context = new BSE365Context())
            {
                var timeBase = DateTime.Now;
                timeBase = timeBase.AddHours(-48);
                var timeBaseParam = new SqlParameter
                {
                    ParameterName = "Time",
                    Value = timeBase,
                    SqlDbType = SqlDbType.DateTime,
                    Direction = ParameterDirection.Input,
                };
                context.Database.ExecuteSqlCommand("EXEC UpdateTransactions @Time GO;", timeBaseParam);
            }
        }
    }
}