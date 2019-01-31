select T0.DocEntry, T0.DocNum, T0.DocDate, T0.JrnlMemo, T0.U_PL_WhsReq, T0.Filler, T0.ToWhsCode, T0.U_MQ_OrigenFol from OWTR T0 
where  (T0.U_PL_WhsReq='{Warehouse}') 
and T0.docnum not in (select A0.U_MQ_OrigenFol from OWTQ A0 where isnull(A0.U_MQ_OrigenFol,'') <> '')
and T0.docnum not in (select A1.U_MQ_OrigenFol from OWTR A1 where isnull(A1.U_MQ_OrigenFol,'') <> '')