SELECT 
	T0.ItemCode,
	T0.MnfSerial AS CardCode,
	T0.DistNumber AS BatchNumber
FROM 
	OBTN T0
INNER JOIN 
	OBTQ T1 
ON 
	T0.ItemCode = T1.ItemCode
AND 
	T0.SysNumber = T1.SysNumber
WHERE 
	T0.ExpDate IS NOT NULL
AND
    Convert(DATE,t0.ExpDate) = COALESCE(NULLIF('{ActAuctionDate}',''),T0.ExpDate)
	
AND
	T1.WhsCode = '{WhsCode}' 
AND 
	T1.Quantity > 0 
ORDER BY 
	T0.CreateDate 