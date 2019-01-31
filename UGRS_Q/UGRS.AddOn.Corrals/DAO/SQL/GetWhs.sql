
select t1.Warehouse from ousr T0
inner join OUDG T1 on t0.DfltsGroup = t1.Code
where t0.USERID='{UsrId}'