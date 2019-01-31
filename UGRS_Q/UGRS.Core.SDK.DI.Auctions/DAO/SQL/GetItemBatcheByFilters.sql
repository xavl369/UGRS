SELECT 
	T0.ItemCode,
	T0.MnfSerial AS CardCode,
	T0.LotNumber AS InitialWarehouse,
	T1.WhsCode AS CurrentWarehouse,
	T0.DistNumber AS BatchNumber,
	T0.U_GLO_Food3 AS ChargeFood,
	T0.U_SU_Payment As Payment,
	T1.Quantity,
	T0.InDate,
	Case when t0.UpdateDate is null then ''
	else t0.UpdateDate end as UpdateDate,
	T0.ExpDate,
	T0.U_GLO_Time,
	T3.U_GLO_Ticket as Folio,
	Case when t3.UpdateTS > (case when U_GLO_TimeModif is null then '' else U_GLO_TimeModif end) then t3.UpdateTS else U_GLO_TimeModif end as UpdateHour
FROM 
	OBTN T0
INNER JOIN 
	OBTQ T1 
ON 
	T0.ItemCode = T1.ItemCode
AND 
	T0.SysNumber = T1.SysNumber
inner join IBT1 T2 on t2.BatchNum = t0.DistNumber and t0.ItemCode = t2.ItemCode
inner join OIGN T3 on t2.BaseEntry= t3.DocEntry and t3.ObjType = t2.BaseType 
WHERE 
	T1.WhsCode = '{WhsCode}' 
AND 
	T0.ItemCode = '{ItemCode}' 
AND 
	T0.DistNumber = '{BatchNumber}'