using System;
namespace Wizard
{
    public interface IDataBaseAccess
    {
        string DatabasePath();
    }

    public class Constants
    {
        public const string OFFLINE_DATABASE_NAME = "wco.db";
    }
}
