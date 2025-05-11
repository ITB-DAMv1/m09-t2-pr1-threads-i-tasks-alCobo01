using System.Text;
using T2_PR1.Models;

namespace T2_PR1.Services
{
    internal class StatsManager
    {
        private readonly string _filePath;
        
        internal StatsManager(string filePath = "dining_stats.csv")
        {
            _filePath = filePath;
        }
        
        internal void SaveStats(Guest[] guests)
        {
            StringBuilder csvContent = new StringBuilder();
            
            // Add header
            csvContent.AppendLine("GuestNumber,MaxHungerTime,MealCount");
            
            // Add data for each guest
            foreach (var guest in guests)
            {
                csvContent.AppendLine($"{guest.Id},{guest.MaxHungerTime},{guest.MealCount}");
            }
            
            // Write to file
            File.WriteAllText(_filePath, csvContent.ToString());
            
            Console.WriteLine($"Statistics saved to {Path.GetFullPath(_filePath)}");
        }
        
        internal void PrintStats(Guest[] guests)
        {
            Console.WriteLine("\n=== DINING PHILOSOPHERS STATISTICS ===");
            Console.WriteLine("Guest\tMax Hunger Time (ms)\tMeals\tTotal Blocked Time (ms)");
            Console.WriteLine("------------------------------------------------------");
            
            foreach (var guest in guests)
            {
                Console.ForegroundColor = guest.TextColor;
                Console.WriteLine($"{guest.Id}\t{guest.MaxHungerTime}\t\t\t{guest.MealCount}\t{guest.TotalBlockedTime}");
            }
            
            Console.ResetColor();
        }
    }
}
