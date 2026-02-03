USE `makalu`;
DROP procedure IF EXISTS `migration_zeroDueMigration`;

DELIMITER $$
USE `makalu`$$
CREATE DEFINER=`root`@`localhost` PROCEDURE `migration_zeroDueMigration`()
BEGIN

declare _invoiceId int;

declare _totalSum float;
declare _zeroInvoiceId int ;
create table distinctIds
select invoiceId from InvoiceItem group by InvoiceId having Sum(Amount) = 0;

  
WHILE Exists(select 1 from distinctIds Limit 1) DO

select invoiceId into _invoiceId from distinctIds Limit 1;

SET SQL_SAFE_UPDATES = 0;
update invoice set statusId=230 where id=_invoiceId;
SET SQL_SAFE_UPDATES = 1;

SET SQL_SAFE_UPDATES = 0;
DELETE FROM distinctIds WHERE  invoiceId = _invoiceId;
SET SQL_SAFE_UPDATES = 1;
 END WHILE;
  
  DROP TABLE distinctIds;
END$$

DELIMITER ;



SET SQL_SAFE_UPDATES = 0;
CALL migration_zeroDueMigration();
SET SQL_SAFE_UPDATES = 1;