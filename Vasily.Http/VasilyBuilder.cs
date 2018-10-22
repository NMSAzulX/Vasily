using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class VasilyBuilder
    {

        public static IServiceCollection AddVasily(this IServiceCollection service, Action<ConnectOption> action = null)
        {
            ConnectOption options = new ConnectOption();
            action?.Invoke(options);
            VasilyRunner.Run();
            return service;
        }

        //public static IServiceCollection AddVasilySqlCache(this IServiceCollection service, Action<SqlOptions> action)
        //{
        //    SqlOptions options = new SqlOptions();
        //    action(options);
        //    return service;
        //}
        //public static IServiceCollection AddVasilyConnectionCache(this IServiceCollection service, Action<ConnectionOptions> action)
        //{
        //    ConnectionOptions options = new ConnectionOptions();
        //    action(options);
        //    return service;
        //}
    }

    public class ConnectOption
    {
        public void Add<T>(string key,string value)
        {
            Connector.Add<T>(key, value);
        }
        public void Add<T>(string key, string reader,string writter)
        {
            Connector.Add<T>(key, reader, writter);
        }
        public void AddRead<T>(string key, string reader)
        {
            Connector.AddRead<T>(key, reader);
        }
        public void AddWrite<T>(string key, string writter)
        {
            Connector.AddWrite<T>(key, writter);
        }
    }
}
