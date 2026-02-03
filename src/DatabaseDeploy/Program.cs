using DatabaseDeploy.Impl;
using System;

namespace DatabaseDeploy
{
    class Program
    {
        static void Main(string[] args)
        {
            var dc = new DataFileSyncronizer();
            try
            {
                Logger.LogStartApplication();
                dc.StartSync();
            }
            catch (Exception ex)
            {
                Logger.LogError("EXCEPTION: Please check the log for details.", ex);
            }

            Console.WriteLine("Hit <Enter> to exit.....");
            Console.ReadLine();
        }
    }
}
