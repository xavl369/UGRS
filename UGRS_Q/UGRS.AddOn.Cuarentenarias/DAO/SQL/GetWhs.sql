select t1.Warehouse from ousr T0
inner join OUDG T1 on t0.DfltsGroup = t1.Code
inner join OWHS T2 on t2.whscode =t1.Warehouse
inner join OLCT T4 on t4.Code = t2.Location
inner join 
(select whscode,Location from OWHS where U_CU_Rejection ='Y')
 T3 on t3.Location = t4.Code
where t0.USERID='{UsrId}'