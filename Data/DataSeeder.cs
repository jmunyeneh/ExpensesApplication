using ExpensesAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExpensesAPI.Data
{
    public class DataSeeder
    {
        public static void Seed(AppDbContext context)
        {
            if (!context.Entries.Any())
            {
                var entries = new List<Entry>
                {
                    new Entry { Description = "test", IsExpense = false, Value = 10.11},
                    new Entry { Description = "test2", IsExpense = false, Value = 13.01}
                };

                context.AddRange(entries);
                context.SaveChanges();
            }
        }
    }
}
