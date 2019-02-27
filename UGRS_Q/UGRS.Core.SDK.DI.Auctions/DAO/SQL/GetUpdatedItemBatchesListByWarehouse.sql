SELECT DISTINCT T0.ItemCode,
	T0.MnfSerial AS CardCode,
	T0.DistNumber AS BatchNumber,
	T0.InDate,
CONVERT(varchar, (CASE WHEN  MAX([dbo].[DateTimeConverter](T0.UpdateDate,T0.U_GLO_TimeModif)) IS NOT NULL   THEN
CASE WHEN  MAX([dbo].[DateTimeConverter](T0.UpdateDate,T0.U_GLO_TimeModif)) > MAX([dbo].[DateTimeConverter](T3.CreateDate,T3.CreateTS)) THEN
CASE WHEN  MAX([dbo].[DateTimeConverter](T0.UpdateDate,T0.U_GLO_TimeModif)) > MAX([dbo].[DateTimeConverter](T3.UpdateDate,t3.UpdateTS))THEN 
CASE WHEN  MAX([dbo].[DateTimeConverter](T0.UpdateDate,T0.U_GLO_TimeModif)) > (case when MAX([dbo].[DateTimeConverter](T4.UpdateDate,t4.UpdateTS)) 
	IS NOT NULL then MAX([dbo].[DateTimeConverter](T4.UpdateDate,t4.UpdateTS)) else CONVERT(datetime2,'1900-01-01 00:00:00') END) THEN 
MAX([dbo].[DateTimeConverter](T0.UpdateDate,T0.U_GLO_TimeModif))
	ELSE MAX([dbo].[DateTimeConverter](T4.UpdateDate,t4.UpdateTS)) END 
	ELSE MAX([dbo].[DateTimeConverter](T3.UpdateDate,t3.UpdateTS))END 
	ELSE MAX([dbo].[DateTimeConverter](T3.UpdateDate,t3.UpdateTS))END 
	ELSE 
CASE WHEN 	
	MAX([dbo].[DateTimeConverter](T3.UpdateDate,t3.UpdateTS)) > MAX([dbo].[DateTimeConverter](T3.CreateDate,T3.CreateTS)) THEN 
CASE WHEN MAX([dbo].[DateTimeConverter](T3.UpdateDate,t3.UpdateTS)) > 
(case when MAX([dbo].[DateTimeConverter](T4.UpdateDate,t4.UpdateTS)) 
	IS NOT NULL then MAX([dbo].[DateTimeConverter](T4.UpdateDate,t4.UpdateTS)) else CONVERT(datetime2,'1900-01-01 00:00:00') END) THEN 
	MAX([dbo].[DateTimeConverter](T3.UpdateDate,t3.UpdateTS))
	ELSE MAX([dbo].[DateTimeConverter](T4.UpdateDate,t4.UpdateTS))END 
	ELSE MAX([dbo].[DateTimeConverter](T3.UpdateDate,t3.UpdateTS))END  
	END), 20) as UpdatedDate

	 from OBTN T0 
INNER JOIN 	OBTQ T1 ON 	T0.ItemCode = T1.ItemCode AND T0.SysNumber = T1.SysNumber
inner join IBT1 T2 on t2.BatchNum = T0.DistNumber and T0.ItemCode = t2.ItemCode
left join OIGN T3 on t2.BaseEntry= t3.DocEntry and t3.ObjType = t2.BaseType 
left join (SELECT A4.UpdateTS, A4.UpdateDate, MAX(A4.DocEntry)DocEntry, A4.ObjType FROM OIGE A4 
group by A4.UpdateTS, A4.UpdateDate, A4.ObjType) T4 on T4.DocEntry = t2.BaseEntry and t2.BaseType = T4.ObjType
where T1.WhsCode = '{WhsCode}'
group by T0.DistNumber,T0.ItemCode,T0.MnfSerial,T0.InDate
ORDER BY UpdatedDate
