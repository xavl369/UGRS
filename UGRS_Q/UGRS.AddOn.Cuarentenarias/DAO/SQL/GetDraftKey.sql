--select DocEntry from ODRF
--where NumAtCard = '{Reference}'
--select top 1 * from drf1
--where FreeTxt = '{Reference}' and ItemCode = '{ItemCode}' order by DocEntry
 

 select FreeTxt,* from ODRF t1
inner join DRF1 t2 on t1.DocEntry = t2.DocEntry
 where FreeTxt = '{Reference}' and ItemCode = '{ItemCode}' and DocStatus = 'O'