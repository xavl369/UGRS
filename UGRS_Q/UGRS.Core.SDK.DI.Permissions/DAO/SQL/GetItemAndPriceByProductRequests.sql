SELECT T2.U_ItemCode, T1.U_Quantity , T3.Price
FROM [@UG_PE_WS_PRRE] T1 
INNER JOIN [@UG_PE_PRRE] T2 ON T2.U_IdPEPRRE = T1.U_ProductId
INNER JOIN ITM1 T3 ON T2.U_ItemCode = T3.ItemCode
INNER JOIN OPLN T4 ON T4.ListNum = T3.PriceList
INNER JOIN [@UG_PE_WS_PERE] T5 ON T5.U_RequestId = T1.U_RequestId
WHERE T4.U_GLO_WhsCode = 'OFGE' AND T1.U_RequestId = '{RequestId}' AND T2.U_IdPEPETY ='{MobilizationTypeId}'
