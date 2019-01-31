SELECT DISTINCT 
	T0.DocType,
	T0.DocNum,
	T0.DocEntry,
	T0.CardCode,
	T1.LineNum,
	T1.WhsCode,
	T1.ItemCode,
	T1.Quantity,
	T1.Price,
	T0.CreateDate,
	T0.UpdateDate,
	T1.TaxCode,
	T1.U_SU_BatchAuc AS BatchNumber,
	T0.CreateTS,
	T0.UpdateTS,
	T0.DocStatus
FROM 
	ODLN T0
INNER JOIN 
	DLN1 T1 
ON 
	T0.DocEntry = T1.DocEntry
WHERE 
	T1.WhsCode = '{WhsCode}'
AND 
	UpdateDate IS NOT NULL
ORDER BY 
	T0.CreateDate