SELECT T1.Price
FROM OPLN T0 WITH (NOLOCK) 
inner join ITM1 T1 WITH (NOLOCK)  on T0.ListNum = T1.PriceList 
where t1.ItemCode ='{Article}' and t0.U_GLO_Location='{WhsCode}'