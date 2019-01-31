select T0.ItemCode, T0.Price from ITM1 T0
INNER JOIN OPLN T1 ON T1.ListNum = T0.PriceList
WHERE T1.U_GLO_Location = '{WhsCode}' AND T0.ItemCode = '{ItemCode}'