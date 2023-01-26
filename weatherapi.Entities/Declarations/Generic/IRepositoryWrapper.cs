using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace weatherapi.Entities.Declarations.Generic
{
    public interface IRepositoryWrapper
    {
        //IWeatherForecastRepository WeatherForecast { get; }

        Task SaveAsync();
    }
}
