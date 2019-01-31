
select  Top 1 DistNumber
			from OBTN T1 
			INNER JOIN OBTQ T2 ON T2.ItemCode=T1.ItemCode AND T2.SysNumber=T1.SysNumber
			inner JOIN IBT1 T3 ON T3.ItemCode=T1.ItemCode AND T3.BatchNum=T1.DistNumber
			where t2.WhsCode ='{WhsCode}'  and 
			t1.DistNumber in (select t0.BatchNum from IBT1 T0 
								where T0.BaseType=60 and T0.BaseEntry = '{BaseEntry}' and T0.WhsCode = '{WhsCode}')