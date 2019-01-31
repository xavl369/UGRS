SELECT   
            'N' AS Chk,
            t4.CardCode AS CardCode,
            t4.CardName AS CardName,
            t5.WhsCode AS WhsCode,
            t5.WhsName AS WhsName,
            T1.ItemCode AS ItemCode,
			T1.itemName AS ItemName,
            sum(t2.Quantity) Cantidad,
   
            CASE WHEN MAX(t7.U_IDInsp) is null then 'Sin Asignar' 
            WHEN MAX(t7.U_IDInsp) =0 then 'Pendiente' 
            WHEN MAX(t7.U_DocEntryGI) is not null then 'Facturado'
            ELSE
            'Inspeccionado'
            END Estatus
            FROM OBTN T1 
            INNER JOIN OBTQ T2 ON T2.ItemCode=T1.ItemCode AND T2.SysNumber=T1.SysNumber
            INNER JOIN OCRD T4 ON T4.CardCode = t1.MnfSerial
            INNER JOIN OWHS T5 ON T5.WhsCode = T2.WhsCode
			inner join OLCT T6 on t6.Code = t5.Location
            left join [@UG_CU_OINS] T7 on t7.U_CardCode = t1.MnfSerial and t7.U_WhsCode=t2.WhsCode and t7.U_IDInsp =0 AND t7.U_Cancel ='N'
            where  t2.Quantity>0  and T5.u_cu_rejection='N' and t6.Location = '{WhsPpal}' 
            and '{DateInsp}' = case when t7.U_IDInsp is null then T1.ExpDate else T7.U_DateInsp end
            group by t4.CardName,t5.WhsName, T1.itemName,T1.itemCode,t4.CardCode,t5.WhsCode
			order by t4.CardName, t5.WhsCode Desc