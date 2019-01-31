SELECT 
	T0.DocType,
	T0.DocNum,
	T0.DocEntry,
	T0.CardCode,
	T1.LineNum,
	T1.WhsCode,
	T1.ItemCode,
	T1.Quantity,
	T1.Price
FROM 
	ODLN T0
INNER JOIN 
	DLN1 T1 
ON 
	T0.DocEntry = T1.DocEntry
WHERE 
	T0.DocStatus = 'O'
AND 
	T0.CardCode = '{CardCode}'
ANd 
	T1.WhsCode = '{WhsCode}'