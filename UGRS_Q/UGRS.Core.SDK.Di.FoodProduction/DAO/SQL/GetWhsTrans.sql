SELECT TOP 1
T1.WhsCode 
FROM
OWHS T0
INNER JOIN (SELECT WhsCode, ItemCode, StockValue FROM OITW) T1 ON T1.WhsCode = T0.WhsCode
WHERE 
T0.U_GLO_WhsTransit = 'S' AND T1.StockValue = 0 AND T1.ItemCode = '{ItemCode}'
