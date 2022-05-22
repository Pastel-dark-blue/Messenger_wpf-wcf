using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ChatHost
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ServiceHost host = new ServiceHost(typeof(WcfService.ServiceChat));

            try
            {
                host.Open();
                Console.WriteLine("Хост запущен!\n" + DateTime.Now);
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Произошла ошибка при запуске хоста.\nТекст ошибки:\t" + ex.Message);
                Console.ReadLine();
            }
            finally
            {
                host.Close();
            }
        }
    }
}
