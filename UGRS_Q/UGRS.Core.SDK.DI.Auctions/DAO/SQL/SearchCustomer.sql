SELECT 
	CardCode, 
	CardName, 
	LicTradNum, 
	validFor, 
	CreateDate, 
	UpdateDate 
FROM 
	OCRD 
WHERE 
	CardCode LIKE '%{Filter}%' 
OR 
	CardName LIKE '%{Filter}%' 
OR 
	LicTradNum LIKE '%{Filter}%'