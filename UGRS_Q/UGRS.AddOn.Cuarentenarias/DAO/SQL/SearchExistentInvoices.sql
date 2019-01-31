
select * from OBTN t1
INNER JOIN OBTQ T2 ON T2.ItemCode=T1.ItemCode AND T2.SysNumber=T1.SysNumber
left join (
select max(isnull(a0.DocDate,'')) Fecha,a1.FreeTxt  from OINV A0

inner join inv1 A1 on a0.docentry = a1.docentry 
where a1.FreeTxt is not null
group by  a1.FreeTxt
) T5 on t5.FreeTxt = T2.WhsCode +'_' +t1.LotNumber
WHERE isnull(t1.LotNumber,'') <>'' and t2.WhsCode ='CUNOR' and T2.Quantity>0 and t5.FreeTxt ='{Reference}'
