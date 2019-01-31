 --select DocEntry from ODRF 
 --where NumAtCard = '{Reference}'


 
 select * from ODRF t1
inner join DRF1 t2 on t1.DocEntry = t2.DocEntry
 where FreeTxt = '{Reference}' and DocStatus = 'O'