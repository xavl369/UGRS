using System;

namespace UGRS.AddOn.AccountingAccounts.Entities
{
    public class AccountingAccount
    {
        //[Historico de Nominas]
        public DateTime Year { get; set; }
        public int Id_Month { get; set; }//->[Catalogo Periodos]        
        public int Id_Type { get; set; }//->[Catalogo Periodos]
        public int Id_Number { get; set; }//->[Catalogo Periodos]
        public int Departament { get; set; }
        public int Account { get; set; }//-> [Catalogo Centros de Costos]
        public string Id_CCTO { get; set; }//-> [Catalogo Centros de Costos]
        public int Level { get; set; }//-> [Catalogo Centros de Costos]
        public string Concep { get; set; }//-> [Catalogo Conceptos]
        public int Id_CONC { get; set; }//-> [Catalogo Conceptos]
        public string ICCTAC { get; set; }//-> [Catalogo Conceptos]
        public string ICCTAA { get; set; }//-> [Catalogo Conceptos]
        public float Amount { get; set; }
        public DateTime DateStart { get; set; }//->[Catalogo Periodos]
        public DateTime DateEnd { get; set; }//->[Catalogo Periodos]
        public int Id_Consecutive { get; set; }
    }
}
