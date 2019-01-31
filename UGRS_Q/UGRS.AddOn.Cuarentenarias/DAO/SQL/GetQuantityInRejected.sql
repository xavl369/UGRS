
select t2.Quantity  from OBTN t1
INNER JOIN OBTQ T2 ON T2.ItemCode=T1.ItemCode AND T2.SysNumber=T1.SysNumber

where DistNumber = '{DistNumber}'