using System;
using System.IO;

namespace SecureEncryptor
{
    public static class TrialLicense
    {
        private static readonly string LicenseFile =
            Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "secureencryptor.trial"
            );

        private const int TrialDays = 2; // días de prueba

        public static bool IsTrialValid()
        {
            try
            {
                if (!File.Exists(LicenseFile))
                {
                    File.WriteAllText(LicenseFile, DateTime.Now.ToString());
                    return true;
                }

                DateTime installDate =
                    DateTime.Parse(File.ReadAllText(LicenseFile));

                double daysUsed = (DateTime.Now - installDate).TotalDays;

                return daysUsed <= TrialDays;
            }
            catch
            {
                return false;
            }
        }
    }
}
