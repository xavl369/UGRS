select

(case when isnull((select count(DocNum) from OINV where (NumAtCard like '%'+t0.U_DocEntryIM+'%' and  ltrim(rtrim( isnull(t0.U_DocEntryIM,''))) != '') or ((NumAtCard like '%'+t0.U_DocEntryIU +'%' and ltrim(rtrim(isnull(t0.U_DocEntryIU,''))) != '' ))),0) > 0 then 'FACTURADO' 
when isnull((select count(*) from ODRF where (NumAtCard like '%'+t0.U_DocEntryIM+'%' or  NumAtCard like '%'+t0.U_DocEntryIU +'%') and ((isnull(t0.U_DocEntryIM,'') != '' or isnull(t0.U_DocEntryIU,'') != '' ))),0) > 0 then 'BORRADOR' 

  else 'PENDIENTE POR FACTURAR' end
)  Stat
 from [@UG_CU_OINS] t0 where U_IDInsp  = '{IdInsp}'