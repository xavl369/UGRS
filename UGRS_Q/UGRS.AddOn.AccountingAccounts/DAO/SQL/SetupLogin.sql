SELECT 
	U_NameServer,
	U_NameDB,
	U_Login,
	U_Password,
	U_AccountingAccount,
	U_Activo,
	U_Status 
FROM [@UG_AA_LOGIN] a 
left join [@UG_AA_DB] b on a.U_IdBD = b.U_Code_UG_AA_LOGIN 
WHERE 
	a.U_Activo='Y' and b.U_Status='Y'