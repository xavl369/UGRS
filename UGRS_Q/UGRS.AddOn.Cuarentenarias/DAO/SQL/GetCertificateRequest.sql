
select Solicitud from (SELECT max(t1.baseref) Solicitud, U_PE_Certificate  from OINV T0
inner join inv1 t1 on t0.docentry = t1.DocEntry
group by t1.Baseref, T0.U_PE_Certificate
union 
select Code as U_PE_Certificate, Code as Solicitud
 from [@UG_CU_SICERT] B1 
 
 ) A1

 WHERE A1.U_PE_Certificate = '{Certificate}'
