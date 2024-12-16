public static class Constants
{
    #if (efpostgres)
    public const string DatabaseInvariant = "Npgsql";
    #else
    public const string DatabaseInvariant = "System.Data.SqlClient";
    #endif
}