
 -- select NumAtCard,* 
 --from [@UG_CU_OINS] t0
 --right join ODRF t1 on t1.NumAtCard = t0.U_DocEntryIM or t1.NumAtCard = t0.U_DocEntryIU
 -- where NumAtCard like '%{Reference}%' and t0.U_IDInsp = '{IdInsp}'


  select t1.DocEntry 
 from [@UG_CU_OINS] t0
 right join ODRF t1 on t1.NumAtCard = t0.U_DocEntryIM or t1.NumAtCard = t0.U_DocEntryIU
 inner join DRF1 t2 on t1.DocEntry = t2.DocEntry
  where t2.FreeTxt like '%{Reference}%' and t0.U_WhsCode = '{Corral}'