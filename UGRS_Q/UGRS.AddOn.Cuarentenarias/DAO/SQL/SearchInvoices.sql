select NumAtCard
from [@UG_CU_OINS] t0
 right join OINV t1 on t1.NumAtCard = t0.U_DocEntryIM or t1.NumAtCard = t0.U_DocEntryIU
   where NumAtCard  like '%{Reference}%' and t0.U_WhsCode = '{Corral}'

--select NumAtCard,*
--from [@UG_CU_OINS] t0
-- inner join OINV t1 on t1.NumAtCard = t0.U_DocEntryIM or t1.NumAtCard = t0.U_DocEntryIU
-- where NumAtCard like '%{Reference}%' and t0.U_IDInsp = '{IdInsp}'