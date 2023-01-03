using SQLite;

namespace ShinyApp.Services;


public class MySqliteConnection : SQLiteAsyncConnection
{
    public MySqliteConnection(IPlatform platform) : base(Path.Combine(platform.AppData.FullName, "app.db"))
    {
        var conn = this.GetConnection();
        // conn.CreateTable<YourModel>();

        conn.EnableWriteAheadLogging();
//-:cnd:noEmit
#if DEBUG
        conn.Trace = true;
        conn.Tracer = sql => Console.WriteLine(sql);
#endif
//+:cnd:noEmit
    }


    // public AsyncTableQuery<YourModel> Logs => this.Table<YourModel>();
}
