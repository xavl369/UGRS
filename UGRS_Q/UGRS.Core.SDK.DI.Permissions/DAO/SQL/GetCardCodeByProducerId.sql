SELECT T2.CardCode AS CardCode
FROM [@UG_PE_WS_PERE] T1
INNER JOIN OCRD T2 ON T2.U_PE_CustomerCode = T1.U_CustomerCode
WHERE T1.U_CustomerCode = '{ProducerId}'
