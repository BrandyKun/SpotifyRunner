using cleanA.Application.Common.Interfaces;

namespace cleanA.Infrastructure.Services;

public class DateTimeService : IDateTime
{
    public DateTime Now => DateTime.Now;
}
