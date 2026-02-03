SET SQL_SAFE_UPDATES = 0;
update invoiceitem set description = REPLACE(description,'Royality','Royalty') where id > 0;
SET SQL_SAFE_UPDATES = 1;