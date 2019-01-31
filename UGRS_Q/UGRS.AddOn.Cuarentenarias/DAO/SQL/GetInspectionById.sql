SELECT
Code AS Code,
U_IDInsp AS ID,
U_DateInsp AS DateInsp,
U_User AS User_,
U_DateSys AS DateSys,
U_CardCode AS CardCode,
U_WhsCode AS WhsCode,
U_Cancel AS Cancel,
U_TotKls AS TotalKg,
U_Quantity AS Quantity,
U_QuantityNP AS NP,
U_QuantityReject AS RE,
U_Payment AS Payment,
U_PaymentCustom AS PaymentCustom,
U_Classification AS TypeG,
U_Series AS Series,
U_AverageW AS Average,
U_DocEntryIM AS DocEntryIM,
U_DocEntryIU AS DocEntryIU,
U_CheckInsp AS ChkInspection

FROM  [@UG_CU_OINS]
WHERE U_IDInsp = '{ID}'