select t0.series,SeriesName from NNM1 t0
inner join NNM2 t1 on t0.Series = t1.Series
where t1.UserSign ='{UsrSign}' and t0.ObjectCode=30