select WhsCode from OWHS T0
inner join OLCT T1 on T0.Location = T1.Code
where T1.Location ='{WareHouse}' or t0.WhsCode ='{WareHouse}'