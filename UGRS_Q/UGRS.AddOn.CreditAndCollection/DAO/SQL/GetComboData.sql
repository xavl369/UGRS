select distinct SUM(t1.U_Amount) as TotSelled, t0.U_Folio,t1.U_SellerCode from [@UG_SU_AUTN] T0 with (nolock)
inner join [@UG_SU_BAHS] T1 on T1.U_AuctionId = T0.U_Id

where U_Folio + U_SellerCode 
not in (
select U_FolioSubasta + U_Auxiliar from [@UG_CC_AUTN] 
)
group by U_SellerCode,t0.U_Folio 