SELECT 
	T0.ItemCode,
	T0.MnfSerial AS CardCode,
	T0.DistNumber AS BatchNumber,
	t0.InDate,
	--T0.UpdateDate
		Case when t0.UpdateDate is null then ''
	else t0.UpdateDate end as UpdateDate
FROM 
	OBTN T0
INNER JOIN 
	OBTQ T1 
ON 
	T0.ItemCode = T1.ItemCode
AND 
	T0.SysNumber = T1.SysNumber
WHERE 
	T1.WhsCode = '{WhsCode}' 
		--and convert(DATE,t0.CreateDate) = '2018-08-10'
ORDER BY 
	T0.UpdateDate