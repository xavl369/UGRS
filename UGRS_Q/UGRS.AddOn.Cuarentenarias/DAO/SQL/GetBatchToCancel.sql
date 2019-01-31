--select * 

--from [@UG_CU_OINS]  t0
--inner join obtn t1 on t1.MnfSerial = t0.U_CardCode
--inner join OBTQ t2 on t2.WhsCode = t0.U_WhsCode and t2.ItemCode = t0.U_Classification and t2.SysNumber = t1.SysNumber

--where U_DateInsp ='{ExpDate}' and MnfSerial = '{CardCode}' and U_User = 12

--select *
--from OBTN t0
--inner join OBTQ t1 on t1.SysNumber = t0.SysNumber

--where InDate = '{ExpDate}' and MnfSerial = '{CardCode}' and DistNumber like 'R_%' and WhsCode = '{MainWhs}'

select *
from OBTN t0
inner join OBTQ t1 on t1.SysNumber = t0.SysNumber

where InDate = '{ExpDate}' and MnfSerial = '{CardCode}' and LotNumber = '{IdInsp}' and WhsCode = '{MainWhs}'