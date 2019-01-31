--SELECT
--   'N' AS Chk,
--   T2.Quantity AS Cantidad,
--   t1.DistNumber,
--   t3.CardCode,
--   t4.ItemName,
--   t1.LotNumber,
--   t1.InDate,
--   T2.ItemCode,
--   t3.CardName,
--   0.00 as Qntity,
--   (select max(isnull(DocDate,''))  from OINV where (NumAtCard like '%'+t1.LotNumber+'' )) as InvDate

--FROM OBTN T1
--INNER JOIN OBTQ T2 ON T2.ItemCode=T1.ItemCode AND T2.SysNumber=T1.SysNumber
--inner join OITM T4 on t4.ItemCode = t2.ItemCode
--inner join OCRD T3 on t3.CardCode = t1.MnfSerial


--and T2.Quantity>0
--WHERE isnull(t1.LotNumber,'') <>'' and t2.WhsCode ='{WhsCode}'
--order by CardName,InDate Desc,LotNumber

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
   0.00 as Qntity,
   t5.Fecha,T2.WhsCode +'_' +t1.LotNumber
FROM OBTN T1
INNER JOIN OBTQ T2 ON T2.ItemCode=T1.ItemCode AND T2.SysNumber=T1.SysNumber
inner join OITM T4 on t4.ItemCode = t2.ItemCode
inner join OCRD T3 on t3.CardCode = t1.MnfSerial
left join (
select max(isnull(a0.DocDate,'')) Fecha,a1.FreeTxt  from OINV A0
inner join inv1 A1 on a0.docentry = a1.docentry 
where a1.FreeTxt is not null
group by  a1.FreeTxt
) T5 on t5.FreeTxt = T2.WhsCode +'_' +t1.LotNumber


WHERE isnull(t1.LotNumber,'') <>'' and t2.WhsCode ='{WhsCode}' and T2.Quantity>0
order by CardName,InDate Desc,LotNumber