SELECT 
	ItemCode, 
	UpdateDate
FROM 
	OITM
WHERE 
	{Property} = 'Y'
AND 
	UpdateDate IS NOT NULL 
ORDER BY 
	CreateDate