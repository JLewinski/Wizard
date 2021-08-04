using System;
using System.IO;
using Wizard.Mobile.Droid;

[assembly: Xamarin.Forms.Dependency(typeof(DatabaseAccess))]
namespace Wizard.Mobile.Droid
{
    public class DatabaseAccess : IDataBaseAccess
    {
        public DatabaseAccess()
        {
        }

        public string DatabasePath()
        {
            var path = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), Constants.OFFLINE_DATABASE_NAME);

            if (!File.Exists(path))
            {
                File.Create(path).Dispose();
                File.Delete(path);
            }

            return path;
        }
    }
}
