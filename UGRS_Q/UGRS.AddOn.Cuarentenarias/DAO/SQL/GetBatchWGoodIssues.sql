SELECT
   T2.Quantity AS Cantidad,
   t1.DistNumber,
   t2.WhsCode,
   t1.MnfSerial,
   T2.ItemCode
FROM OBTN T1
INNER JOIN OBTQ T2 ON T2.ItemCode=T1.ItemCode AND T2.SysNumber=T1.SysNumber
and T2.Quantity=0
WHERE t1.MnfSerial = '{CardCode}' AND t2.WhsCode = '{WhsCode}'