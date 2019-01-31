	select T.U_Folio, T.U_EntryDate, T.U_OutputDate, U_Number, U_BPCode, CardName, U_Driver, U_CarTag, T.U_Folio, U_InputWT, U_OutputWT, 
	sum(U_netWeight) as U_netWeight, BP.CardType, T.U_Amount
	from  [@UG_PL_TCKT] as T
	left join [OCRD] as BP on T.U_BPCode = BP.CardCode
	left join [@UG_PL_TCKD] as Td on T.U_Folio = Td.U_Folio
	where t.U_BPCode like '%{BPCode}%' and
	T.U_CapType like '%{CapType}%' and
	T.U_DocType like '%{DocType}%' and
	CAST(T.U_EntryDate AS DATE) between CAST('{Date1}' AS DATE) and CAST('{Date2}' AS DATE) and
	t.U_Status = '{Status}'
	group by T.U_Folio, T.U_EntryDate, T.U_OutputDate, U_Number, U_BPCode, CardName, U_Driver, U_CarTag, T.U_Folio, U_InputWT, U_OutputWT, 
	T.Code, BP.CardType, T.U_Amount
	order by T.Code desc