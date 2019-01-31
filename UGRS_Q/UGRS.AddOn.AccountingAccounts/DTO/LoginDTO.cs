
namespace UGRS.AddOn.AccountingAccounts.DTO
{
    public class LoginDTO
    {
        public int Code { get; set; }
        public int IdBD { get; set; }
        //public string Name { get; set; }
        public string NameServer {get; set;}
	    public string NameDB {get; set;}
	    public string Login {get; set;}
	    public string Password {get; set;}
	    public string AccountingAccount {get; set;}
	    public string Activo {get; set;}
        public string Status { get; set; }
        public string Descripcion { get; set; }
    }
}
