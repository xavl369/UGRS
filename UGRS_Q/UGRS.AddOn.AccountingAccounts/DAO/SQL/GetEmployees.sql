EXEC [sp_addlinkedsrvlogin] '{ServerName}', 'false', null, '{User}', '{Password}'; 

SELECT distinct(ATRAB) FROM [{ServerName}].[{DBName}].[dbo].[Historico de Nominas] 
WHERE (IPYEAR = '{Year}' OR '{Year}' IS NULL) 
	AND (IPTIPO = '{Tipo}' OR '{Tipo}' IS NULL) 
	AND (IPNO = '{Ipno}' OR '{Ipno}' IS NULL) ORDER BY ATRAB