using weatherapi.Entities.Declarations.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace weatherapi.Core.Serrvices.Generic
{
    public class RepositoryWrapper : IRepositoryWrapper
    {
        private readonly ApplicationContext _context;
        //private IWeatherForecastRepository weatherForecast;

        public RepositoryWrapper(ApplicationContext context)
        {
            _context = context;
        }

        //public IWeatherForecastRepository WeatherForecast
        //{
        //    get
        //    {
        //        if (weatherForecast == null)
        //        {
        //            weatherForecast = new WeatherForecastRepository(context);
        //        }
        //        return seatherForecast;
        //    }
        //}


        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
