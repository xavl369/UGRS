SELECT 
    A.U_Folio AS id, 
    B.U_Number AS lote,
    B.U_ItemTypeCode AS sexo, 
    B.U_Quantity AS cantidad,
    B.U_Buyer AS comprador, 
    B.U_Seller AS vendedor, 
    BPS.LicTradNum AS rfc_vendedor, 
    A.U_Date AS fecha
FROM [@UG_WS_BAHS] AS B
INNER JOIN [@UG_WS_AUTN] AS A 
    ON B.U_AuctionId = A.U_Id 
INNER JOIN [OCRD] AS BPB 
    ON B.U_BuyerCode = BPB.CardCode 
INNER JOIN [OCRD] AS BPS 
    ON B.U_SellerCode = BPS.CardCode
WHERE 
    ('{SellerTaxCode}' = '' OR BPS.LicTradNum = '{SellerTaxCode}') AND
    ('{Seller}' = '' OR BPS.CardName LIKE '%{Seller}%') AND
    ('{Buyer}' = '' OR BPB.CardName LIKE '%{Buyer}%') AND
    ('{Date}' = '' OR CAST(A.U_Date AS DATE) = CAST('{Date}' AS DATE))