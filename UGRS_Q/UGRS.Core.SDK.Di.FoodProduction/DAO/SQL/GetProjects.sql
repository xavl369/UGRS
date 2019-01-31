 --Proyectos
 select PrjCode from OPRJ
 where ValidFrom < GETDATE() and ValidTo > GETDATE() AND Active = 'Y'