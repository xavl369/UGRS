--SELECT
--Code AS Code,
--U_CodeInsp AS CodeInsp,
--U_ItemCode AS ItemCode,
--U_MvtoType AS MvtoType,
--U_Commentary AS Comments,
--U_Quantity AS Quantity

SELECT
t1.Name AS Name,
t0.Code AS Code,
U_CodeInsp AS CodeInsp,
U_ItemCode AS ItemCode,
U_MvtoType AS MvtoType,
U_Commentary AS Comments,
U_Quantity AS Quantity,
U_Tipo


FROM  [@UG_CU_INS1] t0
left join [@UG_CU_MVTY] t1 on t1.Code = t0.U_MvtoType

WHERE U_CodeInsp = '{ID}'