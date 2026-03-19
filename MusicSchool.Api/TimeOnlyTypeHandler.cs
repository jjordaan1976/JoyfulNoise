using Dapper;
using System.Data;

namespace MusicSchool.Api
{
    /// <summary>
    /// Tells Dapper how to read SQL Server TIME columns (returned as TimeSpan by ADO.NET)
    /// into TimeOnly, and how to write TimeOnly back as a TIME parameter.
    /// Register once at startup: SqlMapper.AddTypeHandler(new TimeOnlyTypeHandler());
    /// </summary>
    public class TimeOnlyTypeHandler : SqlMapper.TypeHandler<TimeOnly>
    {
        public override void SetValue(IDbDataParameter parameter, TimeOnly value)
        {
            parameter.DbType = DbType.Time;
            parameter.Value = value.ToTimeSpan();
        }

        public override TimeOnly Parse(object value)
            => TimeOnly.FromTimeSpan((TimeSpan)value);
    }
}
