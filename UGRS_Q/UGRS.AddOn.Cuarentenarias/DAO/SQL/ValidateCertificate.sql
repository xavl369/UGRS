
SELECT  distinct 
case when (t4.Name is not null) then t4.U_UsedDate else GETDATE() end Date,
T1.U_PE_Certificate AS NCertificate,
T1.CardCode AS CardCode,
case when (t4.Name is not null) then (t1.Quantity - t4.U_Quantity) else t1.Quantity end Quantity,
T1.ItemCode,
T3.U_ItemCU AS Item,
--case  when (t4.Name IS NULL) then 'Y' else 'N' end Estatus
case  when (t1.U_PE_Certificate not in (select C1.Code from [@UG_CU_SICERT] C1)) AND (t4.Name IS NULL) then 'Y' else case when (t1.U_PE_Certificate in (select C1.Code from [@UG_CU_SICERT] C1)) then 'N' else 'N' end end Estatus
FROM 
(select b2.Quantity, b2.ItemCode, B1.U_PE_Certificate, b1.CardCode 
 from OINV B1 
 INNER JOIN INV1 B2 ON B2.DocEntry = B1.DocEntry AND B1.CANCELED='N'
 union
 select b1.U_Quantity Quantity, b1.U_ItemCode ItemCode, b1.U_Certificate U_PE_Certificate, b1.Name CardCode 
 from [@UG_CU_SICERT] B1
)  T1
left JOIN [@UG_CU_CUIT] T3 ON T3.U_ItemPE = T1.ItemCode
left JOIN [@UG_CU_CERT] T4 ON t4.Name = t1.U_PE_Certificate
left join [@UG_CU_OINS] t5 on t4.U_IdInsp = t5.U_IDInsp 
WHERE T1.U_PE_Certificate ='{NCertificate}' and t3.U_ItemCU is not null

