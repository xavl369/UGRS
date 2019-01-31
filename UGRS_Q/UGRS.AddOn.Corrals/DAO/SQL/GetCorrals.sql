
	
	
	if OBJECT_ID('tempdb..#UGRStblTempMV') is not null
				drop table #UGRStblTempMV
	CREATE TABLE #UGRStblTempMV(whscode nvarchar(15),basetype int, docdate datetime, quantity int, direction int,InvDate datetime,distnumber nvarchar(30),movtype nvarchar(10),QStock int)
	
	insert into #UGRStblTempMV(whscode,basetype,docdate,quantity,direction,InvDate,distnumber,movtype,QStock)
	select distinct
	t0.whscode, t0.BaseType, t0.DocDate, SUM(t0.Quantity) Quantity,t0.Direction, ISNULL(MAX(t2.DocDate),'1900-01-01')as InvDate, t1.DistNumber, t4.U_GLO_INMO,B2.Quantity as Stock
	from IBT1 T0  with (nolock)
	inner join OBTN T1  with (nolock) on t0.ItemCode = T1.ItemCode and t0.BatchNum = t1.DistNumber
		left join (
				select TT0.BaseCard, TT0.DocDate, tt0.FreeTxt
				from INV1 TT0  with (nolock)
						    
				) T2 on t2.BaseCard = t1.MnfSerial and t2.FreeTxt = t0.WhsCode
	left join OWHS T3 on t3.WhsCode = t0.WhsCode
	left join OIGE t4 on t4.DocEntry  =t0.BaseEntry and t0.BaseType = t4.ObjType
	inner join (select A1.Quantity,A0.DistNumber,A1.WhsCode from OBTN A0 
	inner join OBTQ A1 on A1.SysNumber = A0.SysNumber and A1.ItemCode = A0.ItemCode) B2 on B2.DistNumber = t1.Distnumber and B2.WhsCode = t0.WhsCode
	

			
	where T1.MnfSerial='{CardCode}'and t3.U_GLO_WhsCodePather = 'CRHE'  and t1.DistNumber+'-'+Cast(t0.BaseType as nvarchar(15)) +'-'+Cast(cast(t0.Quantity as int) as nvarchar(15)) 
 not in(
 select isnull(A1.DistNumber,'')+'-59'+'-'+Cast(cast(A0.Quantity as int) as nvarchar(15)) from IBT1 A0
 inner join OBTN A1  with (nolock) on A0.ItemCode = A1.ItemCode and A0.BatchNum = A1.DistNumber
 inner join OIGE A2 with (nolock) on A2.DocEntry  =A0.BaseEntry and A0.BaseType = A2.ObjType
 where A2.U_GLO_INMO = 6)
	group by t0.WhsCode,t0.DocDate,t0.Direction,t1.DistNumber,t0.BaseType,t0.Quantity,t0.BaseNum,t4.U_GLO_INMO,B2.Quantity
	order by t0.whscode,t0.docdate asc

	select t0.whscode,basetype,docdate,t0.quantity,direction,InvDate,t0.distnumber,movtype,QStock from #UGRStblTempMV t0
	where t0.whscode+CAST(t0.quantity as nvarchar(10))+t0.distnumber not in  (select t0.whscode+CAST(t0.quantity as nvarchar(10))+t0.distnumber where  (InvDate >= docdate and QStock = 0))
