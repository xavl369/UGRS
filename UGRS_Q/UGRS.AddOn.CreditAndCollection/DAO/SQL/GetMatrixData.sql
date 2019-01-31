--select 
--T3.CardName,
--SUM(t1.U_Amount) as TotSelled, 
--t2.TotDebit,
--T4.PaidInv,
--'N' AS Chk,
--t1.U_SellerCode,


--case when t0.U_Folio + t1.U_SellerCode 
--not in(
--	select U_FolioSubasta + U_Auxiliar from [@UG_CC_AUTN] where U_Revizado = 'Y'
--	) then 'N' else 'Y' end as Autorized


--FROM [@UG_SU_AUTN] T0 WITH (nolock)
--inner join [@UG_SU_BAHS] T1 ON T1.U_AuctionId = T0.U_Id
--	left join(
--		SELECT sum(A0.Credit)-sum(A0.Debit) AS TotDebit,A0.U_SU_Folio,A0.U_GLO_Auxiliar 
--		FROM JDT1 A0
--		where A0.Account ='{Account}' 
--		GROUP BY A0.U_SU_Folio,A0.U_GLO_Auxiliar
--	) T2 ON T2.U_SU_Folio = t0.U_Folio and t2.U_GLO_Auxiliar = t1.U_SellerCode
--	left join OCRD T3 ON T1.U_SellerCode = t3.CardCode
--	left join(
--			SELECT A1.DocTotal - A1.PaidToDate AS PaidInv, A1.CardCode 
--			FROM OINV A1
--			WHERE A1.DocStatus ='O' and A1.CANCELED='N' and A1.DocDueDate >= '{DateNow}'
--			)T4 ON T4.CardCode = T1.U_SellerCode
--	WHERE (T0.U_Folio = '{Auction}' or t0.U_Date = '{Date}') and t1.U_SellerCode != ''
--	GROUP BY T3.CardName, TotDebit,  t1.U_SellerCode, T4.PaidInv


select 
T3.CardName,
SUM(t1.U_Amount) as TotSelled, 
t2.TotDebit,
 T4.PaidInv,
'N' AS Chk,
t1.U_SellerCode,


case when t0.U_Folio + t1.U_SellerCode 
not in(
	select U_FolioSubasta + U_Auxiliar from [@UG_CC_AUTN] where U_Revizado = 'Y'
	) then 'N' else 'Y' end as Autorized


from [@UG_SU_AUTN] T0 with (nolock)
inner join [@UG_SU_BAHS] T1 on T1.U_AuctionId = T0.U_Id
	left join(
		select sum(A0.Credit)-sum(A0.Debit) as TotDebit,A0.U_SU_Folio,A0.U_GLO_Auxiliar 
		FROM JDT1 A0
		where A0.Account ='{Account}' 
		group by A0.U_SU_Folio,A0.U_GLO_Auxiliar
	) T2 on T2.U_SU_Folio = t0.U_Folio and t2.U_GLO_Auxiliar = t1.U_SellerCode
	left join OCRD T3 on T1.U_SellerCode = t3.CardCode
	left join(
			SELECT SUM(A1.DocTotal) - SUM(A1.PaidToDate) AS PaidInv, A1.CardCode
			FROM OINV A1
			WHERE A1.DocStatus ='O' and A1.CANCELED='N' --and A1.DocDueDate >= '20171124'
			GROUP BY A1.CardCode
			)T4 ON T4.CardCode = T1.U_SellerCode
	where (T0.U_Folio = '{Auction}') and t1.U_SellerCode != '' -- and U_Opened = 'N'
	group by T3.CardName, TotDebit,  t1.U_SellerCode, T4.PaidInv,t0.U_Folio
