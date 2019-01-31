select t1.Warehouse, t0.U_GLO_CostCenter from OUSR T0
inner join oudg t1 on t0.DfltsGroup = t1.Code
where t0.USERID = '{UserID}'