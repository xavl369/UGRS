SELECT 
	CardCode, 
	CardName, 
	CardFName, 
	LicTradNum, 
	validFor, 
	CreateDate, 
	UpdateDate,
	UpdateTS
FROM 
	OCRD 
WHERE 
	CardCode = '{CardCode}'