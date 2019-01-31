SELECT
	T0.ItemCode,
	T0.MnfSerial AS CardCode,
	T0.DistNumber AS BatchNumber,
	T1.Quantity,
	T0.CreateDate,
	T0.UpdateDate,
	T0.U_GLO_Time
FROM 
	OBTN T0
INNER JOIN 
	OBTQ T1 
ON 
	T0.ItemCode = T1.ItemCode
AND 
	T0.SysNumber = T1.SysNumber
WHERE 
	T1.Quantity > 0 
AND 
	T1.WhsCode = '{WhsCode}' 
AND 
	T0.ItemCode = '{ItemCode}' 
AND 
	T0.MnfSerial = '{CardCode}'
ORDER BY 
	InDate
