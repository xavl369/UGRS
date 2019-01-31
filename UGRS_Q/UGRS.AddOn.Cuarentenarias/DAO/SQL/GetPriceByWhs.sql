SELECT T1.Price
FROM 
 ITM1 T1 
INNER JOIN OPLN T2 ON T2.ListNum = T1.PriceList

where ItemCode = '{ItemCode}' and T2.U_GLO_Location = '{WhsCode}'