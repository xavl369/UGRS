SELECT
   'N' AS Chk,
   T2.Quantity AS Cantidad,
   t1.DistNumber,
   t3.CardCode,
   t4.ItemName,
   t1.LotNumber,
   t1.InDate,
   T2.ItemCode,
   t3.CardName,
   0.00 as Qntity

FROM OBTN T1
INNER JOIN OBTQ T2 ON T2.ItemCode=T1.ItemCode AND T2.SysNumber=T1.SysNumber
inner join OITM T4 on t4.ItemCode = t2.ItemCode
inner join OCRD T3 on t3.CardCode = t1.MnfSerial


and T2.Quantity>0
WHERE isnull(t1.LotNumber,'') <>'' and t2.WhsCode ='{WhsCode}' and t3.CardCode = '{CardCode}'