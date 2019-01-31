using UGRS.Core.SDK.DI.GPS.Services;

namespace UGRS.Core.SDK.DI.GPS
{
    /// <summary> The GPS factory services. </summary>
    /// <remarks> Ranaya, 26/05/2017. </remarks>

    public class GPSFactoryServices
    {
        public SetupService GetSetupService()
        {
            return new SetupService();
        }

        public ImportedReportService GetImportedReportService()
        {
            return new ImportedReportService();
        }

        public KilometersTraveledService GetKilometersTraveledService()
        {
            return new KilometersTraveledService();
        }

        public TimeEngineService GetTimeEngineService()
        {
            return new TimeEngineService();
        }
    }
}
