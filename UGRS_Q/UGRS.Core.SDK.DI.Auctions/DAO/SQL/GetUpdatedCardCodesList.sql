SELECT 
	CardCode, 
	UpdateDate,
	UpdateTS
FROM
	OCRD 
WHERE 
	CardType = 'C'
AND 
	UpdateDate IS NOT NULL 
ORDER BY 
	CreateDate