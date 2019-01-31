SELECT DISTINCT
'N' AS Chk,
T1.Code AS Code,
T1.U_IdInsp AS ID,
T1.U_CardCode AS CardCode,
T1.U_DateInsp AS Fecha,
T1.U_WhsCode AS WhsCode,
T2.WhsName AS Corral,
T3.CardName AS Cliente,
T1.U_Classification AS Tipo,
T1.U_Quantity AS Cantidad,
T1.U_Totkls AS TotalKg,
T1.U_QuantityNP AS NP,
T1.U_QuantityReject AS RE,
t1.U_Series as Serie,
t6.itemName as Item,

CASE WHEN (T1.U_IDInsp) is null then 'Sin Asignar' 
            WHEN (T1.U_IDInsp) =0 then 'Pendiente' 
            --WHEN (T1.U_DocEntryGI) is not null then 'Facturado'
            ELSE
            'Inspeccionado'
            END Estatus,
CASE WHEN (t4.U_CodeInsp) is not null THEN 'Capturado'
when (t1.U_QuantityReject+t1.U_QuantityReject) = 0 then 'Sin Detalles'
			ELSE 'Por capturar'
			END Detalles


FROM [@UG_CU_OINS] T1
INNER JOIN OWHS T2 ON T2.WhsCode = T1.U_WhsCode
INNER JOIN OCRD T3 ON T3.CardCode = T1.U_CardCode
left join OITM t6 on t1.U_Classification = t6.ItemCode 
left join [@UG_CU_INS1] t4 on t4.U_CodeInsp = t1.U_IDInsp
WHERE T1.U_Series = '{Series}' AND T1.U_DateInsp  = '{DateInsp}' and t1.U_Cancel = 'N'