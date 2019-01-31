using System;

namespace UGRS.Core.SDK.DI.Permissions.DTO
{
    public class PermissionRequestDTO
    {
        public DateTime Date { get; set; }
        public DateTime CrossingDate { get; set; }
        public int MobilizationTypeId { get; set; }
        public string MobilizationType { get; set; }
        public string UgrsRequest { get; set; }
        public string UgrsFolio { get; set; }
        public string Producer { get; set; }
        public string ProducerTelephone { get; set; }
        public int TransportId { get; set; }
        public string Transport { get; set; }
        public string Location1 { get; set; }
        public string Location2 { get; set; }
        public string OriginState { get; set; }
        public string OriginCity { get; set; }
        public int Customs1 { get; set; }
        public int Customs2 { get; set; }
        public string Entry { get; set; }
        public string Departure { get; set; }
        public string Origin { get; set; }
        public string Destination { get; set; }
        public string CardCode { get; set; }
        public string CustomerLocation { get; set; }
    }
}
