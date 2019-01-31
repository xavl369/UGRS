SELECT 
'N' AS Chk,
'N' AS Chk3pc,
0.00 AS NewWeight,
t0.Name as Named,
t0.U_IDInsp as Inspection,
t0.U_DateInsp as InspectionDate,
t0.U_WhsCode as Corralcode,
t0.U_DocEntryGI as DocEntryGI,
t0.U_Cancel as Cancel,
t0.U_TotKls as TotKg,
t0.U_Quantity as Quantity,
t0.U_QuantityNP as NP,
t0.U_QuantityReject as Rejected,
t0.U_Payment as Payment,
t0.U_PaymentCustom as PaymentCustom,
t0.U_Classification as Article,
t0.U_Series as Serial,
t0.U_CheckInsp as SpecialInsp,
t0.U_DocEntryGR as DocEntryGR,
t0.U_DocEntryIU as DocEntryIU,
t0.U_DocEntryIM as DocEntryIM,
t0.U_AverageW as AvWeight,
t0.U_AverageW*(t0.U_Quantity - (t0.U_QuantityNP + t0.U_QuantityReject)) as RealWeigth,
t0.Code as RowCode,
t1.CardCode as CardCode,
t1.CardName as Name,
t2.WhsName as Corral,
t3.SeriesName as SeriesName,
t6.itemName as Item,
(case when isnull((select count(DocNum) from OINV where (NumAtCard like '%'+t0.U_DocEntryIM+'%' and  ltrim(rtrim( isnull(t0.U_DocEntryIM,''))) != '') or ((NumAtCard like '%'+t0.U_DocEntryIU +'%' and ltrim(rtrim(isnull(t0.U_DocEntryIU,''))) != '' ))),0) > 0 then 'FACTURADO' 
when isnull((select count(*) from ODRF where (NumAtCard like '%'+t0.U_DocEntryIM+'%' or  NumAtCard like '%'+t0.U_DocEntryIU +'%') and ((isnull(t0.U_DocEntryIM,'') != '' or isnull(t0.U_DocEntryIU,'') != '' ))),0) > 0 then 'BORRADOR' 

  else 'PENDIENTE POR FACTURAR' end
)  Stat    
FROM [@UG_CU_OINS] t0
inner join OCRD T1 on t1.CardCode = t0.U_CardCode
inner join OWHS t2 on t2.WhsCode = t0.U_WhsCode
inner join  NNM1 t3 on t3.Series = t0.U_Series
inner join NNM2 t4 on t3.Series = t4.Series
left join OITM t6 on t0.U_Classification = t6.ItemCode 

where t0.U_DateInsp = '{DateInsp}'  and U_IDInsp > 0
and t4.UserSign ='{UserSign}'  and t3.ObjectCode=59 and t0.U_Cancel = 'N'
order by Stat DESC,t1.CardName,Corral