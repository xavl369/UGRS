
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UGRS.AddOn.AccountingAccounts.Entities
{
    public class Nomina
    {
        private System.Data.DataRow dr;

        public Nomina()
        {

        }
        public Nomina(System.Data.DataRow dr)
        {
                ACCTO = dr["ACCTO"].ToString();
                ACCTO_HN = dr["ACCTO_HN"].ToString();
                ACONC = int.Parse(dr["ACONC"].ToString());
                ACONS = int.Parse(dr["ACONS"].ToString());
                ADEP = dr["ADEP"].ToString();
                ADEP_HN = dr["ADEP_HN"].ToString();
                AIMPO = dr["AIMPO"].ToString();
                ATRAB = int.Parse(dr["ATRAB"].ToString());
                CCTO_ID = dr["CCTO_ID"].ToString();
                CUENTA1 = dr["CUENTA1"].ToString();
                CUENTA2 = dr["CUENTA2"].ToString();
                ICCTAA = dr["ICCTAA"].ToString();
                ICDNOM = dr["ICDNOM"].ToString();
                ICNOM = dr["ICNOM"].ToString();
                IPMES = int.Parse(dr["IPMES"].ToString());
                IPNO = int.Parse(dr["IPNO"].ToString());
                IPTIPO = int.Parse(dr["IPTIPO"].ToString());
                IPYEAR = int.Parse(dr["IPYEAR"].ToString());
                NIVEL = dr["NIVEL"].ToString();
                NNOM = dr["NNOM"].ToString();
                NRFC = dr["NRFC"].ToString();
                UUID = dr["UUID"].ToString();
                CUENTA = dr["CUENTA"].ToString();
                ICCTAC = dr["ICCTAC"].ToString();
        }

        public int ACONS { get; set; }
        public int IPYEAR { get; set; }
        public int IPMES { get; set; }
        public int IPTIPO { get; set; }
        public int IPNO { get; set; }
        public int? ATRAB { get; set; }
        public int ACONC { get; set; }
        public string ICNOM { get; set; }
        public string AIMPO { get; set; }
        public string ACCTO_HN { get; set; }
        public string ICDNOM { get; set; }
        public string ADEP_HN { get; set; }
        public string ICCTAA { get; set; }
        public string NIVEL { get; set; }
        public string ACCTO { get; set; }
        public string ADEP { get; set; }
        public string CUENTA1 { get; set; }
        public string CCTO_ID { get; set; }
        public string UUID { get; set; }
        public string CUENTA2 { get; set; }
        public string NNOM { get; set; }
        public string NRFC { get; set; }


        public string ICCTAC { get; set; }

        public string CUENTA { get; set; }
    }
}
