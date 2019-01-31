select *				select 
				      t0.whscode, t0.BaseType, t0.DocDate, t0.Quantity,t0.Direction,t0.BaseEntry
				from IBT1 T0  with (nolock)
				inner join OBTN T1  with (nolock) on t0.ItemCode = T1.ItemCode and t0.BatchNum = t1.DistNumber
				inner join (
				           select TT0.BatchNum,TT0.whscode,TT0.itemcode,tt0.BaseEntry
						   from IBT1 TT0  with (nolock)
						   where TT0.BaseType=59
							) T2 on t2.BatchNum= t0.BatchNum 
				where T1.MnfSerial='{CardCode}'  and t0.ItemCode = '{ItemCode}' and t0.docdate <='{DocDate}' and
				cast(t0.BaseEntry as nvarchar(15))+'-'+cast(t0.BaseType as nvarchar(15)) != cast('' as nvarchar(15))+'-60'
				order by t0.whscode,t0.docdate asc




